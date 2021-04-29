using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

using ThreeDeeHeartPlugins;
using VtkUnityWorkbench;

namespace VtkUnityWorkbench
{
    public class VtkUnityWorkbenchPlugin
    {
        ///////////////////////////////////////////////////
        // Getter for primitive attributes
        public T GetProperty<T>(
            int shapeId,
            string propertyName)
        {
            StringBuilder buffer = new StringBuilder();
            VtkToUnityPlugin.GetShapePrimitiveProperty(shapeId, propertyName, buffer);

            if (buffer.ToString().StartsWith("err"))
            {
                string msg = buffer.ToString().Replace("err::(", "").Replace(")", "");
                throw new VtkUnityWorkbenchException(msg);
            }
            else
            {
                string val = buffer.ToString().Replace("val::(", "").Replace(")", "");
                return StringTo<T>(val);
            }
        }
    }

    [Serializable]
    public class VtkUnityWorkbenchException : Exception
    {
        public VtkUnityWorkbenchException() { }
        public VtkUnityWorkbenchException(string msg) : base(msg) { }
    }
}
