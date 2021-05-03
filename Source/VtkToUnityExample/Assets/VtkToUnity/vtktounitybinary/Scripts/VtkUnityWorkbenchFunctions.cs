using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

using ThreeDeeHeartPlugins;
using VtkUnityWorkbench;

namespace VtkUnityWorkbench
{
    public static class VtkUnityWorkbenchPlugin
    {
        private static Dictionary<string, IComponentFactory> sComponentFactories;

        ///////////////////////////////////////////////////
        // Getter for primitive attributes
        public static T GetProperty<T>(
            int shapeId,
            string propertyName)
            where T : IConvertible
        {
            StringBuilder buffer = new StringBuilder();
            VtkToUnityPlugin.GetShapePrimitiveProperty(shapeId, propertyName, buffer);

            if (buffer.ToString().StartsWith("err"))
            {
                string msg = buffer.ToString().Replace("err::(", "").Replace(")", "");
                throw new VtkUnityFetchException(msg);
            }
            else
            {
                string val = buffer.ToString().Replace("val::(", "").Replace(")", "");
                return VtkUnityWorkbenchHelpers.StringTo<T>(val);
            }
        }

        ///////////////////////////////////////////////////
        // Setter for primitive attributes
        public static void SetProperty<T>(
            int shapeId,
            string propertyName,
            T newValue)
            where T : IConvertible
        {
            try
            {
                string strNewValue = (string)Convert.ChangeType(newValue, typeof(string));
                VtkToUnityPlugin.SetShapePrimitiveProperty(shapeId, propertyName, strNewValue);
            }
            catch (Exception)
            {
                throw new VtkUnityConversionException("string", typeof(T).ToString());
            }
        }

        ///////////////////////////////////////////////////
        // Registration to the plugin for UI call
        public static void RegisterComponentFactory(
            string callbackComponent, 
            IComponentFactory factory)
        {
            if (sComponentFactories == null)
            {
                sComponentFactories = new Dictionary<string, IComponentFactory>();
            }

            sComponentFactories.Add(callbackComponent, factory);
        }

        ///////////////////////////////////////////////////
        // Shows the UI for the registered component
        public static void ShowComponentFor(
            string callbackComponent)
        {
            if (sComponentFactories != null)
            {
                if (sComponentFactories.ContainsKey(callbackComponent))
                {
                    sComponentFactories[callbackComponent].Show();
                }
                else
                {
                    throw new VtkUnityComponentNotFoundException(callbackComponent);
                }
            }
        }

        ///////////////////////////////////////////////////
        // Destroys the UI for the registered component
        public static void DestroyComponentFor(
            string callbackComponent)
        {
            if (sComponentFactories != null)
            {
                if (sComponentFactories.ContainsKey(callbackComponent))
                {
                    sComponentFactories[callbackComponent].Destroy();
                }
                else
                {
                    throw new VtkUnityComponentNotFoundException(callbackComponent);
                }
            }
        }
    }
}
