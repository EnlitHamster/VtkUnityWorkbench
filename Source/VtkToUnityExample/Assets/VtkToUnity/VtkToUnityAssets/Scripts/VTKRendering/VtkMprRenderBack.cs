using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThreeDeeHeartPlugins;

public class VtkMprRenderBack : VtkMprRender
{
    public VtkMprRender FrontMpr = null;

    // Use this for initialization
    new protected IEnumerator Start()
    {
        if (null == FrontMpr)
        {
            yield return null;
        }

        _frontMprId = FrontMpr.VolumeMPRPropId;

        yield return base.Start();
    }

    protected override void CallPluginAtEndOfFramesImpl()
    {
        UpdateMPRTransform();
    }
}
