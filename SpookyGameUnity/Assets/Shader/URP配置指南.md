# Who's Lila Shader - URP配置完整指南

## 🎯 URP版本使用说明

你的项目使用了**Universal Render Pipeline (URP)**，需要特殊的配置方式。

---

## 📋 完整配置步骤（5步）

### 步骤1：检查文件是否已创建

确认以下文件已存在：
- ✅ `Assets/Scripts/WhosLilaRenderPass.cs`
- ✅ `Assets/Scripts/WhosLilaRenderFeature.cs`
- ✅ `Assets/Scripts/WhosLilaController.cs`（可选，用于运行时控制）
- ✅ `Assets/Material/WhosLilaMaterial.mat`
- ✅ `Assets/Shader/PixelateShader.shader`

---

### 步骤2：找到URP Renderer资源

1. 在Project窗口导航到：
   ```
   Assets/Settings/Renderer2D.asset
   ```

2. 如果找不到，尝试：
   - 搜索框输入：`t:ScriptableRendererData`
   - 或查看 `Assets/Settings/` 文件夹

3. **单击选中** `Renderer2D.asset`

---

### 步骤3：添加Render Feature（关键步骤！）

在Inspector中，针对 `Renderer2D.asset`：

1. 找到 **Renderer Features** 列表
2. 点击 **Add Renderer Feature** 按钮（底部的 + 号）
3. 在弹出菜单中选择 **WhosLilaRenderFeature**

```
┌─────────────────────────────────────┐
│ Renderer2D (Scriptable Renderer)    │
├─────────────────────────────────────┤
│ Renderer Features                   │
│   [0] WhosLilaRenderFeature        │ ← 添加后会显示
│                                     │
│   [Add Renderer Feature ▼]         │ ← 点击这里
└─────────────────────────────────────┘
```

---

### 步骤4：配置Render Feature参数

添加后，展开 **WhosLilaRenderFeature**：

```
┌─────────────────────────────────────────┐
│ WhosLilaRenderFeature                   │
├─────────────────────────────────────────┤
│ ☑ Active                                │ ← 确保勾选
│                                         │
│ Settings                                │
│   Effect Material: None (Material)      │ ← 拖入材质
│   Render Pass Event: Before...Post...   │ ← 保持默认
│                                         │
│ Who's Lila Style Settings               │
│   Pixel Size: 120                       │
│   Color Count: 8                        │
│   Contrast: 1.8                         │
│   Brightness: 0                         │
│   Saturation: 0.7                       │
│   ... (其他参数)                         │
└─────────────────────────────────────────┘
```

#### 关键操作：
1. **勾选 Active** - 启用该渲染特性
2. **Effect Material** - 从Project窗口拖入 `WhosLilaMaterial`
3. 调整参数（或使用默认值）

---

### 步骤5：测试效果

1. **保存场景**（Ctrl+S）
2. **播放场景**（Play按钮）
3. **观察效果**

你应该看到：
- ✅ 整个画面被像素化
- ✅ 颜色被限制在几种色调
- ✅ 有抖动和扫描线效果

---

## 🎨 调整参数的两种方式

### 方式A：在Renderer2D中调整（推荐用于固定设置）

1. 选中 `Assets/Settings/Renderer2D.asset`
2. 在Inspector中展开 **WhosLilaRenderFeature**
3. 调整参数
4. **播放场景**查看效果
5. **参数会保存**，每次运行都使用这些设置

**优点：** 设置一次，永久生效  
**缺点：** 运行时不能动态改变

---

### 方式B：使用Controller组件（用于运行时控制）

如果需要在游戏运行时动态改变效果：

1. 创建一个空GameObject：
   ```
   Hierarchy右键 → Create Empty
   命名为 "WhosLilaController"
   ```

2. 添加组件：
   ```
   Add Component → WhosLilaController
   ```

3. 配置组件：
   ```
   ┌─────────────────────────────────┐
   │ WhosLilaController (Script)     │
   ├─────────────────────────────────┤
   │ Renderer Data:                  │
   │   ▸ Renderer2D                 │ ← 拖入Renderer2D.asset
   │                                 │
   │ Preset: Classic Whos Lila       │
   │                                 │
   │ Real-time Parameters:           │
   │   Pixel Size: 120               │
   │   Color Count: 8                │
   │   ... (实时调整)                 │
   └─────────────────────────────────┘
   ```

4. 通过代码控制：
   ```csharp
   WhosLilaController controller = FindObjectOfType<WhosLilaController>();
   controller.contrast = 2.5f;
   controller.pixelSize = 150f;
   ```

---

## ⚠️ 常见问题排查

### 问题1：看不到效果

**检查清单：**
- [ ] Renderer2D.asset 中已添加 WhosLilaRenderFeature
- [ ] WhosLilaRenderFeature 的 Active 已勾选
- [ ] Effect Material 已分配 WhosLilaMaterial
- [ ] WhosLilaMaterial 的 Shader 是 Custom/WhosLilaPixelate
- [ ] 摄像机的 Renderer 设置为 Renderer2D

### 问题2：场景变黑/变色

**可能原因：**
- Shader编译错误
- 材质参数设置不当

**解决方法：**
1. 查看 Console 窗口的错误信息
2. 重置材质参数为默认值
3. 确认shader没有编译错误（红色波浪线）

### 问题3：找不到 WhosLilaRenderFeature

**原因：** 脚本编译失败或未刷新

**解决方法：**
1. 检查 Console 是否有编译错误
2. 修复所有错误
3. 右键 `Assets/Scripts/` → Reimport All
4. 等待编译完成
5. 重新尝试添加

### 问题4：参数调整无效

**原因：** 需要在正确的位置调整参数

**解决方法：**
- 在 **Renderer2D.asset** 中调整参数（不是材质上）
- 调整后需要 **重新播放场景**
- 如果使用Controller，确保脚本正常运行

---

## 🔍 检查渲染管线配置

确认URP设置正确：

### 1. 检查Graphics设置
```
Edit → Project Settings → Graphics
┌─────────────────────────────────────┐
│ Graphics                            │
├─────────────────────────────────────┤
│ Scriptable Render Pipeline:         │
│   ▸ UniversalRenderPipeline...     │ ← 应该有设置
└─────────────────────────────────────┘
```

### 2. 检查摄像机设置
```
Main Camera → Camera组件
┌─────────────────────────────────────┐
│ Camera                              │
├─────────────────────────────────────┤
│ Rendering                           │
│   Renderer: Renderer2D              │ ← 确认这个
│   Post Processing: ☑               │ ← 勾选
└─────────────────────────────────────┘
```

---

## 📸 效果预览与对比

### 配置前（无效果）
- 普通渲染
- 高分辨率
- 全色彩

### 配置后（Who's Lila风格）
- ✅ 像素化块状效果
- ✅ 6-8种颜色
- ✅ 高对比度
- ✅ 抖动过渡
- ✅ CRT扫描线
- ✅ 复古颗粒感

---

## 🎮 预设配置推荐

在 **Renderer2D** 的 **WhosLilaRenderFeature** 中：

### 预设1：经典 Who's Lila
```
Pixel Size: 120
Color Count: 8
Contrast: 1.8
Brightness: 0
Saturation: 0.7
Dither Strength: 0.5
Scanline Intensity: 0.2
Vignette: 0.3
```

### 预设2：极简黑白
```
Pixel Size: 100
Color Count: 4
Contrast: 2.5
Saturation: 0.2
Dither Strength: 0.6
```

### 预设3：恐怖氛围
```
Pixel Size: 140
Color Count: 6
Contrast: 2.0
Brightness: -0.15
Saturation: 0.5
Vignette: 0.5
Grain Amount: 0.15
```

---

## 🔧 调试技巧

### 1. 检查渲染特性是否激活

在 **Game视图** 的右上角：
- 点击 **三条横线图标**
- 查看 **Stats**
- 应该能看到额外的渲染通道

### 2. 使用Frame Debugger

```
Window → Analysis → Frame Debugger
点击 Enable
查看渲染步骤中是否有 "WhosLilaEffect"
```

### 3. Console日志

添加调试代码到 `WhosLilaRenderPass.cs`：
```csharp
public override void Execute(...)
{
    Debug.Log("Who's Lila Effect is running!");
    // ... 其余代码
}
```

---

## 📚 URP vs Built-in 对比

| 特性 | Built-in RP | URP |
|------|-------------|-----|
| 使用方式 | OnRenderImage | ScriptableRenderFeature |
| 配置位置 | 相机组件 | Renderer资源 |
| 难度 | 简单 | 中等 |
| 灵活性 | 中等 | 高 |
| 性能 | 一般 | 优秀 |

---

## ✅ 快速检查清单

使用前请确认：

**文件检查：**
- [ ] WhosLilaRenderPass.cs 已创建
- [ ] WhosLilaRenderFeature.cs 已创建
- [ ] WhosLilaMaterial.mat 存在
- [ ] Shader无编译错误

**Renderer2D配置：**
- [ ] 已添加 WhosLilaRenderFeature
- [ ] Active 已勾选
- [ ] Effect Material 已分配
- [ ] 参数已调整

**摄像机配置：**
- [ ] Renderer 设置为 Renderer2D
- [ ] Post Processing 已勾选

**测试：**
- [ ] 播放场景看到效果
- [ ] 调整参数有变化
- [ ] 无Console错误

---

## 🚀 下一步

配置完成后，你可以：

1. **微调参数** - 找到最适合你游戏的风格
2. **创建多个预设** - 为不同场景准备不同风格
3. **添加运行时控制** - 根据游戏状态动态改变效果
4. **创建调色板** - 使用调色板生成器创建自定义色彩

---

## 💡 额外提示

### 性能优化
- 如果帧率下降，降低 Scanline Count
- 减少 Noise 和 Grain 计算
- 考虑降低游戏分辨率

### 多场景使用
- Renderer Feature 配置全局生效
- 每个场景都会应用相同效果
- 如需不同效果，创建多个Renderer资源

### 移动平台
- 测试性能
- 考虑简化参数
- 可能需要创建Mobile优化版本

---

**配置完成后，你就可以享受Who's Lila风格的像素艺术效果了！** 🎮✨

如果遇到问题，检查Console错误信息，或参考本文档的故障排除章节。

