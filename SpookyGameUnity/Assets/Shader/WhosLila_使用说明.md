# Who's Lila 像素风格 Shader 使用说明

## 📋 美术风格特点分析

**Who's Lila** 的独特视觉风格包含以下核心特征：

1. **低分辨率像素化** - 大块像素颗粒，强烈的复古感
2. **有限色彩调色板** - 通常使用3-8种颜色
3. **高对比度** - 强烈的明暗对比，突出轮廓
4. **抖动效果（Dithering）** - Bayer矩阵抖动创造过渡
5. **低饱和度** - 偏向灰调或单色调
6. **CRT复古效果** - 扫描线、噪点、颗粒感
7. **FMV像素化** - 将真实视频素材转化为像素艺术

---

## 🎮 Shader功能说明

### 核心参数

#### 1. 像素化控制
- **Pixel Size** (10-1000): 像素块大小
  - 推荐值：80-150（Who's Lila风格）
  - 越小越精细，越大越粗糙

#### 2. 色彩风格控制（Who's Lila核心）
- **Color Count** (2-32): 调色板颜色数量
  - 推荐值：6-8（经典Who's Lila风格）
  - 2-4：极简风格
  - 8-16：更丰富的表现

- **Contrast** (0.5-3.0): 对比度
  - 推荐值：1.5-2.0（Who's Lila的高对比度）
  - 增强明暗分离

- **Brightness** (-0.5-0.5): 亮度
  - 推荐值：-0.1到0.1
  - Who's Lila偏向略暗

- **Saturation** (0.0-2.0): 饱和度
  - 推荐值：0.6-0.8（Who's Lila的低饱和度特征）
  - 0.0：完全黑白
  - 0.5-0.8：Who's Lila风格

#### 3. 抖动效果（关键特征）
- **Dither Strength** (0-1): 抖动强度
  - 推荐值：0.4-0.6
  - 用于在有限颜色间创造过渡

- **Dither Scale** (1-8): 抖动图案大小
  - 推荐值：4
  - Bayer矩阵的缩放

#### 4. CRT复古效果
- **Scanline Intensity** (0-1): 扫描线强度
  - 推荐值：0.1-0.3
  - 模拟CRT显示器

- **Scanline Count** (100-1000): 扫描线数量
  - 推荐值：240（经典分辨率）

- **Noise Amount** (0-0.2): 噪点强度
  - 推荐值：0.03-0.05
  - 时间动态噪点

- **Vignette** (0-1): 暗角强度
  - 推荐值：0.2-0.4

#### 5. 颗粒效果
- **Grain Size** (1-4): 颗粒大小
  - 推荐值：2

- **Grain Amount** (0-0.3): 颗粒强度
  - 推荐值：0.05-0.1

---

## 🎨 预设配置推荐

### 配置1：经典 Who's Lila 风格
```
Pixel Size: 120
Color Count: 8
Contrast: 1.8
Brightness: 0.0
Saturation: 0.7
Dither Strength: 0.5
Dither Scale: 4
Scanline Intensity: 0.2
Noise Amount: 0.05
Vignette: 0.3
Grain Amount: 0.08
```

### 配置2：高对比黑白风格
```
Pixel Size: 100
Color Count: 4
Contrast: 2.5
Brightness: 0.0
Saturation: 0.2
Dither Strength: 0.6
Scanline Intensity: 0.3
```

### 配置3：恐怖氛围风格
```
Pixel Size: 140
Color Count: 6
Contrast: 2.0
Brightness: -0.15
Saturation: 0.5
Dither Strength: 0.7
Vignette: 0.5
Grain Amount: 0.15
```

---

## 🛠️ 使用方法

### 方法1：作为后处理效果（推荐）

1. 创建一个Material：
   - 右键 → Create → Material
   - 命名为 "WhosLilaEffect"
   - Shader选择 "Custom/WhosLilaPixelate"

2. 创建后处理脚本（C#）：
```csharp
using UnityEngine;

[ExecuteInEditMode]
public class WhosLilaPostProcess : MonoBehaviour
{
    public Material effectMaterial;
    
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (effectMaterial != null)
        {
            Graphics.Blit(src, dest, effectMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
```

3. 将脚本挂载到主摄像机
4. 将Material拖入脚本的 effectMaterial 字段

### 方法2：使用调色板纹理（LUT）

如果要使用特定的调色板：

1. 创建一个调色板纹理（1D或窄条状2D纹理）
   - 推荐尺寸：256x1或256x8
   - 从左到右：暗到亮的颜色渐变

2. 纹理导入设置：
   - Filter Mode: Point (no filter)
   - Wrap Mode: Clamp
   - Compression: None

3. 在Shader代码中启用调色板映射：
   - 找到第217行左右的注释
   - 取消注释 `color = map_to_palette(color);`

---

## 🎯 调试技巧

1. **先从基础参数开始**：
   - 先设置 Pixel Size
   - 再调整 Color Count
   - 最后微调其他效果

2. **观察参考**：
   - 搜索 Who's Lila 游戏截图作为参考
   - 注意其色彩数量和对比度

3. **性能优化**：
   - 如果性能有问题，降低 Scanline Count
   - 减少 Noise 和 Grain 计算

4. **实时调试**：
   - 在Play模式下调整参数
   - 观察实时效果

---

## 📝 技术细节

### 实现的关键技术

1. **Bayer矩阵抖动**：4x4 Bayer矩阵实现有序抖动
2. **颜色量化**：Floor函数实现颜色级别限制
3. **HSV色彩空间**：用于精确的饱和度控制
4. **时间动态噪点**：使用 _Time 创建动态效果
5. **多层次效果堆叠**：像素化 → 色彩调整 → 量化 → 后处理

### Shader兼容性

- Unity版本：2019.4+
- 渲染管线：Built-in RP / URP（需要适配）
- 平台：所有平台

---

## 🔄 进阶定制

### 添加自定义颜色滤镜

在 `adjust_color` 函数中添加：
```hlsl
// 添加蓝色调
color.b *= 1.2;
```

### 创建动态扫描线

修改 `scanline` 函数：
```hlsl
float line = sin((uv.y + _Time.y * 0.1) * _ScanlineCount * 3.14159);
```

### 区域选择性应用

添加遮罩纹理参数，实现部分区域应用效果。

---

## 📚 参考资源

- Who's Lila 官方游戏
- Bayer矩阵抖动算法
- CRT Shader技术
- Pixel Art理论

---

## ⚠️ 注意事项

1. **首次使用**：从预设配置开始，再根据需求调整
2. **性能**：复杂计算在移动平台可能需要优化
3. **相机设置**：确保相机渲染顺序正确
4. **UI层级**：后处理会影响所有渲染内容

---

祝你创作出独特的 Who's Lila 风格游戏！🎮✨

