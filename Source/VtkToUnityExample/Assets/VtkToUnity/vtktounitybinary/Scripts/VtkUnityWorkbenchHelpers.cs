using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VtkUnityWorkbench
{
    public static T StringTo<T>(string val)
    {
        if (typeof(T) == typeof(double))
        {
            return Convert.ChangeType(Convert.ToDouble(val), typeof(T));
        }
    }
}