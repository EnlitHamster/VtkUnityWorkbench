using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorColorVisible : IndicatorBase {

	public Color ColorOff = Color.grey;
    public bool VisibleOff = true;
	public Color ColorHighlight = Color.green;
    public bool VisibleHighlight = true;
    public Color ColorActive = Color.yellow;
    public bool VisibleActive = true;

    public override IndicateState Indicate
	{
		set {
			{
                Renderer renderer = this.GetComponent<Renderer>();

                if (renderer) {
					switch (value) {
						case IndicateState.Active:
							renderer.material.color = ColorActive;
							renderer.enabled = VisibleActive;
							break;
						case IndicateState.Highlight:
							renderer.material.color = ColorHighlight;
							renderer.enabled = VisibleHighlight;
							break;
						default:
							renderer.material.color = ColorOff;
							renderer.enabled = VisibleOff;
							break;
					}
				}
			}
			
			base.Indicate = value;
		}
	}
}
