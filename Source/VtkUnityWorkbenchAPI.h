#pragma once

#include "Unity/IUnityGraphics.h"

#include "VtkUnityWorkbenchAPIDefines.h"

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
// There are implementations of this base class for D3D9, D3D11, OpenGL etc.; see individual RenderAPI_* files.
class VtkUnityWorkbenchAPI
{
public:
	virtual ~VtkUnityWorkbenchAPI() { }


	// Process general event like initialization, shutdown, device loss/reset etc.
	virtual void ProcessDeviceEvent(UnityGfxDeviceEventType type, IUnityInterfaces* interfaces) = 0;

	typedef std::function<void(DebugLogLevel, std::string)> DebugLogFunc;
	virtual void SetDebugLogFunction(DebugLogFunc func) = 0;
};

