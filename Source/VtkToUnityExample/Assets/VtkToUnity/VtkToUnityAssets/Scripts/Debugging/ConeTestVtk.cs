using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

using ThreeDeeHeartPlugins;

public class ConeTestVtk : MonoBehaviour
{
    [Range(0.001f, 1.0f)]
    public float ConeScale = 0.5f;

    struct IdPosition
    {
        public int Id;
        public VtkToUnityPlugin.Float4 PositionScale;
    }

    private List<IdPosition> _shapeIdPositions = new List<IdPosition>();

    // Start is called before the first frame update
    void Start()
    {
        const int cone = 2;
        const bool wireframe = false;

        Vector3 scale = new Vector3(ConeScale, ConeScale, ConeScale);
        Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        Matrix4x4 transform = Matrix4x4.identity;

        VtkToUnityPlugin.Float4 colour = new VtkToUnityPlugin.Float4();
        colour.SetXYZW(0.0f, 0.0f, 1.0f, 1.0f);

        int id = VtkToUnityPlugin.AddShapePrimitive(cone, colour, wireframe);

        IdPosition idPosition = new IdPosition();
        idPosition.Id = id;
        idPosition.PositionScale.x = 0.0f;
        idPosition.PositionScale.y = 0.0f;
        idPosition.PositionScale.z = 0.0f;
        idPosition.PositionScale.w = ConeScale;

        Vector3 translation = new Vector3(0.0f, 0.0f, 0.0f);
        transform.SetTRS(translation, rotation, scale);

        VtkToUnityPlugin.Float16 transformArray = VtkToUnityPlugin.UnityMatrix4x4ToFloat16(transform);
        VtkToUnityPlugin.SetProp3DTransform(id, transformArray);

        _shapeIdPositions.Add(idPosition);

        VtkToUnityPlugin.SetShapePrimitiveProperty(id, "Height", 0.5f.ToString());

        var buffer = new StringBuilder();
        VtkToUnityPlugin.GetShapePrimitiveProperty(id, "Height", buffer);
        Debug.Log("Height: " + buffer);

        buffer.Clear();
        VtkToUnityPlugin.GetShapePrimitiveProperty(id, "Radius", buffer);
        Debug.Log("Radius: " + buffer);

        buffer.Clear();
        VtkToUnityPlugin.GetShapePrimitiveProperty(id, "Resolution", buffer);
        Debug.Log("Resolution: " + buffer);

        buffer.Clear();
        VtkToUnityPlugin.GetShapePrimitiveProperty(id, "Angle", buffer);
        Debug.Log("Angle: " + buffer);

        buffer.Clear();
        VtkToUnityPlugin.GetShapePrimitiveProperty(id, "Capping", buffer);
        Debug.Log("Capping: " + buffer);

        buffer.Clear();
        VtkToUnityPlugin.GetShapePrimitiveProperty(id, "Center", buffer);
        Debug.Log("Center: " + buffer);

        buffer.Clear();
        VtkToUnityPlugin.GetShapePrimitiveProperty(id, "Direction", buffer);
        Debug.Log("Direction: " + buffer);
    }

    void OnDestroy()
    {
        Debug.Log("ConeTestVtk::OnDestroy");
        foreach (var idPosition in _shapeIdPositions)
        {
            VtkToUnityPlugin.RemoveProp3D(idPosition.Id);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion shapeRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        Matrix4x4 transformMatrix = Matrix4x4.identity;

        Matrix4x4 parentTransformMatrix = transform.localToWorldMatrix;

        foreach (var idPosition in _shapeIdPositions)
        {
            Vector3 scale = new Vector3(
                idPosition.PositionScale.w,
                idPosition.PositionScale.w,
                idPosition.PositionScale.w
            );

            Vector3 translation = new Vector3(
                idPosition.PositionScale.x,
                idPosition.PositionScale.y,
                idPosition.PositionScale.z
            );

            transformMatrix.SetTRS(translation, shapeRotation, scale);
            transformMatrix = parentTransformMatrix * transformMatrix;

            VtkToUnityPlugin.Float16 transformArray = VtkToUnityPlugin.UnityMatrix4x4ToFloat16(transformMatrix);
            VtkToUnityPlugin.SetProp3DTransform(idPosition.Id, transformArray);
        }
    }
}
