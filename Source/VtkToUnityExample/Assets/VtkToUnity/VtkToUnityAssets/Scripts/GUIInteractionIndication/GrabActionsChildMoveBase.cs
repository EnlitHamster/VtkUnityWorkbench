using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabActionsChildMoveBase : GrabActionsBase {

	public Common.Motion MotionType = Common.Motion.Translation;
	private List<ParentMoveBase> _parentMoves = new List<ParentMoveBase>();

	void Awake()
	{
		UpdateParentMoves();
	}


	public void UpdateParentMoves()
	{
		_parentMoves.Clear();
		var allParentMoves = GetComponentsInParent<ParentMoveBase>();

		foreach (var parentMove in allParentMoves)
		{
			if (parentMove.MotionType == this.MotionType)
			{
				_parentMoves.Add(parentMove);
			}
		}
	}

	// Override this method to do something when the controller grabs the object
	// Just before the parent changes
	protected override void OnPreGrab()
	{
		foreach (var parentMove in _parentMoves)
		{
			parentMove.OnChildPreGrab(transform.gameObject);
		}
	}

	//Just after the parent changed
	protected override void OnPostGrab()
	{
		foreach (var parentMove in _parentMoves)
		{
			parentMove.OnChildPostGrab(transform.gameObject);
		}
	}

	//Override this method to do something when the controller releases the object
	//Just before the parent reverts
	protected override void OnPreRelease()
	{
		foreach (var parentMove in _parentMoves)
		{
			parentMove.OnChildPreRelease();
		}
	}

	// Just after the parent reverts
	protected override void OnPostRelease()
	{
		foreach (var parentMove in _parentMoves)
		{
			parentMove.OnChildPostRelease();
		}
	}
}
