using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvertParentsScaleBase : MonoBehaviour
{
	public  bool InitiallyEnabled = false;
	private bool _enabled = false;

	public bool Enabled
	{
		get
		{
			return _enabled;
		}
		set
		{
			if (_referenceTransform)
			{
				if (false == value)
				{
					_onDisableReferenceLocalScale = _referenceTransform.localScale;
				}
				else
				{
					_awakeScale.Scale( new Vector3(
						_referenceTransform.localScale.x / _onDisableReferenceLocalScale.x,
						_referenceTransform.localScale.y / _onDisableReferenceLocalScale.y,
						_referenceTransform.localScale.z / _onDisableReferenceLocalScale.z));
				}
			}
			_enabled = value;
		}
	}

	protected Vector3 _awakeScale;
	protected Transform _referenceTransform;
	private Vector3 _onDisableReferenceLocalScale = Vector3.one;

	private void Awake()
	{
		_enabled = InitiallyEnabled;
		_awakeScale = transform.localScale;
	}

	// Use this for initialization
	void Start()
	{
		var referenceScale = FindObjectOfType<ReferenceScale>();
		if (referenceScale)
		{
			_referenceTransform = referenceScale.transform;
			_onDisableReferenceLocalScale = _referenceTransform.localScale;
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (Enabled && _referenceTransform)
		{
			UpdateImpl();
		}
	}

	protected virtual void UpdateImpl()
	{

	}

	public void ResetAwakeScale()
	{
		_awakeScale = transform.localScale;
	}
}
