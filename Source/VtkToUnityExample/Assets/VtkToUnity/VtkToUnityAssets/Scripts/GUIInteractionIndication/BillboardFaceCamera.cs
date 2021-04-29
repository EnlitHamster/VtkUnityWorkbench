using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardFaceCamera : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
		Vector3 thisPosition = transform.position;
		Vector3 cameraPosition = Camera.main.transform.position;
		Vector3 direction = thisPosition - cameraPosition;
		direction = direction.normalized;

		transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
	}
}
