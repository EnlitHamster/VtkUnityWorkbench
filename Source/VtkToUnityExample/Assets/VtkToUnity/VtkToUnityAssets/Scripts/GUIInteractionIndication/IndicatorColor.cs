using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorColor : IndicatorBase {

	public Color ColorOff = Color.grey;
	public Color ColorHighlight = Color.green;
	public Color ColorActive = Color.yellow;

	public override IndicateState Indicate
	{
		set {
			{
				Renderer renderer = this.GetComponent<Renderer>();

				if (renderer) {
					switch (value) {
						case IndicateState.Active:
							renderer.material.color = ColorActive;
							break;
						case IndicateState.Highlight:
							renderer.material.color = ColorHighlight;
							break;
						default:
							renderer.material.color = ColorOff;
							break;
					}
				}
			}

			base.Indicate = value;
		}
	}
}
