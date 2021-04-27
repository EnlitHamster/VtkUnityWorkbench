#include "VtkUnityWorkbenchAPIFactory.h"
#include "../PlatformBase.h"
#include "../Unity/IUnityGraphics.h"

#include "../VtkUnityWorkbenchAPI_OpenGLCoreES.h"

//static std::shared_ptr<VtkUnityWorkbenchAPI> mInstance;
static std::shared_ptr<VtkUnityWorkbenchAPI_OpenGLCoreES> mInstance;

void VtkUnityWorkbenchAPIFactory::Initialise(UnityGfxRenderer apiType)
{
#	if SUPPORT_OPENGL_UNIFIED
	if (apiType == kUnityGfxRendererOpenGLCore || 
		apiType == kUnityGfxRendererOpenGLES20 || 
		apiType == kUnityGfxRendererOpenGLES30)
	{
		//extern VtkUnityWorkbenchAPI* CreateVtkUnityWorkbenchAPI_OpenGLCoreES(UnityGfxRenderer apiType);
		//mInstance = std::shared_ptr<VtkUnityWorkbenchAPI_OpenGLCoreES>(CreateVtkUnityWorkbenchAPI_OpenGLCoreES(apiType));
		mInstance = std::shared_ptr<VtkUnityWorkbenchAPI_OpenGLCoreES>(new VtkUnityWorkbenchAPI_OpenGLCoreES(apiType));
	}
#	endif // if SUPPORT_OPENGL_UNIFIED

}

void VtkUnityWorkbenchAPIFactory::Shutdown()
{
	mInstance = nullptr;
}

bool VtkUnityWorkbenchAPIFactory::Initialised()
{
	return mInstance != nullptr;
}

std::weak_ptr<VtkUnityWorkbenchAPI> VtkUnityWorkbenchAPIFactory::GetAPI()
{
	//assert(Initialised());
	return std::weak_ptr<VtkUnityWorkbenchAPI>(mInstance);
}

