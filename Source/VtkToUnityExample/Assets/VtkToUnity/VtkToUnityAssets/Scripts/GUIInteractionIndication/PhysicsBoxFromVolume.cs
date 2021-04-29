using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using ThreeDeeHeartPlugins;

public class PhysicsBoxFromVolume : MonoBehaviour {

	[Range(0.0f, 50.0f)]
	public float PaddingMm = 10.0f;

	public bool FlattenX = false;
	public bool FlattenY = false;
	public bool FlattenZ = false;

	public float FlatPhysicsThicknessMm = 40.0f;

    // Use this for initialization
    void Start() {
        UpdatePhysicsBox();
    }


    public void UpdatePhysicsBox() {

        // get the size of the volume, the plugin currently centres the volume so some of
        // this is redundant
        var volumeOriginM = SwizzleAxes(
			VtkToUnityPlugin.Float4ToVector3(VtkToUnityPlugin.GetVolumeOriginM())
		);
		var volumeExtentsMin = SwizzleAxes(
			VtkToUnityPlugin.Float4ToVector3(VtkToUnityPlugin.GetVolumeExtentsMin())
		);
		var volumeExtentsMax = SwizzleAxes(
			VtkToUnityPlugin.Float4ToVector3(VtkToUnityPlugin.GetVolumeExtentsMax())
		);
		var volumeSpacingM = SwizzleAxes(
			VtkToUnityPlugin.Float4ToVector3(VtkToUnityPlugin.GetVolumeSpacingM())
		);

		var paddingVectorM = Vector3.one * PaddingMm * 0.001f;
		var volumeExtentsMinM = Vector3.Scale(volumeExtentsMin, volumeSpacingM) - paddingVectorM;
		var volumeSizeM = 
			Vector3.Scale((volumeExtentsMax - volumeExtentsMin + Vector3.one), volumeSpacingM) + 
			(2.0f * paddingVectorM);

		// set up the collsion box to be slightly smaller than the volume
		var collider = this.GetComponent<BoxCollider>();

		if (FlattenX)
		{
			volumeSizeM.x = FlatPhysicsThicknessMm * 0.001f;
		}
		else if (FlattenY)
		{
			volumeSizeM.y = FlatPhysicsThicknessMm * 0.001f;
		}
		else if (FlattenZ)
		{
			volumeSizeM.z = FlatPhysicsThicknessMm * 0.001f;
		}

		collider.size = volumeSizeM; 

	}

	// because we have to rotate the volume to get the correct initial orientation in
	// Unity we need to fix the dimensions by swizzzing the axes to make them match up
	private Vector3 SwizzleAxes(Vector3 inVector) {
		return new Vector3(inVector.x, inVector.z, inVector.y);

	}
}
