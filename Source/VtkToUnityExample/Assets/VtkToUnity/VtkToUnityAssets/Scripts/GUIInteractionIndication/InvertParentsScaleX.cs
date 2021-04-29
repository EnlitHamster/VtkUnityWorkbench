using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvertParentsScaleX : InvertParentsScaleBase
{
	// Update is called once per frame
	protected override void UpdateImpl()
	{
		transform.localScale = new Vector3(
			(1.0f / _referenceTransform.localScale.x),
			transform.localScale.y,
			transform.localScale.z);
	}
}
