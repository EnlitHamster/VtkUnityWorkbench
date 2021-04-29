using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrameAnimationControl : MonoBehaviour
{
	//public int CurrentFrameIndex = 0;
	//private int _oldCurrentFrameIndex = 0;
	//private int _nFrames = 1;
	public bool Play = false;
	public GameObject PlayButton;

	public VtkVolumeRenderLoadControl VolumeControl; 

	public int NFrames
	{
		get
		{
			return VolumeControl.NFrames;
		}
	}

	public int IFrameSet
	{
		get
		{
			return VolumeControl.FrameIndexSet;
		}
	}

	// Start is called before the first frame update
	IEnumerator Start()
    {
		yield return StartCoroutine("NextFrameEvent");
	}

	// Update is called once per frame
	void Update()
    {
        
    }

	private IEnumerator NextFrameEvent()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.07f);

			// do things
			if (Play)
			{
				//++CurrentFrameIndex;
				//if (CurrentFrameIndex > _nFrames)
				//{
				//	CurrentFrameIndex = 0;
				//}
				IncrementDesiredFrame();
			}
		}
	}

	public void TogglePlay()
	{
		Play = !Play;
	}

	public void OnPrevious()
	{
		// Stop playing if we are playing
		if (Play && PlayButton)
		{
			PlayButton.GetComponent<Toggle>().isOn = false;
		}

		// go on to the previous frame
		int desiredFrameI = VolumeControl.FrameIndexDesired;
		if (desiredFrameI <= 0)
		{
			desiredFrameI = VolumeControl.NFrames - 1;
		}
		else
		{
			--desiredFrameI;
		}

		VolumeControl.FrameIndexDesired = desiredFrameI;
	}

	public void OnNext()
	{
		// Stop playing if we are playing
		if (Play && PlayButton)
		{
			PlayButton.GetComponent<Toggle>().isOn = false;
		}

		// go on to the next frame
		IncrementDesiredFrame();
	}

	private void IncrementDesiredFrame()
	{
		int desiredFrameI = VolumeControl.FrameIndexDesired;
		if (desiredFrameI >= (VolumeControl.NFrames - 1))
		{
			desiredFrameI = 0;
		}
		else
		{
			++desiredFrameI;
		}

		VolumeControl.FrameIndexDesired = desiredFrameI;
	}
}
