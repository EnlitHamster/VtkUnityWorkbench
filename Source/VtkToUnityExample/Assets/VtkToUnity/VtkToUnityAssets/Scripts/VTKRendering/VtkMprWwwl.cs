using System.Collections;
using UnityEngine;
using ThreeDeeHeartPlugins;

public class VtkMprWwwl : MonoBehaviour
{
    private const float _minWindowLevel = -1000.0f;
    private const float _maxWindowLevel = 1000.0f;
    [Range(_minWindowLevel, _maxWindowLevel)]
    public float MprWindowLevel = 105.0f;

    private const float _minWindowWidth = 1.0f;
    private const float _maxWindowWidth = 1000.0f;
    [Range(_minWindowWidth, _maxWindowWidth)]
    public float MprWindowWidth = 150.0f;

    private float _oldMprWindowLevel = 105.0f;
    private float _oldMprWindowWidth = 150.0f;

    // Use this for initialization
    protected IEnumerator Start ()
    {
        VtkToUnityPlugin.SetMPRWWWL(MprWindowWidth, MprWindowLevel);
        _oldMprWindowWidth = MprWindowWidth;
        _oldMprWindowLevel = MprWindowLevel;

        yield return StartCoroutine("CallPluginAtEndOfFrames");
    }


    private IEnumerator CallPluginAtEndOfFrames()
    {
        while (true)
        {
            // Wait until all frame rendering is done
            yield return new WaitForEndOfFrame();

            if (_oldMprWindowWidth != MprWindowWidth
                || _oldMprWindowLevel != MprWindowLevel)
            {
                VtkToUnityPlugin.SetMPRWWWL(MprWindowWidth, MprWindowLevel);
                _oldMprWindowWidth = MprWindowWidth;
                _oldMprWindowLevel = MprWindowLevel;
            }
        }
    }
}
