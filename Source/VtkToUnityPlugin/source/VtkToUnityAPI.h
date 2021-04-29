#pragma once

#include "Unity/IUnityGraphics.h"

#include "VtkToUnityAPIDefines.h"

#include <stddef.h>
#include <array>
#include <functional>
#include <string>
#include <vector>

struct IUnityInterfaces;

// Super-simple "graphics abstraction". This is nothing like how a proper platform abstraction layer would look like;
// all this does is a base interface for whatever our plugin sample needs. Which is only "draw some triangles"
// and "modify a texture" at this point.
//
// There are implementations of this base class for D3D9, D3D11, OpenGL etc.; see individual VtkToUnityAPI_* files.
class VtkToUnityAPI
{
public:
	virtual ~VtkToUnityAPI() { }

	// Process general event like initialization, shutdown, device loss/reset etc.
	virtual void ProcessDeviceEvent(UnityGfxDeviceEventType type, IUnityInterfaces* interfaces) = 0;

	typedef std::function<void(DebugLogLevel, std::string)> DebugLogFunc;
	virtual void SetDebugLogFunction(DebugLogFunc func) = 0;

	// Is the API using "reversed" (1.0 at near plane, 0.0 at far plane) depth buffer?
	// Reversed Z is used on modern platforms, and improves depth buffer precision.
	virtual bool GetUsesReverseZ() = 0;

	virtual bool LoadDicomVolumeFromFolder(const std::string &folder) = 0;
	virtual bool LoadUncMetaImage(const std::string &mhdPath) = 0;
	virtual bool LoadNrrdImage(const std::string &nrrdPath) = 0;

	virtual bool CreatePaddingMask(int paddingValue) = 0;

	virtual void ClearVolumes() = 0;
	virtual int GetNVolumes() = 0;

	virtual Float4 GetVolumeSpacingM() = 0;
	virtual Float4 GetVolumeExtentsMin() = 0;
	virtual Float4 GetVolumeExtentsMax() = 0;
	virtual Float4 GetVolumeOriginM() = 0;

	virtual void SetVolumeIndex(
		const int index) = 0;

	virtual int AddVolumeProp() = 0;

	virtual int AddCropPlaneToVolume(const int volumeId) = 0;

	virtual int GetNTransferFunctions() = 0;
	virtual int GetTransferFunctionIndex() = 0;
	virtual void SetTransferFunctionIndex(const int index) = 0;
	virtual int AddTransferFunction() = 0;
	virtual int ResetTransferFunctions() = 0;
	virtual void SetTransferFunctionPoint(
		const int transferFunctionIndex,
		const double windowFraction,
		const double red1,
		const double green1,
		const double blue1,
		const double opacity1) = 0;
	virtual void SetVolumeWWWL(const double windowWidth, const double windowLevel) = 0;
	virtual void SetVolumeOpactityFactor(const double opacityFactor) = 0;
	virtual void SetVolumeBrightnessFactor(const double brightnessFactor) = 0;

	virtual void SetRenderComposite(const bool composite) = 0;

	virtual void SetTargetFrameRateOn(const bool targetOn) = 0;
	virtual void SetTargetFrameRateFps(const int targetFps) = 0;

	virtual int AddMPR(const int existingMprId, const int flipAxis) = 0;
	virtual void SetMPRWWWL(const double windowWidth, const double windowLevel) = 0;

	/////////////////////////////////////////////
	// Primitive controllers

	virtual int AddShapePrimitive(
		const int shapeType,
		const Float4 &rgbaColour,
		const bool wireframe) = 0;

	virtual void GetShapePrimitiveProperty(
		const int shapeId,
		LPCSTR propertyName,
		char* retValue) = 0;

	virtual void SetShapePrimitiveProperty(
		const int shapeId,
		LPCSTR propertyName,
		LPCSTR retValue) = 0;


	virtual int AddLight() = 0;

	virtual void SetLightingOn(
		bool lightingOn) = 0;

	virtual void SetLightColor(
		int id,
		LightColorType lightColorType,
		Float4 &rgbColor) = 0;

	virtual void SetLightIntensity(
		int id,
		float intensity) = 0;

	virtual void SetVolumeLighting(
		VolumeLightType volumeLightType,
		float lightValue) = 0;

	virtual void RemoveProp3D(
		int id) = 0;

	virtual void SetProp3DTransform(
		int id,
		Float16 transform) = 0;

	virtual void SetMPRTransform(
		const int id,
		Float16 transformVolume) = 0;

	virtual void UpdateVtkCameraAndRender(
		const std::array<double, 16> &viewMatrix,
		const std::array<double, 16> &projectionMatrix) = 0;
};


