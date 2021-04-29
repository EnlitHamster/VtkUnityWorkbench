using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;


public class ViveControllerToolGuiPresser : ViveControllerToolBase {

	private GameObject _collidingObject;

	public GuiPointerInOut PointerInOut = new GuiPointerInOut();

    public ViveControllerToolGuiPresser()
    {
        _id = "UIP";
        _zone = "SwitchToUI";
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        PointerInOut.PointerIn += HandlePointerIn;
        PointerInOut.PointerOut += HandlePointerOut;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        PointerInOut.PointerIn -= HandlePointerIn;
        PointerInOut.PointerOut -= HandlePointerOut;
    }

	private void SetCollidingObject(Collider col, bool fromOnEnter = false)
    {
        GameObject colGameObject = col.gameObject;

        if (_collidingObject == colGameObject)
        {
            return;
        }

        // if we already have an object we've collided with
        if (_collidingObject)
        {
            // and we're not colliding with one of it's children
            if (!colGameObject.transform.IsChildOf(_collidingObject.transform))
            {
                return;
            }

			if (fromOnEnter)
			{
                StandardHapticBuzz();
			}
		}

		_collidingObject = colGameObject;

		GuiPointerInOut.PointerEventArgs argsIn = new GuiPointerInOut.PointerEventArgs();
		argsIn.flags = 0;
		argsIn.distance = 0.0f;
		argsIn.target = _collidingObject.transform;
		PointerInOut.OnPointerIn(argsIn);

		if (fromOnEnter)
		{
            StandardHapticBuzz();
		}
	}

	protected override void OnTriggerEnterImpl(Collider other)
    {
        SetCollidingObject(other, true);
    }

	protected override void OnTriggerStayImpl(Collider other)
    {
        SetCollidingObject(other);
    }

    protected override void OnTriggerExitImpl(Collider other)
    {
        if (!_collidingObject)
        {
            return;
        }

		//if (!_objectInHand)
		{
            StandardHapticBuzz();
		}

		_collidingObject = null;

		GuiPointerInOut.PointerEventArgs argsOut = new GuiPointerInOut.PointerEventArgs();
		argsOut.flags = 0;
		argsOut.distance = 0.0f;
		argsOut.target = null;
		PointerInOut.OnPointerOut(argsOut);
	}


    protected override void OnInteractPressedImpl(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            //Debug.Log("ViveControllerToolGuiPresser::OnInteractPressedImpl");

            ExecuteEvents.Execute(
                EventSystem.current.currentSelectedGameObject,
                new PointerEventData(EventSystem.current),
                ExecuteEvents.submitHandler);

            //EventSystem.current.SetSelectedGameObject(null);
        }
    }

    private void HandlePointerIn(object sender, GuiPointerInOut.PointerEventArgs e)
    {

        if (e.target != null && e.target.gameObject != null)
        {
            //Debug.Log("HandlePointerIn", e.target.gameObject);
            ExecuteEvents.Execute(
                e.target.gameObject,
                new PointerEventData(EventSystem.current),
                ExecuteEvents.selectHandler);

            EventSystem.current.SetSelectedGameObject(e.target.gameObject);
        }
    }

    private void HandlePointerOut(object sender, GuiPointerInOut.PointerEventArgs e)
    {
        if (e.target != null && e.target.gameObject != null)
        {
            ExecuteEvents.Execute(
                e.target.gameObject,
                new PointerEventData(EventSystem.current),
                ExecuteEvents.cancelHandler);

            //Debug.Log("HandlePointerOut", e.target.gameObject);
        }

        EventSystem.current.SetSelectedGameObject(null);
    }
}
