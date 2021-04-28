#pragma once

#include "VtkToUnityAPI.h"
#include "PlatformBase.h"

#include <ExternalVTKWidget.h>
#include <vtkProp3D.h>
#include <vtkNew.h>
#include <vtkColorTransferFunction.h>
#include <vtkGPUVolumeRayCastMapper.h>
#include <vtkImageActor.h>
#include <vtkImageData.h>
#include <vtkImageMapToColors.h>
#include <vtkImageReslice.h>
#include <vtkLookupTable.h>
#include <vtkPiecewiseFunction.h>
#include <vtkTransform.h>
#include <vtkVolumeMapper.h>
#include <vtkVolumeProperty.h>
#include <memory>

#include "vtkExternalOpenGLRenderer3dh.h"

// Renderer Class Declaraion ======================================================================

class VtkToUnityAPI_OpenGLCoreES : public VtkToUnityAPI
{
public:
	typedef std::pair<int, std::array<double, 4>> TransferFunctionPoint;
	typedef std::map<
		TransferFunctionPoint::first_type,
		TransferFunctionPoint::second_type> TransferFunction;

	VtkToUnityAPI_OpenGLCoreES(UnityGfxRenderer apiType);
	virtual ~VtkToUnityAPI_OpenGLCoreES() { }

	virtual void ProcessDeviceEvent(UnityGfxDeviceEventType type, IUnityInterfaces* interfaces);

	virtual void SetDebugLogFunction(DebugLogFunc func);

	virtual bool GetUsesReverseZ() { return false; }

	virtual bool LoadDicomVolumeFromFolder(
		const std::string &folder);
	virtual bool LoadUncMetaImage(
		const std::string &mhdPath);
	virtual bool LoadNrrdImage(
		const std::string &nrrdPath);

	virtual bool CreatePaddingMask(int paddingValue);

	virtual void ClearVolumes();
	virtual int GetNVolumes();

	virtual Float4 GetVolumeSpacingM();
	virtual Float4 GetVolumeExtentsMin();
	virtual Float4 GetVolumeExtentsMax();
	virtual Float4 GetVolumeOriginM();

	virtual void SetVolumeIndex(
		const int index);

	virtual int AddVolumeProp();

	virtual int AddCropPlaneToVolume(const int volumeId);

	virtual int GetNTransferFunctions();
	virtual int GetTransferFunctionIndex();
	virtual void SetTransferFunctionIndex(const int index);
	virtual int AddTransferFunction();
	virtual int ResetTransferFunctions();
	virtual void SetTransferFunctionPoint(
		const int transferFunctionIndex,
		const double windowFraction,
		const double red1,
		const double green1,
		const double blue1,
		const double opacity1);
	virtual void SetVolumeWWWL(const double windowWidth, const double windowLevel);
	virtual void SetVolumeOpactityFactor(const double opacityFactor);
	virtual void SetVolumeBrightnessFactor(const double brightnessFactor);

	virtual void SetRenderComposite(const bool composite);

	virtual void SetTargetFrameRateOn(const bool targetOn);
	virtual void SetTargetFrameRateFps(const int targetFps);

	virtual int AddMPR(const int existingMprId, const int flipAxis);
	virtual void SetMPRWWWL(const double windowWidth, const double windowLevel);

	/////////////////////////////////////////////
	// Primitive controllers

	virtual int AddShapePrimitive(
		const int shapeType,
		const Float4 &rgbaColour,
		const bool wireframe);

	virtual void* GetShapePrimitiveProperty(
		const int shapeId,
		const std::string propertyName);

	virtual int AddLight();

	virtual void SetLightingOn(
		bool lightingOn);

	virtual void SetLightColor(
		int id,
		LightColorType lightColorType,
		Float4 &rgbColor);

	virtual void SetLightIntensity(
		int id,
		float intensity);

	virtual void SetVolumeLighting(
		VolumeLightType volumeLightType,
		float lightValue);

	virtual void RemoveProp3D(
		int id);

	virtual void SetProp3DTransform(
		int id,
		Float16 transform);

	virtual void SetMPRTransform(
		const int id,
		Float16 transformVolume);

	virtual void UpdateVtkCameraAndRender(
		const std::array<double, 16> &viewMatrix,
		const std::array<double, 16> &projectionMatrix);


protected:
	void CreateResources();

	void LogToDebugLog(const DebugLogLevel level, const std::string& message);

	void AddVolume(vtkSmartPointer<vtkImageData> volumeImageData);

	bool CheckVolumeExtentSpacingOrigin(
		vtkSmartPointer<vtkImageData> volumeImageData);

	void ReverseVolumeAlongZ(
		vtkSmartPointer<vtkImageData> volumeImageData);

	void UpdateVolumeColorAndOpacity();

protected:
	UnityGfxRenderer mAPIType;

	// vtk
	vtkNew<ExternalVTKWidget> mExternalVTKWidget;
	vtkSmartPointer<vtkExternalOpenGLRenderWindow> mRenderWindow;
	vtkNew<vtkExternalOpenGLRenderer3dh> mRenderer;

	std::shared_ptr<DebugLogFunc> mDebugLog;

	int mNextActorIndex;
	// So we have a set of actors for the non volumes, e.g. primitives and MPRs etc.
	std::map<int, vtkSmartPointer<vtkProp3D>> mNonVolumeProp3Ds;
	// And a set of vectors of actors for the volumes
	std::map<int, std::vector<vtkSmartPointer<vtkProp3D>>> mVolumeProp3Ds;
	// And a set of lights
	std::map<int, vtkSmartPointer<vtkLight>> mLights;

	// Volume data to render
	std::vector<vtkSmartPointer<vtkImageData>> mVolumeDataVector;
	vtkSmartPointer<vtkImageData> mCurrentVolumeData;
	int mCurrentVolumeIndex;

	vtkSmartPointer<vtkImageData> mVolumeMask;

	// Synthetic volume to fall back on to rendering
	vtkNew<vtkImageData> mSyntheticVolumeData;

	// We also require one volume map per volume prop - so these are now a vector too
	std::map<int, std::vector<vtkSmartPointer<vtkGPUVolumeRayCastMapper>>> mVolumeMappers;
	vtkNew<vtkColorTransferFunction> mVolumeColor;
	vtkNew<vtkPiecewiseFunction> mVolumeOpacity;
	vtkNew<vtkVolumeProperty> mVolumeProperty;

	std::vector<TransferFunction> mTransferFunctions;

	// Each MPR plane requires its transform, and color as well as the image actor.
	// the latter seems to be required in order to get the image to update correctly
	std::map<int, vtkSmartPointer<vtkImageReslice> > mReslice; 
	std::map<int, vtkSmartPointer<vtkTransform> > mResliceTransforms;
	vtkNew<vtkLookupTable> mResliceLookupTable;
	std::map<int, vtkSmartPointer<vtkImageMapToColors> > mResliceColors;

	std::array<int, 6> mVolumeExtent;
	std::array<int, 3> mVolumeExtentMin;
	std::array<int, 3> mVolumeExtentMax;
	std::array<double, 3> mVolumeSpacingM;
	std::array<double, 3> mVolumeOrigin;
	std::array<double, 3> mVolumeCentre;
	std::array<double, 3> mVolumeOriginCentredM;
	std::array<double, 3> mVolumeCroppingPoint;

	std::map<int, vtkSmartPointer<vtkPlane> > mVolumeCropPlanes;

	bool mRenderScene;

	double mWindowWidth;
	double mWindowLevel;
	double mOpacityFactor;
	double mBrightnessFactor;
	int mTransferFunctionIndex;
};

