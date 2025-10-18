using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Who's Lila 调色板纹理生成工具
/// 在Unity编辑器中快速创建自定义调色板
/// </summary>
public class PaletteTextureGenerator : EditorWindow
{
    // 调色板设置
    private int textureWidth = 256;
    private int textureHeight = 8;
    private string fileName = "WhosLilaPalette";
    private string savePath = "Assets/Art/";

    // 预设调色板类型
    private enum PalettePreset
    {
        Custom,
        WhosLilaClassic,
        BlackWhite4Color,
        GameBoy,
        CGA,
        HorrorRed,
        CyberpunkBlue,
        RetroOrange
    }

    private PalettePreset currentPreset = PalettePreset.WhosLilaClassic;

    // 自定义调色板颜色（最多8种）
    private Color[] customColors = new Color[8]
    {
        new Color(0.1f, 0.1f, 0.15f),     // 深色
        new Color(0.2f, 0.2f, 0.3f),
        new Color(0.35f, 0.35f, 0.45f),
        new Color(0.5f, 0.5f, 0.6f),
        new Color(0.65f, 0.65f, 0.75f),
        new Color(0.8f, 0.8f, 0.85f),
        new Color(0.9f, 0.9f, 0.95f),
        new Color(0.95f, 0.95f, 1f)       // 亮色
    };

    private int colorCount = 6;

    [MenuItem("Tools/Who's Lila/调色板生成器")]
    public static void ShowWindow()
    {
        PaletteTextureGenerator window = GetWindow<PaletteTextureGenerator>("调色板生成器");
        window.minSize = new Vector2(400, 600);
    }

    void OnGUI()
    {
        GUILayout.Label("Who's Lila 调色板纹理生成器", EditorStyles.boldLabel);
        GUILayout.Space(10);

        // 预设选择
        EditorGUILayout.LabelField("预设调色板", EditorStyles.boldLabel);
        PalettePreset newPreset = (PalettePreset)EditorGUILayout.EnumPopup("选择预设", currentPreset);
        
        if (newPreset != currentPreset)
        {
            currentPreset = newPreset;
            ApplyPreset(currentPreset);
        }

        GUILayout.Space(10);

        // 自定义颜色
        if (currentPreset == PalettePreset.Custom)
        {
            EditorGUILayout.LabelField("自定义调色板", EditorStyles.boldLabel);
            colorCount = EditorGUILayout.IntSlider("颜色数量", colorCount, 2, 8);

            GUILayout.Label("从暗到亮排列颜色：");
            for (int i = 0; i < colorCount; i++)
            {
                customColors[i] = EditorGUILayout.ColorField($"颜色 {i + 1}", customColors[i]);
            }
        }
        else
        {
            EditorGUILayout.HelpBox($"当前预设：{GetPresetDescription(currentPreset)}", MessageType.Info);
            
            // 显示预设颜色（只读）
            EditorGUILayout.LabelField("预设颜色预览：", EditorStyles.boldLabel);
            GUI.enabled = false;
            for (int i = 0; i < colorCount; i++)
            {
                EditorGUILayout.ColorField($"颜色 {i + 1}", customColors[i]);
            }
            GUI.enabled = true;
        }

        GUILayout.Space(20);

        // 纹理设置
        EditorGUILayout.LabelField("纹理设置", EditorStyles.boldLabel);
        textureWidth = EditorGUILayout.IntField("宽度", textureWidth);
        textureHeight = EditorGUILayout.IntField("高度", textureHeight);

        GUILayout.Space(10);

        // 保存设置
        EditorGUILayout.LabelField("保存设置", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        savePath = EditorGUILayout.TextField("保存路径", savePath);
        if (GUILayout.Button("浏览", GUILayout.Width(60)))
        {
            string path = EditorUtility.OpenFolderPanel("选择保存位置", "Assets", "");
            if (!string.IsNullOrEmpty(path))
            {
                // 转换为相对路径
                if (path.StartsWith(Application.dataPath))
                {
                    savePath = "Assets" + path.Substring(Application.dataPath.Length) + "/";
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        fileName = EditorGUILayout.TextField("文件名", fileName);

        GUILayout.Space(20);

        // 生成按钮
        if (GUILayout.Button("生成调色板纹理", GUILayout.Height(40)))
        {
            GeneratePaletteTexture();
        }

        GUILayout.Space(10);

        // 帮助信息
        EditorGUILayout.HelpBox(
            "使用说明：\n" +
            "1. 选择预设或自定义调色板\n" +
            "2. 调整颜色从暗到亮排列\n" +
            "3. 点击生成按钮创建纹理\n" +
            "4. 将生成的纹理拖入Shader的Palette Texture字段\n" +
            "5. 在Shader代码中启用map_to_palette函数",
            MessageType.Info);
    }

    /// <summary>
    /// 应用预设调色板
    /// </summary>
    void ApplyPreset(PalettePreset preset)
    {
        switch (preset)
        {
            case PalettePreset.WhosLilaClassic:
                colorCount = 6;
                customColors[0] = new Color(0.08f, 0.08f, 0.12f);    // 极深蓝灰
                customColors[1] = new Color(0.15f, 0.15f, 0.22f);    // 深蓝灰
                customColors[2] = new Color(0.28f, 0.28f, 0.35f);    // 中等蓝灰
                customColors[3] = new Color(0.45f, 0.45f, 0.52f);    // 浅蓝灰
                customColors[4] = new Color(0.68f, 0.68f, 0.75f);    // 很浅蓝灰
                customColors[5] = new Color(0.88f, 0.88f, 0.92f);    // 几乎白色
                fileName = "WhosLila_Classic";
                break;

            case PalettePreset.BlackWhite4Color:
                colorCount = 4;
                customColors[0] = new Color(0.0f, 0.0f, 0.0f);       // 纯黑
                customColors[1] = new Color(0.33f, 0.33f, 0.33f);    // 深灰
                customColors[2] = new Color(0.67f, 0.67f, 0.67f);    // 浅灰
                customColors[3] = new Color(1.0f, 1.0f, 1.0f);       // 纯白
                fileName = "BlackWhite_4Color";
                break;

            case PalettePreset.GameBoy:
                colorCount = 4;
                customColors[0] = new Color(0.06f, 0.22f, 0.06f);    // GameBoy深绿
                customColors[1] = new Color(0.19f, 0.38f, 0.19f);    // GameBoy中绿
                customColors[2] = new Color(0.55f, 0.68f, 0.06f);    // GameBoy亮绿
                customColors[3] = new Color(0.61f, 0.74f, 0.06f);    // GameBoy最亮绿
                fileName = "GameBoy_Palette";
                break;

            case PalettePreset.CGA:
                colorCount = 4;
                customColors[0] = new Color(0.0f, 0.0f, 0.0f);       // 黑色
                customColors[1] = new Color(0.0f, 0.67f, 0.67f);     // 青色
                customColors[2] = new Color(0.67f, 0.0f, 0.67f);     // 品红
                customColors[3] = new Color(1.0f, 1.0f, 1.0f);       // 白色
                fileName = "CGA_Palette";
                break;

            case PalettePreset.HorrorRed:
                colorCount = 6;
                customColors[0] = new Color(0.08f, 0.0f, 0.0f);      // 极深红
                customColors[1] = new Color(0.2f, 0.05f, 0.05f);     // 深红
                customColors[2] = new Color(0.35f, 0.08f, 0.08f);    // 暗红
                customColors[3] = new Color(0.55f, 0.15f, 0.15f);    // 红色
                customColors[4] = new Color(0.75f, 0.35f, 0.35f);    // 亮红
                customColors[5] = new Color(0.9f, 0.7f, 0.7f);       // 粉红
                fileName = "Horror_Red";
                break;

            case PalettePreset.CyberpunkBlue:
                colorCount = 6;
                customColors[0] = new Color(0.0f, 0.0f, 0.15f);      // 极深蓝
                customColors[1] = new Color(0.0f, 0.15f, 0.35f);     // 深蓝
                customColors[2] = new Color(0.0f, 0.35f, 0.55f);     // 蓝色
                customColors[3] = new Color(0.15f, 0.55f, 0.75f);    // 亮蓝
                customColors[4] = new Color(0.4f, 0.75f, 0.9f);      // 天蓝
                customColors[5] = new Color(0.7f, 0.9f, 1.0f);       // 很亮蓝
                fileName = "Cyberpunk_Blue";
                break;

            case PalettePreset.RetroOrange:
                colorCount = 6;
                customColors[0] = new Color(0.1f, 0.05f, 0.0f);      // 极深橙棕
                customColors[1] = new Color(0.3f, 0.15f, 0.05f);     // 深橙棕
                customColors[2] = new Color(0.55f, 0.3f, 0.1f);      // 暗橙
                customColors[3] = new Color(0.8f, 0.5f, 0.2f);       // 橙色
                customColors[4] = new Color(0.95f, 0.75f, 0.45f);    // 亮橙
                customColors[5] = new Color(1.0f, 0.95f, 0.8f);      // 米黄
                fileName = "Retro_Orange";
                break;
        }
    }

    /// <summary>
    /// 获取预设描述
    /// </summary>
    string GetPresetDescription(PalettePreset preset)
    {
        switch (preset)
        {
            case PalettePreset.WhosLilaClassic: return "经典Who's Lila风格，6种蓝灰色调";
            case PalettePreset.BlackWhite4Color: return "经典黑白4色调色板";
            case PalettePreset.GameBoy: return "GameBoy经典绿色调色板";
            case PalettePreset.CGA: return "CGA复古4色调色板";
            case PalettePreset.HorrorRed: return "恐怖红色调色板，营造血腥氛围";
            case PalettePreset.CyberpunkBlue: return "赛博朋克蓝色调色板";
            case PalettePreset.RetroOrange: return "复古橙色暖色调色板";
            default: return "自定义调色板";
        }
    }

    /// <summary>
    /// 生成调色板纹理
    /// </summary>
    void GeneratePaletteTexture()
    {
        // 创建纹理
        Texture2D texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGB24, false);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        // 填充纹理
        for (int y = 0; y < textureHeight; y++)
        {
            for (int x = 0; x < textureWidth; x++)
            {
                // 计算当前像素应该使用哪个颜色
                float t = (float)x / (textureWidth - 1);
                Color color = GetColorFromGradient(t);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();

        // 保存为PNG
        byte[] bytes = texture.EncodeToPNG();
        
        // 确保保存路径存在
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        string fullPath = savePath + fileName + ".png";
        File.WriteAllBytes(fullPath, bytes);

        // 刷新资源数据库
        AssetDatabase.Refresh();

        // 设置导入设置
        TextureImporter importer = AssetImporter.GetAtPath(fullPath) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Default;
            importer.filterMode = FilterMode.Point;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.mipmapEnabled = false;
            importer.SaveAndReimport();
        }

        // 显示成功消息
        EditorUtility.DisplayDialog(
            "成功", 
            $"调色板纹理已生成！\n路径：{fullPath}\n\n下一步：\n1. 将纹理拖入Shader的Palette Texture字段\n2. 在Shader代码217行取消注释map_to_palette函数", 
            "确定"
        );

        // 选中生成的纹理
        Object obj = AssetDatabase.LoadAssetAtPath(fullPath, typeof(Texture2D));
        Selection.activeObject = obj;
        EditorGUIUtility.PingObject(obj);

        Debug.Log($"调色板纹理已生成：{fullPath}");
    }

    /// <summary>
    /// 从渐变获取颜色
    /// </summary>
    Color GetColorFromGradient(float t)
    {
        if (colorCount <= 1) return customColors[0];

        // 计算在哪两个颜色之间
        float scaledT = t * (colorCount - 1);
        int lowerIndex = Mathf.FloorToInt(scaledT);
        int upperIndex = Mathf.CeilToInt(scaledT);

        lowerIndex = Mathf.Clamp(lowerIndex, 0, colorCount - 1);
        upperIndex = Mathf.Clamp(upperIndex, 0, colorCount - 1);

        if (lowerIndex == upperIndex)
        {
            return customColors[lowerIndex];
        }

        // 在两个颜色之间插值
        float localT = scaledT - lowerIndex;
        return Color.Lerp(customColors[lowerIndex], customColors[upperIndex], localT);
    }
}

