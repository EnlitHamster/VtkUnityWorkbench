using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ViveControllerToolBase : MonoBehaviour {

    public GameObject DefaultIconPrefab;

    public SteamVR_Action_Boolean BooleanAction = 
        SteamVR_Input.GetAction<SteamVR_Action_Boolean>("InteractSceneObject");
    public SteamVR_Action_Vibration HapticAction = 
        SteamVR_Input.GetAction<SteamVR_Action_Vibration>("VibrationHapticFeedback");
    public SteamVR_Input_Sources InputSource = SteamVR_Input_Sources.Any;

    protected GameObject _iconParent;
    protected GameObject _icon;
	//protected int _iconCloneIndex = 0;

	protected bool _active;

    protected string _id = "XX";
    public string Id
    {
        get
        {
            return _id;
        }
        //set
        //{
        //    _id = value;
        //}
    
    }

    protected string _zone = "";
    public string Zone
    {
        get
        {
            return _zone;
        }
        //set
        //{
        //    _id = value;
        //}

    }

    public virtual bool Busy()
	{
		return false;
	}

    void Awake()
    {
        var iconParentTransform = this.transform.Find("IconParent");
        if (iconParentTransform)
        {
            _iconParent = iconParentTransform.gameObject;
        }

        Activate();
    }

    protected virtual void OnEnable()
    {
        if (null != BooleanAction)
        {
            BooleanAction.AddOnStateDownListener(OnInteractPressed, InputSource);
            BooleanAction.AddOnStateUpListener(OnInteractReleased, InputSource);
        }

        var steamPoseBehaviour = GetComponentInParent<SteamVR_Behaviour_Pose>();
        if (null != steamPoseBehaviour)
        {
            InputSource = steamPoseBehaviour.inputSource;
        }
    }

    protected virtual void OnDisable()
    {
        if (null != BooleanAction)
        {
            BooleanAction.RemoveOnStateDownListener(OnInteractPressed, InputSource);
            BooleanAction.RemoveOnStateUpListener(OnInteractReleased, InputSource);
        }
    }

    // to be overridden by derived classes
    public virtual void Activate()
    {
        DeleteAllIconChildren();
        _icon = AddIconPrefab(DefaultIconPrefab);
		_icon.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

        gameObject.layer = LayerMask.NameToLayer("Physics Controller");

		_active = true;
	}

    public virtual void DeActivate()
	{
		_active = false;
	}

    protected void DeleteAllIconChildren()
    {
        // destroy any existing children
        foreach (Transform child in _iconParent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    protected GameObject AddIconPrefab(GameObject iconPrefab)
    {
        //DeleteAllIconChildren();

        if (_iconParent && iconPrefab)
        {
            var icon = Instantiate(iconPrefab);
            icon.transform.parent = _iconParent.transform;
            icon.transform.localPosition = Vector3.zero;

            var iconIdBase = icon.GetComponentInChildren<IconIdBase>();
            if (!iconIdBase)
            {
                return null;
            }

            icon.name = icon.name + " " + iconIdBase.Handle;
            return icon;
        }

        return null;
    }

	public void OnTriggerEnter(Collider other)
	{
		if (!_active)
		{
			return;
		}

		OnTriggerEnterImpl(other);
	}

	protected virtual void OnTriggerEnterImpl(Collider other)
	{

	}

	public void OnTriggerStay(Collider other)
	{
		if (!_active)
		{
			return;
		}

		OnTriggerStayImpl(other);
	}

	protected virtual void OnTriggerStayImpl(Collider other)
	{

	}

	public void OnTriggerExit(Collider other)
	{
        // this is commented out because of #146
		//if (!_active)
		//{
		//	return;
		//}

		OnTriggerExitImpl(other);
	}

	protected virtual void OnTriggerExitImpl(Collider other)
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (!_active)
		{
			return;
		}

		UpdateImpl();
	}

	protected virtual void UpdateImpl()
	{

	}

    private void OnInteractPressed(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) //) //SteamVR_Action_In actionIn)
    {
        if (!_active)
        {
            return;
        }

        OnInteractPressedImpl(fromAction, fromSource);
    }

    protected virtual void OnInteractPressedImpl(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {

    }

    private void OnInteractReleased(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) //) //SteamVR_Action_In actionIn)
    {
        if (!_active)
        {
            return;
        }

        OnInteractReleasedImpl(fromAction, fromSource);
    }

    protected virtual void OnInteractReleasedImpl(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {

    }

    protected void StandardHapticBuzz()
    {
        float duration = 0.0005f;
        float frequency = 100.0f;
        float amplitude = 1.0f;
        HapticAction.Execute(0, duration, frequency, amplitude, InputSource); // handType);
    }

    protected void HapticBuzz(float durationS, float frequencyHz, float amplitude)
    {
        if (null != HapticAction)
        {
            HapticAction.Execute(0, durationS, frequencyHz, amplitude, InputSource); // handType);
        }
    }
}
