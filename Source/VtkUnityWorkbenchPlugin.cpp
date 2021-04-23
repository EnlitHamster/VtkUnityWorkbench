#pragma once

#include "VtkUnityWorkbenchPlugin.h"

static std::weak_ptr<VtkUnityWorkbenchAPI> s_CurrentAPI;

// --------------------------------------------------------------------------
// Connect to the debugging in unity

static DebugFuncPtr s_DebugFp = nullptr;

static void Debug(DebugLogLevel level, const std::string& message)
{
	if (nullptr == s_DebugFp)
	{
		return;
	}

	s_DebugFp(static_cast<int>(level), message.c_str());
}

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API SetDebugFunction(DebugFuncPtr fp)
{
	s_DebugFp = fp;

	if (auto sharedAPI = s_CurrentAPI.lock()) {
		sharedAPI->SetDebugLogFunction(Debug);
	}
}

// --------------------------------------------------------------------------
// GraphicsDeviceEvent

void VtkUnityWorkbenchPlugin::SetVtkUnityWorkbenchAPI(std::weak_ptr<VtkUnityWorkbenchAPI> api)
{
	s_CurrentAPI = api;
}

bool VtkUnityWorkbenchPlugin::GotVtkUnityWorkbenchAPI()
{
	return (!s_CurrentAPI.expired());
}

void VtkUnityWorkbenchPlugin::ProcessDeviceEventXYZ(UnityGfxDeviceEventType type, IUnityInterfaces* interfaces)
{
	if (auto sharedAPI = s_CurrentAPI.lock()) 
	{
		sharedAPI->ProcessDeviceEvent(type, interfaces);
	}
}

// --------------------------------------------------------------------------
// OnRenderEvent
// This will be called for GL.IssuePluginEvent script calls; eventID will
// be the integer passed to IssuePluginEvent. In this example, we just ignore
// that value.
static void UNITY_INTERFACE_API OnRenderEvent(int eventID)
{
	VtkUnityWorkbenchPlugin::UpdateCachedData();
	VtkUnityWorkbenchPlugin::DoRender();
}

// update all of the cached data
void VtkUnityWorkbenchPlugin::UpdateCachedData()
{
	// Unknown / unsupported graphics device type? Do nothing
	auto sharedAPI = s_CurrentAPI.lock();
	if (!sharedAPI) 
	{
		return;
	}
}

// actually do the render
void VtkUnityWorkbenchPlugin::DoRender()
{
	// Unknown / unsupported graphics device type? Do nothing
	if (auto sharedAPI = s_CurrentAPI.lock()) 
	{
	}
}


// --------------------------------------------------------------------------
// GetRenderEventFunc, an example function we export which is used to get a rendering event callback function.

extern "C" UnityRenderingEvent UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API GetRenderEventFunc()
{
	return OnRenderEvent;
}
