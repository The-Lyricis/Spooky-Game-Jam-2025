using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Who's Lila 风格渲染特性 - URP版本
/// 在URP渲染管线中添加后处理效果
/// </summary>
public class WhosLilaRenderFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        [Tooltip("Who's Lila 效果材质")]
        public Material effectMaterial = null;

        [Tooltip("渲染通道事件")]
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;

        [Header("Who's Lila 风格参数")]
        [Range(10, 1000)]
        public float pixelSize = 120f;

        [Range(2, 32)]
        public int colorCount = 8;

        [Range(0.5f, 3f)]
        public float contrast = 1.8f;

        [Range(-0.5f, 0.5f)]
        public float brightness = 0f;

        [Range(0f, 2f)]
        public float saturation = 0.7f;

        [Range(0f, 1f)]
        public float ditherStrength = 0.5f;

        [Range(1, 8)]
        public float ditherScale = 4f;

        [Range(0f, 1f)]
        public float scanlineIntensity = 0.2f;

        [Range(100, 1000)]
        public float scanlineCount = 240f;

        [Range(0f, 0.2f)]
        public float noiseAmount = 0.05f;

        [Range(0f, 1f)]
        public float vignette = 0.3f;

        [Range(1, 4)]
        public float grainSize = 2f;

        [Range(0f, 0.3f)]
        public float grainAmount = 0.08f;
    }

    public Settings settings = new Settings();
    private WhosLilaRenderPass renderPass;

    /// <summary>
    /// 创建渲染通道
    /// </summary>
    public override void Create()
    {
        renderPass = new WhosLilaRenderPass("WhosLilaEffect");
        renderPass.renderPassEvent = settings.renderPassEvent;
    }

    /// <summary>
    /// 添加渲染通道
    /// </summary>
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.effectMaterial == null)
        {
            Debug.LogWarning("Who's Lila Effect Material is not assigned!");
            return;
        }

        // 更新材质参数
        UpdateMaterialProperties();

        // 设置材质（不在这里设置source，改为在Execute中获取）
        renderPass.SetMaterial(settings.effectMaterial);

        // 添加渲染通道
        renderer.EnqueuePass(renderPass);
    }

    /// <summary>
    /// 更新材质属性
    /// </summary>
    private void UpdateMaterialProperties()
    {
        if (settings.effectMaterial == null) return;

        settings.effectMaterial.SetFloat("_PixelSize", settings.pixelSize);
        settings.effectMaterial.SetFloat("_ColorCount", settings.colorCount);
        settings.effectMaterial.SetFloat("_Contrast", settings.contrast);
        settings.effectMaterial.SetFloat("_Brightness", settings.brightness);
        settings.effectMaterial.SetFloat("_Saturation", settings.saturation);
        settings.effectMaterial.SetFloat("_DitherStrength", settings.ditherStrength);
        settings.effectMaterial.SetFloat("_DitherScale", settings.ditherScale);
        settings.effectMaterial.SetFloat("_ScanlineIntensity", settings.scanlineIntensity);
        settings.effectMaterial.SetFloat("_ScanlineCount", settings.scanlineCount);
        settings.effectMaterial.SetFloat("_NoiseAmount", settings.noiseAmount);
        settings.effectMaterial.SetFloat("_Vignette", settings.vignette);
        settings.effectMaterial.SetFloat("_GrainSize", settings.grainSize);
        settings.effectMaterial.SetFloat("_GrainAmount", settings.grainAmount);
    }

    /// <summary>
    /// 应用预设
    /// </summary>
    public void ApplyPreset(WhosLilaPreset preset)
    {
        switch (preset)
        {
            case WhosLilaPreset.ClassicWhosLila:
                settings.pixelSize = 120f;
                settings.colorCount = 8;
                settings.contrast = 1.8f;
                settings.brightness = 0f;
                settings.saturation = 0.7f;
                settings.ditherStrength = 0.5f;
                settings.ditherScale = 4f;
                settings.scanlineIntensity = 0.2f;
                settings.scanlineCount = 240f;
                settings.noiseAmount = 0.05f;
                settings.vignette = 0.3f;
                settings.grainSize = 2f;
                settings.grainAmount = 0.08f;
                break;

            case WhosLilaPreset.HighContrastBW:
                settings.pixelSize = 100f;
                settings.colorCount = 4;
                settings.contrast = 2.5f;
                settings.brightness = 0f;
                settings.saturation = 0.2f;
                settings.ditherStrength = 0.6f;
                settings.scanlineIntensity = 0.3f;
                break;

            case WhosLilaPreset.HorrorAtmosphere:
                settings.pixelSize = 140f;
                settings.colorCount = 6;
                settings.contrast = 2.0f;
                settings.brightness = -0.15f;
                settings.saturation = 0.5f;
                settings.ditherStrength = 0.7f;
                settings.vignette = 0.5f;
                settings.grainAmount = 0.15f;
                break;
        }
    }

    public enum WhosLilaPreset
    {
        Custom,
        ClassicWhosLila,
        HighContrastBW,
        HorrorAtmosphere
    }
}

