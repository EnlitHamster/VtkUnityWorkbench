using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Collections;
using System.Runtime.InteropServices;

using ThreeDeeHeartPlugins;

public class VtkCamera : MonoBehaviour
{
	Camera _camera;

	private CommandBuffer _vtkRenderCommandBuffer;
	private bool _rendering = false;

	void Start()
	{
        _camera = Camera.main;

		_vtkRenderCommandBuffer = new CommandBuffer();
		_vtkRenderCommandBuffer.name = "Do a vtk render";
		_vtkRenderCommandBuffer.IssuePluginEvent(VtkToUnityPlugin.GetRenderEventFunc(), 1);

		// As we have transluscent objects rendering after the forward alpha seems appropriate
		// maybe adding a
		_camera.AddCommandBuffer(CameraEvent.AfterForwardAlpha, _vtkRenderCommandBuffer);
		_rendering = true;
	}

	void OnPreRender() {
		
		if (!_rendering)
		{
			return;
		}
		
		// defualt to the mono rendering as we'll get a result 
		// - even if it is the wrong one (blearch if you're wearing a headset)
		Matrix4x4 viewMatrix = _camera.worldToCameraMatrix;
		Matrix4x4 projectionMatrix = _camera.projectionMatrix;

		if (_camera.stereoEnabled)
		{
			Camera.MonoOrStereoscopicEye activeEye = _camera.stereoActiveEye;

			if (!(Camera.MonoOrStereoscopicEye.Mono == activeEye))
			{
				Camera.StereoscopicEye stereoEye = Camera.StereoscopicEye.Left;

				if (Camera.MonoOrStereoscopicEye.Right == activeEye) {
					stereoEye = Camera.StereoscopicEye.Right;
				}
				
				viewMatrix = _camera.GetStereoViewMatrix(stereoEye);
				projectionMatrix = _camera.GetStereoProjectionMatrix(stereoEye);
			} 
		}

		VtkToUnityPlugin.SetViewMatrix(VtkToUnityPlugin.UnityMatrix4x4ToFloat16ColMajor(viewMatrix));
		VtkToUnityPlugin.SetProjectionMatrix(VtkToUnityPlugin.UnityMatrix4x4ToFloat16ColMajor(projectionMatrix));
	}

	public void SuspendRendering()
	{
		_camera.RemoveCommandBuffer(CameraEvent.AfterForwardAlpha, _vtkRenderCommandBuffer);
		_rendering = false;
	}

	public void ResumeRendering()
	{
		SuspendRendering();
		_camera.AddCommandBuffer(CameraEvent.AfterForwardAlpha, _vtkRenderCommandBuffer);
		_rendering = true;
	}
}
