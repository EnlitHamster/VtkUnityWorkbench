using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorBase : MonoBehaviour
{
	public enum IndicateState
	{
		Off = 0,
		Highlight,
		Active,
		NStates
	};

	private IndicateState _indicate = IndicateState.Off;

	public virtual IndicateState Indicate
	{
		get
		{
			return _indicate;
		}

		set
		{
			_indicate = value;
		}
	}

    private void Awake()
    {
        Indicate = IndicateState.Off;
    }
}
