#include "VtkToUnityAPIFactory.h"
#include "../PlatformBase.h"
#include "../Unity/IUnityGraphics.h"

#include "../VtkToUnityAPI_OpenGLCoreES.h"

//static std::shared_ptr<VtkToUnityAPI> mInstance;
static std::shared_ptr<VtkToUnityAPI_OpenGLCoreES> mInstance;

void VtkToUnityAPIFactory::Initialise(UnityGfxRenderer apiType)
{
#	if SUPPORT_OPENGL_UNIFIED
	if (apiType == kUnityGfxRendererOpenGLCore || 
		apiType == kUnityGfxRendererOpenGLES20 || 
		apiType == kUnityGfxRendererOpenGLES30)
	{
		//extern VtkToUnityAPI* CreateVtkToUnityAPI_OpenGLCoreES(UnityGfxRenderer apiType);
		//mInstance = std::shared_ptr<VtkToUnityAPI_OpenGLCoreES>(CreateVtkToUnityAPI_OpenGLCoreES(apiType));
		mInstance = std::shared_ptr<VtkToUnityAPI_OpenGLCoreES>(new VtkToUnityAPI_OpenGLCoreES(apiType));
	}
#	endif // if SUPPORT_OPENGL_UNIFIED

}

void VtkToUnityAPIFactory::Shutdown()
{
	mInstance = nullptr;
}

bool VtkToUnityAPIFactory::Initialised()
{
	return mInstance != nullptr;
}

std::weak_ptr<VtkToUnityAPI> VtkToUnityAPIFactory::GetAPI()
{
	//assert(Initialised());
	return std::weak_ptr<VtkToUnityAPI>(mInstance);
}

