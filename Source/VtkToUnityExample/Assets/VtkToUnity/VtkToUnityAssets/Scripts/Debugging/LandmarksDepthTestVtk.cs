using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ThreeDeeHeartPlugins;

public class LandmarksDepthTestVtk : MonoBehaviour {

	[Range(-1.5f, 1.5f)]
	public float LandmarkDepth = 0.0f; 
	private float _oldLandmarkDepth = 0.0f;
	private float _halfVolumeDepth = 0.0f;

	[Range(0.001f, 0.02f)]
	public float LandmarkScale = 0.005f;

	[Range(0.005f, 0.05f)]
	public float LandmarkSpacing = 0.02f;

	struct IdPosition
	{
		public int Id;
		public VtkToUnityPlugin.Float4 PositionScale;
	};

	private List<IdPosition> _landmarkIdPositons = new List<IdPosition>();

	// Use this for initialization
	void Start () {
		////////////////////////////////////////////////////
		// Get the size of the volume, so we can scale the depth and set the number of landmarks

		// Check we actually have some volumes (be sure to run this after the script which loads them!)
		if (VtkToUnityPlugin.GetNVolumes() < 1)
		{
			return;
		}

		// Get the spacing and extents to determine the volume size
		VtkToUnityPlugin.Float4 volumeSpacingM = VtkToUnityPlugin.GetVolumeSpacingM(); // * 1000.0f - changes from M to mm?
		VtkToUnityPlugin.Float4 volumeExtentsMin = VtkToUnityPlugin.GetVolumeExtentsMin();
		VtkToUnityPlugin.Float4 volumeExtentsMax = VtkToUnityPlugin.GetVolumeExtentsMax();

		float volumeWidth = volumeSpacingM.x * (volumeExtentsMax.x - volumeExtentsMin.x);
		int nLandmarksWidth = (int)Math.Ceiling(volumeWidth / LandmarkSpacing);
		float volumeHeight = volumeSpacingM.y * (volumeExtentsMax.y - volumeExtentsMin.y);
		int nLandmarksHeight = (int)Math.Ceiling(volumeHeight / LandmarkSpacing);
		_halfVolumeDepth = 0.5f * volumeSpacingM.z * (volumeExtentsMax.z - volumeExtentsMin.z);

		const int sphere = 1;
		const bool wireframe = false;

		Vector3 scale = new Vector3(LandmarkScale, LandmarkScale, LandmarkScale);
		Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
		Matrix4x4 transform = Matrix4x4.identity;

		float redStep = 1.0f / (nLandmarksWidth -1);
		float red = 0.0f;
		float greenStep = 1.0f / (nLandmarksHeight -1);
		float green = 0.0f;
		float blue = 1.0f;

		// Create the landmarks!
		for (int x = 0; x < nLandmarksWidth; ++x)
		{
			for (int y = 0; y < nLandmarksHeight; ++y)
			{
				VtkToUnityPlugin.Float4 colour = new VtkToUnityPlugin.Float4();
				colour.SetXYZW(red, green, blue, 1.0f);

				int id = VtkToUnityPlugin.AddShapePrimitive(sphere, colour, wireframe);

				IdPosition idPosition = new IdPosition();
				idPosition.Id = id;
				idPosition.PositionScale.x = (float)((-0.5* volumeWidth) + (x * LandmarkSpacing));
				idPosition.PositionScale.y = (float)((-0.5* volumeWidth) + (y * LandmarkSpacing));
				idPosition.PositionScale.z = _halfVolumeDepth * LandmarkDepth;
				idPosition.PositionScale.w = LandmarkScale;

				Vector3 translation = new Vector3(
					(float)((-0.5* volumeWidth) + (x * LandmarkSpacing)), 
					(float)((-0.5* volumeHeight) + (y * LandmarkSpacing)), 
					_halfVolumeDepth * LandmarkDepth);
				transform.SetTRS(translation, rotation, scale);
				VtkToUnityPlugin.Float16 transformArray = 
					VtkToUnityPlugin.UnityMatrix4x4ToFloat16(transform);
				VtkToUnityPlugin.SetProp3DTransform(id, transformArray);

				_landmarkIdPositons.Add(idPosition);

				green += greenStep;
			}

			red += redStep;
			green = 0.0f;
		}

		_oldLandmarkDepth = LandmarkDepth; 
	}
	
	void OnDestroy ()
	{
		// delete all of the landmark objects
		foreach (var idPosition in _landmarkIdPositons)
		{
			VtkToUnityPlugin.RemoveProp3D(idPosition.Id);
		}
	}

	// Update is called once per frame
	void Update ()
	{

		// update the position scale etc. of the landmakrs if they have changed
		// if (_oldLandmarkDepth != LandmarkDepth)
		{
			Quaternion sphereRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
			Matrix4x4 transformMatrix = Matrix4x4.identity;

			Matrix4x4 parentTransformMatrix = transform.localToWorldMatrix;

			foreach (var idPosition in _landmarkIdPositons)
			{
				Vector3 scale = new Vector3(
					idPosition.PositionScale.w, 
					idPosition.PositionScale.w, 
					idPosition.PositionScale.w);
				
				Vector3 translation = new Vector3(
					idPosition.PositionScale.x, 
					idPosition.PositionScale.y, 
					_halfVolumeDepth * LandmarkDepth);

				transformMatrix.SetTRS(translation, sphereRotation, scale);
				transformMatrix = parentTransformMatrix * transformMatrix;
				VtkToUnityPlugin.Float16 transformArray = 
					VtkToUnityPlugin.UnityMatrix4x4ToFloat16(transformMatrix);

				VtkToUnityPlugin.SetProp3DTransform(idPosition.Id, transformArray);
			}

			_oldLandmarkDepth = LandmarkDepth; 
		}
	}

}
