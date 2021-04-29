using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FrameCounterText : MonoBehaviour
{
	private TextMeshPro _textUi;
	private FrameAnimationControl _frameAnimationControl;
	private string _frameFormat = "00";
    private int _oldIFrame = -1;
    private int _oldNFrames = -1;

    // Start is called before the first frame update
    void Start()
    {
        _textUi = GetComponent<TextMeshPro>();
        _frameAnimationControl = GetComponentInParent<FrameAnimationControl>();
		if (_frameAnimationControl.NFrames < 10)
		{
			_frameFormat = "0";
		}
		else if (_frameAnimationControl.NFrames > 99)
		{
			_frameFormat = "000";
		}
        _oldIFrame = _frameAnimationControl.IFrameSet - 1;
        _oldNFrames = _frameAnimationControl.NFrames - 1;
    }

// Update is called once per frame
void Update()
	{
        if (_oldIFrame != _frameAnimationControl.IFrameSet ||
            _oldNFrames != _frameAnimationControl.NFrames)
        {
            // because there is a delay between selecting and index and it being used,
            // I'm going to (expensively) update this every frame for now
            string dataStr =
                (_frameAnimationControl.IFrameSet + 1).ToString(_frameFormat) + "/" +
                _frameAnimationControl.NFrames.ToString(_frameFormat);
            _textUi.text = dataStr;

            _oldIFrame = _frameAnimationControl.IFrameSet;
            _oldNFrames = _frameAnimationControl.NFrames;
        }
    }
}
