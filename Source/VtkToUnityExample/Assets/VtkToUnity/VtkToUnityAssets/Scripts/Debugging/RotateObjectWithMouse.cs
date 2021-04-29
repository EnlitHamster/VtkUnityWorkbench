using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObjectWithMouse : MonoBehaviour {

	private Vector3 _initialMousePosition;
	private bool _mouseDown = false;
	private Vector3 _initialPosition;
	private Quaternion _initialRotation;
	private Quaternion _rotationToAdd = new Quaternion();
	private const float _magicFactor = 0.2f;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButtonDown(0))
		{
			_initialMousePosition = Input.mousePosition;
			_mouseDown = true;
			_initialPosition = transform.position;
			_initialRotation = transform.rotation;
			// Debug.Log("Mouse Press");
		}
		else if (Input.GetMouseButton(0) && _mouseDown)
		{
			Vector3 currentMousePosition = Input.mousePosition;
			Vector3 mouseDelta = currentMousePosition - _initialMousePosition;
			_rotationToAdd.eulerAngles = new Vector3(mouseDelta.y * _magicFactor, -mouseDelta.x * _magicFactor, 0.0f);
			transform.SetPositionAndRotation(_initialPosition,  _rotationToAdd * _initialRotation);
		}
		else if (Input.GetMouseButtonUp(0))
		{
			_mouseDown = false;
			// Debug.Log("Mouse Release");
		}
	}
}
