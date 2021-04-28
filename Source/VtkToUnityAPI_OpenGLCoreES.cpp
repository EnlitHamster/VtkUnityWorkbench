#include "VtkToUnityAPI_OpenGLCoreES.h"

#include "VtkToUnityAPI.h"
#include "PlatformBase.h"

#include "VtkToUnityInternalHelpers.h"

#define _USE_MATH_DEFINES
#include <math.h>

#include <thread>

// OpenGL Core profile (desktop) or OpenGL ES (mobile) implementation of VtkToUnityAPI.
// Supports several flavors: Core, ES2, ES3

//vtk headers
#include "vtkAutoInit.h" 
VTK_MODULE_INIT(vtkRenderingOpenGL2); // VTK was built with vtkRenderingOpenGL2
VTK_MODULE_INIT(vtkRenderingVolumeOpenGL2); // Required for the smart volume mapper
#include <vtkProperty.h>
#include <vtkCallbackCommand.h>
#include <vtkCamera.h>
#include <vtkCubeSource.h>
#include <vtkExternalOpenGLRenderWindow.h>
#include <vtkExternalOpenGLCamera.h>
#include <vtkLight.h>
#include <vtkLightActor.h>
#include <vtkLightCollection.h>
#include <vtkPolyDataMapper.h>
#include "vtkWindows.h" // Needed to include OpenGL header on Windows.
#include <vtk_glew.h>

#include <vtkDICOMImageReader.h>
#include <vtkMetaImageReader.h>
#include <vtkNrrdReader.h>
#include <vtkMatrix4x4.h>
#include <vtkPlane.h>
#include <vtkSphereSource.h>
#include <vtkConeSource.h>
#include <vtkCullerCollection.h>
#include <vtkFrustumCoverageCuller.h>

#include <vtkImageFlip.h>
#include <vtkImageThreshold.h>

#include <vtkAlgorithmOutput.h>


#if SUPPORT_OPENGL_UNIFIED

#include <assert.h>
#if UNITY_IPHONE
#	include <OpenGLES/ES2/gl.h>
#elif UNITY_ANDROID || UNITY_WEBGL
#	include <GLES2/gl2.h>
#else
#	include "GLEW/glew.h"
#endif

// there will be a clamp function when we get to C++ 17, which can replace this
static double clip(double lower, double upper, double n) {
	return std::max(lower, std::min(n, upper));
}

static const double sMmToMConversion(0.001);
static const double sMinTransferFunctionStep(0.2);
static const double sWindowFractionDoubleToInteger(100.0);
static const double sWindowFractionIntegerToDouble(1.0 / sWindowFractionDoubleToInteger);
static const unsigned int sRedIndex(0U);
static const unsigned int sGreenIndex(1U);
static const unsigned int sBlueIndex(2U);
static const unsigned int sOpacityIndex(3U);


static int WindowFractionDoubleToInteger(const double windowFractionIn)
{
	return static_cast<int>(
		(windowFractionIn * sWindowFractionDoubleToInteger) + 0.5);
}

static VtkToUnityAPI_OpenGLCoreES::TransferFunction InitialiseGreyLinear()
{
	VtkToUnityAPI_OpenGLCoreES::TransferFunction colours;

	colours.insert(
		VtkToUnityAPI_OpenGLCoreES::TransferFunctionPoint(
			WindowFractionDoubleToInteger(-0.5),
			{ {  0.0, 0.0, 0.0, 0.0 } }));
	
	colours.insert(
		VtkToUnityAPI_OpenGLCoreES::TransferFunctionPoint(
			WindowFractionDoubleToInteger(0.5),
			{ {  1.0, 1.0, 1.0, 1.0 } }));

	return colours;
}



// Renderer Creation and implementation ===========================================================

VtkToUnityAPI* CreateVtkToUnityAPI_OpenGLCoreES(UnityGfxRenderer apiType)
{
	return new VtkToUnityAPI_OpenGLCoreES(apiType);
}


VtkToUnityAPI_OpenGLCoreES::VtkToUnityAPI_OpenGLCoreES(UnityGfxRenderer apiType)
	: mAPIType(apiType)
{
}


void VtkToUnityAPI_OpenGLCoreES::ProcessDeviceEvent(UnityGfxDeviceEventType type, IUnityInterfaces* interfaces)
{
	if (type == kUnityGfxDeviceEventInitialize)
	{
		CreateResources();
	}
	else if (type == kUnityGfxDeviceEventShutdown)
	{
		//@TODO: release resources
	}
}


void VtkToUnityAPI_OpenGLCoreES::SetDebugLogFunction(DebugLogFunc func)
{
	mDebugLog = std::make_shared<DebugLogFunc>(func);
}


bool VtkToUnityAPI_OpenGLCoreES::LoadDicomVolumeFromFolder(
	const std::string &dicomFolder)
{
	vtkNew<vtkDICOMImageReader> dicomReader;
	dicomReader->SetDirectoryName(dicomFolder.c_str());
	dicomReader->Update();

	vtkSmartPointer<vtkImageData> volumeImageData = 
		vtkSmartPointer<vtkImageData>::New();
	volumeImageData->DeepCopy(dicomReader->GetOutput());

	if (!CheckVolumeExtentSpacingOrigin(volumeImageData))
	{
		return false;
	}

	AddVolume(volumeImageData);
	return true;
}

bool VtkToUnityAPI_OpenGLCoreES::LoadUncMetaImage(
	const std::string &mhdPath)
{
	vtkNew<vtkMetaImageReader> mhdReader;
	mhdReader->SetFileName(mhdPath.c_str());
	mhdReader->Update();

	vtkSmartPointer<vtkImageData> volumeImageData =
		vtkSmartPointer<vtkImageData>::New();
	volumeImageData->DeepCopy(mhdReader->GetOutput());

	if (!CheckVolumeExtentSpacingOrigin(volumeImageData))
	{
		return false;
	}

	AddVolume(volumeImageData);
	return true;
}

bool VtkToUnityAPI_OpenGLCoreES::LoadNrrdImage(
	const std::string &nrrdPath)
{
	vtkNew<vtkNrrdReader> nrrdReader;
	nrrdReader->SetFileName(nrrdPath.c_str());
	nrrdReader->Update();

	vtkSmartPointer<vtkImageData> volumeImageData =
		vtkSmartPointer<vtkImageData>::New();
	volumeImageData->DeepCopy(nrrdReader->GetOutput());

	if (!CheckVolumeExtentSpacingOrigin(volumeImageData))
	{
		return false;
	}

	AddVolume(volumeImageData);
	return true;
}


bool VtkToUnityAPI_OpenGLCoreES::CreatePaddingMask(int paddingValue)
{
	// return if we have already generated a mask or there are no volume
	if (nullptr != mVolumeMask ||
		nullptr == mCurrentVolumeData)
	{
		return false;
	}

	auto imageThreshold = vtkSmartPointer<vtkImageThreshold>::New();
	imageThreshold->SetInputData(mCurrentVolumeData);
	imageThreshold->SetInValue(0.0);
	imageThreshold->SetOutValue(255.0);
	imageThreshold->SetOutputScalarTypeToUnsignedChar();
	imageThreshold->ThresholdBetween(paddingValue - 0.5, paddingValue + 0.5);
	imageThreshold->Update();

	mVolumeMask = vtkSmartPointer<vtkImageData>::New();
	mVolumeMask->DeepCopy(imageThreshold->GetOutput());

	return true;
}


void VtkToUnityAPI_OpenGLCoreES::ClearVolumes()
{
	mVolumeDataVector.clear();
	SetVolumeIndex(-1);
	mVolumeMask = nullptr;
}


int VtkToUnityAPI_OpenGLCoreES::GetNVolumes()
{
	return static_cast<int>(mVolumeDataVector.size());
}


Float4 VtkToUnityAPI_OpenGLCoreES::GetVolumeSpacingM()
{
	return StdArray3ToFloat4(mVolumeSpacingM);
}


Float4 VtkToUnityAPI_OpenGLCoreES::GetVolumeExtentsMin()
{
	return StdArray3ToFloat4(mVolumeExtentMin);
}


Float4 VtkToUnityAPI_OpenGLCoreES::GetVolumeExtentsMax()
{
	return StdArray3ToFloat4(mVolumeExtentMax);
}

Float4 VtkToUnityAPI_OpenGLCoreES::GetVolumeOriginM()
{
	return StdArray3ToFloat4(mVolumeOriginCentredM);
}


void VtkToUnityAPI_OpenGLCoreES::SetVolumeIndex(
	const int index)
{
	int newIndex = index;
	// This may be an odd looking set of if/else, but...
	// if the index is invalid, show the synthetic volume, indicated by 
	if (newIndex < 0 ||
		newIndex >= mVolumeDataVector.size())
	{
		newIndex = -1;
	}

	if (newIndex ==-1 )
	{
		// if the index is invalid, show the synthetic volume
		mCurrentVolumeData->ShallowCopy(mSyntheticVolumeData.GetPointer());
		mCurrentVolumeIndex = -1;
		return;
	}

	if (newIndex == mCurrentVolumeIndex)
	{
		// if the index is valid but unchanged, so we don't need to update the 
		// current volume data
		return;
	}

	mCurrentVolumeIndex = newIndex;
	mCurrentVolumeData->ShallowCopy(mVolumeDataVector[newIndex]);

	for (auto volumePropsVectorPair : mVolumeProp3Ds)
	{
		auto volumePropsVector = volumePropsVectorPair.second;

		for (int iVolumeProp = 0; iVolumeProp < volumePropsVector.size(); ++iVolumeProp)
		{
			volumePropsVector[iVolumeProp]->SetVisibility(iVolumeProp == mCurrentVolumeIndex);
		}
	}

	// update all of the reslices, is all of this really necessary?
	for (auto & reslicePair : mReslice)
	{
		auto id = reslicePair.first;
		auto reslice = reslicePair.second;

		auto resliceTransformIter = mResliceTransforms.find(id);
		auto resliceColorIter = mResliceColors.find(id);

		if (resliceTransformIter != mResliceTransforms.end() &&
			resliceColorIter != mResliceColors.end())
		{
			reslice->RemoveAllInputs();
			reslice->SetInputData(mCurrentVolumeData.GetPointer());
			reslice->SetResliceTransform((*resliceTransformIter).second);
			reslice->SetInterpolationModeToLinear();
			reslice->Update();

			(*resliceColorIter).second->Update();
		}
	}
}


int VtkToUnityAPI_OpenGLCoreES::AddVolumeProp()
{
	// We need a volume mapper for each volume prop as the clipping planes are 
	// attached to the volume mapper
	std::vector<vtkSmartPointer<vtkProp3D>> volumePropsVector;
	volumePropsVector.reserve(GetNVolumes());
	std::vector<vtkSmartPointer<vtkGPUVolumeRayCastMapper>> volumeMappersVector;
	volumeMappersVector.reserve(GetNVolumes());
	vtkTypeBool visibility(true);

	for (auto volumeData : mVolumeDataVector)
	{
		auto volumeMapper = vtkSmartPointer<vtkGPUVolumeRayCastMapper>::New();
		volumeMapper->SetBlendModeToComposite();
		volumeMapper->SetInputData(volumeData);
		volumeMapper->Update();

		// we need to scale the volume mapper steps as we scale the data by a 
		// factor of 1000 to account for mm to m as the base unit
		volumeMapper->SetSampleDistance(
			volumeMapper->GetSampleDistance() * sMmToMConversion);

		// if a volume mask has been defined set it
		if (nullptr != mVolumeMask)
		{
			volumeMapper->SetMaskInput(mVolumeMask);
			volumeMapper->SetMaskTypeToBinary();
		}

		// connect up the volume to the property and the mapper
		auto volumeProp = vtkSmartPointer<vtkVolume>::New();
		volumeProp->SetProperty(mVolumeProperty.GetPointer());
		volumeProp->SetVisibility(visibility);
		visibility = false;
		volumeProp->SetMapper(volumeMapper);

		volumePropsVector.push_back(volumeProp);
		volumeMappersVector.push_back(volumeMapper);
		mRenderer->AddViewProp(volumeProp);
	}

	mVolumeProp3Ds.insert(std::make_pair(mNextActorIndex, volumePropsVector));
	mVolumeMappers.insert(std::make_pair(mNextActorIndex, volumeMappersVector));

	return (mNextActorIndex++);
}


int VtkToUnityAPI_OpenGLCoreES::AddCropPlaneToVolume(const int volumeId)
{
	// Let's just check that we have a volume mapper to apply the croping plane to
	auto mapperIter = mVolumeMappers.find(volumeId);

	if (mVolumeMappers.end() == mapperIter)
	{
		return -1;
	}

	auto volumeMappersVector = (*mapperIter).second;

	// so create the plane and add it to the mapper
	auto volumeCropPlane = vtkSmartPointer<vtkPlane>::New();
	mVolumeCropPlanes.insert(std::make_pair(mNextActorIndex, volumeCropPlane));

	for (auto volumeMapper : volumeMappersVector)
	{
		if (NULL == volumeMapper)
		{
			return -1;
		}

		volumeMapper->AddClippingPlane(volumeCropPlane.GetPointer());
	}

	return (mNextActorIndex++);
}


int VtkToUnityAPI_OpenGLCoreES::GetNTransferFunctions()
{
	return static_cast<int>(mTransferFunctions.size());
}


int VtkToUnityAPI_OpenGLCoreES::GetTransferFunctionIndex()
{
	return static_cast<int>(mTransferFunctionIndex);
}


void VtkToUnityAPI_OpenGLCoreES::SetTransferFunctionIndex(const int index)
{
	if (index < 0 || index >= GetNTransferFunctions())
	{
		return;
	}

	mTransferFunctionIndex = index;
	UpdateVolumeColorAndOpacity();
}


int VtkToUnityAPI_OpenGLCoreES::AddTransferFunction()
{
	mTransferFunctions.push_back(InitialiseGreyLinear());
	return static_cast<int>(mTransferFunctions.size() - 1);
}


int VtkToUnityAPI_OpenGLCoreES::ResetTransferFunctions()
{
	mTransferFunctions.clear();
	mTransferFunctionIndex = AddTransferFunction();
	return mTransferFunctionIndex;
}


void VtkToUnityAPI_OpenGLCoreES::SetTransferFunctionPoint(
	const int transferFunctionIndex,
	const double windowFraction,
	const double red1,
	const double green1,
	const double blue1,
	const double opacity1)
{
	if (transferFunctionIndex < 0 
		|| transferFunctionIndex >= GetNTransferFunctions())
	{
		return;
	}

	auto& transferFunction =
		mTransferFunctions[transferFunctionIndex];

	// if there is an existing point, update it
	// otherwise add a new point
	const int windowFractionInt(WindowFractionDoubleToInteger(windowFraction));
	auto &transferFunctionPointIter = transferFunction.find(windowFractionInt);

	if (transferFunctionPointIter != transferFunction.end())
	{
		auto &colourArray = (*transferFunctionPointIter).second;
		colourArray[sRedIndex] = red1;
		colourArray[sGreenIndex] = green1;
		colourArray[sBlueIndex] = blue1;
		colourArray[sOpacityIndex] = opacity1;
	}
	else
	{
		transferFunction.insert(
			VtkToUnityAPI_OpenGLCoreES::TransferFunctionPoint(
				windowFractionInt,
				{ { red1, green1, blue1, opacity1 } }));
	}

	UpdateVolumeColorAndOpacity();
}


void VtkToUnityAPI_OpenGLCoreES::SetVolumeWWWL(const double windowWidth, const double windowLevel)
{
	mWindowWidth = windowWidth;
	mWindowLevel = windowLevel;
	UpdateVolumeColorAndOpacity();
}


void VtkToUnityAPI_OpenGLCoreES::SetVolumeOpactityFactor(const double opacityFactor)
{
	mOpacityFactor = opacityFactor;
	UpdateVolumeColorAndOpacity();
}


void VtkToUnityAPI_OpenGLCoreES::SetVolumeBrightnessFactor(const double brightnessFactor)
{
	mBrightnessFactor = brightnessFactor;
	UpdateVolumeColorAndOpacity();
}


void VtkToUnityAPI_OpenGLCoreES::SetRenderComposite(const bool composite)
{
	for (auto & volumeMapperPair : mVolumeMappers)
	{
		auto volumeMappersVector = volumeMapperPair.second;

		for (auto volumeMapper : volumeMappersVector)
		{
			if (volumeMapper)
			{
				if (composite)
				{
					volumeMapper->SetBlendModeToComposite();
				}
				else
				{
					volumeMapper->SetBlendModeToMaximumIntensity();
				}
			}
		}
	}
}

void VtkToUnityAPI_OpenGLCoreES::SetTargetFrameRateOn(const bool targetOn)
{
	for (auto & volumeMapperPair : mVolumeMappers)
	{
		auto volumeMappersVector = volumeMapperPair.second;

		for (auto volumeMapper : volumeMappersVector)
		{
			if (volumeMapper)
			{
				volumeMapper->SetAutoAdjustSampleDistances(targetOn);
			}
		}
	}

}

void VtkToUnityAPI_OpenGLCoreES::SetTargetFrameRateFps(const int targetFps)
{
	mRenderWindow->SetDesiredUpdateRate(targetFps);
}

int VtkToUnityAPI_OpenGLCoreES::AddMPR(const int existingMprId, const int flipAxis)
{
	// are we dealing with a new or existing MPR?
	// - if new - create the reslice and its transform
	// - if existing - use the existing reslice and its transform
	auto existingResliceColors = mResliceColors.find(existingMprId);

	vtkSmartPointer<vtkImageMapToColors> resliceColor;

	if (mResliceColors.end() == existingResliceColors)
	{
		auto resliceTransform = vtkSmartPointer<vtkTransform>::New();
		resliceTransform->Identity();

		// create a reslice object, make it 2D and associate it with the current volume
		auto reslice = vtkSmartPointer<vtkImageReslice>::New();
		reslice->SetOutputDimensionality(2);
		reslice->SetInputData(mCurrentVolumeData);
		reslice->SetResliceTransform(resliceTransform);
		reslice->SetInterpolationModeToLinear();

		// Map the image through the lookup table
		resliceColor = vtkSmartPointer<vtkImageMapToColors>::New();
		resliceColor->SetLookupTable(mResliceLookupTable);

		if (0 > flipAxis)
		{
			resliceColor->SetInputConnection(reslice->GetOutputPort());
		}
		else
		{
			// try and flip this image
			auto resliceFlip = vtkSmartPointer<vtkImageFlip>::New();
			resliceFlip->SetFilteredAxis(flipAxis);
			resliceFlip->SetInputConnection(reslice->GetOutputPort());
			resliceColor->SetInputConnection(resliceFlip->GetOutputPort());
		}

		resliceColor->Update();

		mReslice.insert(std::make_pair(mNextActorIndex, reslice));
		mResliceTransforms.insert(std::make_pair(mNextActorIndex, resliceTransform));
		mResliceColors.insert(std::make_pair(mNextActorIndex, resliceColor));
	}
	else
	{
		resliceColor = (*existingResliceColors).second;
	}

	// Display the image
	auto resliceImageActor = vtkSmartPointer<vtkImageActor>::New();
	resliceImageActor->SetInputData(resliceColor->GetOutput());

	mNonVolumeProp3Ds.insert(std::make_pair(mNextActorIndex, resliceImageActor));

	mRenderer->AddActor(resliceImageActor);

	return (mNextActorIndex++);
}


void VtkToUnityAPI_OpenGLCoreES::SetMPRWWWL(const double windowWidth, 
											const double windowLevel)
{
	mResliceLookupTable->SetTableRange(
		windowLevel - (0.5 * windowWidth),
		windowLevel + (0.5 * windowWidth)); // image intensity range
	mResliceLookupTable->Build();
}


int VtkToUnityAPI_OpenGLCoreES::AddShapePrimitive(
	const int shapeType,
	const Float4 &rgbaColour,
	const bool wireframe)
{
	vtkNew<vtkPolyDataMapper> mapper;

	if (0 == shapeType)
	{
		vtkNew<vtkCubeSource> cs;

		mapper->SetInputConnection(cs->GetOutputPort());
	}
	else if (1 == shapeType)
	{
		vtkNew<vtkSphereSource> sphereSource;
		sphereSource->SetPhiResolution(21);
		sphereSource->SetThetaResolution(21);

		mapper->SetInputConnection(sphereSource->GetOutputPort());
	}
	else if (2 == shapeType)
	{
		vtkNew<vtkConeSource> coneSource;
		coneSource->Update();

		mapper->SetInputConnection(coneSource->GetOutputPort());
	}
	else
	{
		return -1;
	}

	vtkSmartPointer<vtkActor> actor = vtkSmartPointer<vtkActor>::New();
	actor->SetMapper(mapper.GetPointer());
	actor->GetProperty()->SetColor(rgbaColour.x, rgbaColour.y, rgbaColour.z);
	actor->GetProperty()->SetOpacity(rgbaColour.w);

	if (wireframe)
	{
		actor->GetProperty()->SetRepresentationToWireframe();
	}

	mNonVolumeProp3Ds.insert(std::make_pair(mNextActorIndex, actor));
	mRenderer->AddActor(actor);

	return (mNextActorIndex++);
}


void* VtkToUnityAPI_OpenGLCoreES::GetShapePrimitiveProperty(
	const int shapeId,
	const std::string propertyName)
{
	auto actorIter = mNonVolumeProp3Ds.find(shapeId);

	if (mNonVolumeProp3Ds.end() != actorIter)
	{
		auto actor = vtkActor::SafeDownCast(actorIter->second);
		// Based on https://kitware.github.io/vtk-examples/site/Cxx/Visualization/ReverseAccess/
		vtkSmartPointer<vtkAlgorithm> algorithm = actor->GetMapper()->GetInputConnection(0, 0)->GetProducer();
		auto coneSource = dynamic_cast<vtkConeSource*>(algorithm.GetPointer());

		if (propertyName == "height")
		{
			// Based on https://stackoverflow.com/questions/54635806/how-to-store-a-double-in-a-void-in-c
			double val = coneSource->GetHeight();
			void* ret = malloc(sizeof val);
			*(double *) ret = val;
			return ret;
		}
		else
		{
			std::string msg = "Property not found";
			void* ret = malloc(sizeof msg);
			*(std::string *) ret = msg;
			return ret;
		}
	}

	std::string msg = "vtkObject not found";
	void* ret = malloc(sizeof msg);
	*(std::string *) ret = msg;
	return ret;
}


int VtkToUnityAPI_OpenGLCoreES::AddLight()
{
	auto light = vtkSmartPointer<vtkLight>::New();
	light->SetLightTypeToHeadlight();

	mRenderer->AddLight(light); 

	mLights.insert(std::make_pair(mNextActorIndex, light));

	return (mNextActorIndex++);
}


void VtkToUnityAPI_OpenGLCoreES::SetLightingOn(
	bool lightingOn)
{
	// check we have some lights
	if (mRenderer->GetLights()->GetNumberOfItems() > 0)
	{
		// even if we have, if there has not been one render loop this may still fail
		// so may need to add a 'first rendrer' flag, and store the desired state
		mVolumeProperty->SetShade(lightingOn ? 1 : 0);

		//mNonVolumeProp3Ds

		auto nonVolumePropIter = mNonVolumeProp3Ds.begin();

		while (nonVolumePropIter != mNonVolumeProp3Ds.end())
		{
			auto actor = vtkActor::SafeDownCast(nonVolumePropIter->second);

			if (NULL != actor)
			{
				actor->GetProperty()->SetLighting(lightingOn);
			}

			++nonVolumePropIter;
		}
	}
}


void VtkToUnityAPI_OpenGLCoreES::SetLightColor(
	int id,
	LightColorType lightColorType,
	Float4 &rgbColor)
{
	// find the light
	auto lightIter = mLights.find(id);

	if (mLights.end() == lightIter)
	{
		return;
	}

	auto light = lightIter->second;

	// set that lights colour property
	if (LightColorAmbient == lightColorType)
	{
		light->SetAmbientColor(rgbColor.x, rgbColor.y, rgbColor.z);
	}
	else if (LightColorDiffuse == lightColorType)
	{
		light->SetDiffuseColor(rgbColor.x, rgbColor.y, rgbColor.z);
	}
	else if (LightColorSpecular == lightColorType)
	{
		light->SetSpecularColor(rgbColor.x, rgbColor.y, rgbColor.z);
	}
}


void VtkToUnityAPI_OpenGLCoreES::SetLightIntensity(
	int id,
	float intensity)
{
	// find the light
	auto lightIter = mLights.find(id);

	if (mLights.end() == lightIter)
	{
		return;
	}

	auto light = lightIter->second;

	light->SetIntensity(intensity);
}


void VtkToUnityAPI_OpenGLCoreES::SetVolumeLighting(
	VolumeLightType volumeLightType,
	float lightValue)
{
	// set the volume properties colour
	if (VolumeLightAmbient == volumeLightType)
	{
		mVolumeProperty->SetAmbient(lightValue);
	}
	else if (VolumeLightDiffuse == volumeLightType)
	{
		mVolumeProperty->SetDiffuse(lightValue);
	}
	else if (VolumeLightSpecular == volumeLightType)
	{
		mVolumeProperty->SetSpecular(lightValue);
	}
	else if (VolumeLightSpecularPower == volumeLightType)
	{
		mVolumeProperty->SetSpecularPower(lightValue);
	}
}


void VtkToUnityAPI_OpenGLCoreES::RemoveProp3D(
	int id)
{
	{
		// was it a non-volume prop, e.g. primitive, mpr
		auto actorIter = mNonVolumeProp3Ds.find(id);

		// We've found it, so destroy it!
		if (mNonVolumeProp3Ds.end() != actorIter)
		{
			mRenderer->RemoveActor(actorIter->second);
			mNonVolumeProp3Ds.erase(id);
		}
	}

	{
		// was it a volume prop
		auto actorVectorIter = mVolumeProp3Ds.find(id);

		// We've found it, so destroy it!
		if (mVolumeProp3Ds.end() != actorVectorIter)
		{
			for (auto actorIter : actorVectorIter->second)
			{
				mRenderer->RemoveActor(actorIter);
			}

			mVolumeProp3Ds.erase(id);
		}
	}

	// remove any volume mappers
	auto volumeMapperIter = mVolumeMappers.find(id);

	// This was a volume so destory the mapper too
	if (mVolumeMappers.end() != volumeMapperIter)
	{
		// todo - Hmm, I'm not 100% on how to get rid of the mapper properly
		//*volumeMapperIter = NULL;
		mVolumeMappers.erase(id);
	}


	// This was a reslice, so destroy that object too
	{
		// may need to do some other operations here to properly clean up
		mReslice.erase(id);
		mResliceTransforms.erase(id);
		mResliceColors.erase(id);
	}

	{
		// was it a light
		auto lightIter = mLights.find(id);

		// We've found it, so destroy it!
		if (mLights.end() != lightIter)
		{
			mRenderer->RemoveLight(lightIter->second);
			mLights.erase(id);
		}
	}

}


void VtkToUnityAPI_OpenGLCoreES::SetProp3DTransform(
	int id,
	Float16 transform)
{
	// is it a not-a-volume prop?
	{
		auto actorIter = mNonVolumeProp3Ds.find(id);

		if (mNonVolumeProp3Ds.end() != actorIter)
		{
			auto prop3D = actorIter->second;
			auto vtkMatrix = Float16ToVtkMatrix4x4(transform);
			prop3D->SetUserMatrix(vtkMatrix);
			prop3D->Modified();
			return;
		}
	}

	// is it a volume prop?
	{
		auto volumeProp3DsIter = mVolumeProp3Ds.find(id);

		if (mVolumeProp3Ds.end() != volumeProp3DsIter)
		{
			auto volumePropsVector = (*volumeProp3DsIter).second;

			for (auto volumeProp : volumePropsVector)
			{
				//auto prop3D = actorIter->second;
				auto vtkMatrix = Float16ToVtkMatrix4x4(transform);
				volumeProp->SetUserMatrix(vtkMatrix);
				volumeProp->Modified();
			}
			
			return;
		}
	}

	// is it a plane?
	{
		auto planeIter = mVolumeCropPlanes.find(id);

		if (mVolumeCropPlanes.end() != planeIter)
		{
			auto vtkMatrix = Float16ToVtkMatrix4x4(transform);
			vtkNew<vtkTransform> vtkTransform;
			vtkTransform->SetMatrix(vtkMatrix);

			auto plane = planeIter->second;
			plane->SetOrigin(vtkTransform->TransformPoint(0.0, 0.0, 0.0));
			plane->SetNormal(vtkTransform->TransformNormal(0.0, 1.0, 0.0));
			plane->Modified();
			return;
		}
	}

	// no it's an unidentified object! Could have an error message here
}


void VtkToUnityAPI_OpenGLCoreES::SetMPRTransform(
	const int id,
	Float16 transformVolume)
{
	auto resliceTransformIter = mResliceTransforms.find(id);
	auto resliceColorIter = mResliceColors.find(id);

	if (mResliceTransforms.end() == resliceTransformIter || 
		mResliceColors.end() == resliceColorIter)
	{
		return;
	}

	auto resliceTransform = resliceTransformIter->second;
	auto vtkMatrix = Float16ToVtkMatrix4x4(transformVolume);
	resliceTransform->SetMatrix(vtkMatrix);
	resliceTransform->Modified();

	// this should force the image to be updated
	auto resliceColor = resliceColorIter->second;
	resliceColor->Update();
}


void VtkToUnityAPI_OpenGLCoreES::UpdateVtkCameraAndRender(
	const std::array<double, 16> &viewMatrix,
	const std::array<double, 16> &projectionMatrix)
{
	mRenderer->SetViewMatrix(viewMatrix);
	mRenderer->SetProjectionMatrix(projectionMatrix);

	mRenderer->ResetCameraClippingRange();

	if (mRenderScene)
	{
		mExternalVTKWidget->GetRenderWindow()->Render();
	}
}


void VtkToUnityAPI_OpenGLCoreES::LogToDebugLog(
	const DebugLogLevel level,
	const std::string& message)
{
	if (nullptr == mDebugLog)
	{
		return;
	}

	(*mDebugLog)(level, message);
}


void VtkToUnityAPI_OpenGLCoreES::CreateResources()
{
	mNextActorIndex = 0;
	mNonVolumeProp3Ds.clear();
	mVolumeProp3Ds.clear();
	mLights.clear();

	// create the VTK external renderer
	mRenderWindow = vtkSmartPointer<vtkExternalOpenGLRenderWindow>::New();
	mExternalVTKWidget->SetRenderWindow(mRenderWindow);
	mExternalVTKWidget->GetRenderWindow()->AddRenderer(mRenderer.GetPointer());

	vtkFrustumCoverageCuller *frustrumCoverageCuller = vtkFrustumCoverageCuller::SafeDownCast(mRenderer->GetCullers()->GetLastItem());
	frustrumCoverageCuller->SetSortingStyleToBackToFront();

	mCurrentVolumeIndex = -1;

	// Set up the Volume transfer functions, mappers, props etc.
	mWindowWidth = 150.0;
	mWindowLevel = 100.0;
	mOpacityFactor = 1.0;
	mBrightnessFactor = 1.0;

	ResetTransferFunctions();

	UpdateVolumeColorAndOpacity();

	mVolumeProperty->SetColor(mVolumeColor.GetPointer());
	mVolumeProperty->SetScalarOpacity(mVolumeOpacity.GetPointer());
	mVolumeProperty->SetInterpolationTypeToLinear();
	mVolumeProperty->SetScalarOpacityUnitDistance(0.001); // compensate for m / mm

	mVolumeProperty->SetAmbient(0.1);
	mVolumeProperty->SetDiffuse(0.9);
	mVolumeProperty->SetSpecular(0.2);
	mVolumeProperty->SetSpecularPower(10.0);

	mVolumeMappers.clear();

	// Create a synthetic volume to default to rendering
	{
		//Assigning Values , Allocating Memory
		const int maxX = 200;
		const int maxY = 200;
		const int maxZ = 150;
		mSyntheticVolumeData->SetDimensions(maxX, maxY, maxZ);
		mSyntheticVolumeData->AllocateScalars(VTK_INT, 1);
		vtkTypeInt32 *voxel = static_cast<vtkTypeInt32*>(mSyntheticVolumeData->GetScalarPointer());
		mSyntheticVolumeData->UpdateCellGhostArrayCache();

		for (int z = 0; z < maxZ; z++)
		{
			for (int y = 0; y < maxY; y++)
			{
				for (int x = 0; x < maxX; x++)
				{
					*voxel++ = z * 3;
				}
			}
		}

		std::array<double, 3> spacing = { {0.001, 0.001, 0.001} };
		mSyntheticVolumeData->SetSpacing(spacing.data());
	}

	// And set the current volume to point to it
	mCurrentVolumeData = vtkSmartPointer<vtkImageData>::New();
	mCurrentVolumeData->ShallowCopy(mSyntheticVolumeData.GetPointer());

	// Initialise the MPR looup table
	// this is a good default for US images
	mResliceLookupTable->SetRange(0.0, 350.0); // image intensity range
	mResliceLookupTable->SetValueRange(0.0, 1.0); // from black to white
	mResliceLookupTable->SetSaturationRange(0.0, 0.0); // no color saturation
	mResliceLookupTable->SetRampToLinear();
	mResliceLookupTable->Build();

	mRenderer->ResetCamera();
	mRenderer->SetLightFollowCamera(false);

	mRenderScene = true;
}

void VtkToUnityAPI_OpenGLCoreES::AddVolume(vtkSmartPointer<vtkImageData> volumeImageData)
{
	// LogToDebugLog(DebugLogLevel::DebugLog, "VtkToUnityAPI_OpenGLCoreES: AddVolume: Test Message");
	ReverseVolumeAlongZ(volumeImageData);
	mVolumeDataVector.push_back(volumeImageData);

	const int index(static_cast<int>(mVolumeDataVector.size()) - 1);
	SetVolumeIndex(index);
}

bool VtkToUnityAPI_OpenGLCoreES::CheckVolumeExtentSpacingOrigin(
	vtkSmartPointer<vtkImageData> volumeImageData)
{
	// Check that the volume given has the same extents etc. as the others
	std::array<int, 6> extent;
	volumeImageData->GetExtent(extent.data());

	// origin is in mm? converting to m
	std::array<double, 3> origin;
	volumeImageData->GetOrigin(origin.data());

	// DICOM (and mhd?) dimensions are in mm, Unity is in m, updating the spacing to be in m
	std::array<double, 3> spacing;
	volumeImageData->GetSpacing(spacing.data());
	std::for_each(spacing.begin(), spacing.end(), [](double &s) { s *= sMmToMConversion; });
	volumeImageData->SetSpacing(spacing.data());

	// if this is the first volume, just remember the values
	if (mVolumeDataVector.empty())
	{
		mVolumeExtent = extent;
		mVolumeSpacingM = spacing;
		mVolumeOrigin = origin;

		mVolumeExtentMin = { mVolumeExtent[0], mVolumeExtent[2], mVolumeExtent[4] };
		mVolumeExtentMax = { mVolumeExtent[1], mVolumeExtent[3], mVolumeExtent[5] };

		for (auto &volumeCentreElem : mVolumeCentre)
		{
			auto i = &volumeCentreElem - &mVolumeCentre[0];
			volumeCentreElem = mVolumeSpacingM[i] * -0.5 * (mVolumeExtentMin[i] + mVolumeExtentMax[i]);
		}

		// set the origin to be the centre of the volume for rotational niceness, 
		// if not absolute correctness - at least it is clear we're doing this
		volumeImageData->SetOrigin(mVolumeCentre.data());
		volumeImageData->GetOrigin(origin.data());
		mVolumeOriginCentredM = origin;

		return true;
	}

	// otherwise check that they match
	if (mVolumeExtent != extent)
	{
		return false;
	}

	const double maxError(1e-6);

	for (int i = 0; i < 3; ++i)
	{
		if (maxError < fabs(mVolumeSpacingM[i] - spacing[i]) ||
			maxError < fabs(mVolumeOrigin[i] - origin[i]))
		{
			return false;
		}
	}

	volumeImageData->SetOrigin(mVolumeCentre.data());

	return true;
}

void VtkToUnityAPI_OpenGLCoreES::ReverseVolumeAlongZ(
	vtkSmartPointer<vtkImageData> volumeImageData)
{
	auto imageCopy = vtkSmartPointer<vtkImageData>::New();
	imageCopy->DeepCopy(volumeImageData);

	std::array<int, 6> volumeExtent;
	volumeImageData->GetExtent(volumeExtent.data());

	int scalarSize = volumeImageData->GetScalarSize();

	std::array<vtkIdType, 3> increments;
	volumeImageData->GetIncrements(increments.data());

	int scalarType = volumeImageData->GetScalarType();

	unsigned char* inputDataPtr = static_cast<unsigned char*>(volumeImageData->GetScalarPointer());
	unsigned char* imageCopyDataPtr = static_cast<unsigned char*>(imageCopy->GetScalarPointer());
	int sliceSize = increments[2] * scalarSize;
	imageCopyDataPtr += sliceSize * (volumeExtent[5] - 1);

	for (int z = volumeExtent[2]; z < volumeExtent[5]; ++z)
	{
		std::memcpy(inputDataPtr, imageCopyDataPtr, sliceSize);
		inputDataPtr += sliceSize;
		imageCopyDataPtr -= sliceSize;
	}
}


void VtkToUnityAPI_OpenGLCoreES::UpdateVolumeColorAndOpacity()
{
	auto transferFunction = mTransferFunctions[mTransferFunctionIndex];

	double windowMin(-DBL_MAX);
	double lastWindowPoint(windowMin);

	mVolumeColor->RemoveAllPoints();
	mVolumeOpacity->RemoveAllPoints();

	for (auto transferFunctionPoint : transferFunction)
	{
		const double windowPointFromFraction(
			mWindowLevel
			+ (transferFunctionPoint.first * sWindowFractionIntegerToDouble * mWindowWidth));

		const double windowPoint(
			std::max(windowMin, windowPointFromFraction));

		if (windowPoint <= (lastWindowPoint + sMinTransferFunctionStep))
		{
			continue;
		}

		auto colourArray = transferFunctionPoint.second;

		mVolumeColor->AddRGBPoint(
			windowPoint,
			clip(0.0, 1.0, colourArray[sRedIndex] * mBrightnessFactor),
			clip(0.0, 1.0, colourArray[sGreenIndex] * mBrightnessFactor),
			clip(0.0, 1.0, colourArray[sBlueIndex] * mBrightnessFactor));

		mVolumeOpacity->AddPoint(
			windowPoint,
			clip(0.0, 1.0, colourArray[sOpacityIndex] * mOpacityFactor));

		lastWindowPoint = windowPoint;
	}
}

#endif // #if SUPPORT_OPENGL_UNIFIED
