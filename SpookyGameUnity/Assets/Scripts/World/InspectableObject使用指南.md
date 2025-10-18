# 物品特写查看系统使用指南

## 📁 文件说明

- **InspectableObject.cs** - 可查看特写的物体脚本（继承自 Interactable）
- **CloseupViewUI.cs** - 特写查看 UI 控制器

---

## 🎮 快速开始

### 步骤 1：创建特写 UI

#### 1. 创建 Canvas 和 UI 结构

```
Hierarchy > 右键 > UI > Canvas
重命名为 "CloseupViewCanvas"

CloseupViewCanvas
├─ Sort Order: 100  ← 确保在最上层
└─ CloseupPanel (Panel)
    ├─ Image: 半透明黑色背景 (RGBA: 0, 0, 0, 150)
    ├─ Anchor: Stretch 全屏
    └─ CloseupImage (Image)
        ├─ Width: 800
        ├─ Height: 600
        ├─ Preserve Aspect: ✓ 勾选
        └─ Color: 白色
```

#### 2. 挂载脚本

1. 选中 `CloseupViewCanvas` GameObject
2. 添加 `CloseupViewUI.cs` 脚本
3. 在 Inspector 中配置：
   - **Closeup Panel**：拖入 CloseupPanel
   - **Closeup Image**：拖入 CloseupImage
   - **Image Transform**：自动获取（或手动拖入 CloseupImage 的 RectTransform）
   - **Min Zoom**: 0.5（最小缩放）
   - **Max Zoom**: 3（最大缩放）
   - **Zoom Speed**: 0.1（缩放速度）
   - **Enable Drag**: ✓（启用拖动）
   - **Drag Speed**: 1（拖动速度）

---

### 步骤 2：创建可查看的物体

#### 1. 在场景中创建物体

```
Hierarchy > 右键 > 2D Object > Sprite
重命名为 "InspectableItem"
```

#### 2. 配置 Sprite 和 Collider

1. 设置 **Sprite**（物体在场景中的外观）
2. 添加 **Collider2D**（如果没有自动添加）
   - 推荐使用 `BoxCollider2D` 或 `PolygonCollider2D`

#### 3. 挂载脚本

1. 添加 `InspectableObject.cs` 脚本
2. 在 Inspector 中配置：

**Interaction Settings**（继承自 Interactable）：
- **Interaction Prompt**: "查看"
- **Can Interact Multiple Times**: ✓（可以重复查看）
- **Interaction Cooldown**: 0.5

**Closeup Settings**：
- **Closeup Sprite**: 拖入要显示的特写图片
  - 或勾选 **Use Object Sprite** 使用物体自身的图片
- **Use Object Sprite**: 如果勾选，将使用物体的 SpriteRenderer 图片

**UI Reference**：
- **Closeup UI**: 拖入 CloseupViewCanvas 上的 CloseupViewUI 组件
- **Auto Find UI**: ✓（自动查找 UI，推荐勾选）

---

## 🎨 UI 设计建议

### Canvas 设置

```
Canvas 组件:
├─ Render Mode: Screen Space - Overlay
├─ Pixel Perfect: ✓
└─ Sort Order: 100  ← 确保在其他 UI 之上

Canvas Scaler:
├─ UI Scale Mode: Scale With Screen Size
├─ Reference Resolution: 1920x1080
└─ Match: 0.5 (Width/Height)
```

### CloseupPanel（背景遮罩）

```
Image 组件:
├─ Color: RGBA(0, 0, 0, 150)  ← 半透明黑色
├─ Raycast Target: ✓  ← 必须勾选，用于检测点击外部

RectTransform:
├─ Anchor: Stretch (全屏)
├─ Left, Right, Top, Bottom: 0
```

### CloseupImage（特写图片）

```
Image 组件:
├─ Source Image: 留空（运行时动态设置）
├─ Preserve Aspect: ✓  ← 保持图片宽高比
├─ Raycast Target: ✓  ← 用于检测拖动

RectTransform:
├─ Anchor: Center
├─ Width: 800
├─ Height: 600
```

---

## 🎮 操作说明

### 玩家操作

1. **查看物品**：点击场景中的物体
2. **缩放图片**：鼠标滚轮上下滚动
3. **拖动图片**：鼠标左键按住图片拖动
4. **关闭特写**：点击图片外的黑色区域

---

## 💻 代码使用示例

### 示例 1：在脚本中显示特写

```csharp
using SpookyGame.UI;
using UnityEngine;

public class MyScript : MonoBehaviour
{
    public CloseupViewUI closeupUI;
    public Sprite mySprite;
    
    public void ShowMyCloseup()
    {
        closeupUI.ShowCloseup(mySprite);
    }
    
    public void HideCloseup()
    {
        closeupUI.HideCloseup();
    }
}
```

### 示例 2：动态设置特写图片

```csharp
using SpookyGame.World;

public class MyScript : MonoBehaviour
{
    public InspectableObject inspectable;
    public Sprite newSprite;
    
    void Start()
    {
        // 运行时更改特写图片
        inspectable.SetCloseupSprite(newSprite);
    }
}
```

### 示例 3：检查特写是否显示

```csharp
if (closeupUI.IsVisible)
{
    Debug.Log("特写正在显示中");
}
```

---

## 🔧 高级配置

### 自定义缩放范围

```csharp
// 设置缩放范围为 0.3 到 5 倍
closeupUI.SetZoomRange(0.3f, 5f);
```

### 禁用拖动功能

在 Inspector 中取消勾选 `Enable Drag`。

### 调整淡入淡出速度

在 Inspector 中修改：
- **Fade In Duration**: 淡入时长（秒）
- **Fade Out Duration**: 淡出时长（秒）

---

## 🎯 使用场景示例

### 场景 1：侦探游戏 - 查看线索

```
场景中的物品：
- 血迹（点击查看特写，可以放大观察细节）
- 字条（点击查看特写，可以放大阅读文字）
- 钥匙（点击查看特写，观察钥匙上的编号）
```

### 场景 2：密室逃脱 - 查看机关

```
场景中的机关：
- 密码锁（点击查看特写，放大观察数字）
- 壁画（点击查看特写，拖动查看各个部分）
- 保险箱（点击查看特写，缩放观察细节）
```

### 场景 3：恐怖游戏 - 查看日记

```
场景中的物品：
- 日记本（点击查看特写，放大阅读文字）
- 照片（点击查看特写，缩放观察人物）
- 地图（点击查看特写，拖动查看不同区域）
```

---

## 🎨 美术资源建议

### 特写图片分辨率

推荐尺寸：
- **小物品**：1024x1024
- **文档/照片**：1920x1080 或 2048x1536
- **大型物品**：2048x2048 或更大

### 图片格式

- **PNG**（推荐）：支持透明背景
- **JPG**：文件更小，但不支持透明

### Unity 导入设置

```
Texture Import Settings:
├─ Texture Type: Sprite (2D and UI)
├─ Max Size: 2048 或 4096（取决于图片质量需求）
├─ Compression: High Quality（保持清晰度）
└─ Generate Mip Maps: ✓（可选，用于缩放优化）
```

---

## 🐛 常见问题

### Q: 点击物体没有显示特写？

A: 检查：
1. 物体是否有 `Collider2D` 组件
2. `Closeup Sprite` 是否已设置
3. `Closeup UI` 是否已赋值
4. Console 是否有错误日志

### Q: 无法拖动图片？

A: 检查：
1. `Enable Drag` 是否勾选
2. `CloseupImage` 的 `Raycast Target` 是否勾选
3. Canvas 的 `Graphic Raycaster` 组件是否存在

### Q: 点击图片外没有关闭？

A: 检查：
1. `CloseupPanel` 的 `Raycast Target` 是否勾选
2. Canvas 是否有 `Graphic Raycaster` 组件

### Q: 图片显示变形？

A: 确保 `CloseupImage` 的 `Preserve Aspect` 勾选。

### Q: 缩放或拖动不灵敏？

A: 调整：
- `Zoom Speed`（增大以加快缩放）
- `Drag Speed`（增大以加快拖动）

---

## 🔄 与其他系统集成

### 与 FlagService 集成

检查玩家是否查看过某个物品：

```csharp
using SpookyGame.Core;

public class InspectableWithFlag : InspectableObject
{
    [SerializeField] private string flagToSet = "item_inspected";
    
    protected override void OnInteract(GameObject actor)
    {
        base.OnInteract(actor);
        
        // 设置旗标
        FlagService.SetFlag(flagToSet, true);
        Debug.Log($"设置旗标: {flagToSet}");
    }
}
```

### 与交互效果系统集成

查看物品后触发效果：

```csharp
using SpookyGame.World.InteractionEffects;

// 在 InspectableObject 上添加其他效果组件
// 例如：ShowMessageEffect、SetFlagEffect 等
```

---

## 📝 完整示例场景

### 场景结构

```
Scene_0
├─ UI_Root
│   └─ CloseupViewCanvas
│       └─ CloseupViewUI.cs
│           ├─ CloseupPanel (背景)
│           └─ CloseupImage (特写图片)
│
└─ Stage_D1
    ├─ Item_Diary (日记)
    │   ├─ SpriteRenderer: diary_icon
    │   ├─ BoxCollider2D
    │   └─ InspectableObject.cs
    │       ├─ Closeup Sprite: diary_closeup
    │       └─ Interaction Prompt: "阅读日记"
    │
    ├─ Item_Photo (照片)
    │   ├─ SpriteRenderer: photo_icon
    │   ├─ BoxCollider2D
    │   └─ InspectableObject.cs
    │       ├─ Closeup Sprite: photo_closeup
    │       └─ Interaction Prompt: "查看照片"
    │
    └─ Item_Map (地图)
        ├─ SpriteRenderer: map_icon
        ├─ BoxCollider2D
        └─ InspectableObject.cs
            ├─ Closeup Sprite: map_closeup
            └─ Interaction Prompt: "查看地图"
```

---

## 🎉 完成！

现在你的游戏已经有了完整的物品特写查看系统！

玩家可以：
- ✅ 点击物体查看特写
- ✅ 缩放图片观察细节
- ✅ 拖动图片查看不同部分
- ✅ 点击外部关闭特写

如需更多功能或自定义，可以修改脚本中的参数或扩展功能。

