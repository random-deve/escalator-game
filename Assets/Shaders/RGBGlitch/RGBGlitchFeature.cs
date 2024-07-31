using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RGBGlitchFeature : ScriptableRendererFeature
{
    class RGBGlitchPass : ScriptableRenderPass
    {
        public Material glitchMaterial = null;
        public float glitchAmount = 0.0f;
        public float timeOffset = 0.0f;

        private RenderTargetIdentifier source { get; set; }
        private RenderTargetHandle destination { get; set; }

        public RGBGlitchPass()
        {
            destination.Init("_GlitchTempTexture");
        }

        public void Setup(RenderTargetIdentifier source)
        {
            this.source = source;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (glitchMaterial == null)
            {
                Debug.LogError("Missing glitch material");
                return;
            }

            CommandBuffer cmd = CommandBufferPool.Get("RGB Glitch Effect");

            glitchMaterial.SetFloat("_Amount", glitchAmount);
            glitchMaterial.SetFloat("_TimeOffset", timeOffset);

            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;

            cmd.GetTemporaryRT(destination.id, opaqueDesc, FilterMode.Bilinear);
            Blit(cmd, source, destination.Identifier(), glitchMaterial, 0);
            Blit(cmd, destination.Identifier(), source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(destination.id);
        }
    }

    [System.Serializable]
    public class RGBGlitchSettings
    {
        public Material glitchMaterial = null;
        [Range(0, 1)] public float glitchAmount = 0.0f;
    }

    public RGBGlitchSettings settings = new RGBGlitchSettings();

    RGBGlitchPass glitchPass;

    public override void Create()
    {
        glitchPass = new RGBGlitchPass();
        glitchPass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        glitchPass.glitchMaterial = settings.glitchMaterial;
        glitchPass.glitchAmount = settings.glitchAmount;
        glitchPass.timeOffset = Time.time;
        glitchPass.Setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(glitchPass);
    }
}
