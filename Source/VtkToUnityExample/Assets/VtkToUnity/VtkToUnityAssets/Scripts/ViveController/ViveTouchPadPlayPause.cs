using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

using ThreeDeeHeartPlugins;

public class ViveTouchPadPlayPause : MonoBehaviour {

	private VtkVolumeRenderLoadControl _volumeRenderAnimation;

    public GameObject ToggleButton;

    public SteamVR_Action_Boolean AnimationPlayPauseBoolean =
        SteamVR_Input.GetAction<SteamVR_Action_Boolean>("AnimationPlayPause");
    public SteamVR_Input_Sources InputSource = SteamVR_Input_Sources.Any;

	// Use this for initialization
	IEnumerator Start ()
	{
		var sceneObject = GameObject.Find("Scene");

		if (null == sceneObject)
		{
			return null;
		}

		_volumeRenderAnimation = sceneObject.GetComponentInChildren<VtkVolumeRenderLoadControl>();

		return null;
	}

    void OnEnable()
    {
        if (null != AnimationPlayPauseBoolean)
        {
            AnimationPlayPauseBoolean.AddOnStateDownListener(OnAnimationPlayPausePressed, InputSource);
        }
    }

    void OnDisable()
    {
        if (null != AnimationPlayPauseBoolean)
        {
            AnimationPlayPauseBoolean.RemoveOnStateDownListener(OnAnimationPlayPausePressed, InputSource);
        }
    }

    private void OnAnimationPlayPausePressed(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) 
    {
        if (ToggleButton && ToggleButton.GetComponent<Toggle>())
        {
            ToggleButton.GetComponent<Toggle>().isOn = !ToggleButton.GetComponent<Toggle>().isOn;
        }
    }
}
