#pragma once

#include "VtkUnityWorkbenchAPI.h"
#include "PlatformBase.h"

#include <vtkNew.h>

// Renderer Class Declaraion ======================================================================

class VtkUnityWorkbenchAPI_OpenGLCoreES : public VtkUnityWorkbenchAPI
{
public:
	VtkUnityWorkbenchAPI_OpenGLCoreES(UnityGfxRenderer apiType);
	virtual ~VtkUnityWorkbenchAPI_OpenGLCoreES() { }

	virtual void ProcessDeviceEvent(UnityGfxDeviceEventType type, IUnityInterfaces* interfaces);

	virtual void SetDebugLogFunction(DebugLogFunc func);

	virtual bool GetUsesReverseZ() { return false; }

protected:
	UnityGfxRenderer m_APIType;

};