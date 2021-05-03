using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.ComponentModel;
using System.Globalization;

namespace VtkUnityWorkbench
{
    public static class VtkUnityWorkbenchHelpers
    {
        public static T StringTo<T>(
            string val) 
            where T : IConvertible
        {
            // Type of T to check whether it is a specialized convertible type 
            // (non-native convertible type)
            var typeT = typeof(T);
            // Type of the specialized convertible type to check for inheritance
            var intVSC = typeof(IVtkStringConvertible);

            if (intVSC.IsAssignableFrom(typeT))
            {
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
                return (T)converter.ConvertFromInvariantString(val);
            }

            try
            {
                return (T)Convert.ChangeType(val, typeof(T));
            }
            catch (Exception)
            {
                throw new VtkUnityConversionException(typeof(T).ToString(), "string");
            }
        }
    }

    [Serializable]
    public class VtkUnityConversionException : Exception
    {
        public VtkUnityConversionException() : base() { }
        public VtkUnityConversionException(string typeFrom, string typeTo) 
            : base(String.Format("Type {0} is not obtainable from {1}", typeFrom, typeTo)) { }
    }

    [Serializable]
    public class VtkUnityFetchException : Exception
    {
        public VtkUnityFetchException() { }
        public VtkUnityFetchException(string msg) : base(msg) { }
    }

    [Serializable]
    public class VtkUnityComponentNotFoundException : Exception
    {
        public VtkUnityComponentNotFoundException() { }
        public VtkUnityComponentNotFoundException(string callbackComponent) : base(
            String.Format("No registered IComponentFactory for {0}", callbackComponent)) { }
    }

    [Serializable]
    public class VtkUnityComponentAlreadyShownException : Exception
    {
        public VtkUnityComponentAlreadyShownException() { }
        public VtkUnityComponentAlreadyShownException(string callbackComponent) : base(
            String.Format("Cannot show UI for {0}, already shown", callbackComponent))
        { }
    }

    [Serializable]
    public class VtkUnityComponentNotShownException : Exception
    {
        public VtkUnityComponentNotShownException() { }
        public VtkUnityComponentNotShownException(string callbackComponent) : base(
            String.Format("Cannot destroy UI for {0}, not yet shown", callbackComponent))
        { }
    }

    [TypeConverter(typeof(Double3Converter))]
    public struct Double3 : IVtkStringConvertible
    {
        public double x;
        public double y;
        public double z;

        public Double3(double xIn, double yIn, double zIn)
        {
            x = xIn;
            y = yIn;
            z = zIn;
        }

        public void SetXYZ(double xIn, double yIn, double zIn)
        {
            x = xIn;
            y = yIn;
            z = zIn;
        }

        public TypeCode GetTypeCode()
        {
            throw new NotImplementedException();
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public byte ToByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public char ToChar(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public double ToDouble(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public short ToInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public int ToInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public long ToInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public float ToSingle(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public string ToString(IFormatProvider provider)
        {
            return String.Format("{0},{1},{2}", x, y, z);
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public Double3(string value)
        {
            string[] components = value.Split(',');
            x = Convert.ToDouble(components[0]);
            y = Convert.ToDouble(components[1]);
            z = Convert.ToDouble(components[2]);
        }
    }

    // Based on https://stackoverflow.com/questions/8003488/convert-string-to-custom-type-using-convert
    public class Double3Converter : TypeConverter
    {
        public override object ConvertFrom(
            ITypeDescriptorContext context,
            CultureInfo culture,
            object value)
        {
            string strValue = value as string;

            if (strValue != null)
            {
                return new Double3(strValue);
            }
            else
            {
                return base.ConvertFrom(context, culture, value);
            }
        }
    }

    public interface IComponentFactory
    {
        ///////////////////////////////////////////////////
        // Build, package and show the UI component on the Scene.
        void Show();

        ///////////////////////////////////////////////////
        // Remove the UI component from the Scene.
        void Destroy();
    }

    public interface IVtkStringConvertible : IConvertible { }
}