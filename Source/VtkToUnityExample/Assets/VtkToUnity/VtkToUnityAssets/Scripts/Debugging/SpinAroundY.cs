using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAroundY : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		// Spin around the Y axis
		// transform.Rotate(Vector3.up * Time.deltaTime, Space.World);
		transform.Rotate(0, Time.deltaTime * 30.0f, 0, Space.World);
		
	}
}
