using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabActionsApplyChildren : GrabActionsBase {

	// Use this for initialization
	// void Start () {

	// }

	// Update is called once per frame
	// void Update () {

	// }
	public override void OnGrabOrRelease(GrabReleaseActions action)
	{
		foreach (Transform child in transform)
		{
			var grabActions = child.gameObject.GetComponentsInChildren<GrabActionsBase>();

			foreach (var grabAction in grabActions)
			{
				grabAction.OnGrabOrRelease(action);
			}
		}
	}
}
