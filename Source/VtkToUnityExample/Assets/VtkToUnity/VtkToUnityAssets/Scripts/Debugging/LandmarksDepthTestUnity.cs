using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ThreeDeeHeartPlugins;

public class LandmarksDepthTestUnity : MonoBehaviour {

	public GameObject LandmarkPrefab;


	// Use this for initialization
	void Start () {

		var landmarkSpacing = 0.025f;

		var markerStepOffset = -2;
		var nMarkerSteps = 5;

		var initialXYZ = markerStepOffset * landmarkSpacing;

		var runningX = initialXYZ;
		var runningY = initialXYZ;
		var runningZ = initialXYZ;

		var initialColour = 0.0f;
		var colourStep = 1.0f /nMarkerSteps;

		var runningR = initialColour;
		var runningG = initialColour;
		var runningB = initialColour;

		// Create the landmarks!
		for (int x = 0; x <= nMarkerSteps; ++x)
		{
			for (int y = 0; y <= nMarkerSteps; ++y)
			{
				for (int z = 0; z <= nMarkerSteps; ++z)
				{
					Vector3 position = new Vector3(
						runningX,
						runningY,
						runningZ);
					var marker = Instantiate(LandmarkPrefab, position, Quaternion.identity);
					marker.transform.parent = transform;

					Color altColor = Color.black;
					altColor.r = runningR;
					altColor.g = runningG;
					altColor.b = runningB;
					// altColor.a = 0f;

					marker.GetComponent<Renderer>().material.color = altColor;

					runningZ += landmarkSpacing;

					runningB += colourStep;
				}

				runningZ = initialXYZ;
				runningY += landmarkSpacing;

				runningB = initialColour;
				runningG += colourStep;
			}

			runningY = initialXYZ;
			runningX += landmarkSpacing;

			runningG = initialColour;
			runningR += colourStep;
		}
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
