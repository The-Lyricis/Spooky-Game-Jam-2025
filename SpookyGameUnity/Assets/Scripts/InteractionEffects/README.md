# 交互效果系统 (Interaction Effects System)

## 📖 概述

这是一个灵活的、基于组件的交互效果系统，专为2D点击解谜游戏设计。通过组合不同的效果组件，你可以在Unity Editor中直接配置复杂的交互逻辑，无需编写代码。

## 🎯 核心优势

✅ **无需编程** - 所有交互都可以在Inspector中配置  
✅ **高度灵活** - 通过组合多个效果实现复杂交互  
✅ **易于扩展** - 可以轻松创建新的效果组件  
✅ **支持延迟** - 每个效果都可以设置执行延迟  
✅ **一次性执行** - 支持只执行一次的效果  

## 🧩 核心组件

### 1. CompositeInteractable
可组合的交互对象，是所有交互物体的基础。

**使用步骤：**
1. 在2D物体上添加 `CompositeInteractable` 组件
2. 确保物体有 `Collider2D` 组件（BoxCollider2D/CircleCollider2D等）
3. 添加一个或多个效果组件（见下方列表）
4. 配置每个效果的参数

### 2. 效果组件列表

#### ShowMessageEffect - 显示文字提示
- **功能：** 点击后显示文字消息
- **参数：**
  - `Message` - 要显示的文字内容
  - `Display Duration` - 显示持续时间（秒）
  - `Use Debug Log` - 是否同时在控制台输出

#### UnlockInteractableEffect - 解锁交互物体
- **功能：** 解锁其他物体的交互功能
- **参数：**
  - `Interactables To Unlock` - 要解锁的物体数组
  - `Unlock State` - true=解锁，false=锁定

#### ChangeAppearanceEffect - 改变物体外观
- **功能：** 改变精灵图、颜色、缩放、显示/隐藏
- **参数：**
  - `Change Sprite` - 是否更换精灵图
  - `New Sprite` - 新的精灵图
  - `Change Color` - 是否改变颜色
  - `New Color` - 新颜色
  - `Toggle Visibility` - 是否切换可见性
  - `Change Scale` - 是否改变缩放
  - `Use Animation` - 是否使用动画过渡

#### PlayAnimationEffect - 播放动画
- **功能：** 播放Animator动画或简单动画
- **参数：**
  - `Target Animator` - 目标Animator组件
  - `Animation Trigger` - 动画触发器名称
  - `Use Simple Animation` - 使用简单动画（无需Animator）
  - `Animation Type` - 动画类型：Bounce/Shake/Pulse

#### ActivateGameObjectEffect - 激活/禁用物体
- **功能：** 显示或隐藏游戏物体
- **参数：**
  - `Target Objects` - 目标物体数组
  - `Activate State` - true=激活，false=禁用

#### SetFlagEffect - 设置游戏旗标
- **功能：** 设置游戏进度标记
- **参数：**
  - `Flag Key` - 旗标键名
  - `Flag Value` - 旗标值（true/false）

#### PlaySoundEffect - 播放音效
- **功能：** 播放音效
- **参数：**
  - `Audio Clip` - 音频剪辑
  - `Volume` - 音量（0-1）
  - `Pitch` - 音调（0-3）

## 📝 使用示例

### 示例1：简单的门锁机制

```
【门物体】
- CompositeInteractable
  - Interaction Prompt: "门已锁定"
  - Can Interact Multiple Times: false

【钥匙物体】
- CompositeInteractable
  - Interaction Prompt: "拾取钥匙"
- ShowMessageEffect
  - Message: "你获得了钥匙！"
- UnlockInteractableEffect
  - Interactables To Unlock: [门物体]
- ChangeAppearanceEffect
  - Change Color: true
  - New Color: 灰色
- SetFlagEffect
  - Flag Key: "has_key"
  - Flag Value: true
```

### 示例2：机关按钮触发连锁反应

```
【按钮物体】
- CompositeInteractable
- PlayAnimationEffect (delay: 0s)
  - Animation Type: Bounce
- ShowMessageEffect (delay: 0.5s)
  - Message: "机关已启动"
- ActivateGameObjectEffect (delay: 1s)
  - Target Objects: [隐藏门物体]
- PlaySoundEffect (delay: 1s)
  - Audio Clip: 机关音效
- UnlockInteractableEffect (delay: 1.5s)
  - Interactables To Unlock: [下一区域]
```

### 示例3：带条件的交互

如果需要根据条件（如持有特定物品）触发不同效果，可以：

1. **方案A：** 创建多个 CompositeInteractable，根据条件启用/禁用
2. **方案B：** 使用 FlagService 配合条件检查
3. **方案C：** 继承 BaseInteractionEffect 创建自定义条件效果

```csharp
public class ConditionalEffect : BaseInteractionEffect
{
    [SerializeField] private string requiredFlag;
    [SerializeField] private IInteractionEffect effectIfTrue;
    [SerializeField] private IInteractionEffect effectIfFalse;
    
    protected override void ExecuteImmediate(GameObject actor)
    {
        bool flagValue = FlagService.GetFlag(requiredFlag);
        if (flagValue && effectIfTrue != null)
        {
            effectIfTrue.Execute(actor);
        }
        else if (!flagValue && effectIfFalse != null)
        {
            effectIfFalse.Execute(actor);
        }
    }
}
```

## 🔧 创建自定义效果

如果现有效果不满足需求，可以创建自定义效果：

```csharp
using UnityEngine;
using SpookyGame.World.InteractionEffects;

public class MyCustomEffect : BaseInteractionEffect
{
    [Header("Custom Settings")]
    [SerializeField] private string myParameter;
    
    protected override void ExecuteImmediate(GameObject actor)
    {
        // 在这里实现你的自定义逻辑
        Debug.Log($"Custom effect executed: {myParameter}");
    }
    
    public override bool CanExecute(GameObject actor)
    {
        // 可选：添加自定义条件判断
        if (!base.CanExecute(actor))
        {
            return false;
        }
        
        // 添加你的条件
        return true;
    }
}
```

## 🎮 与现有系统集成

### EventBus 集成
效果系统已与 EventBus 集成，例如：
- `ShowMessageEffect` 发送 `MessageEvent`
- 你可以订阅这些事件来更新UI或触发其他逻辑

### FlagService 集成
- `SetFlagEffect` 直接操作 FlagService
- 可以用于保存游戏进度、检查解谜状态等

## 💡 最佳实践

1. **组件化思维** - 将复杂交互拆分成多个小效果
2. **使用延迟** - 利用 `executeDelay` 创建时间序列效果
3. **善用旗标** - 使用 FlagService 记录游戏状态
4. **调试信息** - 开发时启用 Debug Log 查看执行流程
5. **性能考虑** - 避免在一个物体上添加过多效果（建议<10个）

## 📚 相关文件

- `Interactable.cs` - 交互基类
- `CompositeInteractable.cs` - 可组合交互对象
- `Interactor.cs` - 鼠标交互管理器
- `IInteractionEffect.cs` - 效果接口
- `BaseInteractionEffect.cs` - 效果基类

## 🆘 常见问题

**Q: 为什么点击没反应？**  
A: 检查：1) 是否有Collider2D，2) Collider是否启用，3) Interactor是否在场景中

**Q: 如何实现点击后播放动画？**  
A: 添加 PlayAnimationEffect 组件，配置Animator或使用简单动画

**Q: 如何让效果只执行一次？**  
A: 在效果组件上启用 `Execute Once` 选项

**Q: 如何创建延迟效果链？**  
A: 给每个效果设置递增的 `Execute Delay` 值

**Q: 原来的 Say.cs 还能用吗？**  
A: 可以！旧代码完全兼容。但建议新功能使用 CompositeInteractable

---

更新时间：2025-10-18

