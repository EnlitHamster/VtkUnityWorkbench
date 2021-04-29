using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace ThreeDeeHeartPlugins
{
    public static class VtkToUnityPlugin //: MonoBehaviour
    {
        // array of floats (for rgba as well as xyz)
        public struct Float4
        {
            public float x;
            public float y;
            public float z;
            public float w;

            public Float4(float xIn, float yIn, float zIn, float wIn)
            {
                x = xIn;
                y = yIn;
                z = zIn;
                w = wIn;
            }

            public void SetXYZW(float xIn, float yIn, float zIn, float wIn)
            {
                x = xIn;
                y = yIn;
                z = zIn;
                w = wIn;
            }

            // for debugging
            public override String ToString()
            {
                return "{" + x + "," + y + "," + z + "," + z + "}";
            }
        }

        public static Vector3 Float4ToVector3(Float4 inF4)
        {
            return new Vector3(inF4.x, inF4.y, inF4.z);
        }

        // array of 16 floats to pass 4x4 transform matrices
        [StructLayout(LayoutKind.Sequential)]
        public struct Float16
        {
            [MarshalAsAttribute(UnmanagedType.LPArray, SizeConst = 16)]
            public float[] elements;
        }

        public static Float16 UnityMatrix4x4ToFloat16(Matrix4x4 unityMatrix)
        {
            VtkToUnityPlugin.Float16 pluginMatrix = new VtkToUnityPlugin.Float16()
            {
                elements = new float[16]
            };

            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    pluginMatrix.elements[(row * 4) + col] = unityMatrix[row, col];
                }
            }

            return pluginMatrix;
        }

        public static Float16 UnityMatrix4x4ToFloat16ColMajor(Matrix4x4 unityMatrix)
        {
            VtkToUnityPlugin.Float16 pluginMatrix = new VtkToUnityPlugin.Float16()
            {
                elements = new float[16]
            };

            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    pluginMatrix.elements[(col * 4) + row] = unityMatrix[row, col];
                }
            }

            return pluginMatrix;
        }

        public enum DebugLogLevel
        {
            DebugImmediate = 0,
            DebugLog,
            DebugLogWarning,
            DebugLogError
        };

        // enumerations for lighting
        public enum LightColorType
        {
            LightColorAmbient = 0,
            LightColorDiffuse,
            LightColorSpecular,
            NLightColorType
        }

        public enum VolumeLightType
        {
            VolumeLightAmbient = 0,
            VolumeLightDiffuse,
            VolumeLightSpecular,
            VolumeLightSpecularPower,
            NVolumeLightType
        }

        // Hook up the debug string callback, and define the callback for it
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DebugDelegate(string str);

        public static void CallBackFunction(string str)
        {
            Debug.Log("::CallBack : " + str);
        }

#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
		[DllImport ("__Internal")]
#else
        [DllImport("VtkToUnityPlugin")]
#endif
        public static extern void SetDebugFunction(IntPtr fp);

        // Native plugin rendering events are only called if a plugin is used
        // by some script. This means we have to DllImport at least
        // one function in some active script.

        // Load in a DICOM volume 
#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
		[DllImport ("__Internal")]
#else
        [DllImport("VtkToUnityPlugin")]
#endif
        public static extern bool LoadDicomVolume(
            string dicomFolder);

        // Load in a MHD volume 
#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
		[DllImport ("__Internal")]
#else
        [DllImport("VtkToUnityPlugin")]
#endif
        public static extern bool LoadMhdVolume(
            string mhdPath);

        // Load in an Nrrd volume 
#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
		[DllImport ("__Internal")]
#else
        [DllImport("VtkToUnityPlugin")]
#endif
        public static extern bool LoadNrrdVolume(
            string nrrdPath);

        // Load in an Nrrd volume 
#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
		[DllImport ("__Internal")]
#else
        [DllImport("VtkToUnityPlugin")]
#endif
        public static extern bool CreatePaddingMask(
            int paddingValue);

        // Clear all loaded volumes
#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
		[DllImport ("__Internal")]
#else
        [DllImport("VtkToUnityPlugin")]
#endif
        public static extern void ClearVolumes();

#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
		[DllImport ("__Internal")]
#else
        [DllImport("VtkToUnityPlugin")]
#endif
        public static extern int GetNVolumes();

#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
		[DllImport ("__Internal")]
#else
        [DllImport("VtkToUnityPlugin")]
#endif
        public static extern Float4 GetVolumeSpacingM();

#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
		[DllImport ("__Internal")]
#else
        [DllImport("VtkToUnityPlugin")]
#endif
        public static extern Float4 GetVolumeExtentsMin();

#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
		[DllImport ("__Internal")]
#else
        [DllImport("VtkToUnityPlugin")]
#endif
        public static extern Float4 GetVolumeExtentsMax();

#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
		[DllImport ("__Internal")]
#else
        [DllImport("VtkToUnityPlugin")]
#endif
        public static extern Float4 GetVolumeOriginM();

#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
		[DllImport ("__Internal")]
#else
        [DllImport("VtkToUnityPlugin")]
#endif
        public static extern int AddVolumeProp();


#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
		[DllImport ("__Internal")]
#else
        [DllImport("VtkToUnityPlugin")]
#endif
        public static extern int AddCropPlaneToVolume(int volumeId);


#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
		[DllImport ("__Internal")]
#else
        [DllImport("VtkToUnityPlugin")]
#endif
        public static extern void SetVolumeIndex(int index);


#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
		[DllImport ("__Internal")]
#else
        [DllImport("VtkToUnityPlugin")]
#endif
        public static extern int GetNTransferFunctions();


#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
		[DllImport ("__Internal")]
#else
        [DllImport("VtkToUnityPlugin")]
#endif
        public static extern int GetTransferFunctionIndex();


#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
		[DllImport ("__Internal")]
#else
        [DllImport("VtkToUnityPlugin")]
#endif
        public static extern void SetTransferFunctionIndex(int index);


#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
		[DllImport ("__Internal")]
#else
        [DllImport("VtkToUnityPlugin")]
#endif
        public static extern int AddTransferFunction();


#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
		[DllImport ("__Internal")]
#else
        [DllImport("VtkToUnityPlugin")]
#endif
        public static extern int ResetTransferFunctions();


#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
		[DllImport ("__Internal")]
#else
        [DllImport("VtkToUnityPlugin")]
#endif
        public static extern void SetTransferFunctionPoint(
            int transferFunctionIndex,
            double windowFraction,
            double red1,
            double green1,
            double blue1,
            double opacity1);


#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
		[DllImport ("__Internal")]
#else
        [DllImport("VtkToUnityPlugin")]
#endif
        public static extern void SetVolumeWWWL(float windowWidth, float windowLevel);


#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
		[DllImport ("__Internal")]
#else
        [DllImport("VtkToUnityPlugin")]
#endif
        public static extern void SetVolumeOpacityFactor(float opacityFactor);


#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
		[DllImport ("__Internal")]
#else
        [DllImport("VtkToUnityPlugin")]
#endif
        public static extern void SetVolumeBrightnessFactor(float brightnessFactor);


#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
		[DllImport ("__Internal")]
#else
        [DllImport("VtkToUnityPlugin")]
#endif
        public static extern void SetRenderComposite(bool composite);


#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
		[DllImport ("__Internal")]
#else
        [DllImport("VtkToUnityPlugin")]
#endif
        public static extern void SetTargetFrameRateOn(bool targetOn);


#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
		[DllImport ("__Internal")]
#else
        [DllImport("VtkToUnityPlugin")]
#endif
        public static extern void SetTargetFrameRateFps(int targetFps);


#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
		[DllImport ("__Internal")]
#else
        [DllImport("VtkToUnityPlugin")]
#endif
        public static extern int AddMPR(int existingMprId);


#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
		[DllImport ("__Internal")]
#else
        [DllImport("VtkToUnityPlugin")]
#endif
        public static extern int AddMPRFlipped(int existingMprId, int flipAxis);


#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
		[DllImport ("__Internal")]
#else
        [DllImport("VtkToUnityPlugin")]
#endif
        public static extern void SetMPRWWWL(float windowWidth, float windowLevel);


#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
		[DllImport ("__Internal")]
#else
        [DllImport("VtkToUnityPlugin")]
#endif
        public static extern int AddShapePrimitive(
            int shapeType,
            Float4 rgbaColour,
            bool wireframe);


#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
        [DllImport ("__Internal")]
#else
        [DllImport("VtkToUnityPlugin", CallingConvention = CallingConvention.Cdecl)]
#endif
        public static extern void GetShapePrimitiveProperty(
            int shapeId,
            [MarshalAs(UnmanagedType.LPStr)] string propertyName,
            StringBuilder retValue);


#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
        [DllImport ("__Internal")]
#else
        [DllImport("VtkToUnityPlugin", CallingConvention = CallingConvention.Cdecl)]
#endif
        public static extern void SetShapePrimitiveProperty(
            int shapeId,
            [MarshalAs(UnmanagedType.LPStr)] string propertyName,
            [MarshalAs(UnmanagedType.LPStr)] string newValue);


#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
		[DllImport ("__Internal")]
#else
        [DllImport("VtkToUnityPlugin")]
#endif
        public static extern int AddLight();


#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
		[DllImport ("__Internal")]
#else
        [DllImport("VtkToUnityPlugin")]
#endif
        public static extern void SetLightingOn(
            bool lightingOn);


#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
		[DllImport ("__Internal")]
#else
        [DllImport("VtkToUnityPlugin")]
#endif
        public static extern void SetLightColor(
            int id,
            LightColorType lightColorType,
            Float4 rgbColor);



#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
		[DllImport ("__Internal")]
#else
        [DllImport("VtkToUnityPlugin")]
#endif
        public static extern void SetLightIntensity(
            int id,
            float intensity);


#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
		[DllImport ("__Internal")]
#else
        [DllImport("VtkToUnityPlugin")]
#endif
        public static extern void SetVolumeLighting(
            VolumeLightType volumeLightType,
            float lightValue);


#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
		[DllImport ("__Internal")]
#else
        [DllImport("VtkToUnityPlugin")]
#endif
        public static extern void RemoveProp3D(int id);


#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
		[DllImport ("__Internal")]
#else
        [DllImport("VtkToUnityPlugin")]
#endif
        public static extern void SetProp3DTransform(
            int id,
            Float16 transformWorldM);


#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
		[DllImport ("__Internal")]
#else
        [DllImport("VtkToUnityPlugin")]
#endif
        public static extern void SetMPRTransform(
            int id,
            Float16 transformVolumeM);


#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
		[DllImport ("__Internal")]
#else
        [DllImport("VtkToUnityPlugin")]
#endif
        // private static extern void SetViewMatrix(System.IntPtr view4x4);
        public static extern void SetViewMatrix(Float16 view4x4);


        // Set the camera Projection matrix
#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
		[DllImport ("__Internal")]
#else
        [DllImport("VtkToUnityPlugin")]
#endif
        public static extern void SetProjectionMatrix(Float16 projection4x4);


#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
		[DllImport ("__Internal")]
#else
        [DllImport("VtkToUnityPlugin")]
#endif
        public static extern IntPtr GetRenderEventFunc();

#if UNITY_WEBGL && !UNITY_EDITOR
		[DllImport ("__Internal")]
		public static extern void RegisterPlugin();
#endif

    }
}