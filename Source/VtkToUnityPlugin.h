#pragma once
// Example low level rendering Unity plugin

#include "PlatformBase.h"
#include "VtkToUnityAPI.h"

#include <memory>

#define PLUGINEX(rtype) extern "C" UNITY_INTERFACE_EXPORT rtype UNITY_INTERFACE_API

// --------------------------------------------------------------------------
// Connect to the debugging in unity

typedef void(*DebugFuncPtr)(int, const char*);

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API SetDebugFunction(DebugFuncPtr fp);

class VtkToUnityPlugin
{
public:
	static void SetVtkToUnityAPI(std::weak_ptr<VtkToUnityAPI> api);
	static bool GotVtkToUnityAPI();
	static void ProcessDeviceEventXYZ(UnityGfxDeviceEventType type, IUnityInterfaces* interfaces);

	// update all of the cached data
	static void UpdateCachedData();

	// actually do the render
	static void DoRender();
};

// --------------------------------------------------------------------------
// Volume loading and display methods

PLUGINEX(bool) LoadDicomVolume(const char *dicomFolder);
PLUGINEX(bool) LoadMhdVolume(const char *mhdPath);
PLUGINEX(bool) LoadNrrdVolume(const char *nrrdPath);

PLUGINEX(bool) CreatePaddingMask(int paddingValue);

PLUGINEX(void) ClearVolumes();
PLUGINEX(int) GetNVolumes();

PLUGINEX(Float4) GetVolumeSpacingM();
PLUGINEX(Float4) GetVolumeExtentsMin();
PLUGINEX(Float4) GetVolumeExtentsMax();
PLUGINEX(Float4) GetVolumeOriginM();

PLUGINEX(int) AddVolumeProp();

PLUGINEX(int) AddCropPlaneToVolume(int volumeId);

PLUGINEX(void) SetVolumeIndex(int index);

PLUGINEX(int) GetNTransferFunctions();
PLUGINEX(int) GetTransferFunctionIndex();
PLUGINEX(void) SetTransferFunctionIndex(int index);
PLUGINEX(int) AddTransferFunction();
PLUGINEX(int) ResetTransferFunctions();
PLUGINEX(void) SetTransferFunctionPoint(
	int transferFunctionIndex,
	double windowFraction,
	double red1,
	double green1,
	double blue1,
	double opacity1);

PLUGINEX(void) SetVolumeWWWL(float windowWidth, float windowLevel);
PLUGINEX(void) SetVolumeOpacityFactor(float opacityFactor);
PLUGINEX(void) SetVolumeBrightnessFactor(float brightnessFactor);

PLUGINEX(void) SetRenderComposite(bool composite);

PLUGINEX(void) SetTargetFrameRateOn(bool targetOn);
PLUGINEX(void) SetTargetFrameRateFps(int targetFps);

// --------------------------------------------------------------------------
// Shape primitive methods

PLUGINEX(int) AddMPR(int existingMprId);
PLUGINEX(int) AddMPRFlipped(int existingMprId, int flipAxis);

PLUGINEX(void) SetMPRWWWL(float windowWidth, float windowLevel);

// Add a primitive shape to the scene, returns the shape ID
PLUGINEX(int) AddShapePrimitive(
	int shapeType,
	Float4 &rgbaColour,
	bool wireframe);

PLUGINEX(void*) GetShapePrimitiveProperty(
	int shapeId,
	std::string propertyName);

// --------------------------------------------------------------------------
// General lighting methods

// Add a scene light to the scene, return it's handle
PLUGINEX(int) AddLight();

// Turn lighting on or off 
// Won't do anything unless a light has first been added to the scene
PLUGINEX(void) SetLightingOn(
	bool lightingOn);

// Set light color 
PLUGINEX(void) SetLightColor(
	int id,
	LightColorType lightColorType,
	Float4 &rgbColor);

// Set light intensity 
PLUGINEX(void) SetLightIntensity(
	int id,
	float intensity);

// --------------------------------------------------------------------------
// Volume lighting methods

// Set volume rendering lighting properties
PLUGINEX(void) SetVolumeLighting(
	VolumeLightType volumeLightType,
	float lightValue);

// --------------------------------------------------------------------------
// General prop methods

// remove entity with the given id
PLUGINEX(void) RemoveProp3D(
	int id);

PLUGINEX(void) SetProp3DTransform(
	int id,
	Float16 &transformWorldM);

PLUGINEX(void) SetMPRTransform(
	int id,
	Float16 &transformVolumeM);

// --------------------------------------------------------------------------
// Set the camera View matrix (column major array, Open GL style)
extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API SetViewMatrix(
	Float16 &view4x4ColMajor);

// Set the camera Projection matrix (column major array, Open GL style)
extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API SetProjectionMatrix(
	Float16 &projection4x4ColMajor);

// --------------------------------------------------------------------------
// OnRenderEvent
// This will be called for GL.IssuePluginEvent script calls; eventID will
// be the integer passed to IssuePluginEvent. In this example, we just ignore
// that value.
static void UNITY_INTERFACE_API OnRenderEvent(int eventID);

// --------------------------------------------------------------------------
// GetRenderEventFunc, an example function we export which is used to get a rendering event callback function.

extern "C" UnityRenderingEvent UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API GetRenderEventFunc();

