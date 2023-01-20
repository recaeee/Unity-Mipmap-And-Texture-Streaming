using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MipmapDebugPassFeature : ScriptableRendererFeature
{
    public Material overrideMaterial;
    class MipmapDebugRenderPass : ScriptableRenderPass
    {
        public Material overrideMaterial { get; set; }

        private List<ShaderTagId> m_ShaderTagIdList = new List<ShaderTagId>()
        {
            new ShaderTagId("SRPDefaultUnlit"),
            new ShaderTagId("UniversalForward"),
        };
        
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            
        }
        
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            SortingCriteria sortingCriteria = SortingCriteria.CommonOpaque;
            var drawSettings = CreateDrawingSettings(m_ShaderTagIdList, ref renderingData, sortingCriteria);
            drawSettings.overrideMaterial = overrideMaterial;
            var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
            context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref filteringSettings);
        }
        
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }
    }

    MipmapDebugRenderPass mipmapDebugRenderPass;
    
    public override void Create()
    {
        mipmapDebugRenderPass = new MipmapDebugRenderPass()
        {
            overrideMaterial = overrideMaterial
        };
        mipmapDebugRenderPass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }
    
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(mipmapDebugRenderPass);
    }
}


