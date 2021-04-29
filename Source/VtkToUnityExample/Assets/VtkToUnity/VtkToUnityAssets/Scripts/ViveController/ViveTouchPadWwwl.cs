using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

using ThreeDeeHeartPlugins;

public class ViveTouchPadWwwl : MonoBehaviour {

	private VtkVolumeRenderLoadControl _volumeRenderGain;

	[Range(25.0f, 100.0f)]
	public float TouchpadSensitivity = 50.0f;

	private bool _touchpadDown = false;

    public SteamVR_Action_Boolean VolumeGainContrastOn =
        SteamVR_Input.GetAction<SteamVR_Action_Boolean>("VolumeGainContrastOn");
    public SteamVR_Action_Vector2 VolumeGainContrastDelta =
        SteamVR_Input.GetAction<SteamVR_Action_Vector2>("VolumeGainContrastDelta");
    public SteamVR_Input_Sources InputSource = SteamVR_Input_Sources.Any;

	public static float Clamp( float value, float min, float max )
	{
		return (value < min) ? min : (value > max) ? max : value;
	}

	// Use this for initialization
	IEnumerator Start()
	{
		var sceneObject = GameObject.Find("Scene");

		if (null == sceneObject)
		{
			return null;
		}

		_volumeRenderGain = sceneObject.GetComponentInChildren<VtkVolumeRenderLoadControl>();

		return null;
	}

    void OnEnable()
	{
        if (null != VolumeGainContrastDelta && null != VolumeGainContrastOn)
        {
            VolumeGainContrastOn.AddOnStateDownListener(OnVolumeGainContrastPressed, InputSource);
            VolumeGainContrastOn.AddOnStateUpListener(OnVolumeGainContrastReleased, InputSource);
            VolumeGainContrastDelta.AddOnChangeListener(OnVolumeGainContrastChanged, InputSource);
        }
	}
	
    void OnDisable()
	{
        if (null != VolumeGainContrastDelta)
		{
            VolumeGainContrastOn.RemoveOnStateDownListener(OnVolumeGainContrastPressed, InputSource);
            VolumeGainContrastOn.RemoveOnStateUpListener(OnVolumeGainContrastReleased, InputSource);
            VolumeGainContrastDelta.RemoveOnChangeListener(OnVolumeGainContrastChanged, InputSource);
        }
	}

    private void OnVolumeGainContrastPressed(
        SteamVR_Action_Boolean fromAction,
        SteamVR_Input_Sources fromSource)
	{
		_touchpadDown = true;
	}

    private void OnVolumeGainContrastReleased(
        SteamVR_Action_Boolean fromAction,
        SteamVR_Input_Sources fromSource)
	{
		_touchpadDown = false;
	}

    private void OnVolumeGainContrastChanged(
        SteamVR_Action_Vector2 fromAction,
        SteamVR_Input_Sources fromSource,
        Vector2 axis,
        Vector2 delta)
    {
        if (null == _volumeRenderGain || !_touchpadDown)
        {
            return;
        }

        _volumeRenderGain.ChangeWindowLevel(delta.y * TouchpadSensitivity);
        _volumeRenderGain.ChangeWindowWidth(delta.x * TouchpadSensitivity);
    }

	void OnGUI()
	{
		if (null == _volumeRenderGain)
		{
			return;
		}

		GUI.Label(
			new Rect(0, 0, 200, 50), 
			"WLWW: " + (int)_volumeRenderGain.VolumeWindowLevel + 
			", " + (int)_volumeRenderGain.VolumeWindowWidth);
	}
}
