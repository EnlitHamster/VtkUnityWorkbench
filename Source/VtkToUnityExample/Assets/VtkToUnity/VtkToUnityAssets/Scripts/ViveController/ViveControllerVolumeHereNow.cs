using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ViveControllerVolumeHereNow : MonoBehaviour
{
	private GameObject _targetObject;
    public SteamVR_Action_Boolean VolumeHereNow =
        SteamVR_Input.GetAction<SteamVR_Action_Boolean>("VolumeHereNow");
    public SteamVR_Input_Sources InputSource = SteamVR_Input_Sources.Any;

    void Awake()
	{
		var foundTargets = FindObjectsOfType<ApplyDataStoreLocation>();

		if (null != foundTargets && 0 != foundTargets.Length)
	{
			_targetObject = foundTargets[0].gameObject;
		}
	}

    void OnEnable()
	{
        if (null != VolumeHereNow)
        {
            VolumeHereNow.AddOnStateDownListener(OnVolumeHereNowPressed, InputSource);
        }
	}

    void OnDisable()
	{
        if (null != VolumeHereNow)
        {
            VolumeHereNow.RemoveOnStateDownListener(OnVolumeHereNowPressed, InputSource);
	}
    }

    private void OnVolumeHereNowPressed(
        SteamVR_Action_Boolean fromAction, 
        SteamVR_Input_Sources fromSource)
		{
        DataStore.Instance.StorePositionRotation(transform.gameObject);
        DataStore.Instance.ApplyPositonRotationY(_targetObject);
	}
}
