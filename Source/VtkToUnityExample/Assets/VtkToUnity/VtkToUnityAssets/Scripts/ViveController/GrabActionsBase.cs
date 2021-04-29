using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabActionsBase : MonoBehaviour {

	// Use this for initialization
	// void Start () {

	// }

	// Update is called once per frame
	// void Update () {

	// }

	public enum GrabReleaseActions
	{
		OnPreGrab,
		OnPostGrab,
		OnPreRelease,
		OnPostRelease,
        OnCreationRelease
	};

	public virtual void OnGrabOrRelease(GrabReleaseActions action)
	{
		switch (action)
		{
			case (GrabReleaseActions.OnPreGrab):
				OnPreGrab();
				break;
			case (GrabReleaseActions.OnPostGrab):
				OnPostGrab();
				break;
			case (GrabReleaseActions.OnPreRelease):
				OnPreRelease();
				break;
			case (GrabReleaseActions.OnPostRelease):
				OnPostRelease();
				break;
            case (GrabReleaseActions.OnCreationRelease):
                OnCreationRelease();
                break;
		}
	}

	// Override this method to do something when the controller grabs the object
	// Just before the parent changes
	protected virtual void OnPreGrab() {}
	// Just after the parent changed
	protected virtual void OnPostGrab() {}

	// Override this method to do something when the controller releases the object
	// Just before the parent reverts
	protected virtual void OnPreRelease() {}
	// Just after the parent reverts
	protected virtual void OnPostRelease() {}
    // Just after the gameobject is initiated by the controller
    protected virtual void OnCreationRelease() {}
}
