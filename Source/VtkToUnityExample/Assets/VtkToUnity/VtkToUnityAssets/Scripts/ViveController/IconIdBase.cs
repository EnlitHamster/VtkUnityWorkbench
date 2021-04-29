using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconIdBase : MonoBehaviour
{
	static int _sIconCloneIndex = 0;
	protected string _handle = "NOTSET";

	public string Handle
	{
		get
		{
			return _handle;
		}
	}

	void Awake()
	{
		_handle = _sIconCloneIndex.ToString();
		++_sIconCloneIndex;
	}
}
