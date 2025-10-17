# 2D 解密游戏框架

这是一个为 2D 解密游戏设计的 Unity 框架，提供了完整的游戏架构和核心功能。

## 架构概述

### Core 核心系统
- **Bootstrap**: 游戏启动入口，初始化所有服务
- **EventBus**: 全局事件中枢，处理事件发布和订阅
- **GameState**: 游戏状态枚举（Boot, Loading, Exploring, InPuzzle, Paused）
- **SceneService**: 场景加载与过渡管理
- **FlagService**: 全局旗标管理，用于门/机关/条件查询

### Input & Interaction 输入和交互系统
- **InputService**: 简化输入接口，只处理交互和暂停
- **Interactor**: 鼠标悬停交互探测和触发

### World / Interactables 世界交互系统
- **Interactable**: 可交互对象的抽象基类

### UI 界面系统
- **PromptUI**: 交互提示显示
- **PauseMenu**: 暂停菜单（接口保留，功能已移除）
- **WinScreen**: 通关界面

## 使用方法

### 1. 设置游戏启动

1. 在场景中创建一个空的 GameObject
2. 添加 `Bootstrap` 组件
3. 设置首关场景名称

### 2. 创建交互器

1. 创建一个空的 GameObject
2. 添加 `Interactor` 组件
3. 设置可交互对象的 Layer Mask

### 3. 创建可交互对象

1. 创建一个 GameObject
2. 添加继承自 `Interactable` 的脚本
3. 实现 `OnInteract` 方法
4. 设置交互范围和提示文本

### 4. 设置 UI

1. 创建 Canvas
2. 添加 `PromptUI` 组件用于交互提示
3. 添加 `PauseMenu` 组件用于暂停功能
4. 添加 `WinScreen` 组件用于通关界面

### 5. 添加游戏管理器

1. 创建一个空的 GameObject
2. 添加 `GameManager` 组件用于更新输入服务

## 事件系统

框架使用事件驱动架构，主要事件包括：

- `GameStateChangedEvent`: 游戏状态变化
- `PromptEvent`: 交互提示显示/隐藏
- `PuzzleSolvedEvent`: 谜题解决
- `FlagChangedEvent`: 旗标变化

## 输入系统

支持以下输入：
- 交互：E 键（点击可交互对象）
- 鼠标悬停：显示交互提示

## 旗标系统

使用 `FlagService` 管理游戏状态：
```csharp
// 设置旗标
FlagService.SetFlag("door_unlocked", true);

// 获取旗标
bool isUnlocked = FlagService.GetFlag("door_unlocked");
```

## 示例代码

参考 `Examples` 文件夹中的示例脚本：
- `ExampleInteractable`: 展示如何创建可交互对象
- `GameManager`: 展示如何管理游戏状态

## 注意事项

1. 确保所有场景都添加到 Build Settings 中
2. 可交互对象需要 Collider2D 组件
3. UI 组件需要正确设置引用
4. 交互器需要添加到场景中
5. 使用 Unity 内置的 Input 系统，无需额外包

## 扩展建议

- 添加音频系统
- 实现更复杂的谜题类型
- 添加道具系统
- 实现对话系统
- 添加特效系统
