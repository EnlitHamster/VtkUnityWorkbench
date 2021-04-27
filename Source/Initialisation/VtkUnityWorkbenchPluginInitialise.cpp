// Example low level rendering Unity plugin
#include "VtkUnityWorkbenchPluginInitialise.h"

#include "../VtkUnityWorkbenchPlugin.h"

#include "VtkUnityWorkbenchAPIFactory.h"

#include <assert.h>
#include <math.h>
#include <map>
#include <queue>
#include <vector>
#include <sstream>

#include <queue>
#include <mutex>
#include <condition_variable>



// --------------------------------------------------------------------------
// UnitySetInterfaces

static void UNITY_INTERFACE_API OnGraphicsDeviceEvent(UnityGfxDeviceEventType eventType);

static IUnityInterfaces* sUnityInterfaces = NULL;
static IUnityGraphics* sGraphics = NULL;

extern "C" void	UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityPluginLoad(IUnityInterfaces* unityInterfaces)
{
	sUnityInterfaces = unityInterfaces;
	sGraphics = sUnityInterfaces->Get<IUnityGraphics>();
	sGraphics->RegisterDeviceEventCallback(OnGraphicsDeviceEvent);
	
	// Run OnGraphicsDeviceEvent(initialize) manually on plugin load
	OnGraphicsDeviceEvent(kUnityGfxDeviceEventInitialize);
}

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityPluginUnload()
{
	sGraphics->UnregisterDeviceEventCallback(OnGraphicsDeviceEvent);
}

#if UNITY_WEBGL
typedef void	(UNITY_INTERFACE_API * PluginLoadFunc)(IUnityInterfaces* unityInterfaces);
typedef void	(UNITY_INTERFACE_API * PluginUnloadFunc)();

extern "C" void	UnityRegisterVtkUnityWorkbenchPlugin(PluginLoadFunc loadPlugin, PluginUnloadFunc unloadPlugin);

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API RegisterPlugin()
{
	UnityRegisterVtkUnityWorkbenchPlugin(UnityPluginLoad, UnityPluginUnload);
}
#endif

// --------------------------------------------------------------------------
// GraphicsDeviceEvent

static UnityGfxRenderer sDeviceType = kUnityGfxRendererNull;

static void UNITY_INTERFACE_API OnGraphicsDeviceEvent(UnityGfxDeviceEventType eventType)
{
	// Create graphics API implementation upon initialization
	if (eventType == kUnityGfxDeviceEventInitialize)
	{
		//assert(sCurrentAPI == NULL);
		//assert(sCurrentAPI.expired());
		assert(!VtkUnityWorkbenchPlugin::GotVtkUnityWorkbenchAPI());
		//assert(VtkUnityWorkbenchAPIFactory::Initialised());
		sDeviceType = sGraphics->GetRenderer();
		//sCurrentAPI = CreateVtkUnityWorkbenchAPI(sDeviceType);
		VtkUnityWorkbenchAPIFactory::Initialise(sDeviceType);
		VtkUnityWorkbenchPlugin::SetVtkUnityWorkbenchAPI(VtkUnityWorkbenchAPIFactory::GetAPI());
	}

	// Let the implementation process the device related events
	//if (VtkUnityWorkbenchAPIFactory::Initialised())
	//if (auto sharedAPI = sCurrentAPI.lock())
	//{
	//	sharedAPI->ProcessDeviceEvent(eventType, sUnityInterfaces);
	//}

	VtkUnityWorkbenchPlugin::ProcessDeviceEventXYZ(eventType, sUnityInterfaces);

	// Cleanup graphics API implementation upon shutdown
	if (eventType == kUnityGfxDeviceEventShutdown)
	{
		VtkUnityWorkbenchAPIFactory::Shutdown();
		sDeviceType = kUnityGfxRendererNull;
	}
}


