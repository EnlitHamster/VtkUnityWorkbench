#include "VtkUnityWorkbenchPlugin.h"
#include "VtkUnityWorkbenchInternalHelpers.h"

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
// Set the camera View matrix (column major array, Open GL style)
static SafeQueue<std::array<double, 16> > s_ViewMatrixColMajor;

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API SetViewMatrix(
	Float16 & view4x4ColMajor)
{
	auto sharedAPI = s_CurrentAPI.lock();
	if (!sharedAPI) {
		return;
	}

	std::array<double, 16> matrixColMajor;

	for (unsigned int i = 0u; i < 16; ++i) {
		matrixColMajor[i] = static_cast<double>(view4x4ColMajor.elements[i]);
	}

	s_ViewMatrixColMajor.enqueue(matrixColMajor);
}

// Set the camera Projection matrix (column major array, Open GL style)
static SafeQueue<std::array<double, 16> > s_ProjectionMatrixColMajor;

extern "C" void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API SetProjectionMatrix(
	Float16 & projection4x4ColMajor)
{
	auto sharedAPI = s_CurrentAPI.lock();
	if (!sharedAPI) {
		return;
	}

	std::array<double, 16> matrixColMajor;

	for (unsigned int i = 0u; i < 16; ++i) {
		matrixColMajor[i] = static_cast<double>(projection4x4ColMajor.elements[i]);
	}

	s_ProjectionMatrixColMajor.enqueue(matrixColMajor);
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
		sharedAPI->UpdateVtkCameraAndRender(s_ViewMatrixColMajor.dequeue(), s_ProjectionMatrixColMajor.dequeue());
	}
}



// --------------------------------------------------------------------------
// GetRenderEventFunc, an example function we export which is used to get a rendering event callback function.

extern "C" UnityRenderingEvent UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API GetRenderEventFunc()
{
	return OnRenderEvent;
}
