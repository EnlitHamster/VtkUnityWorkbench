using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvertParentsScaleYZ : InvertParentsScaleBase
{
	// Update is called once per frame
	protected override void UpdateImpl()
	{
		transform.localScale = new Vector3(
			transform.localScale.x,
			(1.0f / _referenceTransform.localScale.y),
			(1.0f / _referenceTransform.localScale.z));
	}
}
