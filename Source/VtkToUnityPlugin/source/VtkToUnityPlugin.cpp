// Example low level rendering Unity plugin

#include "VtkToUnityPlugin.h"

#include "VtkToUnityInternalHelpers.h"

#include <assert.h>
#include <math.h>
#include <map>
#include <queue>
#include <vector>
#include <sstream>

//#include <queue>
//#include <mutex>
//#include <condition_variable>


static Float4 ZeroFloat4()
{
	Float4 zeroFloat4;
	zeroFloat4.x = 0.0f;
	zeroFloat4.y = 0.0f;
	zeroFloat4.z = 0.0f;
	zeroFloat4.w = 0.0f;
	return zeroFloat4;
};

static std::weak_ptr<VtkToUnityAPI> sCurrentAPI;

// --------------------------------------------------------------------------
// Connect to the debugging in unity

static DebugFuncPtr sDebugFp = nullptr;

static void Debug(DebugLogLevel level, const std::string& message)
{
	if (nullptr == sDebugFp)
	{
		return;
	}

	sDebugFp(static_cast<int>(level), message.c_str());
}

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API SetDebugFunction(
	DebugFuncPtr fp)
{
	sDebugFp = fp;

	if (auto sharedAPI = sCurrentAPI.lock()) {
		sharedAPI->SetDebugLogFunction(Debug);
	}
}


// --------------------------------------------------------------------------
// GraphicsDeviceEvent

void VtkToUnityPlugin::SetVtkToUnityAPI(std::weak_ptr<VtkToUnityAPI> api)
{
	sCurrentAPI = api;
}

bool VtkToUnityPlugin::GotVtkToUnityAPI()
{
	return (!sCurrentAPI.expired());
}

void VtkToUnityPlugin::ProcessDeviceEventXYZ(UnityGfxDeviceEventType type, IUnityInterfaces* interfaces)
{
	if (auto sharedAPI = sCurrentAPI.lock()) {
		sharedAPI->ProcessDeviceEvent(type, interfaces);
	}
}


// --------------------------------------------------------------------------
// Load in a DICOM volume from the specified folder

extern "C" bool UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API LoadDicomVolume(
	const char *dicomFolder)
{
	//Debug("LoadDicomVolume: Start");

	// load in the volume for the volume renderer too
	if (dicomFolder == NULL) {
		Debug(
			DebugLogLevel::DebugLogWarning, 
			"LoadDicomVolume: NULL string pointer passed in");
		return false;
	}

	if (*dicomFolder == '\0') {
		Debug(
			DebugLogLevel::DebugLogWarning,
			"LoadDicomVolume: string with no length passed in");
		return false;
	}

	std::string dicomFolderStr(dicomFolder);

	Debug(
		DebugLogLevel::DebugLog,
		std::string("LoadMhdVolume: Loading Dicom Data from ") + dicomFolderStr);

	if (auto sharedAPI = sCurrentAPI.lock()) {
		return sharedAPI->LoadDicomVolumeFromFolder(dicomFolderStr);
	}

	return false;
}

extern "C" bool UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API LoadMhdVolume(
	const char *mhdPath)
{
	// load in the volume for the volume renderer too
	if (mhdPath == NULL) {
		Debug(
			DebugLogLevel::DebugLogWarning,
			"LoadMhdVolume: NULL string pointer passed in");
		return false;
	}

	if (*mhdPath == '\0') {
		Debug(
			DebugLogLevel::DebugLogWarning,
			"LoadMhdVolume: string with no length passed in");
		return false;
	}

	std::string mhdPathStr(mhdPath);

	Debug(
		DebugLogLevel::DebugLog,
		std::string("LoadMhdVolume: Loading MHD Data from ") + mhdPathStr);

	if (auto sharedAPI = sCurrentAPI.lock()) {
		return sharedAPI->LoadUncMetaImage(mhdPathStr);
	}

	return false;
}

extern "C" bool UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API LoadNrrdVolume(
	const char *nrrdPath)
{
	//Debug("LoadMhdVolume: Start");

	// load in the volume for the volume renderer too
	if (nrrdPath == NULL) {
		Debug(
			DebugLogLevel::DebugLogWarning,
			"LoadNrrdVolume: NULL string pointer passed in");
		return false;
	}

	if (*nrrdPath == '\0') {
		Debug(
			DebugLogLevel::DebugLogWarning,
			"LoadNrrdVolume: string with no length passed in");
		return false;
	}

	std::string nrrdPathStr(nrrdPath);

	Debug(
		DebugLogLevel::DebugLog,
		std::string("LoadMhdVolume: Loading NRRD Data from ") + nrrdPathStr);

	if (auto sharedAPI = sCurrentAPI.lock()) {
		return sharedAPI->LoadNrrdImage(nrrdPathStr);
	}

	return false;
}


extern "C" bool CreatePaddingMask(int paddingValue)
{
	if (auto sharedAPI = sCurrentAPI.lock()) {
		return sharedAPI->CreatePaddingMask(paddingValue);
	}

	return false;
}

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API ClearVolumes()
{
	if (auto sharedAPI = sCurrentAPI.lock()) {
		sharedAPI->ClearVolumes();
	}
}


extern "C" int UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API GetNVolumes()
{
	if (auto sharedAPI = sCurrentAPI.lock()) {
		return sharedAPI->GetNVolumes();
	}

	return -1;
}


PLUGINEX(Float4) GetVolumeSpacingM()
{
	if (auto sharedAPI = sCurrentAPI.lock()) {
		return sharedAPI->GetVolumeSpacingM();
	}

	return ZeroFloat4();
}


PLUGINEX(Float4) GetVolumeExtentsMin()
{
	if (auto sharedAPI = sCurrentAPI.lock()) {
		return sharedAPI->GetVolumeExtentsMin();
	}

	return ZeroFloat4();
}


PLUGINEX(Float4) GetVolumeExtentsMax()
{
	if (auto sharedAPI = sCurrentAPI.lock()) {
		return sharedAPI->GetVolumeExtentsMax();
	}

	return ZeroFloat4();
}


PLUGINEX(Float4) GetVolumeOriginM()
{
	if (auto sharedAPI = sCurrentAPI.lock()) {
		return sharedAPI->GetVolumeOriginM();
	}

	return ZeroFloat4();
}


PLUGINEX(int) AddVolumeProp()
{
	if (auto sharedAPI = sCurrentAPI.lock()) {
		return sharedAPI->AddVolumeProp();
	}

	return -1;
}


PLUGINEX(int) AddCropPlaneToVolume(int volumeId)
{
	if (auto sharedAPI = sCurrentAPI.lock()) {
		return sharedAPI->AddCropPlaneToVolume(volumeId);
	}

	return -1;
}


static SafeQueue<int> sNewVolumeIndex;
PLUGINEX(void) SetVolumeIndex(int index)
{
	sNewVolumeIndex.enqueue(index);
}


PLUGINEX(int) GetNTransferFunctions()
{
	if (auto sharedAPI = sCurrentAPI.lock()) {
		return sharedAPI->GetNTransferFunctions();
	}

	return -1;
}


PLUGINEX(int) GetTransferFunctionIndex()
{
	if (auto sharedAPI = sCurrentAPI.lock()) {
		return sharedAPI->GetTransferFunctionIndex();
	}

	return -1;
}


static SafeQueue<int> sNewTransferFunctionIndex;
PLUGINEX(void) SetTransferFunctionIndex(int index)
{
	sNewTransferFunctionIndex.enqueue(index);
}


PLUGINEX(int) AddTransferFunction()
{
	if (auto sharedAPI = sCurrentAPI.lock()) {
		return sharedAPI->AddTransferFunction();
	}

	return -1;
}


PLUGINEX(int) ResetTransferFunctions()
{
	if (auto sharedAPI = sCurrentAPI.lock()) {
		return sharedAPI->ResetTransferFunctions();
	}

	return -1;
}


PLUGINEX(void) SetTransferFunctionPoint(
	int transferFunctionIndex,
	double windowFraction,
	double red1,
	double green1,
	double blue1,
	double opacity1)
{
	if (auto sharedAPI = sCurrentAPI.lock()) {
		sharedAPI->SetTransferFunctionPoint(
			transferFunctionIndex,
			windowFraction,
			red1,
			green1,
			blue1,
			opacity1);
	}
}


static SafeQueue<std::pair<float, float>> sNewVolumeWWWL;
PLUGINEX(void) SetVolumeWWWL(
	float windowWidth, float windowLevel)
{
	sNewVolumeWWWL.enqueue(std::make_pair(windowWidth, windowLevel));
}


static SafeQueue<float> sNewOpacityFactor;
PLUGINEX(void) SetVolumeOpacityFactor(float opacityFactor)
{
	sNewOpacityFactor.enqueue(opacityFactor);
}


static SafeQueue<float> sNewBrightnessFactor;
PLUGINEX(void) SetVolumeBrightnessFactor(float brightnessFactor)
{
	sNewBrightnessFactor.enqueue(brightnessFactor);
}


static SafeQueue<bool> sNewRenderComposite;
PLUGINEX(void) SetRenderComposite(bool composite)
{
	sNewRenderComposite.enqueue(composite);
}


static SafeQueue<bool> sNewTargetFramerateOn;
PLUGINEX(void) SetTargetFrameRateOn(bool targetOn)
{
	sNewTargetFramerateOn.enqueue(targetOn);
}

static SafeQueue<int> sNewTargetFramerateFps;
PLUGINEX(void) SetTargetFrameRateFps(int targetFps)
{
	sNewTargetFramerateFps.enqueue(targetFps);
}


PLUGINEX(int) AddMPR(int existingMprId)
{
	if (auto sharedAPI = sCurrentAPI.lock()) {
		return sharedAPI->AddMPR(existingMprId, -1);
	}

	return -1;
}


PLUGINEX(int) AddMPRFlipped(int existingMprId, int flipAxis)
{
	if (auto sharedAPI = sCurrentAPI.lock()) {
		return sharedAPI->AddMPR(existingMprId, flipAxis);
	}

	return -1;
}


static SafeQueue<std::pair<float, float>> sNewMPRWWWL;
PLUGINEX(void) SetMPRWWWL(
	float windowWidth, float windowLevel)
{
	sNewMPRWWWL.enqueue(std::make_pair(windowWidth, windowLevel));
}


// Add a primitive shape to the scene
PLUGINEX(int) AddShapePrimitive(
	int shapeType,
	Float4& rgbaColour,
	bool wireframe)
{
	if (auto sharedAPI = sCurrentAPI.lock()) {
		return (sharedAPI->AddShapePrimitive(
			shapeType,
			rgbaColour,
			wireframe));
	}

	return -1;
}


PLUGINEX(void) GetShapePrimitiveProperty(
	int shapeId,
	LPCSTR propertyName,
	char* retValue)
{
	if (auto sharedAPI = sCurrentAPI.lock())
	{
		sharedAPI->GetShapePrimitiveProperty(
			shapeId,
			propertyName,
			retValue);
	}
	else
	{
		retValue = "err::(No valid Api set)";
	}
}


PLUGINEX(void) SetShapePrimitiveProperty(
	int shapeId,
	LPCSTR propertyName,
	LPCSTR newValue)
{
	if (auto sharedAPI = sCurrentAPI.lock())
	{
		sharedAPI->SetShapePrimitiveProperty(
			shapeId,
			propertyName,
			newValue);
	}
}


// --------------------------------------------------------------------------
// Lighting methods

PLUGINEX(int) AddLight()
{
	if (auto sharedAPI = sCurrentAPI.lock()) {
		return sharedAPI->AddLight();
	}

	return -1;
}

PLUGINEX(void) SetLightingOn(
	bool lightingOn)
{
	if (auto sharedAPI = sCurrentAPI.lock()) {
		sharedAPI->SetLightingOn(
			lightingOn);
	}
}

PLUGINEX(void) SetLightColor(
	int id,
	LightColorType lightingType,
	Float4 &rgbColor)
{
	if (auto sharedAPI = sCurrentAPI.lock()) {
		sharedAPI->SetLightColor(
			id,
			lightingType,
			rgbColor);
	}
}

// Set light intensity 
PLUGINEX(void) SetLightIntensity(
	int id,
	float intensity)
{
	if (auto sharedAPI = sCurrentAPI.lock()) {
		sharedAPI->SetLightIntensity(
			id,
			intensity);
	}
}


// --------------------------------------------------------------------------
// Volume lighting methods

PLUGINEX(void) SetVolumeLighting(
	VolumeLightType volumeLightType,
	float lightValue)
{
	if (auto sharedAPI = sCurrentAPI.lock()) {
		sharedAPI->SetVolumeLighting(
			volumeLightType,
			lightValue);
	}
}


// --------------------------------------------------------------------------
// General Prop methods

// remove the shape primitve with the given index
PLUGINEX(void) RemoveProp3D(
	int id)
{
	if (auto sharedAPI = sCurrentAPI.lock()) {
		sharedAPI->RemoveProp3D(
			id);
	}
}


static SafeQueue<std::pair<int, Float16> > sPropTransformsWorldM;
PLUGINEX(void) SetProp3DTransform(
	int id,
	Float16 &transformWorldM)
{
	sPropTransformsWorldM.enqueue(std::make_pair(id, transformWorldM));
}


static SafeQueue<std::pair<int, Float16> > sMPRTransformsVolumeM;
PLUGINEX(void) SetMPRTransform(
	int id,
	Float16 &transformVolumeM)
{
	sMPRTransformsVolumeM.enqueue(std::make_pair(id, transformVolumeM));
}


// --------------------------------------------------------------------------
// Set the camera View matrix (column major array, Open GL style)
static SafeQueue<std::array<double, 16> > sViewMatrixColMajor;

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API SetViewMatrix(
	Float16 &view4x4ColMajor)
{
	auto sharedAPI = sCurrentAPI.lock();
	if (!sharedAPI) {
		return;
	}

	std::array<double, 16> matrixColMajor;

	for (unsigned int i = 0u; i < 16; ++i) {
		matrixColMajor[i] = static_cast<double>(view4x4ColMajor.elements[i]);
	}

	sViewMatrixColMajor.enqueue(matrixColMajor);
}

// Set the camera Projection matrix (column major array, Open GL style)
static SafeQueue<std::array<double, 16> > sProjectionMatrixColMajor;

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API SetProjectionMatrix(
	Float16 &projection4x4ColMajor)
{
	auto sharedAPI = sCurrentAPI.lock();
	if (!sharedAPI) {
		return;
	}

	std::array<double, 16> matrixColMajor;

	for (unsigned int i = 0u; i < 16; ++i) {
		matrixColMajor[i] = static_cast<double>(projection4x4ColMajor.elements[i]);
	}

	sProjectionMatrixColMajor.enqueue(matrixColMajor);
}

// --------------------------------------------------------------------------
// OnRenderEvent
// This will be called for GL.IssuePluginEvent script calls; eventID will
// be the integer passed to IssuePluginEvent. In this example, we just ignore
// that value.
static void UNITY_INTERFACE_API OnRenderEvent(int eventID)
{
	VtkToUnityPlugin::UpdateCachedData();
	VtkToUnityPlugin::DoRender();
}

// update all of the cached data
void VtkToUnityPlugin::UpdateCachedData()
{
	// Unknown / unsupported graphics device type? Do nothing
	auto sharedAPI = sCurrentAPI.lock();
	if (!sharedAPI) {
		return;
	}

	while (!sPropTransformsWorldM.empty())
	{
		std::pair<int, Float16> propTransformWorldM = sPropTransformsWorldM.dequeue();
		sharedAPI->SetProp3DTransform(
			propTransformWorldM.first,
			propTransformWorldM.second);
	}

	// set the volume index to the latest index
	{
		int index(-1);
		bool setIndex(false);

		while (!sNewVolumeIndex.empty())
		{
			index = sNewVolumeIndex.dequeue();
			setIndex = true;
		}

		if (setIndex)
		{
			sharedAPI->SetVolumeIndex(index);
		}
	}

	// MPR transforms
	// If there are multiple requests to move the same MPR just use the most recent one
	{
		std::map<int, Float16> thinnedMprTransformsM;

		while (!sMPRTransformsVolumeM.empty())
		{
			std::pair<int, Float16> mprTransformVolumeM = sMPRTransformsVolumeM.dequeue();
			thinnedMprTransformsM.insert(mprTransformVolumeM);
		}

		for (auto const& mprTransformsM : thinnedMprTransformsM) {
			sharedAPI->SetMPRTransform(
				mprTransformsM.first,
				mprTransformsM.second);
		}
	}

	while (!sNewTransferFunctionIndex.empty())
	{
		int transferFunctionIndex = sNewTransferFunctionIndex.dequeue();
		sharedAPI->SetTransferFunctionIndex(transferFunctionIndex);
	}

	while (!sNewVolumeWWWL.empty())
	{
		std::pair<float, float> wwwl = sNewVolumeWWWL.dequeue();
		sharedAPI->SetVolumeWWWL(wwwl.first, wwwl.second);
	}

	while (!sNewOpacityFactor.empty())
	{
		float opacityFactor = sNewOpacityFactor.dequeue();
		sharedAPI->SetVolumeOpactityFactor(opacityFactor);
	}

	while (!sNewBrightnessFactor.empty())
	{
		float brightnessFactor = sNewBrightnessFactor.dequeue();
		sharedAPI->SetVolumeBrightnessFactor(brightnessFactor);
	}

	while (!sNewRenderComposite.empty())
	{
		const bool composite = sNewRenderComposite.dequeue();
		sharedAPI->SetRenderComposite(composite);
	}

	while (!sNewTargetFramerateOn.empty())
	{
		const bool targetFramerateOn = sNewTargetFramerateOn.dequeue();
		sharedAPI->SetTargetFrameRateOn(targetFramerateOn);
	}

	while (!sNewTargetFramerateFps.empty())
	{
		const int targetFramerateFps = sNewTargetFramerateFps.dequeue();
		sharedAPI->SetTargetFrameRateFps(targetFramerateFps);
	}

	while (!sNewMPRWWWL.empty())
	{
		std::pair<float, float> wwwl = sNewMPRWWWL.dequeue();
		sharedAPI->SetMPRWWWL(wwwl.first, wwwl.second);
	}
}

// actually do the render
void VtkToUnityPlugin::DoRender()
{
	// Unknown / unsupported graphics device type? Do nothing
	if (auto sharedAPI = sCurrentAPI.lock()) {
		sharedAPI->UpdateVtkCameraAndRender(
			sViewMatrixColMajor.dequeue(),
			sProjectionMatrixColMajor.dequeue());
	}
}

// --------------------------------------------------------------------------
// GetRenderEventFunc, an example function we export which is used to get a rendering event callback function.

extern "C" UnityRenderingEvent UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API GetRenderEventFunc()
{
	return OnRenderEvent;
}

