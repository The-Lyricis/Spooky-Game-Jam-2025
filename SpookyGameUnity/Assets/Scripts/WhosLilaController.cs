using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Who's Lila 效果控制器 - URP版本
/// 用于运行时动态控制效果参数
/// </summary>
[ExecuteInEditMode]
public class WhosLilaController : MonoBehaviour
{
    [Header("渲染器引用")]
    [Tooltip("拖入 Renderer2D 或其他 URP Renderer 资源")]
    public ScriptableRendererData rendererData;

    [Header("预设")]
    public WhosLilaPreset preset = WhosLilaPreset.ClassicWhosLila;

    [Header("实时参数")]
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

    private WhosLilaRenderFeature renderFeature;
    private WhosLilaPreset lastPreset;

    public enum WhosLilaPreset
    {
        Custom,
        ClassicWhosLila,
        HighContrastBW,
        HorrorAtmosphere,
        RetroGaming,
        DreamLike
    }

    void Start()
    {
        FindRenderFeature();
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

        // 实时更新参数
        UpdateParameters();
    }

    /// <summary>
    /// 查找渲染特性
    /// </summary>
    private void FindRenderFeature()
    {
        if (rendererData == null)
        {
            Debug.LogError("请在Inspector中分配 Renderer Data！\n" +
                          "路径：Assets/Settings/Renderer2D.asset");
            return;
        }

        // 注意：这里需要手动在Renderer Data中添加 WhosLilaRenderFeature
        // 无法通过代码自动添加
    }

    /// <summary>
    /// 更新参数到材质
    /// </summary>
    private void UpdateParameters()
    {
        // 参数通过 WhosLilaRenderFeature 自动更新
        // 这里只是为了在Inspector中实时调整
    }

    /// <summary>
    /// 应用预设
    /// </summary>
    public void ApplyPreset(WhosLilaPreset presetType)
    {
        switch (presetType)
        {
            case WhosLilaPreset.ClassicWhosLila:
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
                break;

            case WhosLilaPreset.HighContrastBW:
                pixelSize = 100f;
                colorCount = 4;
                contrast = 2.5f;
                brightness = 0f;
                saturation = 0.2f;
                ditherStrength = 0.6f;
                scanlineIntensity = 0.3f;
                break;

            case WhosLilaPreset.HorrorAtmosphere:
                pixelSize = 140f;
                colorCount = 6;
                contrast = 2.0f;
                brightness = -0.15f;
                saturation = 0.5f;
                ditherStrength = 0.7f;
                vignette = 0.5f;
                grainAmount = 0.15f;
                break;

            case WhosLilaPreset.RetroGaming:
                pixelSize = 80f;
                colorCount = 16;
                contrast = 1.5f;
                brightness = 0.05f;
                saturation = 1.0f;
                ditherStrength = 0.4f;
                scanlineIntensity = 0.15f;
                break;

            case WhosLilaPreset.DreamLike:
                pixelSize = 150f;
                colorCount = 12;
                contrast = 1.3f;
                brightness = 0.1f;
                saturation = 0.9f;
                ditherStrength = 0.3f;
                scanlineIntensity = 0.1f;
                break;
        }

        lastPreset = presetType;
    }
}

