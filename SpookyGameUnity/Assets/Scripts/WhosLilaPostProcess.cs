using UnityEngine;

/// <summary>
/// Who's Lila 风格后处理效果
/// 将此脚本挂载到主摄像机上以应用像素化效果
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class WhosLilaPostProcess : MonoBehaviour
{
    [Header("Shader Material")]
    [Tooltip("使用 Custom/WhosLilaPixelate shader 的材质")]
    public Material effectMaterial;

    [Header("预设配置")]
    [Tooltip("选择预设风格")]
    public WhosLilaPreset preset = WhosLilaPreset.Custom;

    [Header("像素化设置")]
    [Range(10, 1000)]
    public float pixelSize = 120f;

    [Header("色彩风格（Who's Lila核心）")]
    [Range(2, 32)]
    [Tooltip("调色板颜色数量 - Who's Lila 推荐 6-8")]
    public int colorCount = 8;

    [Range(0.5f, 3f)]
    [Tooltip("对比度 - Who's Lila 推荐 1.5-2.0")]
    public float contrast = 1.8f;

    [Range(-0.5f, 0.5f)]
    [Tooltip("亮度调整")]
    public float brightness = 0f;

    [Range(0f, 2f)]
    [Tooltip("饱和度 - Who's Lila 推荐 0.6-0.8")]
    public float saturation = 0.7f;

    [Header("抖动效果（Dithering）")]
    [Range(0f, 1f)]
    [Tooltip("抖动强度 - 用于在有限颜色间创造过渡")]
    public float ditherStrength = 0.5f;

    [Range(1, 8)]
    [Tooltip("抖动图案缩放")]
    public float ditherScale = 4f;

    [Header("CRT复古效果")]
    [Range(0f, 1f)]
    [Tooltip("扫描线强度")]
    public float scanlineIntensity = 0.2f;

    [Range(100, 1000)]
    [Tooltip("扫描线数量")]
    public float scanlineCount = 240f;

    [Range(0f, 0.2f)]
    [Tooltip("噪点强度")]
    public float noiseAmount = 0.05f;

    [Range(0f, 1f)]
    [Tooltip("暗角强度")]
    public float vignette = 0.3f;

    [Header("颗粒效果")]
    [Range(1, 4)]
    [Tooltip("颗粒大小")]
    public float grainSize = 2f;

    [Range(0f, 0.3f)]
    [Tooltip("颗粒强度")]
    public float grainAmount = 0.08f;

    [Header("调色板纹理（可选）")]
    [Tooltip("留空则使用颜色量化，设置则使用LUT映射")]
    public Texture2D paletteTexture;

    private WhosLilaPreset lastPreset = WhosLilaPreset.Custom;

    public enum WhosLilaPreset
    {
        Custom,              // 自定义
        ClassicWhosLila,     // 经典 Who's Lila
        HighContrastBW,      // 高对比黑白
        HorrorAtmosphere,    // 恐怖氛围
        RetroGaming,         // 复古游戏
        DreamLike           // 梦境风格
    }

    void Start()
    {
        // 如果没有设置材质，尝试自动创建
        if (effectMaterial == null)
        {
            Shader shader = Shader.Find("Custom/WhosLilaPixelate");
            if (shader != null)
            {
                effectMaterial = new Material(shader);
                Debug.Log("自动创建了 WhosLilaPixelate 材质");
            }
            else
            {
                Debug.LogError("找不到 Custom/WhosLilaPixelate shader！请确保shader已正确导入。");
            }
        }

        ApplyPreset(preset);
    }

    void Update()
    {
        // 检测预设变化
        if (preset != lastPreset && preset != WhosLilaPreset.Custom)
        {
            ApplyPreset(preset);
            lastPreset = preset;
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (effectMaterial != null)
        {
            // 更新所有shader参数
            UpdateShaderProperties();

            // 应用后处理效果
            Graphics.Blit(source, destination, effectMaterial);
        }
        else
        {
            // 如果没有材质，直接输出
            Graphics.Blit(source, destination);
        }
    }

    /// <summary>
    /// 更新Shader属性
    /// </summary>
    void UpdateShaderProperties()
    {
        effectMaterial.SetFloat("_PixelSize", pixelSize);
        effectMaterial.SetFloat("_ColorCount", colorCount);
        effectMaterial.SetFloat("_Contrast", contrast);
        effectMaterial.SetFloat("_Brightness", brightness);
        effectMaterial.SetFloat("_Saturation", saturation);
        effectMaterial.SetFloat("_DitherStrength", ditherStrength);
        effectMaterial.SetFloat("_DitherScale", ditherScale);
        effectMaterial.SetFloat("_ScanlineIntensity", scanlineIntensity);
        effectMaterial.SetFloat("_ScanlineCount", scanlineCount);
        effectMaterial.SetFloat("_NoiseAmount", noiseAmount);
        effectMaterial.SetFloat("_Vignette", vignette);
        effectMaterial.SetFloat("_GrainSize", grainSize);
        effectMaterial.SetFloat("_GrainAmount", grainAmount);

        // 如果设置了调色板纹理
        if (paletteTexture != null)
        {
            effectMaterial.SetTexture("_PaletteTexture", paletteTexture);
        }
    }

    /// <summary>
    /// 应用预设配置
    /// </summary>
    public void ApplyPreset(WhosLilaPreset presetType)
    {
        switch (presetType)
        {
            case WhosLilaPreset.ClassicWhosLila:
                // 经典 Who's Lila 风格
                pixelSize = 120f;
                colorCount = 8;
                contrast = 1.8f;
                brightness = 0f;
                saturation = 0.7f;
                ditherStrength = 0.5f;
                ditherScale = 4f;
                scanlineIntensity = 0.2f;
                scanlineCount = 240f;
                noiseAmount = 0.05f;
                vignette = 0.3f;
                grainSize = 2f;
                grainAmount = 0.08f;
                Debug.Log("应用预设：经典 Who's Lila");
                break;

            case WhosLilaPreset.HighContrastBW:
                // 高对比黑白风格
                pixelSize = 100f;
                colorCount = 4;
                contrast = 2.5f;
                brightness = 0f;
                saturation = 0.2f;
                ditherStrength = 0.6f;
                ditherScale = 4f;
                scanlineIntensity = 0.3f;
                scanlineCount = 240f;
                noiseAmount = 0.05f;
                vignette = 0.4f;
                grainSize = 2f;
                grainAmount = 0.1f;
                Debug.Log("应用预设：高对比黑白");
                break;

            case WhosLilaPreset.HorrorAtmosphere:
                // 恐怖氛围风格
                pixelSize = 140f;
                colorCount = 6;
                contrast = 2.0f;
                brightness = -0.15f;
                saturation = 0.5f;
                ditherStrength = 0.7f;
                ditherScale = 4f;
                scanlineIntensity = 0.25f;
                scanlineCount = 180f;
                noiseAmount = 0.08f;
                vignette = 0.5f;
                grainSize = 2f;
                grainAmount = 0.15f;
                Debug.Log("应用预设：恐怖氛围");
                break;

            case WhosLilaPreset.RetroGaming:
                // 复古游戏风格
                pixelSize = 80f;
                colorCount = 16;
                contrast = 1.5f;
                brightness = 0.05f;
                saturation = 1.0f;
                ditherStrength = 0.4f;
                ditherScale = 2f;
                scanlineIntensity = 0.15f;
                scanlineCount = 240f;
                noiseAmount = 0.03f;
                vignette = 0.2f;
                grainSize = 1f;
                grainAmount = 0.05f;
                Debug.Log("应用预设：复古游戏");
                break;

            case WhosLilaPreset.DreamLike:
                // 梦境风格
                pixelSize = 150f;
                colorCount = 12;
                contrast = 1.3f;
                brightness = 0.1f;
                saturation = 0.9f;
                ditherStrength = 0.3f;
                ditherScale = 6f;
                scanlineIntensity = 0.1f;
                scanlineCount = 200f;
                noiseAmount = 0.02f;
                vignette = 0.25f;
                grainSize = 3f;
                grainAmount = 0.06f;
                Debug.Log("应用预设：梦境风格");
                break;

            case WhosLilaPreset.Custom:
                // 保持当前自定义设置
                break;
        }

        lastPreset = presetType;
    }

    /// <summary>
    /// 运行时切换效果开关
    /// </summary>
    public void ToggleEffect(bool enable)
    {
        this.enabled = enable;
    }

    /// <summary>
    /// 重置为默认设置
    /// </summary>
    public void ResetToDefault()
    {
        preset = WhosLilaPreset.ClassicWhosLila;
        ApplyPreset(preset);
    }
}

