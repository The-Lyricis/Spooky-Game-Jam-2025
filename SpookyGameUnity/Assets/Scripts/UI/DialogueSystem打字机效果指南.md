# DialogueSystem 打字机效果指南 ⌨️

## 📖 概述

`DialogueSystem` 现已支持**打字机效果**（逐字显示文本），提供更好的阅读体验和沉浸感。

---

## ✨ 功能特性

- ✅ 逐字显示对话文本
- ✅ 可调节打字速度（字符/秒）
- ✅ 点击跳过打字机效果
- ✅ 自动防止点击穿透
- ✅ 可选启用/禁用

---

## 🚀 快速配置

### Inspector 配置

```
DialogueSystem (Script)
├─ [UI References]
│  ├─ Dialogue Panel: DialoguePanel
│  └─ Dialogue Text: DialogueText
│
├─ [Settings]
│  ├─ Auto Find UI: ✓
│  └─ Enable Debug Log: ✓
│
└─ [Typewriter Settings]
   ├─ Enable Typewriter: ✓ ← 启用打字机效果
   ├─ Typewriter Speed: 30 ← 打字速度（字符/秒）
   └─ Can Skip Typewriter: ✓ ← 点击可跳过
```

**推荐速度设置**：
- **快速**：40-50 字符/秒
- **中速**：25-35 字符/秒（推荐）
- **慢速**：15-20 字符/秒
- **超慢**：5-10 字符/秒（用于紧张氛围）

---

## 🎮 交互行为

### 情况 1：打字机正在打字

```
用户操作：点击对话面板
行为：跳过打字机，立即显示全部文字
结果：文字全部显示，等待下一次点击
```

### 情况 2：打字机已完成

```
用户操作：点击对话面板
行为：推进到下一行对话
结果：开始显示下一行的打字机效果
```

### 情况 3：最后一行对话完成

```
用户操作：点击对话面板
行为：结束对话，触发回调
结果：对话面板隐藏，执行后续逻辑
```

---

## 🎯 使用示例

### 基础使用（无需额外代码）

```csharp
// Window.cs
public class Window : Interactable
{
    [SerializeField] private DialogueSystem dialogueSystem;
    [SerializeField] private string[] dialogueLines = new string[]
    {
        "窗外的景色很平静...",
        "但总感觉有什么不对劲。"
    };
    
    protected override void OnInteract(GameObject actor)
    {
        // 自动使用打字机效果（如果启用）
        dialogueSystem.StartDialogue(dialogueLines, OnDialogueComplete);
    }
    
    private void OnDialogueComplete()
    {
        Debug.Log("对话完成！");
    }
}
```

**用户体验**：
1. 点击窗户 → 对话面板显示，文字逐字出现："窗外的景色很平静..."
2. 点击面板 → 跳过打字，立即显示全部文字
3. 再次点击 → 下一行开始打字："但总感觉有什么不对劲。"
4. 点击跳过 → 文字全部显示
5. 再次点击 → 对话结束

---

### 动态控制打字速度

```csharp
public class DynamicDialogue : MonoBehaviour
{
    [SerializeField] private DialogueSystem dialogueSystem;
    
    public void ShowNormalDialogue()
    {
        // 使用默认速度（30 字符/秒）
        dialogueSystem.StartDialogue(new string[]
        {
            "这是正常速度的对话。"
        });
    }
    
    public void ShowSlowDialogue()
    {
        // 注意：目前需要在 Inspector 中手动调整速度
        // 或者通过反射/公共 API 修改
        dialogueSystem.StartDialogue(new string[]
        {
            "这是慢速对话，营造紧张氛围..."
        });
    }
}
```

---

### 禁用打字机效果

在 Inspector 中：
```
DialogueSystem (Script)
└─ [Typewriter Settings]
   └─ Enable Typewriter: ✗ ← 取消勾选
```

**效果**：所有对话立即全部显示，无打字机效果。

---

## 🔧 高级配置

### 自定义打字速度的公共 API

如果需要在代码中动态修改打字速度，可以添加以下方法到 `DialogueSystem.cs`：

```csharp
// 添加到 DialogueSystem.cs

/// <summary>
/// 设置打字机速度
/// </summary>
/// <param name="speed">字符/秒</param>
public void SetTypewriterSpeed(float speed)
{
    typewriterSpeed = Mathf.Max(1f, speed); // 最小 1 字符/秒
}

/// <summary>
/// 启用/禁用打字机效果
/// </summary>
public void SetTypewriterEnabled(bool enabled)
{
    enableTypewriter = enabled;
}
```

**使用示例**：
```csharp
// 快速对话
dialogueSystem.SetTypewriterSpeed(50f);
dialogueSystem.StartDialogue(normalLines);

// 慢速紧张对话
dialogueSystem.SetTypewriterSpeed(10f);
dialogueSystem.StartDialogue(suspenseLines);
```

---

### 不同字符不同速度（高级）

如果想让标点符号停顿更久，可以修改 `TypewriterEffect()` 协程：

```csharp
// 修改 DialogueSystem.cs 的 TypewriterEffect()

private System.Collections.IEnumerator TypewriterEffect(string fullText)
{
    _isTyping = true;
    dialogueText.text = "";
    
    float baseDelay = 1f / typewriterSpeed;
    
    for (int i = 0; i < fullText.Length; i++)
    {
        char c = fullText[i];
        dialogueText.text += c;
        
        // 标点符号停顿更久
        float delay = baseDelay;
        if (c == '。' || c == '！' || c == '？' || c == '…')
        {
            delay *= 3f; // 句末停顿 3 倍时间
        }
        else if (c == '，' || c == '、')
        {
            delay *= 1.5f; // 逗号停顿 1.5 倍时间
        }
        
        yield return new UnityEngine.WaitForSeconds(delay);
    }
    
    _isTyping = false;
    _typewriterCoroutine = null;
}
```

---

### 添加打字音效

在 `TypewriterEffect()` 中添加音效：

```csharp
[Header("Audio Settings")]
[SerializeField] private AudioSource audioSource;
[SerializeField] private AudioClip typeSound;

private System.Collections.IEnumerator TypewriterEffect(string fullText)
{
    _isTyping = true;
    dialogueText.text = "";
    
    float delay = 1f / typewriterSpeed;
    
    for (int i = 0; i < fullText.Length; i++)
    {
        dialogueText.text += fullText[i];
        
        // 播放打字音效
        if (audioSource != null && typeSound != null)
        {
            audioSource.PlayOneShot(typeSound, 0.3f);
        }
        
        yield return new UnityEngine.WaitForSeconds(delay);
    }
    
    _isTyping = false;
    _typewriterCoroutine = null;
}
```

---

## 🐛 常见问题

### Q: 打字机效果没有启动

**A**: 检查：
1. **Enable Typewriter 勾选了吗？**
   - Inspector > Typewriter Settings > Enable Typewriter: ✓
2. **Typewriter Speed > 0 吗？**
   - 必须大于 0，推荐 25-35
3. **对话文本是否为空？**
   - 空字符串会跳过打字机效果

---

### Q: 打字速度太快/太慢

**A**: 调整 `Typewriter Speed`：
- 太快：降低数值（如 30 → 20）
- 太慢：提高数值（如 30 → 40）

**公式**：
```
实际速度（字符/秒）= Typewriter Speed
每个字符延迟（秒）= 1 / Typewriter Speed
```

**示例**：
- Speed = 30 → 每个字符 0.033 秒
- Speed = 10 → 每个字符 0.1 秒（很慢）
- Speed = 60 → 每个字符 0.017 秒（很快）

---

### Q: 点击无法跳过打字机

**A**: 检查：
1. **Can Skip Typewriter 勾选了吗？**
   - Inspector > Can Skip Typewriter: ✓
2. **点击在对话面板上吗？**
   - 必须点击对话面板区域（有 Image 组件的区域）
3. **DialoguePanel 的 Image.Raycast Target 勾选了吗？**
   - 必须勾选 ✓

---

### Q: 打字机播放一半时推进到下一行，前一行没完成

**A**: 这是正常的！当前行会被停止，立即开始下一行。

如果想强制完成当前行再推进：
```csharp
// 修改 NextLine()
public void NextLine()
{
    // 如果正在打字，先跳过
    if (_isTyping)
    {
        SkipTypewriter();
        return; // 等待下一次点击
    }
    
    // ... 原有推进逻辑
}
```

---

### Q: 如何让某些对话不使用打字机？

**A**: 两种方法：

**方法 1：全局禁用**
```
DialogueSystem > Enable Typewriter: ✗
```

**方法 2：空字符串自动跳过**
```csharp
// 打字机会检查字符串是否为空
if (enableTypewriter && !string.IsNullOrEmpty(line))
{
    // 使用打字机
}
else
{
    // 直接显示
}
```

---

## 📊 性能优化

### 对于长文本

长文本（100+ 字符）可能导致卡顿，优化建议：

1. **分割长文本**：
```csharp
// 不推荐
string[] lines = new string[]
{
    "这是一段非常非常非常非常非常长的对话，可能会导致打字机效果持续很久..."
};

// 推荐
string[] lines = new string[]
{
    "这是一段比较长的对话，",
    "为了更好的阅读体验，",
    "我们把它分成了多行。"
};
```

2. **提高打字速度**：
```
Typewriter Speed: 40-50（长文本推荐）
```

3. **允许跳过**：
```
Can Skip Typewriter: ✓（必须勾选）
```

---

## 🎨 视觉效果增强

### 1. 添加闪烁光标

在 DialogueText 后面添加一个光标：

```
DialoguePanel
├─ DialogueText
└─ Cursor (Text)
   ├─ Text: "▌"
   ├─ Anchor: 与 DialogueText 右侧对齐
   └─ Cursor (Script) ← 闪烁脚本
```

```csharp
// Cursor.cs - 简单闪烁脚本
public class Cursor : MonoBehaviour
{
    private Text _text;
    private float _blinkTime = 0f;
    
    void Start()
    {
        _text = GetComponent<Text>();
    }
    
    void Update()
    {
        _blinkTime += Time.deltaTime;
        if (_blinkTime > 0.5f)
        {
            _text.enabled = !_text.enabled;
            _blinkTime = 0f;
        }
    }
}
```

---

### 2. 添加继续提示

在对话文本下方添加提示：

```
DialoguePanel
├─ DialogueText
└─ ContinueHint (Text)
   ├─ Text: "▼ 点击继续 ▼"
   ├─ Anchor: Bottom-Center
   └─ ContinueHint (Script) ← 只在打字完成时显示
```

```csharp
// ContinueHint.cs
public class ContinueHint : MonoBehaviour
{
    [SerializeField] private DialogueSystem dialogueSystem;
    private Text _text;
    
    void Start()
    {
        _text = GetComponent<Text>();
    }
    
    void Update()
    {
        // 只在打字完成且对话激活时显示
        _text.enabled = dialogueSystem.IsDialogueActive && !dialogueSystem.IsTyping;
    }
}
```

**注意**：需要给 `DialogueSystem` 添加 `IsTyping` 属性：
```csharp
// DialogueSystem.cs
public bool IsTyping => _isTyping;
```

---

## ✅ 最终检查清单

配置检查：
- [ ] Enable Typewriter 已勾选 ✓
- [ ] Typewriter Speed 设置为 25-35
- [ ] Can Skip Typewriter 已勾选 ✓
- [ ] DialoguePanel 有 Image 组件
- [ ] Image.Raycast Target 已勾选 ✓

功能测试：
- [ ] 对话文字逐字出现 ✅
- [ ] 打字速度合适（不太快/不太慢） ✅
- [ ] 点击可跳过打字机效果 ✅
- [ ] 跳过后再点击推进到下一行 ✅
- [ ] 对话结束后正常触发回调 ✅
- [ ] Console 没有错误日志 ✅

---

## 🎉 效果展示

### 用户体验流程

```
1. 点击窗户
   ↓
2. 对话面板显示
   ↓
3. 文字逐字出现："窗外的景色很平静..."
   （打字机效果，约 1-2 秒）
   ↓
4. 用户点击（不想等待）
   ↓
5. 立即显示全部文字："窗外的景色很平静..."
   ↓
6. 用户再次点击
   ↓
7. 下一行开始打字："但总感觉有什么不对劲。"
   ↓
8. 打字完成（或用户跳过）
   ↓
9. 用户再次点击
   ↓
10. 对话结束，面板隐藏
```

---

现在你的对话系统有了专业的打字机效果！⌨️✨

