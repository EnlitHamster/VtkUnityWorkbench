using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ThreeDeeHeartPlugins;

public class VtkVolumeRenderLighting : MonoBehaviour
{
	[Range(0.0f, 1.0f)]
	public float Ambient = 0.0f;

	[Range(0.0f, 1.0f)]
	public float Diffuse = 0.75f;

	[Range(0.0f, 1.0f)]
	public float Specular = 0.35f;

	[Range(0.0f, 50.0f)]
	public float SpecularPower = 40.0f;

	private float _oldAmbient = -0.1f;
	private float _oldDiffuse = -0.1f;
	private float _oldSpecular = -0.1f;
	private float _oldSpecularPower = -0.1f;

	// Start is called before the first frame update
	IEnumerator Start()
	{
		yield return StartCoroutine("CallPluginAtEndOfFrames");
	}

	// Update is called once per frame
	//void Update()
	//{

	//}

	private IEnumerator CallPluginAtEndOfFrames()
	{
		while (true)
		{
			yield return new WaitForEndOfFrame();

			if (_oldAmbient != Ambient)
			{
				VtkToUnityPlugin.SetVolumeLighting(
					VtkToUnityPlugin.VolumeLightType.VolumeLightAmbient,
					Ambient);
				_oldAmbient = Ambient;
			}

			if (_oldDiffuse != Diffuse)
			{
				VtkToUnityPlugin.SetVolumeLighting(
					VtkToUnityPlugin.VolumeLightType.VolumeLightDiffuse,
					Diffuse);
				_oldDiffuse = Diffuse;
			}

			if (_oldSpecular != Specular)
			{
				VtkToUnityPlugin.SetVolumeLighting(
					VtkToUnityPlugin.VolumeLightType.VolumeLightSpecular,
					Specular);
				_oldSpecular = Specular;
			}

			if (_oldSpecularPower != SpecularPower)
			{
				VtkToUnityPlugin.SetVolumeLighting(
					VtkToUnityPlugin.VolumeLightType.VolumeLightSpecularPower,
					SpecularPower);
				_oldSpecularPower = SpecularPower;
			}
		}
	}

}
