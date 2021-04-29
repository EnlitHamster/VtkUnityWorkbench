using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvertParentsScaleXYZ : InvertParentsScaleBase
{
	// Update is called once per frame
	protected override void UpdateImpl()
	{
		transform.localScale = new Vector3(
			_awakeScale.x / _referenceTransform.localScale.x,
			_awakeScale.y / _referenceTransform.localScale.y,
			_awakeScale.z / _referenceTransform.localScale.z);
	}
}
