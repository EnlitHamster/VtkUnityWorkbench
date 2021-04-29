using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorParent : IndicatorBase
{
	public override IndicateState Indicate
	{
		set
		{
			if (transform.parent)
			{
				var parentIndicator = transform.parent.GetComponent<IndicatorBase>();

				if (parentIndicator)
				{
					parentIndicator.Indicate = value;
				}
			}

			base.Indicate = value;
		}
	}
}
