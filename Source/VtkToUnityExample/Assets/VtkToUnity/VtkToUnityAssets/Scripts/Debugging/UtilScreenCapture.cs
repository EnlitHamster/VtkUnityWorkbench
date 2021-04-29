using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilScreenCapture : GenericSingletonClass<UtilScreenCapture>
{
	public int CaptureSuperSize = 1;

    void OnMouseDown()
    {
		ScreenCapture.CaptureScreenshot(
			UtilFileManager.Instance.OutputPathDateTimeName("mouse", "png")
		);
	}

	public void ScreenCaptureToDateTimeName()
	{
		ScreenCapture.CaptureScreenshot(
			UtilFileManager.Instance.OutputPathDateTimeName("png")
		); 
	}
}
