using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KeyboardChooseData : MonoBehaviour
{
	// Update is called once per frame
	void Update () 
	{
		if (!Input.anyKeyDown)
		{
			return;
		}

		if (1 != Input.inputString.Length)
		{
			return;
		}

		int choiceI = -1;
		if (int.TryParse(Input.inputString, out choiceI))
		{
			if (0 == choiceI)
			{
				choiceI += 10;
			}
		}
		else
		{
			char c = Input.inputString[0];
			choiceI = (int)c % 32;
			choiceI += 10;
		}

		// all the above are 1 indexed, data are 0 indexed
		choiceI -= 1;

		if (choiceI < 0 || choiceI >= DataStore.Instance.NDataFolders)
		{
			return;
		}

		DataStore.Instance.IDataFolder = choiceI; 
		SceneManager.LoadScene(0, LoadSceneMode.Single);
	}
}
