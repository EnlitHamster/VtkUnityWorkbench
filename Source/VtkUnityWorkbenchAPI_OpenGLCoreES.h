#pragma once

#include "PlatformBase.h"
#include "VtkUnityWorkbenchAPI.h"
#include "vtkExternalOpenGLRenderer3dh.h"

#include <vtkConeSource.h>
#include <vtkNew.h>
#include <vtkActor.h>
#include <vtkNamedColors.h>
#include <vtkPolyDataMapper.h>
#include <ExternalVTKWidget.h>

// Renderer Class Declaraion ======================================================================

class VtkUnityWorkbenchAPI_OpenGLCoreES : public VtkUnityWorkbenchAPI
{
public:
	VtkUnityWorkbenchAPI_OpenGLCoreES(UnityGfxRenderer apiType);
	virtual ~VtkUnityWorkbenchAPI_OpenGLCoreES() { }

	virtual void ProcessDeviceEvent(UnityGfxDeviceEventType type, IUnityInterfaces* interfaces);

	virtual void SetDebugLogFunction(DebugLogFunc func);

	virtual bool GetUsesReverseZ() { return false; }

	//virtual void SetTargetFrameRateOn(const bool targetOn);
	virtual void SetTargetFrameRateFps(const int targetFps);

	virtual void VtkUnityWorkbenchAPI_OpenGLCoreES::UpdateVtkCameraAndRender(const std::array<double, 16>& viewMatrix, const std::array<double, 16>& projectionMatrix);

protected:
	UnityGfxRenderer m_APIType;
	std::shared_ptr<DebugLogFunc> m_DebugLog;

	vtkNew<ExternalVTKWidget> m_ExternalVTKWidget;
	vtkSmartPointer<vtkExternalOpenGLRenderWindow> m_RenderWindow;
	vtkNew<vtkExternalOpenGLRenderer3dh> m_Renderer;

	bool m_RenderScene;

	// Cone data ==================================================================================
	vtkNew<vtkNamedColors> m_Colors;
	vtkNew<vtkConeSource> m_ConeSource;
	vtkNew<vtkPolyDataMapper> m_Mapper;
	vtkNew<vtkActor> m_Actor;

	void CreateResources();

	void LogToDebugLog(const DebugLogLevel level, const std::string& message);
};