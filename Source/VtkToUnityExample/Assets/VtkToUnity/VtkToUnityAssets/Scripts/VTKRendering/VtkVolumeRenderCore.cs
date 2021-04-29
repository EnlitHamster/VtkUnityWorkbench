using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

using ThreeDeeHeartPlugins;

public class VtkVolumeRenderCore : MonoBehaviour
{
	protected int _volumePropId = -1;

	public int VolumePropId
    {
		get
		{
			return _volumePropId;
		}
    }

	IEnumerator Start()
	{
		return StartImpl();
	}

	protected virtual IEnumerator StartImpl()
	{
		_volumePropId = VtkToUnityPlugin.AddVolumeProp();

		yield return StartCoroutine("CallPluginAtEndOfFrames");
	}

	void OnDestroy ()
	{
		OnDestroyImpl();
	}

	protected virtual void OnDestroyImpl()
	{
		UnloadVolume();
	}

	public virtual void UnloadVolume()
    {
        VtkToUnityPlugin.RemoveProp3D(_volumePropId);
		_volumePropId = -1;
	}

	public virtual void AddVolumeProp()
	{
		if (_volumePropId >= 0)
		{
			return;
		}

		_volumePropId = VtkToUnityPlugin.AddVolumeProp();
	}

	private IEnumerator CallPluginAtEndOfFrames()
	{
		while (true) {
			// Wait until all frame rendering is done
			yield return new WaitForEndOfFrame();

			CallPluginAtEndOfFramesBody();
		}
	}

	protected virtual void CallPluginAtEndOfFramesBody()
	{
		Matrix4x4 unityMatrix = transform.localToWorldMatrix;
		VtkToUnityPlugin.Float16 pluginMatrix = VtkToUnityPlugin.UnityMatrix4x4ToFloat16(unityMatrix);
		VtkToUnityPlugin.SetProp3DTransform(_volumePropId, pluginMatrix);
	}
}
