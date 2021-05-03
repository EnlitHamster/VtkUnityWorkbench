using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

using ThreeDeeHeartPlugins;
using VtkUnityWorkbench;

class VtkConeSourceUIFactory : IComponentFactory
{
    public void Destroy()
    {
        // TODO: implement UI hiding routine



        throw new NotImplementedException();
    }

    public void Show()
    {
        // TODO: implement UI showing routine



        throw new NotImplementedException();
    }
}

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

        VtkUnityWorkbenchPlugin.SetProperty<double>(id, "Height", 0.5f);
        VtkUnityWorkbenchPlugin.SetProperty(id, "Resolution", 200);

        var coneHeight = VtkUnityWorkbenchPlugin.GetProperty<double>(id, "Height");
        var coneRadius = VtkUnityWorkbenchPlugin.GetProperty<double>(id, "Radius");
        var coneResolution = VtkUnityWorkbenchPlugin.GetProperty<int>(id, "Resolution");
        var coneAngle = VtkUnityWorkbenchPlugin.GetProperty<double>(id, "Angle");
        var coneCapping = VtkUnityWorkbenchPlugin.GetProperty<int>(id, "Capping");
        var coneCenter = VtkUnityWorkbenchPlugin.GetProperty<Double3>(id, "Center");
        var coneDirection = VtkUnityWorkbenchPlugin.GetProperty<Double3>(id, "Direction");

        var coneFactory = new VtkConeSourceUIFactory();
        VtkUnityWorkbenchPlugin.RegisterComponentFactory("vtkConeSource", coneFactory);

        VtkUnityWorkbenchPlugin.ShowComponentFor("vtkConeSource");
    }

    void OnDestroy()
    {
        Debug.Log("ConeTestVtk::OnDestroy");
        foreach (var idPosition in _shapeIdPositions)
        {
            VtkToUnityPlugin.RemoveProp3D(idPosition.Id);
        }

        VtkUnityWorkbenchPlugin.DestroyComponentFor("vtkConeSource");
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
