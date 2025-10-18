# CloseupViewUI 使用指南 🖼️

## ✅ 已完成：统一特写系统

`CloseupViewUI` 现在支持**单层和多层**显示！

---

## 📖 功能特性

### 支持两种模式

| 模式 | API | 用途 |
|------|-----|------|
| **单层** | `ShowCloseup(Sprite)` | 照片、简单物体 |
| **多层** | `ShowMultiLayerCloseup(Sprite[])` | 窗户（背景+人+窗框） |

**向下兼容**：旧代码无需修改 ✅

---

## 🚀 快速配置

### UI 层级（新增 LayerContainer）

```
Canvas
└─ CloseupViewUI (Panel)
   ├─ Background (Image，半透明黑色)
   ├─ CloseupImage (Image) ← 单层显示用
   └─ LayerContainer (Empty) ← 多层显示用（新增）
```

**LayerContainer 配置**：
```
LayerContainer (Empty GameObject)
├─ RectTransform
│  ├─ Anchor: Stretch-Stretch
│  ├─ Pos: (0, 0, 0)
│  └─ Size Delta: (0, 0)
```

---

## 🎮 使用方式

### 照片（单层）✅

```csharp
// PhotoInspection 继承自 InspectableObject
// 调用父类的 OnInteract()
base.OnInteract(actor);

// InspectableObject.OnInteract() 内部：
closeupUI.ShowCloseup(closeupSprite);

// 结果：显示单张照片 ✅
```

**配置**：
```
Photo
└─ PhotoInspection
   ├─ Closeup Sprite: 照片图
   └─ Auto Find UI: ✓
```

---

### 窗户（单层）✅

```csharp
// WindowInspection 继承自 InspectableObject
// 使用方式和照片完全一样
closeupUI.ShowCloseup(windowSprite);

// 结果：显示单张窗户图 ✅
```

**配置**：
```
Window
└─ WindowInspection
   ├─ Closeup Sprite: 窗户图
   └─ Auto Find UI: ✓
```

---

### 窗户（多层）✅

```csharp
// WindowMultiLayerInspection 直接调用多层 API
Sprite[] layers = new Sprite[] 
{ 
    backgroundLayer,   // 窗外背景
    middleLayer,       // 窗后的人
    foregroundLayer    // 窗口框架
};

closeupUI.ShowMultiLayerCloseup(layers);

// 结果：显示背景+人物+窗框（叠加）✅
```

**配置**：
```
Window
└─ WindowMultiLayerInspection
   ├─ Background Layer: 窗外背景图
   ├─ Middle Layer: 窗后人物图
   ├─ Foreground Layer: 窗框图
   └─ Auto Find UI: ✓
```

---

## 📊 单层 vs 多层

### 单层模式（原有功能）

```
ShowCloseup(sprite)
└─ 显示在 CloseupImage 上
   └─ 支持缩放、拖动
   └─ 点击关闭
```

---

### 多层模式（新功能）✅

```
ShowMultiLayerCloseup([bg, person, frame])
├─ 隐藏 CloseupImage
├─ 在 LayerContainer 中动态创建图层
│  ├─ Layer_0 (背景)
│  ├─ Layer_1 (人物)
│  └─ Layer_2 (窗框)
└─ 点击关闭
   └─ 清理所有图层
   └─ 恢复 CloseupImage
```

---

## 🎨 图层渲染

### 渲染顺序（从后到前）

```
数组索引 = 渲染顺序：
├─ layers[0] → Layer_0（最下层，背景）
├─ layers[1] → Layer_1（中间层，人物）
└─ layers[2] → Layer_2（最上层，窗框）

视觉效果：
┌─────────────────────────┐
│  前景层（窗框）          │ ← 最前
├─────────────────────────┤
│  中景层（人物）          │ ← 中间
├─────────────────────────┤
│  背景层（窗外背景）      │ ← 最后
└─────────────────────────┘
```

---

## 🔧 配置步骤

### 步骤 1：更新 CloseupViewUI

在现有的 `CloseupViewUI` Panel 下添加：

```
CloseupViewUI (Panel)
├─ Background（已存在）
├─ CloseupImage（已存在）
└─ LayerContainer（新建 Empty GameObject）
   └─ RectTransform（Stretch-Stretch）
```

---

### 步骤 2：配置脚本引用

```
CloseupViewUI (Script)
├─ Closeup Panel: CloseupViewUI（已存在）
├─ Closeup Image: CloseupImage（已存在）
├─ Image Transform: CloseupImage（已存在）
└─ Layer Container: LayerContainer ← 拖拽新建的 LayerContainer
```

---

### 步骤 3：准备图层资源（多层模式）

```
Assets/Sprites/Window/
├─ window_background.png （窗外背景，不透明）
├─ window_person.png     （人物，透明背景）
└─ window_frame.png      （窗框，透明背景）

导入设置：
└─ Texture Type: Sprite (2D and UI)
```

---

## 🐛 调试

### Console 日志

**单层模式**：
```
[CloseupViewUI] Showing closeup: photo_closeup
```

**多层模式**：
```
[CloseupViewUI] Created layer 0: window_background
[CloseupViewUI] Created layer 1: window_person
[CloseupViewUI] Created layer 2: window_frame
[CloseupViewUI] Showing 3 layers
```

**关闭时**：
```
[CloseupViewUI] Hiding closeup
```

---

## ✅ 测试清单

### 照片测试
- [ ] 点击照片显示特写（单层）
- [ ] 可以缩放、拖动
- [ ] 点击关闭特写
- [ ] 设置旗标 inspected.photo

### 窗户（单层）测试
- [ ] 点击窗户显示特写（单层）
- [ ] 可以缩放、拖动
- [ ] 点击关闭特写
- [ ] 设置旗标 inspected.window

### 窗户（多层）测试
- [ ] 点击窗户显示特写（多层）
- [ ] 显示背景+人+窗框（叠加正确）
- [ ] 图层顺序正确
- [ ] 点击关闭特写
- [ ] 图层已清理
- [ ] 设置旗标 inspected.window

---

## 🎯 内部实现

### ShowMultiLayerCloseup() 逻辑

```csharp
1. 检查图层数量
   └─ 如果只有 1 层 → 调用 ShowCloseup()（单层模式）

2. 设置多层模式标志
   └─ _isMultiLayer = true

3. 隐藏单层 Image
   └─ closeupImage.gameObject.SetActive(false)

4. 清空旧图层
   └─ ClearLayers()

5. 创建新图层
   └─ for each sprite in layers:
      └─ CreateLayer(sprite, index)
         ├─ 创建 GameObject
         ├─ 添加 RectTransform（拉伸填充）
         ├─ 添加 Image 组件
         ├─ 设置 sprite
         └─ 设置 SiblingIndex（确保顺序）

6. 显示面板
   └─ closeupPanel.SetActive(true)

7. 淡入动画
   └─ StartCoroutine(FadeIn())
```

---

### HideCloseup() 逻辑

```csharp
1. 检查是否多层模式
   └─ if (_isMultiLayer):
      ├─ ClearLayers()（销毁所有图层）
      ├─ _isMultiLayer = false
      └─ closeupImage.gameObject.SetActive(true)

2. 淡出动画
   └─ StartCoroutine(FadeOut())

3. 隐藏面板
   └─ closeupPanel.SetActive(false)
```

---

## 📝 API 参考

### 公共方法

```csharp
// 显示单层特写
public void ShowCloseup(Sprite sprite)

// 显示多层特写
public void ShowMultiLayerCloseup(Sprite[] layers)

// 隐藏特写
public void HideCloseup()

// 获取是否显示
public bool IsVisible { get; }

// 设置缩放范围
public void SetZoomRange(float min, float max)
```

---

## 🎉 总结

**一套 UI，两种模式**：

- **单层**：照片、简单物体 → `ShowCloseup(sprite)`
- **多层**：窗户等复杂场景 → `ShowMultiLayerCloseup(layers)`

**优势**：
- ✅ 保持原有功能（向下兼容）
- ✅ 支持多层叠加（新功能）
- ✅ 统一管理，易于维护
- ✅ 自动清理，无内存泄漏

---

现在 `CloseupViewUI` 是一个统一、强大的特写系统！🎨✨

