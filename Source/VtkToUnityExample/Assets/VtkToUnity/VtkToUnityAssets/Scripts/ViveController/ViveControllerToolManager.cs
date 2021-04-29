using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViveControllerToolManager : MonoBehaviour
{
	private ViveControllerToolBase[] _tools;
	
	private string _mode = "XX";
	private string _requestedMode = "XX";

	public string DefaultMode = "MO";

	void Awake()
	{
		_tools = GetComponents<ViveControllerToolBase>();

		// This is hard coded on the assumption we always have a move tool 
		// finding a more generic way of naming it would be good
		UpdateMode(DefaultMode);
	}

	public void OnTriggerEnter(Collider other)
	{
		//Debug.Log("ToolManager: OnTriggerEnter: Start: " + LayerMask.LayerToName(other.gameObject.layer));
		var zoneTool = GetToolForZone(other.gameObject.layer);
		if (zoneTool &&
			!CurrentTool().Busy() &&
			!CurrentToolIsZoneTool())
		{
			//Debug.Log("ToolManager: OnTriggerEnter: Updating to zone tool: " + zoneTool.Id + ", " + zoneTool.Zone);
			_requestedMode = _mode;
			UpdateMode(zoneTool.Id);
		}
		//Debug.Log("ToolManager: OnTriggerEnter: End: " + LayerMask.LayerToName(other.gameObject.layer));
	}

	//public void OnTriggerStay(Collider other)
	//{
	//	if (_moveTool != null && _guiPresserTool != null)
	//	{

	//	}
	//}

	public void OnTriggerExit(Collider other)
	{
		if (CurrentToolIsZoneTool() &&
			LayerMask.LayerToName(other.gameObject.layer) == CurrentTool().Zone)
		{
			UpdateMode(_requestedMode);
		}
	}

	// Use this for initialization
	//void Start()
	//{

	//}

	//// Update is called once per frame
	//void Update () {

	//}

	//public void SelectMoveMode()
	//{
	//	if (!CurrentToolIsZoneTool())
	//	{
	//		UpdateMode("MO");
	//		return;
	//	}

	//	_requestedMode = "MO";
	//}

	public void SelectMode(string mode)
	{
		//Debug.Log("ToolManager: SelectMode: Start: " + mode);
		if (CurrentToolIsZoneTool())
		{
			//Debug.Log("ToolManager: SelectMode: Requesting Mode: " + mode);
			_requestedMode = mode;
			return;
		}

		//Debug.Log("ToolManager: SelectMode: Updating to Mode: " + mode);
		UpdateMode(mode);
		//Debug.Log("ToolManager: SelectMode: End");
	}

	private void UpdateMode(string requestedMode)
	{
		//Debug.Log("ToolManager: UpdateMode: Start: " + requestedMode);
		if (_mode == requestedMode)
		{
			//Debug.Log("ToolManager: UpdateMode: Early Exit: " + _mode + " == " + requestedMode);
			return;
		}

		//Debug.Log("ToolManager: UpdateMode: DeActivating Tools");
		foreach (var tool in _tools)
		{
			if (tool)
			{
				tool.DeActivate();
			}
		}

		{
			var tool = GetTool(requestedMode);
			if (!tool)
			{
				//Debug.Log("ToolManager: UpdateMode: Requested tool found, using default: " + DefaultMode);
				// fall back to move if line measurement is not available and asked for
				requestedMode = DefaultMode;
			}

			//Debug.Log("ToolManager: UpdateMode: Activating Tool");
			tool.Activate();
			_mode = requestedMode;
		}
		//Debug.Log("ToolManager: UpdateMode: End");
	}

	private ViveControllerToolBase CurrentTool()
	{
		return GetTool(_mode);
	}

	private bool CurrentToolIsZoneTool()
	{
		return (!string.IsNullOrEmpty(CurrentTool().Zone));
	}

	private ViveControllerToolBase GetTool(string toolId)
	{
		return Array.Find(_tools, x => x.Id == toolId);
	}

	private ViveControllerToolBase GetToolForZone(int layer)
	{
		return Array.Find(_tools, x => x.Zone == LayerMask.LayerToName(layer));
	}
}
