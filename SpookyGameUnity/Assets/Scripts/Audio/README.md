# 背景音乐系统使用指南

## 📁 文件说明

- **BackgroundMusicManager.cs** - 背景音乐管理器（单例）
- **MusicPlayer.cs** - 音乐播放器组件（可挂载在场景对象上）

---

## 🎵 快速开始

### 方法 1：使用 BackgroundMusicManager（推荐）

#### 1. 在场景中创建音乐管理器

```
Hierarchy > 右键 > Create Empty
重命名为 "BackgroundMusicManager"
挂载 BackgroundMusicManager.cs 脚本
```

#### 2. 配置管理器

在 Inspector 中设置：
- **Default Music**：拖入默认背景音乐的 AudioClip
- **Play On Awake**：勾选（游戏开始时自动播放）
- **Loop**：勾选（循环播放）
- **Volume**：设置音量（0-1）
- **Fade In Duration**：淡入时长（秒）
- **Fade Out Duration**：淡出时长（秒）

#### 3. 在代码中使用

```csharp
using SpookyGame.Audio;

// 播放音乐
BackgroundMusicManager.Instance.PlayMusic(myAudioClip);

// 停止音乐
BackgroundMusicManager.Instance.StopMusic();

// 暂停/恢复
BackgroundMusicManager.Instance.PauseMusic();
BackgroundMusicManager.Instance.ResumeMusic();

// 设置音量
BackgroundMusicManager.Instance.SetVolume(0.5f);

// 淡入/淡出
BackgroundMusicManager.Instance.FadeIn(2f);
BackgroundMusicManager.Instance.FadeOut(2f);
```

---

### 方法 2：使用 MusicPlayer 组件

适合在特定场景或关卡播放不同的背景音乐。

#### 1. 创建音乐播放器

```
Hierarchy > 右键 > Create Empty
重命名为 "LevelMusic"
挂载 MusicPlayer.cs 脚本
```

#### 2. 配置播放器

在 Inspector 中设置：
- **Music Clip**：拖入要播放的音乐
- **Play On Start**：勾选（场景开始时播放）
- **Fade In Duration**：淡入时长
- **Stop Previous Music**：勾选（停止之前的音乐）

#### 3. 在交互效果中使用

可以通过 UnityEvent 调用 `MusicPlayer.PlayMusic()` 方法。

---

## 🎮 使用场景示例

### 场景 1：全局背景音乐

在 Bootstrap 场景或主菜单场景中：
1. 创建 BackgroundMusicManager 对象
2. 设置 Default Music 为主题音乐
3. 勾选 Play On Awake 和 Loop

由于使用了 DontDestroyOnLoad，音乐会在所有场景中持续播放。

---

### 场景 2：不同关卡使用不同音乐

在每个关卡的 Stage 节点下：
1. 添加 MusicPlayer 组件
2. 设置对应关卡的音乐
3. 勾选 Play On Enable

当关卡激活时，会自动切换到对应的音乐。

---

### 场景 3：触发特定音乐

在某个可交互物体上：
1. 添加 MusicPlayer 组件
2. 设置特殊事件音乐
3. 通过交互效果或脚本调用 `PlayMusic()`

例如：发现重要线索时播放紧张的音乐。

---

## 🔧 高级功能

### 交叉淡变（Crossfade）

当播放新音乐时，会自动进行交叉淡变：
- 旧音乐淡出（1秒）
- 新音乐淡入（1秒）

```csharp
// 自动进行交叉淡变
BackgroundMusicManager.Instance.PlayMusic(newClip, 2f);
```

---

### 音量控制

```csharp
// 立即设置音量
BackgroundMusicManager.Instance.SetVolume(0.3f, false);

// 平滑过渡到新音量
BackgroundMusicManager.Instance.SetVolume(0.8f, true);

// 获取当前音量
float currentVolume = BackgroundMusicManager.Instance.GetVolume();
```

---

### 检查播放状态

```csharp
if (BackgroundMusicManager.Instance.IsPlaying())
{
    Debug.Log("音乐正在播放");
}

AudioClip currentClip = BackgroundMusicManager.Instance.GetCurrentClip();
Debug.Log("当前音乐: " + currentClip.name);
```

---

## 🎨 与游戏系统集成

### 与 GameConfig 集成

可以在 `GameConfig.cs` 中添加音乐配置：

```csharp
[Header("Audio Settings")]
public AudioClip mainMenuMusic;
public AudioClip gameplayMusic;
public float musicVolume = 0.5f;
```

然后在 Bootstrap 中初始化：

```csharp
private void InitializeAudio()
{
    BackgroundMusicManager.Instance.SetVolume(gameConfig.musicVolume);
    BackgroundMusicManager.Instance.PlayMusic(gameConfig.mainMenuMusic);
}
```

---

### 与场景切换集成

在 `SceneService.cs` 中，可以在场景切换时控制音乐：

```csharp
// 场景切换时淡出音乐
BackgroundMusicManager.Instance.FadeOut(1f);

// 新场景加载完成后淡入音乐
BackgroundMusicManager.Instance.FadeIn(2f);
```

---

## 🐛 调试技巧

### Inspector 右键菜单

在 BackgroundMusicManager 组件上右键，可以看到调试菜单：
- 播放测试音乐
- 停止音乐
- 暂停音乐
- 恢复播放

### Console 日志

音乐管理器会输出以下日志：
```
[BackgroundMusicManager] Initialized
[BackgroundMusicManager] Playing: YourMusicClip
[BackgroundMusicManager] Stopping music
[BackgroundMusicManager] Paused
[BackgroundMusicManager] Resumed
```

---

## 📝 注意事项

1. **单例模式**：BackgroundMusicManager 是单例，整个游戏只有一个实例
2. **跨场景持久化**：使用 DontDestroyOnLoad，在场景切换时不会被销毁
3. **自动创建**：如果场景中没有 BackgroundMusicManager，第一次访问时会自动创建
4. **淡入淡出**：建议使用淡入淡出效果，避免音乐突然开始或停止

---

## 🎯 推荐工作流程

### 开发阶段
1. 在 Bootstrap 场景中创建 BackgroundMusicManager
2. 设置默认音乐用于测试
3. 使用右键菜单快速测试播放/停止

### 正式游戏
1. 在 GameConfig 中配置所有音乐资源
2. 在 Bootstrap 初始化时播放主题音乐
3. 在各个关卡使用 MusicPlayer 切换音乐
4. 在关键事件中使用代码控制音乐变化

---

## 📚 扩展功能建议

如果需要更多功能，可以扩展：
- 音乐播放列表
- 随机播放
- 音乐过滤器效果（回声、混响等）
- 根据游戏状态动态调整音乐（战斗、探索等）
- 保存音量设置到 PlayerPrefs

