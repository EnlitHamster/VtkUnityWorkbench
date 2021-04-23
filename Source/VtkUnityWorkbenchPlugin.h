#pragma once

#include "PlatformBase.h"
#include "VtkUnityWorkbenchApi.h"

#include <memory>

#define PLUGINFUN_EX(rtype) extern "C" UNITY_INTERFACE_EXPORT rtype UNITY_INTERFACE_API

// --------------------------------------------------------------------------
// Connect to the debugging in Unity

typedef void(*DebugFuncPtr)(int, const char*);

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API SetDebugFunction(DebugFuncPtr fp);

class VtkUnityWorkbenchPlugin
{
public:
	static void SetVtkUnityWorkbenchAPI(std::weak_ptr<VtkUnityWorkbenchAPI> api);
	static bool GotVtkUnityWorkbenchAPI();
	static void ProcessDeviceEventXYZ(UnityGfxDeviceEventType type, IUnityInterfaces* interfaces);

	// Update all of the cached data
	static void UpdateCachedData();

	// Actually do the rendering
	static void DoRender();
};

// --------------------------------------------------------------------------
// OnRenderEvent
// This will be called for GL.IssuePluginEvent script calls; eventID will
// be the integer passed to IssuePluginEvent. In this example, we just ignore
// that value.
static void UNITY_INTERFACE_API OnRenderEvent(int eventID);

// --------------------------------------------------------------------------
// GetRenderEventFunc, an example function we export which is used to get a rendering event callback function.

extern "C" UnityRenderingEvent UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API GetRenderEventFunc();
