#include "VtkUnityWorkbenchAPI_OpenGLCoreES.h"

#include "VtkUnityWorkbenchAPI.h"
#include "PlatformBase.h"

#include <vtkPolyData.h>
#include <vtkProperty.h>
#include <vtkRenderWindow.h>
#include <vtkRenderWindowInteractor.h>
#include <vtkRenderer.h>

#define _USE_MATH_DEFINES
#include <math.h>

#include <thread>

#if SUPPORT_OPENGL_UNIFIED

#include <assert.h>
#if UNITY_IPHONE
#	include <OpenGLES/ES2/gl.h>
#elif UNITY_ANDROID || UNITY_WEBGL
#	include <GLES2/gl2.h>
#else
#	include "GL/gl3w.h"
#endif

VtkUnityWorkbenchAPI_OpenGLCoreES::VtkUnityWorkbenchAPI_OpenGLCoreES(UnityGfxRenderer apiType) : m_APIType(apiType)
{
}

void VtkUnityWorkbenchAPI_OpenGLCoreES::ProcessDeviceEvent(UnityGfxDeviceEventType type, IUnityInterfaces* interfaces)
{
	if (type == kUnityGfxDeviceEventInitialize)
	{
		CreateResources();
	}
	else if (type == kUnityGfxDeviceEventShutdown)
	{
		//@TODO: release resources
	}
}

void VtkUnityWorkbenchAPI_OpenGLCoreES::SetDebugLogFunction(DebugLogFunc func)
{
	m_DebugLog = std::make_shared<DebugLogFunc>(func);
}

//void VtkUnityWorkbenchAPI_OpenGLCoreES::SetTargetFrameRateOn(const bool targetOn)
//{
//	for (auto& volumeMapperPair : m_VolumeMappers)
//	{
//		auto volumeMappersVector = volumeMapperPair.second;
//
//		for (auto volumeMapper : volumeMappersVector)
//		{
//			if (volumeMapper)
//			{
//				volumeMapper->SetAutoAdjustSampleDistances(targetOn);
//			}
//		}
//	}
//
//}

void VtkUnityWorkbenchAPI_OpenGLCoreES::CreateResources()
{
	m_RenderWindow = vtkSmartPointer<vtkExternalOpenGLRenderWindow>::New();
	m_ExternalVTKWidget->SetRenderWindow(m_RenderWindow);
	m_ExternalVTKWidget->GetRenderWindow()->AddRenderer(m_Renderer.GetPointer());
	m_RenderWindow->AddRenderer(m_Renderer);

	// Creating cone ----------------------------
	m_ConeSource->Update();
	m_Mapper->SetInputConnection(m_ConeSource->GetOutputPort());
	m_Actor->SetMapper(m_Mapper);
	m_Actor->GetProperty()->SetDiffuseColor(m_Colors->GetColor3d("bisque").GetData());

	m_Renderer->AddActor(m_Actor);
	m_Renderer->SetBackground(m_Colors->GetColor3d("Salmon").GetData());
	m_Renderer->ResetCamera();
	m_Renderer->SetLightFollowCamera(false);

	m_RenderScene = true;
}

void VtkUnityWorkbenchAPI_OpenGLCoreES::SetTargetFrameRateFps(const int targetFps)
{
	m_RenderWindow->SetDesiredUpdateRate(targetFps);
}

void VtkUnityWorkbenchAPI_OpenGLCoreES::LogToDebugLog(const DebugLogLevel level, const std::string& message)
{
	if (nullptr == m_DebugLog)
	{
		return;
	}

	(*m_DebugLog)(level, message);
}

void VtkUnityWorkbenchAPI_OpenGLCoreES::UpdateVtkCameraAndRender(const std::array<double, 16>& viewMatrix, const std::array<double, 16>& projectionMatrix)
{
	m_Renderer->SetViewMatrix(viewMatrix);
	m_Renderer->SetProjectionMatrix(projectionMatrix);

	m_Renderer->ResetCameraClippingRange();

	if (m_RenderScene)
	{
		m_ExternalVTKWidget->GetRenderWindow()->Render();
	}
}

#endif // #if SUPPORT_OPENGL_UNIFIED