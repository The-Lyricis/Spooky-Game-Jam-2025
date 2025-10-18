# Who's Lila URP版 - 5分钟快速设置

## 🎯 你需要做的（5个步骤）

### ✅ 步骤1：找到Renderer2D资源
```
Project窗口 → Assets/Settings/Renderer2D.asset
点击选中它
```

### ✅ 步骤2：添加渲染特性
```
Inspector → Renderer Features
点击 [Add Renderer Feature ▼]
选择 "WhosLilaRenderFeature"
```

### ✅ 步骤3：分配材质
```
在 WhosLilaRenderFeature 中：
☑ Active （勾选）
Effect Material: 拖入 "WhosLilaMaterial"
```

### ✅ 步骤4：检查摄像机
```
选中 Main Camera
Camera组件 → Rendering
Renderer: Renderer2D ✓
Post Processing: ☑ 勾选
```

### ✅ 步骤5：测试
```
点击 Play ▶️
观察效果
调整参数（在Renderer2D的Feature中）
```

---

## 📍 关键位置示意

### 在Renderer2D.asset中配置：
```
┌─────────────────────────────────────┐
│ Renderer2D                          │
├─────────────────────────────────────┤
│ Renderer Features                   │
│                                     │
│ [0] WhosLilaRenderFeature          │
│     ☑ Active                       │
│     ┌─────────────────────────┐    │
│     │ Effect Material:        │    │
│     │   ▸ WhosLilaMaterial   │    │
│     │                         │    │
│     │ Pixel Size: 120         │    │
│     │ Color Count: 8          │    │
│     │ Contrast: 1.8           │    │
│     │ ...                     │    │
│     └─────────────────────────┘    │
│                                     │
│ [Add Renderer Feature ▼]           │
└─────────────────────────────────────┘
```

---

## ⚠️ 如果看不到效果或出现错误

### 常见错误1：cameraColorTarget错误
```
错误信息：pipeline camera target texture might have not been created...
解决方法：代码已修复，确保使用最新版本的脚本
```

### 检查清单：
1. [ ] Console窗口无红色错误
2. [ ] WhosLilaRenderFeature已添加到Renderer2D
3. [ ] Active已勾选（绿色勾）
4. [ ] Effect Material已分配（显示WhosLilaMaterial）
5. [ ] 材质的Shader是Custom/WhosLilaPixelate
6. [ ] 摄像机的Renderer是Renderer2D
7. [ ] 摄像机的Post Processing已勾选

---

## 🎨 推荐参数（复制使用）

### 经典Who's Lila风格
```
Pixel Size: 120
Color Count: 8
Contrast: 1.8
Brightness: 0
Saturation: 0.7
Dither Strength: 0.5
Dither Scale: 4
Scanline Intensity: 0.2
Scanline Count: 240
Noise Amount: 0.05
Vignette: 0.3
Grain Size: 2
Grain Amount: 0.08
```

### 高对比黑白风格
```
Pixel Size: 100
Color Count: 4
Contrast: 2.5
Saturation: 0.2
Dither Strength: 0.6
Scanline Intensity: 0.3
```

### 恐怖氛围风格
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

## 💡 提示

- **所有参数调整**都在 Renderer2D.asset 的 WhosLilaRenderFeature 中
- **不是**在材质上调整
- **不是**在摄像机组件上调整
- 调整后**必须重新播放场景**才能看到变化

---

## 🔗 详细文档

如需更多信息，请查看：
- `URP配置指南.md` - 完整配置说明
- `快速开始指南.md` - 通用使用指南
- `项目总览.md` - 项目架构和原理

---

**完成以上步骤后，你就能看到Who's Lila风格的像素化效果了！** 🎮✨

