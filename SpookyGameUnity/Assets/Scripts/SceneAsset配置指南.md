# SceneAsset 配置指南 🎯

## ✨ 新特性

现在 `GameConfig` 支持**直接拖拽场景文件**，不再需要手动输入场景名称！

---

## 🎯 优势对比

### ❌ 旧方式（手动输入场景名）
```
GameConfig.asset
├─ Main Menu Scene Name: "MainMenu"  ← 可能拼写错误
└─ Game Scene Name: "Scene_0"        ← 需要手动输入
```

**问题**：
- 容易拼写错误
- 场景重命名后需要手动更新
- 没有自动验证

---

### ✅ 新方式（拖拽场景文件）
```
GameConfig.asset
├─ Scene References
│   ├─ Main Menu Scene Asset: [拖入 MainMenu.unity]   ← 直接拖拽
│   └─ Game Scene Asset: [拖入 Scene_0.unity]         ← 直接拖拽
└─ Scene Names (Auto-generated)
    ├─ Main Menu Scene Name: "MainMenu"   ← 自动生成
    └─ Game Scene Name: "Scene_0"         ← 自动生成
```

**优势**：
- ✅ 不会拼写错误（直接引用文件）
- ✅ 场景重命名后自动更新
- ✅ 自动验证（如果场景被删除会显示警告）
- ✅ 在 Inspector 中一键跳转到场景

---

## 📝 配置步骤

### 1. 打开 GameConfig
在 Project 窗口找到 `GameConfig.asset` 并选中。

### 2. 拖拽场景文件
在 Inspector 中：

#### Scene References 区域：
1. **Main Menu Scene Asset**
   - 从 Project 窗口将 `Assets/Scenes/MainMenu.unity` 拖入

2. **Game Scene Asset**
   - 从 Project 窗口将 `Assets/Scenes/Scene_0.unity` 拖入

### 3. 自动同步
拖入场景后，**Scene Names** 区域会自动更新为正确的场景名称！

### 4. 保存
按 `Ctrl+S` 保存配置。

---

## 🖼️ Inspector 示例

```
┌─────────────────────────────────────────────────┐
│  GameConfig                                     │
├─────────────────────────────────────────────────┤
│  Scene References                               │
│  ├─ Main Menu Scene Asset                       │
│  │   └─ [MainMenu (SceneAsset)]  ← 拖入场景    │
│  └─ Game Scene Asset                            │
│      └─ [Scene_0 (SceneAsset)]   ← 拖入场景    │
│                                                 │
│  Scene Names (Auto-generated)                   │
│  ├─ Main Menu Scene Name: "MainMenu"    ✅      │
│  └─ Game Scene Name: "Scene_0"          ✅      │
│                                                 │
│  Stage Configuration                            │
│  ├─ First Stage Id: "D1"                        │
│  └─ Stages (Size: 1)                            │
└─────────────────────────────────────────────────┘
```

---

## 🔄 工作原理

### Editor 模式
在 Unity Editor 中：
- 使用 `SceneAsset` 类型存储场景引用
- 自动从 `SceneAsset` 提取场景名称
- 存储到 `mainMenuSceneName` 和 `gameSceneName` 字段

### Runtime 模式
在打包后的游戏中：
- `SceneAsset` 不可用（Unity 限制）
- 使用预先存储的 `mainMenuSceneName` 和 `gameSceneName`
- 代码通过 `MainMenuSceneName` 和 `GameSceneName` 属性访问

---

## 💻 代码实现

### 在 GameConfig 中
```csharp
#if UNITY_EDITOR
[SerializeField] private SceneAsset mainMenuSceneAsset;
#endif

[SerializeField] private string mainMenuSceneName = "MainMenu";

public string MainMenuSceneName
{
    get
    {
        #if UNITY_EDITOR
        if (mainMenuSceneAsset != null)
        {
            return mainMenuSceneAsset.name;  // Editor: 从 SceneAsset 获取
        }
        #endif
        return mainMenuSceneName;  // Runtime: 从字符串获取
    }
}
```

### 在其他脚本中使用
```csharp
// ❌ 旧方式
SceneService.LoadScene(gameConfig.gameSceneName);

// ✅ 新方式
SceneService.LoadScene(gameConfig.GameSceneName);
```

**注意**：使用属性 `GameSceneName`（大写开头）而不是字段 `gameSceneName`。

---

## 🎨 自动验证功能

打开 `GameConfig.asset`，在 Inspector 底部会显示验证信息：

```
┌─────────────────────────────────────────────────┐
│  配置验证                                        │
├─────────────────────────────────────────────────┤
│  ✓ 主菜单场景: MainMenu                         │
│  ✓ 游戏场景: Scene_0                            │
│  ✓ 已配置 1 个关卡                              │
│  ✓ 首关 'D1' 配置正确                           │
└─────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────┐
│  Build Settings 验证                            │
├─────────────────────────────────────────────────┤
│  ✓ 'MainMenu' 已添加到 Build Settings           │
│  ✓ 'Scene_0' 已添加到 Build Settings            │
└─────────────────────────────────────────────────┘
```

### 如果配置错误：
```
┌─────────────────────────────────────────────────┐
│  配置验证                                        │
├─────────────────────────────────────────────────┤
│  ⚠️ 主菜单场景 'MainMenu' 不存在于 Assets/Scenes/ │
│  ❌ 游戏场景 '' 未配置！                         │
└─────────────────────────────────────────────────┘
```

---

## 🔧 常见问题

### Q: 为什么还有 Scene Names 字段？
**A**: 因为 `SceneAsset` 只能在 Unity Editor 中使用，打包后的游戏需要使用字符串名称。`Scene Names` 字段会在你拖入场景时自动同步，你不需要手动填写。

### Q: 如果我重命名了场景文件会怎样？
**A**: GameConfig 会自动更新场景名称，无需手动修改！

### Q: 如果我删除了场景文件会怎样？
**A**: Inspector 中会显示 `None (SceneAsset)`，验证器会显示警告。

### Q: 旧的配置会失效吗？
**A**: 不会！如果你已经手动填写了 `Scene Names`，游戏仍然可以正常运行。拖入 `SceneAsset` 后会自动覆盖旧的名称。

### Q: 我能直接点击场景引用跳转到场景吗？
**A**: 可以！在 Inspector 中双击 `SceneAsset` 字段就能打开对应的场景。

---

## 🎯 迁移步骤（从旧配置迁移）

### 如果你已经有配置：

1. **打开 GameConfig.asset**
2. **不要清空现有的场景名称字段**
3. **直接拖入对应的场景文件**：
   - `Main Menu Scene Asset` ← 拖入 `MainMenu.unity`
   - `Game Scene Asset` ← 拖入 `Scene_0.unity`
4. **场景名称会自动更新为正确的值**
5. **保存**

完成！旧的配置会被自动替换。

---

## 📊 字段对照表

| 字段类型 | 用途 | 何时使用 | 是否需要手动填写 |
|---------|------|----------|----------------|
| **Scene Asset** | Editor 中引用场景文件 | Unity Editor | ✅ 拖拽场景文件 |
| **Scene Name** | Runtime 中加载场景 | 打包后的游戏 | ❌ 自动生成 |

---

## 🎉 总结

### 旧方式 vs 新方式

| 操作 | 旧方式 | 新方式 |
|------|--------|--------|
| **配置场景** | 手动输入场景名 | 拖拽场景文件 |
| **验证正确性** | 运行时报错 | 编辑器实时验证 |
| **场景重命名** | 手动更新所有引用 | 自动更新 |
| **拼写错误** | 容易发生 | 不可能发生 |

### 推荐做法

1. ✅ **在 Editor 中**：拖拽 `SceneAsset`
2. ✅ **在代码中**：使用属性 `config.MainMenuSceneName`
3. ❌ **不要手动编辑** `Scene Names` 字段（它们是自动生成的）

现在配置场景更安全、更方便了！🚀

