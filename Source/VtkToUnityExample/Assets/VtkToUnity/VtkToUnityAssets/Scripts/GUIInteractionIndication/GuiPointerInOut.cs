using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GuiPointerInOut {

	public struct PointerEventArgs
	{
		public uint controllerIndex;
		public uint flags;
		public float distance;
		public Transform target;
	}

	public delegate void PointerEventHandler(object sender, PointerEventArgs e);


	public event PointerEventHandler PointerIn;
	public event PointerEventHandler PointerOut;

	public virtual void OnPointerIn(PointerEventArgs e)
	{
		if (PointerIn != null)
		{
			PointerIn(this, e);
		}
	}

	public virtual void OnPointerOut(PointerEventArgs e)
	{
		if (PointerOut != null)
		{
			PointerOut(this, e);
		}
	}
}
