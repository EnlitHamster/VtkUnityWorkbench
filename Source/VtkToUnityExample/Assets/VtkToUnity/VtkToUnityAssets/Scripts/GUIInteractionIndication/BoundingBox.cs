using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using ThreeDeeHeartPlugins;

public class BoundingBox : MonoBehaviour {

	public GameObject CornerParent;
	public GameObject EdgeParent;
	public GameObject FaceParent;

	public GameObject CornerPrefab;
	public GameObject EdgePrefab;
	public GameObject FacePrefab;

	[Range(0.0f, 50.0f)]
	private float _paddingMm = 20.0f;
	private float _paddingM;

	public Camera ViewCamera;

	private List<GameObject> _corners = new List<GameObject>();
	private List<GameObject> _edges = new List<GameObject>();
	private List<GameObject> _faces = new List<GameObject>();

    // Use this for initialization
    void Start()
	{
		_paddingM = _paddingMm * 0.001f * 2.0f;
        CreateBoundingBox();
    }

    // Update is called once per frame
    void Update()
    {
		if (ViewCamera != null)
		{
			var cameraPosition = ViewCamera.transform.position;
			// We bit shift the mask to get it to work
			int layerMask = 1 << LayerMask.NameToLayer("Physics Bounding Box"); //8;

			foreach (var corner in _corners)
			{

				var cornerPosition = corner.transform.position;
				var rayDirection = cornerPosition - cameraPosition;
				var cornerDistance = rayDirection.magnitude;
				rayDirection = rayDirection.normalized;

				RaycastHit hit;
				if (Physics.Raycast(
					cameraPosition,
					rayDirection,
					out hit,
					Mathf.Infinity,
					layerMask)
				)
				{
					if (hit.distance > cornerDistance)
					{
						EnableObjectAndChildRenderers(corner, false);
						Debug.DrawRay(cameraPosition, rayDirection * hit.distance, Color.yellow);
						continue;
					}
				}

				EnableObjectAndChildRenderers(corner, true);
				Debug.DrawRay(cameraPosition, rayDirection * 10, Color.white);
			}

			foreach (var edge in _edges)
			{

				var edgePosition = edge.transform.position;
				var rayDirection = edgePosition - cameraPosition;
				var edgeDistance = rayDirection.magnitude;
				rayDirection = rayDirection.normalized;

				RaycastHit hit;
				if (Physics.Raycast(
					cameraPosition,
					rayDirection,
					out hit,
					Mathf.Infinity,
					layerMask)
				)
				{
					if (hit.distance > edgeDistance)
					{
						EnableObjectAndChildRenderers(edge, false);
						Debug.DrawRay(cameraPosition, rayDirection * hit.distance, Color.yellow);
						continue;
					}
				}

				EnableObjectAndChildRenderers(edge, true);
				Debug.DrawRay(cameraPosition, rayDirection * 10, Color.white);
			}
		}
    }

    public void CreateBoundingBox() {

        ClearBoundingBox();

		// set up the collsion box to be slightly smaller than the volume
		var collider = this.GetComponent<BoxCollider>();
		if (collider == null)
		{
			return;
		}

		var colliderCenterM = collider.center;
		var colliderSizeM = collider.size;
		var frameSizeM = colliderSizeM + (Vector3.one * _paddingM);
		// we move the size out a little bit, so the corners don't flicker due to the physics ray casting
		frameSizeM *= 1.01f; 


		var cornerSize = 0.0f;

		if (null == CornerParent)
		{
			CornerParent = this.gameObject;
		}

		if (CornerPrefab != null)
		{
			for (float z = -0.5f; z <= 0.5f; z += 1.0f)
			{
				for (float y = -0.5f; y <= 0.5f; y += 1.0f)
				{
					for (float x = -0.5f; x <= 0.5f; x += 1.0f)
					{
						var corner = Instantiate(CornerPrefab);
						corner.transform.parent = CornerParent.transform;

						var cornerChildMoves = corner.GetComponentsInChildren<GrabActionsChildMoveBase>();
						foreach (var cornerChildMove in cornerChildMoves)
						{
							cornerChildMove.UpdateParentMoves();
						}

						var xyzFactor = new Vector3(x, y, z);
						corner.transform.localPosition =
							colliderCenterM +
							Vector3.Scale(frameSizeM, xyzFactor);

						_corners.Add(corner);
					}
				}
			}

			//cornerSize = CornerPrefab.transform.localScale.x;
		}

		UpdateIndicatorChildren(CornerParent);

		var edgeWidth = 0.0f;
		var edgeOffset = Vector3.zero;
		var edgePositionMin = Vector3.zero;
		var edgeMaxOffset = frameSizeM + (2.0f * edgeOffset);

		if (null == EdgeParent)
		{
			EdgeParent = this.gameObject;
		}

		if (EdgePrefab != null)
		{
			edgeWidth = EdgePrefab.transform.localScale.y;
			edgeOffset = Vector3.one * (0.5f * (cornerSize - edgeWidth));
			edgePositionMin = colliderCenterM + (frameSizeM * -0.5f) - edgeOffset;
			edgeMaxOffset = frameSizeM + (2.0f * edgeOffset);

			var edgeLength = EdgePrefab.transform.localScale.x;

			for (int axis = 0; axis < 3; ++axis) {
				var zeroAxisFactor = SingleAxisVectorComb(axis, true);

				var edgeScale = Vector3.Scale(
					EdgePrefab.transform.localScale, 
					new Vector3(colliderSizeM[axis]/edgeLength, 1.0f, 1.0f));

				var edgeRotation = new Vector3(0.0f, 0.0f, 0.0f);
				var edgeAxialRotationModifer = Vector3.right;

				switch (axis){
					case (1):
						edgeRotation.z = -90.0f;
						edgeAxialRotationModifer = Vector3.down;
						break;
					case (2):
						edgeRotation.y = 90.0f;
						edgeAxialRotationModifer = Vector3.left;
						break;
				}

				for (int axisB = 0; axisB <= 1; ++axisB) {
					for (int axisA = 0; axisA <= 1; ++axisA) {
						var edge = Instantiate(EdgePrefab);
						edge.transform.parent = EdgeParent.transform;

						var rotationAxesFactor = MultiAxisVectorComb(axis, axisA, axisB);

						edge.transform.localPosition = 
							Vector3.Scale(
								zeroAxisFactor,
								edgePositionMin + Vector3.Scale(rotationAxesFactor, edgeMaxOffset));

						edge.transform.localScale = edgeScale;

						var edgeAxialRotation = 0.0f;

						if (1 == axisA)
						{
							edgeAxialRotation = 90.0f;

							if (1 == axisB)
							{
								edgeAxialRotation = 180.0f;
							}
						}
						else if (1 == axisB)
						{
							edgeAxialRotation = -90.0f;
						}

						edge.transform.localEulerAngles =
							edgeRotation + (edgeAxialRotation * edgeAxialRotationModifer);

						_edges.Add(edge);
					}
				}
			}
		}

		UpdateIndicatorChildren(EdgeParent);

		if (null == FaceParent)
		{
			FaceParent = this.gameObject;
		}

		if (FacePrefab != null)
		{
			for (int axis = 0; axis < 3; ++axis)
			{

				var faceScale = Vector3.Scale(
					FacePrefab.transform.localScale,
					new Vector3(
						axis == 0 ? frameSizeM.z : frameSizeM.x,
						axis == 1 ? frameSizeM.z : frameSizeM.y,
						1.0f));

				var positionFilter = new Vector3(
					Convert.ToInt32(axis == 0),
					Convert.ToInt32(axis == 1),
					Convert.ToInt32(axis == 2));

				for (int i = 0; i <= 1; ++i)
				{
					var face = Instantiate(FacePrefab);
					face.transform.parent = FaceParent.transform;

					face.transform.localPosition =
						Vector3.Scale((edgePositionMin + (i * edgeMaxOffset)), positionFilter);

					face.transform.localScale = faceScale;

					Vector3 faceRotation = Vector3.zero;
					switch (axis)
					{
						case 0:
							// x face rotation
							faceRotation = new Vector3(0.0f, -90.0f + (180.0f * i), 0.0f);
							break;
						case 1:
							// y face rotation
							faceRotation = new Vector3(90.0f + (180.0f * i), 0.0f, 0.0f);
							break;
						case 2:
							// z face rotation
							faceRotation = new Vector3(0.0f, -180.0f + (i * 180.0f), 0.0f);
							break;
					}
					face.transform.localEulerAngles = faceRotation;

					_faces.Add(face);
				}
			}
		}

		UpdateIndicatorChildren(FaceParent);
		UpdateIndicatorChildren(this.gameObject);

		// {
		// 	// Add the measurement combs
		// 	for (int axis=0; axis<3; ++axis) {

		// 		// For one axis we want to half distance to the edge of the bounding box
		// 		// plus a little bit of padding to set the ticks away from the bounding
		// 		// box...
		// 		// ...for the other axis we don't want the padding...
		// 		// ...or maybe some negative padding, who knows for the moment 


		// 		for (int rotation=0; rotation<4; ++rotation) {

		// 			float runningDistance = 0.0f;
		// 			float maximumDistance = volumeSizeM[axis] * (axis==2? 1.0 : 0.5);

		// 			var markerPosition = 

		// 			do {
		// 				// for the z axis (depth) start at the top and work down

		// 				// for the x and y axes start in the middle and move out in a symmetric way

		// 			} while ((runningDistance + 1.0f) < maximumDistance)

		// 		}
		// 	}
		// }
	}

	public void ClearBoundingBox()
    {
        foreach (var gameObject in _corners) {
            Destroy(gameObject);
        }

        foreach (var gameObject in _edges)
        {
            Destroy(gameObject);
        }

        foreach (var gameObject in _faces)
        {
            Destroy(gameObject);
        }

        _corners.Clear();
        _edges.Clear();
        _faces.Clear();

    }

	// Make a filter comb where only the selected axis is on (or off if it is inverted)
	private Vector3 SingleAxisVectorComb(int axis, bool invert = false) {
		return new Vector3(
			Convert.ToInt32((axis==0) ^ invert), 
			Convert.ToInt32((axis==1) ^ invert), 
			Convert.ToInt32((axis==2) ^ invert));
	} 

	// Make a filter comb where only the selected axis is on (or off if it is inverted)
	private Vector3 MultiAxisVectorComb(int zeroAxis, int axisA, int axisB) {
		return new Vector3(
			zeroAxis == 0 ? 0.0f : axisA, 
			zeroAxis == 1 ? 0.0f : (zeroAxis == 0 ? axisA : axisB), 
			zeroAxis == 2 ? 0.0f : axisB);
	}

	// en/dis able all renderers and child renderers
	private void EnableObjectAndChildRenderers(GameObject gameObject, bool enabled)
	{
		var renderers = gameObject.GetComponentsInChildren<Renderer>();
		foreach (var renderer in renderers)
		{
			renderer.enabled = enabled;
		}
	}

	private void UpdateIndicatorChildren(GameObject gameObjectToUpdate)
	{
		if (null != gameObjectToUpdate)
		{
			var indicatorChildren = gameObjectToUpdate.GetComponent<IndicatorChildren>();
			if (null != indicatorChildren)
			{
				indicatorChildren.UpdateChildIndicators();
			}
		}
	}
}
