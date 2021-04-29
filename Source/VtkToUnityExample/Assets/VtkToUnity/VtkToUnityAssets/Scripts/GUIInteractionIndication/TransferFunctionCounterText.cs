using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TransferFunctionCounterText : MonoBehaviour
{
	private TextMeshPro _textUi;

    // Start is called before the first frame update
    void Start()
    {
		// Debug.Log("TransferFunctionCounterText::Start");
		_textUi = GetComponent<TextMeshPro>();
		string dataStr = (1).ToString() + "/" + (1).ToString();
		_textUi.text = dataStr;
		//UpdateDataText();
    }

	public void SetCount(int i, int n)
	{
		// Debug.Log("TransferFunctionCounterText::SetCount");
		string dataStr = (i + 1).ToString() + "/" + n.ToString();
		_textUi.text = dataStr;
	}

}
