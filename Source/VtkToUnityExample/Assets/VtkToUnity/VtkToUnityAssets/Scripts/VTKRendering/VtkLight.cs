using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ThreeDeeHeartPlugins;

public class VtkLight : MonoBehaviour
{
	public bool AmbientOn = true;
	public bool DiffuseOn = true;
	public bool SpecularOn = true;

	private Light _light;

	private int _lightId = -1;
	private Color _oldColor;
	private float _oldIntensity = -1.0f;

	private bool _oldAmbientOn = false;
	private bool _oldDiffuseOn = false;
	private bool _oldSpecularOn = false;

	private VtkToUnityPlugin.Float4 _colorOff = new VtkToUnityPlugin.Float4(0.0f, 0.0f, 0.0f, 0.0f);

	// Start is called before the first frame update
	IEnumerator Start()
    {
		if (null == (_light = GetComponent<Light>()))
		{
			yield break;
		};

		if (0 > (_lightId = VtkToUnityPlugin.AddLight()))
		{
			yield break;
		}

		yield return StartCoroutine("CallPluginAtEndOfFrames");
	}

	// Update is called once per frame
	//void Update()
	//   {

	//   }

	void OnDestroy()
	{
		VtkToUnityPlugin.RemoveProp3D(_lightId);
	}

	private IEnumerator CallPluginAtEndOfFrames()
	{
		while (true)
		{
			yield return new WaitForEndOfFrame();

			Matrix4x4 unityMatrix = transform.localToWorldMatrix;
			VtkToUnityPlugin.Float16 pluginMatrix = VtkToUnityPlugin.UnityMatrix4x4ToFloat16(unityMatrix);
			VtkToUnityPlugin.SetProp3DTransform(_lightId, pluginMatrix);

			if (_oldColor == null ||
				_oldColor != _light.color ||
				_oldAmbientOn != AmbientOn ||
				_oldDiffuseOn != DiffuseOn ||
				_oldSpecularOn != SpecularOn)
			{
				var color = new VtkToUnityPlugin.Float4(
					_light.color.r,
					_light.color.g,
					_light.color.b,
					1.0f);

				if (AmbientOn)
				{
					VtkToUnityPlugin.SetLightColor(
						_lightId,
						VtkToUnityPlugin.LightColorType.LightColorAmbient,
						color);
				}
				else
				{
					VtkToUnityPlugin.SetLightColor(
						_lightId,
						VtkToUnityPlugin.LightColorType.LightColorAmbient,
						_colorOff);
				}

				if (DiffuseOn)
				{
					VtkToUnityPlugin.SetLightColor(
						_lightId,
						VtkToUnityPlugin.LightColorType.LightColorDiffuse,
						color);
				}
				else
				{
					VtkToUnityPlugin.SetLightColor(
						_lightId,
						VtkToUnityPlugin.LightColorType.LightColorDiffuse,
						_colorOff);
				}

				if (SpecularOn)
				{
					VtkToUnityPlugin.SetLightColor(
						_lightId,
						VtkToUnityPlugin.LightColorType.LightColorSpecular,
						color);
				}
				else
				{
					VtkToUnityPlugin.SetLightColor(
						_lightId,
						VtkToUnityPlugin.LightColorType.LightColorSpecular,
						_colorOff);
				}

				_oldColor = _light.color;
				_oldAmbientOn = AmbientOn;
				_oldDiffuseOn = DiffuseOn;
				_oldSpecularOn = SpecularOn;
			}

			if (_oldIntensity != _light.intensity)
			{
				VtkToUnityPlugin.SetLightIntensity(
					_lightId,
					_light.intensity);

				_oldIntensity = _light.intensity;
			}
		}
	}
}
