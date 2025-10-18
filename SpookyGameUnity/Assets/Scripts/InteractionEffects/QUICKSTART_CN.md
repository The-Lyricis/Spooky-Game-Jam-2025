# 快速开始指南 - 交互效果系统

## 🚀 5分钟快速上手

### 第一步：创建一个可点击的物体

1. 在场景中创建一个2D物体（例如 Sprite）
2. 确保它有 `SpriteRenderer` 和 `Collider2D`（BoxCollider2D 或 CircleCollider2D）
3. 添加 `CompositeInteractable` 组件

```
Hierarchy:
└─ Button (GameObject)
    ├─ Sprite Renderer
    ├─ Box Collider 2D
    └─ Composite Interactable (Script)
```

### 第二步：添加效果

在 Inspector 中点击 "Add Component"，搜索并添加：

#### 示例A：显示消息
```
Add Component → ShowMessageEffect
    - Message: "你好，世界！"
    - Display Duration: 3
```

#### 示例B：播放动画
```
Add Component → PlayAnimationEffect
    - Use Simple Animation: ✓
    - Animation Type: Bounce
    - Animation Duration: 0.5
```

#### 示例C：解锁其他物体
```
Add Component → UnlockInteractableEffect
    - Interactables To Unlock: [拖入要解锁的物体]
    - Unlock State: ✓
```

### 第三步：测试

1. 确保场景中有 `Interactor` 组件（通常在玩家或摄像机上）
2. 运行游戏
3. 点击你创建的物体
4. 观察效果！

---

## 🎮 常用场景配置

### 场景1：拾取钥匙解锁门

#### 钥匙物体
```
GameObject: Key
├─ CompositeInteractable
│   └─ Interaction Prompt: "拾取钥匙"
│   └─ Can Interact Multiple Times: ☐
├─ ShowMessageEffect
│   └─ Message: "你获得了钥匙"
├─ PlaySoundEffect
│   └─ Audio Clip: [钥匙音效]
├─ ChangeAppearanceEffect
│   └─ Toggle Visibility: ✓
│   └─ New Visibility State: ☐
└─ UnlockInteractableEffect
    └─ Interactables To Unlock: [Door]
```

#### 门物体
```
GameObject: Door
├─ CompositeInteractable
│   └─ Interaction Prompt: "打开门"
│   └─ Interaction Enabled: ☐ (初始锁定)
├─ ShowMessageEffect
│   └─ Message: "门打开了"
└─ PlayAnimationEffect
    └─ Animation Trigger: "Open"
```

---

### 场景2：按钮触发机关

```
GameObject: Button
├─ CompositeInteractable
│   └─ Interaction Prompt: "按下按钮"
├─ PlayAnimationEffect (Delay: 0s)
│   └─ Animation Type: Bounce
├─ PlaySoundEffect (Delay: 0s)
│   └─ Audio Clip: [按钮音效]
├─ ShowMessageEffect (Delay: 0.5s)
│   └─ Message: "机关已激活"
├─ ActivateGameObjectEffect (Delay: 1s)
│   └─ Target Objects: [HiddenPlatform]
│   └─ Activate State: ✓
└─ SetFlagEffect (Delay: 1s)
    └─ Flag Key: "button_pressed"
    └─ Flag Value: ✓
```

---

### 场景3：密码面板

```
GameObject: PasswordPanel
├─ CompositeInteractable
│   └─ Interaction Prompt: "输入密码"
├─ ShowMessageEffect (条件：密码错误)
│   └─ Message: "密码错误"
├─ PlayAnimationEffect (条件：密码错误)
│   └─ Animation Type: Shake
└─ UnlockInteractableEffect (条件：密码正确)
    └─ Interactables To Unlock: [SafeBox]
```

注：条件判断需要自定义效果，见高级用法

---

## 🎨 设置交互提示

### 在 CompositeInteractable 中配置

```
Composite Interactable
├─ Interaction Prompt: "点击交互"  ← 悬停时显示的文字
├─ Can Interact Multiple Times: ✓  ← 是否可以多次点击
└─ Interaction Cooldown: 0.5       ← 点击冷却时间（秒）
```

### 动态改变提示文字

```csharp
// 通过代码改变提示
GetComponent<CompositeInteractable>().SetInteractionPrompt("新的提示");
```

---

## ⚙️ 效果组件通用设置

每个效果组件都有这些通用设置：

```
Effect Settings
├─ Execute Delay: 0        ← 延迟执行时间（秒）
└─ Execute Once: ☐         ← 是否只执行一次
```

### 创建时间序列

通过设置不同的延迟，创建连续效果：

```
物体:
├─ 效果1 (Delay: 0s)    ← 立即执行
├─ 效果2 (Delay: 0.5s)  ← 0.5秒后
├─ 效果3 (Delay: 1s)    ← 1秒后
└─ 效果4 (Delay: 1.5s)  ← 1.5秒后
```

---

## 🔍 调试技巧

### 1. 启用调试日志

在大多数效果组件中都有：
```
☑ Show Debug Info
```

启用后会在 Console 中输出执行信息。

### 2. 使用 Gizmos

`Interactable` 会在 Scene 视图中显示 Collider 边界（绿色半透明）

### 3. 检查交互状态

```csharp
// 检查物体是否已交互
bool hasInteracted = interactable.HasInteracted;

// 检查是否启用交互
bool enabled = interactable.InteractionEnabled;

// 检查是否可以交互
bool canInteract = interactable.CanInteract(actorObject);
```

---

## ⚠️ 常见问题排查

### 问题：点击没反应

**检查清单：**
- [ ] 物体是否有 `Collider2D`？
- [ ] `Collider2D` 是否启用？
- [ ] 物体是否在正确的 Layer 上？
- [ ] 场景中是否有 `Interactor`？
- [ ] `Interactor` 的 `Interactable Layer Mask` 是否包含该物体？

### 问题：UI 挡住了点击

这已经自动处理！`Interactor` 会自动忽略 UI 上的点击。

### 问题：效果没有执行

**检查清单：**
- [ ] 效果组件是否启用？
- [ ] `Execute Once` 是否勾选且已执行过？
- [ ] `CanExecute()` 是否返回 true？
- [ ] Console 中是否有错误？

### 问题：找不到效果组件

**解决方法：**
1. 检查命名空间：`using SpookyGame.World.InteractionEffects;`
2. 确保脚本编译成功
3. 重启 Unity Editor

---

## 🎓 学习路径

### 初学者
1. ✅ 阅读本快速开始指南
2. ✅ 创建一个简单的可点击物体
3. ✅ 尝试不同的效果组件
4. ✅ 创建一个简单的解谜场景

### 进阶
1. 📖 阅读 `README.md` 了解所有效果
2. 📖 阅读 `ARCHITECTURE_CN.md` 理解系统设计
3. 🔧 组合多个效果创建复杂交互
4. 🔧 创建自定义效果组件

### 高级
1. 💻 研究现有效果的源代码
2. 💻 创建条件效果系统
3. 💻 集成其他游戏系统（背包、对话等）
4. 💻 优化性能和内存使用

---

## 📚 相关文档

- `README.md` - 完整功能说明和 API 文档
- `ARCHITECTURE_CN.md` - 系统架构和设计模式
- `PuzzleItemExample.cs` - 代码示例

---

## 🆘 获取帮助

如果遇到问题：

1. 检查 Console 中的错误信息
2. 查看相关文档
3. 检查示例场景
4. 搜索常见问题

---

**现在开始创建你的第一个交互物体吧！** 🎉

