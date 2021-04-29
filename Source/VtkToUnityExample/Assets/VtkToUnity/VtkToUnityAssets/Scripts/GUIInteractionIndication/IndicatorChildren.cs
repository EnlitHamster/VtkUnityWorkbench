using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorChildren : IndicatorBase
{
	List<IndicatorBase> _childIndicators = new List<IndicatorBase>();

	void Start()
	{
		UpdateChildIndicators();
	}

	public override IndicateState Indicate
	{
		set
		{
			foreach (var childIndicator in _childIndicators)
			{
				childIndicator.Indicate = value;
			}

			base.Indicate = value;
		}
	}

	public void UpdateChildIndicators()
	{
		_childIndicators.Clear();
		GetAllChildIndicators(transform);
	}

	private void GetAllChildIndicators(Transform transformIn)
	{
		foreach (Transform childTransform in transformIn)
		{
			var childIndicators = childTransform.GetComponents<IndicatorBase>();

			bool iterateToChild = true;

			foreach (var childIndicator in childIndicators)
			{
				if (childIndicator && childIndicator.GetType() == typeof(IndicatorChildren))
				{
					iterateToChild = false;
					continue;
				}

				if (childIndicator.GetType() == typeof(IndicatorParent))
				{
					continue;
				}

				_childIndicators.Add(childIndicator);
			}

			if (iterateToChild)
			{
				GetAllChildIndicators(childTransform);
			}
		}
	}
}
