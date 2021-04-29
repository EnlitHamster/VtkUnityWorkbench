using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParentMoveBase : MonoBehaviour {

	protected Common.Motion _motionType = Common.Motion.Translation;

	public Common.Motion MotionType
	{
		get { return _motionType; }
	}

	public virtual void OnChildPreGrab(GameObject childControl)
	{
	}

	public virtual void OnChildPostGrab(GameObject childControl)
	{
	}

	public virtual void OnChildPreRelease()
	{
	}

	public virtual void OnChildPostRelease()
	{
	}
}
