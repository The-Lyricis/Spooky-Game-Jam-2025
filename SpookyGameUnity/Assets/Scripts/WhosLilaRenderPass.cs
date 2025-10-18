using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Who's Lila 风格渲染通道 - URP版本
/// </summary>
public class WhosLilaRenderPass : ScriptableRenderPass
{
    private Material effectMaterial;
    private RenderTargetIdentifier currentTarget;
    private RenderTargetHandle tempTexture;

    private string profilerTag;

    public WhosLilaRenderPass(string profilerTag)
    {
        this.profilerTag = profilerTag;
        tempTexture.Init("_TempWhosLilaTexture");
    }

    /// <summary>
    /// 设置效果材质
    /// </summary>
    public void SetMaterial(Material material)
    {
        effectMaterial = material;
    }

    /// <summary>
    /// 配置渲染目标（在Execute中调用）
    /// </summary>
    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        // 获取当前相机的颜色目标
        currentTarget = renderingData.cameraData.renderer.cameraColorTarget;
    }

    /// <summary>
    /// 执行渲染通道
    /// </summary>
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (effectMaterial == null)
        {
            Debug.LogWarning("Who's Lila Effect Material is null!");
            return;
        }

        CommandBuffer cmd = CommandBufferPool.Get(profilerTag);

        // 获取相机目标描述符
        RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
        descriptor.depthBufferBits = 0; // 不需要深度缓冲

        // 获取当前渲染目标
        RenderTargetIdentifier source = renderingData.cameraData.renderer.cameraColorTarget;

        // 获取临时渲染纹理
        cmd.GetTemporaryRT(tempTexture.id, descriptor, FilterMode.Point);

        // 应用后处理效果
        // source -> tempTexture (应用效果)
        Blit(cmd, source, tempTexture.Identifier(), effectMaterial, 0);
        
        // tempTexture -> source (写回)
        Blit(cmd, tempTexture.Identifier(), source);

        // 执行命令缓冲
        context.ExecuteCommandBuffer(cmd);
        
        // 释放命令缓冲
        CommandBufferPool.Release(cmd);
    }

    /// <summary>
    /// 清理
    /// </summary>
    public override void FrameCleanup(CommandBuffer cmd)
    {
        cmd.ReleaseTemporaryRT(tempTexture.id);
    }
}

