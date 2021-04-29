using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityCameraClippingPlanes : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        Camera camera = Camera.main;

		if (camera == null)
		{
			return;
		}

		camera.nearClipPlane = 0.0001f;
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
