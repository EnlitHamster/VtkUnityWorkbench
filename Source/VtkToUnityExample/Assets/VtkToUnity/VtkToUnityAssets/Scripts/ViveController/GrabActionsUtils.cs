using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabActionsUtils : GenericSingletonClass<GrabActionsUtils>
{
	public void GrabOrRelease(
		GameObject gameObject,
		GrabActionsBase.GrabReleaseActions action)
	{
		var grabActions = gameObject.GetComponents<GrabActionsBase>();

		foreach (var grabAction in grabActions)
		{
			grabAction.OnGrabOrRelease(action);
		}
	}
}

