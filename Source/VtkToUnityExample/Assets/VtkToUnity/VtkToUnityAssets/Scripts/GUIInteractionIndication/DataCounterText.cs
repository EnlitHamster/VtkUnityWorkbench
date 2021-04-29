using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DataCounterText : MonoBehaviour
{
	private TextMeshPro _textUi;
    // Start is called before the first frame update
    void Start()
    {
		_textUi = GetComponent<TextMeshPro>();
		UpdateCounterString();
	}

	public void UpdateCounterString()
	{
		string dataStr = (DataStore.Instance.IDataFolder + 1).ToString() +
			"/" + DataStore.Instance.NDataFolders.ToString();
		_textUi.text = dataStr;
	}
}
