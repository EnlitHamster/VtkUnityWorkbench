using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThreeDeeHeartPlugins;

public class VtkMprRender : MonoBehaviour {
    public GameObject CropPlane;
    public int FlipAxis = -1;

    protected int _volumeMPRPropId = -1;
    public int VolumeMPRPropId
    {
        get
        {
            return _volumeMPRPropId;
        }
    }
    protected int _frontMprId = -1;
    protected int _volumeCropPlaneId = -1;
    protected GameObject _volumeProxy;

    protected Vector3 _initNormal;
    protected Quaternion _initCropRotation;

    protected Matrix4x4 _oldUnityMatrix;

    // Use this for initialization
    protected IEnumerator Start ()
    {
		_volumeProxy = CropPlane.GetComponent<VtkVolumeCropPlane>().VolumeProxy;
        LoadMprPlane();

        yield return StartCoroutine("CallPluginAtEndOfFrames");
    }

    void OnDestroy()
    {
        VtkToUnityPlugin.RemoveProp3D(_volumeMPRPropId);
    }

    public void UnloadMprPlane()
    {
        VtkToUnityPlugin.RemoveProp3D(_volumeMPRPropId);
		_volumeMPRPropId = -1;
	}

    public void LoadMprPlane()
    {
		if (_volumeProxy && CropPlane)
		{
			_initNormal = 
                _volumeProxy.transform.InverseTransformDirection(CropPlane.transform.up);
			_initCropRotation = 
                Quaternion.Inverse(_volumeProxy.transform.rotation) * 
                CropPlane.transform.rotation;

			_volumeCropPlaneId = CropPlane.GetComponent<VtkVolumeCropPlane>().CropPlaneID;
			_volumeMPRPropId = VtkToUnityPlugin.AddMPRFlipped(_frontMprId, FlipAxis);
			{
				var unityMatrix = Matrix4x4.TRS(
                    transform.position, 
                    transform.rotation, 
                    new Vector3(4.0f, 4.0f, 4.0f));
				var pluginMatrix = VtkToUnityPlugin.UnityMatrix4x4ToFloat16(unityMatrix);
				VtkToUnityPlugin.SetProp3DTransform(_volumeMPRPropId, pluginMatrix);
			}
		}
    }

    protected IEnumerator CallPluginAtEndOfFrames()
    {
        while (true)
        {
			// Wait until all frame rendering is done
			yield return new WaitForEndOfFrame();

			if (_volumeMPRPropId > -1)
            {
                CallPluginAtEndOfFramesImpl();
            }
		}
	}

    protected virtual void CallPluginAtEndOfFramesImpl()
    {
        // only update the mpr position if we are the front
        // i.e. the front mpr id is less than zero
        Vector3 unitScale = new Vector3(1.0f, 1.0f, 1.0f);

        Vector3 inversedPosition = _volumeProxy.transform.InverseTransformPoint(
            CropPlane.transform.position);
        Vector3 mprPosition = new Vector3(
            inversedPosition.x,
            inversedPosition.y,
            inversedPosition.z * 1.0f);

        // rotation of the normal of the crop plane
        Quaternion rot = Quaternion.FromToRotation(
            _initNormal,
            _volumeProxy.transform.InverseTransformDirection(
                CropPlane.transform.up));
        // apply rotation of the normal to the crop plane
        Quaternion mprRotation = rot * _initCropRotation;//inversedRotation;
        // convert from vtk coord sys to unity coord sys
        Quaternion rotatedRotation =
            mprRotation * Quaternion.Euler(90.0f, 0.0f, 0.0f);
        // if not ignoring rotation around normal then rotatedRotation should be
        //Quaternion.Inverse(_volumeProxy.transform.rotation) * CropPlane.transform.rotation * Quaternion.Euler(90, 0, 0)

        Matrix4x4 unityMatrix =
            //Matrix4x4.TRS(transform.localPosition, rotatedRotation, unitScale);
            Matrix4x4.TRS(mprPosition, rotatedRotation, unitScale);
        VtkToUnityPlugin.Float16 pluginMatrix =
            VtkToUnityPlugin.UnityMatrix4x4ToFloat16(unityMatrix);
        SetMPRTransform(pluginMatrix);

        _oldUnityMatrix = unityMatrix;

        UpdateMPRTransform();
    }

    protected void UpdateMPRTransform()
    {
        var _unityMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        var _pluginMatrix = VtkToUnityPlugin.UnityMatrix4x4ToFloat16(_unityMatrix);
        VtkToUnityPlugin.SetProp3DTransform(_volumeMPRPropId, _pluginMatrix);
    }

    protected virtual void SetMPRTransform(VtkToUnityPlugin.Float16 pluginMatrix)
    {
        VtkToUnityPlugin.SetMPRTransform(_volumeMPRPropId, pluginMatrix);
    }
}
