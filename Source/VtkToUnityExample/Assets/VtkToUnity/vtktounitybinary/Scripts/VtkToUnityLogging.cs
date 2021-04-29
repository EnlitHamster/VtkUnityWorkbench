using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

using ThreeDeeHeartPlugins;

public class VtkToUnityLogging : MonoBehaviour
{
    // Hook up the debug string callback, and define the callback for it
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void DebugLogDelegate(int level, string message);

    static private DebugLogDelegate _callbackDelegate;
    static IntPtr _intptrDelegate = IntPtr.Zero;

    static private string _lastImmediateMessage = "flip flops";

    static void DebugCallBackFunction(int level, string message)
    {
        _lastImmediateMessage = "No Last Message";

        if ((int)VtkToUnityPlugin.DebugLogLevel.DebugImmediate == level)
        {
            _lastImmediateMessage = message;
        }
        else if ((int)VtkToUnityPlugin.DebugLogLevel.DebugLog == level)
        {
            Debug.Log("Log::VtkToUnity:: " + message);
        }
        else if ((int)VtkToUnityPlugin.DebugLogLevel.DebugLogWarning == level)
        {
            Debug.LogWarning("Log::VtkToUnity:: " + message);
        }
        else //if ((int)RenderingPlugin.DebugLogLevel.DebugLogError == level)
        {
            Debug.LogError("Log::VtkToUnity:: " + message);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        if (_callbackDelegate is null && _intptrDelegate == IntPtr.Zero)
        {
            // dll debug
            _callbackDelegate = new DebugLogDelegate(DebugCallBackFunction);
            // Convert callback_delegate into a function pointer that can be
            // used in unmanaged code.
            _intptrDelegate = Marshal.GetFunctionPointerForDelegate(_callbackDelegate);
            // Call the API passing along the function pointer.
            VtkToUnityPlugin.SetDebugFunction(_intptrDelegate);
        }
    }

    private void OnDestroy()
    {
        if (!(_callbackDelegate is null) && _intptrDelegate != IntPtr.Zero)
        {
            VtkToUnityPlugin.SetDebugFunction(IntPtr.Zero);
            _intptrDelegate = IntPtr.Zero;
            _callbackDelegate = null;
        }
    }

    // GW - commenting this out, as it may be causing Unity to lock up
    // Will create an issue to investigate further
    //public void OnGUI()
    //{
    //    int guiTextPos = Screen.height - 25;

    //    GUI.Label(
    //        new Rect(0, guiTextPos, 200, guiTextPos + 25),
    //        _lastImmediateMessage);
    //}
}
