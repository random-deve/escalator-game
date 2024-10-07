using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PosterizeEffect : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {
        public Material posterizeMaterial;
        private RenderTargetIdentifier currentTarget;

        public void Setup(RenderTargetIdentifier target)
        {
            this.currentTarget = target;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.isSceneViewCamera)
                return;

            if (posterizeMaterial == null)
                return;

            CommandBuffer cmd = CommandBufferPool.Get("Posterize Effect");

            Blit(cmd, currentTarget, currentTarget, posterizeMaterial);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    CustomRenderPass m_ScriptablePass;
    public Material posterizeMaterial;

    public override void Create()
    {
        m_ScriptablePass = new CustomRenderPass
        {
            posterizeMaterial = posterizeMaterial,
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        m_ScriptablePass.Setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(m_ScriptablePass);
    }
}
