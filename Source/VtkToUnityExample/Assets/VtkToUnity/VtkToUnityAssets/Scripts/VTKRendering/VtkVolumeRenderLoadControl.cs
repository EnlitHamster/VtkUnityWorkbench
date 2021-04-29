using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine.UI;

using ThreeDeeHeartPlugins;

public class VtkVolumeRenderLoadControl : VtkVolumeRenderCore
{
	// Allow user to set the path to a DICOM volume
	//public string DicomFolder = "C:\\ImagingData\\3DHeart\\frames";

	//[Range(0, 26)]
	private int _desiredFrameIndex = 0;
	private int _setFrameIndex = 0;
	private int _nFrames = 1;
	public bool Play = false;
    public GameObject PlayButton;

	[Range(0, 8)] 
	public int TransferFunctionIndex = 0;

	private const float _minWindowLevel = -1000.0f;
	private const float _maxWindowLevel = 1000.0f;
	[Range(_minWindowLevel, _maxWindowLevel)]
	public float VolumeWindowLevel = 105.0f;

	private const float _minWindowWidth = 1.0f;
	private const float _maxWindowWidth = 1000.0f;
	[Range(_minWindowWidth, _maxWindowWidth)]
	public float VolumeWindowWidth = 150.0f;

	[Range(0.01f, 2.0f)] 
	public float VolumeOpacityFactor = 1.0f;

	[Range(0.01f, 2.0f)] 
	public float VolumeBrightnessFactor = 1.0f;

	public bool RenderComposite = true;
	public bool TargetFramerateOn = false;
	[Range(1, 400)]
	public int TargetFramerateFps = 125;

	public bool LightingOn = false;


	private int _oldTransferFunctionIndex = 0; 
	private float _oldVolumeWindowLevel = 105.0f;
	private float _oldVolumeWindowWidth = 150.0f;
	private float _oldVolumeOpacityFactor = 1.0f;
	private float _oldVolumeBrightnessFactor = 1.0f;

	private bool _oldRenderComposite = true;
	private bool _oldTargetFramerateOn = false;
	private int _oldTargetFramerateFps = 200;

	private bool _oldLightingOn = false;

	public int NFrames
	{
		get
		{
			return _nFrames;
		}
	}

	public int FrameIndexSet
	{
		get
		{
			return _setFrameIndex;
		}
	}

	public int FrameIndexDesired
	{
		get
		{
			return _desiredFrameIndex;
		}
		set
		{
			if (value < 0 || value >= _nFrames)
			{
				return;
			}

			_desiredFrameIndex = value;
		}
	}

	//public int GetVolumePropId()
	//{
	//    return _volumePropId;
	//}

	protected override IEnumerator StartImpl()
	{
#if UNITY_WEBGL && !UNITY_EDITOR
		VtkToUnityPlugin.RegisterPlugin();
#endif

		LoadDicomOrMhdFromFolder();

		TransferFunctionIndex = VtkToUnityPlugin.GetTransferFunctionIndex();
		// Debug.Log("VtkVolumeRenderLoadControl::StartImpl - GetTransferFunctionIndex: " + TransferFunctionIndex.ToString());
		_oldTransferFunctionIndex = TransferFunctionIndex;

		VtkToUnityPlugin.SetVolumeWWWL(VolumeWindowWidth, VolumeWindowLevel);
		_oldVolumeWindowWidth = VolumeWindowWidth;
		_oldVolumeWindowLevel = VolumeWindowLevel;

		VtkToUnityPlugin.SetVolumeOpacityFactor(VolumeOpacityFactor);
		_oldVolumeOpacityFactor = VolumeOpacityFactor;

		VtkToUnityPlugin.SetVolumeBrightnessFactor(VolumeBrightnessFactor);
		_oldVolumeBrightnessFactor = VolumeBrightnessFactor;

		VtkToUnityPlugin.SetVolumeIndex(_desiredFrameIndex);
		_setFrameIndex = _desiredFrameIndex;

		VtkToUnityPlugin.SetRenderComposite(RenderComposite);
		_oldRenderComposite = RenderComposite;

		VtkToUnityPlugin.SetTargetFrameRateOn(TargetFramerateOn);
		_oldTargetFramerateOn = TargetFramerateOn;

		VtkToUnityPlugin.SetTargetFrameRateFps(TargetFramerateFps);
		_oldTargetFramerateFps = TargetFramerateFps;

		StartCoroutine("NextFrameEvent");

		//yield return StartCoroutine("CallPluginAfterUpdate");
		return base.StartImpl();
	}

	void OnDestroy () {

		VtkToUnityPlugin.RemoveProp3D(_volumePropId);
		VtkToUnityPlugin.ClearVolumes();
	}

    public override void UnloadVolume()
    {
		base.UnloadVolume();
		VtkToUnityPlugin.ClearVolumes();
    }

	public void LoadDicomOrMhdFromFolder()
	{
		var dataFolder = DataStore.Instance.ImageDataFolder;

		if (!Directory.Exists(dataFolder))
		{
			return;
		}
		
		// Get a list all of the files in the folder
		string[] filepaths = Directory.GetFiles(dataFolder);

		foreach (string filepath in filepaths)
		{
			string extension = Path.GetExtension(filepath);

			if (0 == String.Compare(extension, ".dcm", true) ||
				0 == String.Compare(extension, "", true))
			{
				// Is there a dicom file?
				// just pass in the folder name to the plugin
				// (We are assuming only one volume in a folder)
				VtkToUnityPlugin.LoadDicomVolume(dataFolder);
				break; 
			}
			else if (0 == String.Compare(extension, ".mhd", true))
			{
				// otherwise do we have mdh files?
				// Get all of the mhd files and load them in
				VtkToUnityPlugin.LoadMhdVolume(filepath);
			}
			else if (0 == String.Compare(extension, ".seq.nrrd", true))
			{
				continue;
			}
			else if (0 == String.Compare(extension, ".nrrd", true))
			{
				// otherwise do we have mdh files?
				// Get all of the mhd files and load them in
				VtkToUnityPlugin.LoadNrrdVolume(filepath);
			}
		}

		_nFrames = VtkToUnityPlugin.GetNVolumes();

		if (0 < _nFrames && DataStore.Instance.GeneratePaddingMask)
		{
			VtkToUnityPlugin.CreatePaddingMask(DataStore.Instance.PaddingValue);
		}
	}

	protected override void CallPluginAtEndOfFramesBody()
	{
		if (_desiredFrameIndex != _setFrameIndex)
		{
			if (_desiredFrameIndex >= 0 &&
				_desiredFrameIndex < _nFrames)
			{
				VtkToUnityPlugin.SetVolumeIndex(_desiredFrameIndex);
				_setFrameIndex = _desiredFrameIndex;
			}
		}

		if (_oldTransferFunctionIndex != TransferFunctionIndex)
		{
			// Debug.Log("VtkVolumeRenderLoadControl::CallPluginAtEndOfFramesBody - SetTransferFunctionIndex: " + TransferFunctionIndex.ToString());
			VtkToUnityPlugin.SetTransferFunctionIndex(TransferFunctionIndex);
			_oldTransferFunctionIndex = TransferFunctionIndex;
		}

		if (_oldVolumeWindowWidth != VolumeWindowWidth 
			|| _oldVolumeWindowLevel != VolumeWindowLevel)
		{
			VtkToUnityPlugin.SetVolumeWWWL(VolumeWindowWidth, VolumeWindowLevel);
			_oldVolumeWindowWidth = VolumeWindowWidth;
			_oldVolumeWindowLevel = VolumeWindowLevel;
		}

		if (_oldVolumeOpacityFactor != VolumeOpacityFactor)
		{
			VtkToUnityPlugin.SetVolumeOpacityFactor(VolumeOpacityFactor);
			_oldVolumeOpacityFactor = VolumeOpacityFactor;
		}

		if (_oldVolumeBrightnessFactor != VolumeBrightnessFactor)
		{
			VtkToUnityPlugin.SetVolumeBrightnessFactor(VolumeBrightnessFactor);
			_oldVolumeBrightnessFactor = VolumeBrightnessFactor;
		}

		if (RenderComposite != _oldRenderComposite)
		{
			VtkToUnityPlugin.SetRenderComposite(RenderComposite);
			_oldRenderComposite = RenderComposite;
		}

		if (TargetFramerateOn != _oldTargetFramerateOn)
		{
			VtkToUnityPlugin.SetTargetFrameRateOn(TargetFramerateOn);
			_oldTargetFramerateOn = TargetFramerateOn;
		}

		if (TargetFramerateFps != _oldTargetFramerateFps)
		{
			VtkToUnityPlugin.SetTargetFrameRateFps(TargetFramerateFps);
			_oldTargetFramerateFps = TargetFramerateFps;
		}

		if (LightingOn != _oldLightingOn)
		{
			VtkToUnityPlugin.SetLightingOn(LightingOn);
			_oldLightingOn = LightingOn;
		}

		base.CallPluginAtEndOfFramesBody();
	}
     
    private IEnumerator NextFrameEvent()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.07f);

            // do things
			if (Play)
			{
				++_desiredFrameIndex;
				if (_desiredFrameIndex > _nFrames)
				{
					_desiredFrameIndex = 0;
				}
			}
        }
    }

    public void TogglePlay()
    {
        Play = !Play;
    }

    public void OnPrevious()
    {
        //Play = false;
        if (Play && PlayButton)
        {
            PlayButton.GetComponent<Toggle>().isOn = false;
        }
        --_desiredFrameIndex;
        if (_desiredFrameIndex < 0)
        {
            _desiredFrameIndex = _nFrames;
        }
    }

    public void OnNext()
    {
        //Play = false;
        if (Play && PlayButton)
        {
            PlayButton.GetComponent<Toggle>().isOn = false;
        }
        ++_desiredFrameIndex;
        if (_desiredFrameIndex > _nFrames)
        {
            _desiredFrameIndex = 0;
        }
    }

	private static float Clamp(float value, float min, float max)
	{
		return (value < min) ? min : (value > max) ? max : value;
	}

	public void ChangeWindowLevel(float levelChange)
	{
		VolumeWindowLevel = 
			Clamp(VolumeWindowLevel + levelChange, _minWindowLevel, _maxWindowLevel);
	}

	public void ChangeWindowWidth(float widthChange)
	{
		VolumeWindowWidth = 
			Clamp(VolumeWindowWidth + widthChange, _minWindowWidth, _maxWindowWidth);
	}
}
