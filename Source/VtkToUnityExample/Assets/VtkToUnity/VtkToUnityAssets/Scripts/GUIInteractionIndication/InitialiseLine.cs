using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialiseLine : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		Initialise();
    }


	public void Initialise()
	{
		var grabAction = GrabActionsBase.GrabReleaseActions.OnPostRelease;
		var indicatorState = IndicatorBase.IndicateState.Off;

		GrabActionsUtils.Instance.GrabOrRelease(gameObject, grabAction);

		foreach (Transform child in transform)
		{
			GrabActionsUtils.Instance.GrabOrRelease(child.gameObject, grabAction);

			var indicators = gameObject.GetComponentsInChildren<IndicatorBase>();

			foreach (var indicator in indicators)
			{
				indicator.Indicate = indicatorState;
			}
		}

	}
}
