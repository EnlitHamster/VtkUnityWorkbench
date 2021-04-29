using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

using ThreeDeeHeartPlugins;

public class VtkVolumeCropPlane : MonoBehaviour
{
    //GameObject volumeProxy;
	private VtkVolumeRenderCore _volumeScript;
	private int _volumeCropPlaneId = -1;

	public int CropPlaneID
	{
		get
		{
			return _volumeCropPlaneId;
		}
	}

	public GameObject VolumeProxy
	{
		get
		{
			return _volumeScript.gameObject;
		}
	}

	IEnumerator Start()
    {
        _volumeScript = GetComponentInParent<VtkVolumeRenderCore>();

        if (_volumeScript)
        {
            int volumePropId = _volumeScript.VolumePropId;
            _volumeCropPlaneId = VtkToUnityPlugin.AddCropPlaneToVolume(volumePropId);
        }

        yield return StartCoroutine("CallPluginAtEndOfFrames");
    }


    void OnDestroy()
    {
        VtkToUnityPlugin.RemoveProp3D(_volumeCropPlaneId);
    }

    public void UnloadCropPlane()
    {
        VtkToUnityPlugin.RemoveProp3D(_volumeCropPlaneId);
		_volumeCropPlaneId = -1;
	}

    public void LoadCropPlane()
    {
        if (_volumeScript)
        {
            int volumePropId = _volumeScript.VolumePropId;
            _volumeCropPlaneId = VtkToUnityPlugin.AddCropPlaneToVolume(volumePropId);
        }
    }

    private IEnumerator CallPluginAtEndOfFrames()
    {
        while (true)
        {
            // Wait until all frame rendering is done
            yield return new WaitForEndOfFrame();

            if (_volumeCropPlaneId > -1)
            {
                Matrix4x4 unityMatrix = transform.localToWorldMatrix;
                VtkToUnityPlugin.Float16 pluginMatrix = VtkToUnityPlugin.UnityMatrix4x4ToFloat16(unityMatrix);
                VtkToUnityPlugin.SetProp3DTransform(_volumeCropPlaneId, pluginMatrix);
            }
        }
    }
}