using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParentMoveScaleCentre : ParentMoveBase
{
	public float MinimumScale = 0.5f;
	public float MaximumScale = 10.0f;

	private GameObject _activeChildControl;
	private Vector3 _childControlInitialLocalPosition;
	private Quaternion _childControlInitialLocalRotation;
	private Vector3 _childControlInitialLocalScale;

	private Vector3 _parentInitialPosition;
	private Vector3 _parentInitialLocalScale;

	private Vector3 _initialScalingDirection;
	private float _initialScalingDistance;

	public ParentMoveScaleCentre()
	{
		base._motionType = Common.Motion.Scale;
	}

	// Update is called once per frame
	void Update ()
	{
		if (null == _activeChildControl)
		{
			return;
		}

		// Set the position of this to be the closest point projected on to the original axis
		var scalingTarget = Math3d.ProjectPointOnLine(
			_parentInitialPosition, 
			_initialScalingDirection, 
			_activeChildControl.transform.position);

		var scalingDistance =
			(scalingTarget - transform.position).magnitude;

		var scalingRatio = scalingDistance / _initialScalingDistance;

		var finalLocalScale = _parentInitialLocalScale * scalingRatio;
		finalLocalScale = Vector3.Min(Vector3.one * MaximumScale, finalLocalScale);
		finalLocalScale = Vector3.Max(Vector3.one * MinimumScale, finalLocalScale);

		transform.localScale = finalLocalScale;
	}

	public override void OnChildPreGrab(GameObject childControl)
	{
		// These are for putting the child control back where we found it
		_childControlInitialLocalPosition = childControl.transform.localPosition;
		_childControlInitialLocalRotation = childControl.transform.localRotation;
		_childControlInitialLocalScale = childControl.transform.localScale;

		// Get the parent's starting position and the axis from the centre 
		// of the parent to the child
		_parentInitialPosition = transform.position;
		_parentInitialLocalScale = transform.localScale;

		//_initialParentNormal = transform.TransformDirection(Vector3.up);
		_initialScalingDirection = 
			(childControl.transform.position - transform.position).normalized;
		_initialScalingDistance =
			(childControl.transform.position - transform.position).magnitude;

		// get the child's object, and initial position and orientation 
		_activeChildControl = childControl;
	}

	public override void OnChildPostRelease()
	{
		if (!_activeChildControl)
		{
			return;
		}

		// reset the child's position and rotation back to where it was
		_activeChildControl.transform.localRotation = _childControlInitialLocalRotation;
		_activeChildControl.transform.localPosition = _childControlInitialLocalPosition;
		_activeChildControl.transform.localScale = _childControlInitialLocalScale;

		_activeChildControl = null;
	}
}
