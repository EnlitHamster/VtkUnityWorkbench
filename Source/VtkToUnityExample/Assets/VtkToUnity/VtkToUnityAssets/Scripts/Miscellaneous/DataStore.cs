using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataStore : GenericSingletonClass<DataStore>
{
	// Data folders for DICOM, MHD etc. -------------------------------------------------
	public string[] ImageDataFolders = new string[1];
	public int IDataFolder = 0;

	public bool GeneratePaddingMask = false;
	public int PaddingValue = 0;

	public int NDataFolders
	{
		get
		{
			return ImageDataFolders.Length;
		}
	}

	public string ImageDataFolder
	{
		get
		{
			if (0 > IDataFolder || ImageDataFolders.Length <= IDataFolder)
			{
				throw new System.Exception("Data Folders not initialised, or data folder index out of range");
			}

			return ImageDataFolders[IDataFolder];
		}
	}


	// Transform for keeping the set scene location -------------------------------------
	private Vector3 _storedPosition;
	private Vector3 _storedEulerAngles;

	public void StorePositionRotation(GameObject gameObject)
	{
		_storedPosition = gameObject.transform.position;
		_storedEulerAngles = gameObject.transform.eulerAngles;
	}

	public void ApplyPositonRotationY(GameObject gameObject)
	{
		if (null == _storedPosition || null == _storedEulerAngles)
		{
			return;
		}

		gameObject.transform.position = _storedPosition;
		var yRotation = _storedEulerAngles.y;
		gameObject.transform.eulerAngles = new Vector3(0, yRotation, 0);
	}
}
