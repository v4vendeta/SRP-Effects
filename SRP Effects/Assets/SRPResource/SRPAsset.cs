using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "SRPResource/SRP")]
public class SRPAsset : RenderPipelineAsset
{
    protected override RenderPipeline CreatePipeline()
    {
        return new SRP();
    }

}
