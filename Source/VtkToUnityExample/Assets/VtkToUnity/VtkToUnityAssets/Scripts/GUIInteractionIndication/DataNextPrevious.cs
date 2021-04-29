using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataNextPrevious : MonoBehaviour
{
	public void OnReset()
	{
		LoadDataI(DataStore.Instance.IDataFolder);
	}

	public void OnNextData()
	{
		var dataI = DataStore.Instance.IDataFolder + 1;
		if (dataI >= DataStore.Instance.NDataFolders)
		{
			return;
		}

		LoadDataI(dataI);
	}

	public void OnPreviousData()
	{
		var dataI = DataStore.Instance.IDataFolder - 1;
		if (dataI < 0)
		{
			return;
		}

		LoadDataI(dataI);
	}

	private void LoadDataI(int dataI)
	{
		DataStore.Instance.IDataFolder = dataI; 
		SceneManager.LoadScene(0, LoadSceneMode.Single);
	}
}
