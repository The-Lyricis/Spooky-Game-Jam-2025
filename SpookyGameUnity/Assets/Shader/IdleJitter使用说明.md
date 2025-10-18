# Idle Jitter Shader 使用说明

## 🎯 功能介绍

这是一个专为2D游戏设计的抖动 Idle 动画 Shader，可以创造手绘风格的抖动效果，特别适合：
- 恐怖游戏的角色 Idle 动画
- 手绘线条风格的游戏
- 需要不稳定/惊恐效果的场景物体

## 🔧 核心原理

### 顶点抖动
在顶点着色器中，通过以下公式实现抖动：

```
ΔX = Strength_X × sin(Frequency_X × Time + Seed)
ΔY = Strength_Y × cos(Frequency_Y × Time + Seed)
P_new = P_original + (ΔX, ΔY)
```

其中：
- **Strength**：抖动幅度
- **Frequency**：抖动频率
- **Time**：游戏运行时间
- **Seed**：基于顶点坐标的随机种子（确保每个顶点抖动不同步）

### 额外效果
- **UV抖动**：纹理坐标的微小随机偏移
- **颜色抖动**：RGB通道的轻微失真

---

## 📋 参数说明

### 顶点抖动参数

| 参数 | 范围 | 推荐值 | 说明 |
|------|------|--------|------|
| **Jitter Strength X** | 0-0.1 | 0.01-0.02 | X方向抖动幅度 |
| **Jitter Strength Y** | 0-0.1 | 0.01-0.02 | Y方向抖动幅度 |
| **Jitter Frequency X** | 1-50 | 10-20 | X方向抖动频率（越大越快）|
| **Jitter Frequency Y** | 1-50 | 12-22 | Y方向抖动频率 |
| **Jitter Speed** | 0.1-10 | 1.0-2.0 | 整体抖动速度倍率 |

### UV抖动参数（可选）

| 参数 | 范围 | 推荐值 | 说明 |
|------|------|--------|------|
| **UV Jitter Strength** | 0-0.05 | 0.003-0.008 | UV偏移幅度 |
| **UV Jitter Frequency** | 1-30 | 15-20 | UV抖动频率 |

### 颜色抖动参数（可选）

| 参数 | 范围 | 推荐值 | 说明 |
|------|------|--------|------|
| **Color Jitter Strength** | 0-0.2 | 0.02-0.05 | 颜色失真强度 |
| **Color Jitter Speed** | 0.5-5 | 2-3 | 颜色抖动速度 |

---

## 🎮 预设配置

### 配置1：轻微抖动（常规Idle）
```
Jitter Strength X: 0.008
Jitter Strength Y: 0.008
Jitter Frequency X: 12
Jitter Frequency Y: 15
Jitter Speed: 1.0
UV Jitter Strength: 0.003
Color Jitter Strength: 0.01
```
**效果**：轻微的呼吸感，适合普通待机状态

### 配置2：中等抖动（紧张状态）
```
Jitter Strength X: 0.015
Jitter Strength Y: 0.015
Jitter Frequency X: 18
Jitter Frequency Y: 20
Jitter Speed: 1.5
UV Jitter Strength: 0.006
Color Jitter Strength: 0.03
```
**效果**：明显的抖动，适合紧张或恐惧状态

### 配置3：强烈抖动（惊恐/受伤）
```
Jitter Strength X: 0.025
Jitter Strength Y: 0.03
Jitter Frequency X: 25
Jitter Frequency Y: 28
Jitter Speed: 2.5
UV Jitter Strength: 0.01
Color Jitter Strength: 0.06
```
**效果**：剧烈抖动，适合惊恐、受伤或濒死状态

### 配置4：手绘线条风格
```
Jitter Strength X: 0.012
Jitter Strength Y: 0.012
Jitter Frequency X: 10
Jitter Frequency Y: 11
Jitter Speed: 0.8
UV Jitter Strength: 0.008
Color Jitter Strength: 0.02
```
**效果**：模拟手绘动画的不稳定线条

---

## 🚀 使用步骤

### 步骤1：创建材质（已完成）
材质已自动创建：`Assets/Material/IdleJitterMaterial.mat`

### 步骤2：分配到Sprite Renderer

#### 方法A：在Unity Editor中（推荐）
1. 在场景中选中`teethandshaver1`物体
2. 在Inspector中找到`Sprite Renderer`组件
3. 将`Material`字段改为`IdleJitterMaterial`

#### 方法B：自动应用（已完成）
脚本已自动修改场景文件，材质已分配

### 步骤3：调整参数
1. 选中`IdleJitterMaterial`材质
2. 在Inspector中调整参数
3. 实时查看效果（在Game视图或Scene视图）

---

## 💡 高级技巧

### 技巧1：动态控制抖动强度

创建脚本动态调整：

```csharp
using UnityEngine;

public class PlayerJitterController : MonoBehaviour
{
    private Material jitterMaterial;
    private SpriteRenderer spriteRenderer;
    
    [Header("状态抖动参数")]
    public float normalStrength = 0.008f;
    public float nervousStrength = 0.015f;
    public float panicStrength = 0.03f;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        jitterMaterial = spriteRenderer.material; // 获取材质实例
    }
    
    // 设置为正常状态
    public void SetNormalState()
    {
        jitterMaterial.SetFloat("_JitterStrengthX", normalStrength);
        jitterMaterial.SetFloat("_JitterStrengthY", normalStrength);
        jitterMaterial.SetFloat("_JitterSpeed", 1.0f);
    }
    
    // 设置为紧张状态
    public void SetNervousState()
    {
        jitterMaterial.SetFloat("_JitterStrengthX", nervousStrength);
        jitterMaterial.SetFloat("_JitterStrengthY", nervousStrength);
        jitterMaterial.SetFloat("_JitterSpeed", 1.5f);
    }
    
    // 设置为惊恐状态
    public void SetPanicState()
    {
        jitterMaterial.SetFloat("_JitterStrengthX", panicStrength);
        jitterMaterial.SetFloat("_JitterStrengthY", panicStrength);
        jitterMaterial.SetFloat("_JitterSpeed", 2.5f);
    }
    
    // 根据生命值动态调整
    public void UpdateByHealth(float healthPercent)
    {
        float strength = Mathf.Lerp(panicStrength, normalStrength, healthPercent);
        jitterMaterial.SetFloat("_JitterStrengthX", strength);
        jitterMaterial.SetFloat("_JitterStrengthY", strength);
    }
}
```

### 技巧2：平滑过渡效果

```csharp
IEnumerator TransitionToState(float targetStrength, float duration)
{
    float startStrength = jitterMaterial.GetFloat("_JitterStrengthX");
    float elapsed = 0f;
    
    while (elapsed < duration)
    {
        elapsed += Time.deltaTime;
        float t = elapsed / duration;
        float currentStrength = Mathf.Lerp(startStrength, targetStrength, t);
        
        jitterMaterial.SetFloat("_JitterStrengthX", currentStrength);
        jitterMaterial.SetFloat("_JitterStrengthY", currentStrength);
        
        yield return null;
    }
}

// 使用示例
StartCoroutine(TransitionToState(0.03f, 1.0f)); // 1秒内过渡到惊恐状态
```

### 技巧3：局部应用

将Shader应用于特定部件：
- 头部抖动更明显（惊恐表情）
- 手部抖动（紧张动作）
- 只给背景物体添加抖动（营造不稳定感）

### 技巧4：与动画系统结合

```csharp
// 在动画事件中触发抖动状态变化
public void OnDamaged()
{
    StartCoroutine(DamageJitter());
}

IEnumerator DamageJitter()
{
    // 瞬间强烈抖动
    jitterMaterial.SetFloat("_JitterStrengthX", 0.05f);
    jitterMaterial.SetFloat("_JitterSpeed", 5.0f);
    
    yield return new WaitForSeconds(0.3f);
    
    // 恢复正常
    StartCoroutine(TransitionToState(normalStrength, 0.5f));
}
```

---

## ⚠️ 注意事项

### 性能考虑
1. **顶点计算**：每帧都会计算，但开销很小
2. **多个物体**：如果场景中有很多使用此Shader的物体，考虑：
   - 降低频率参数
   - 关闭UV和颜色抖动（设为0）
   - 对远处物体禁用效果

### 材质实例
```csharp
// ❌ 错误：修改共享材质会影响所有使用它的物体
spriteRenderer.sharedMaterial.SetFloat("_JitterStrengthX", 0.1f);

// ✅ 正确：修改材质实例只影响当前物体
spriteRenderer.material.SetFloat("_JitterStrengthX", 0.1f);
```

### 参数范围
- **不要设置过大的Strength值**：>0.05会导致物体扭曲严重
- **不要设置过高的Frequency值**：>50可能看起来像闪烁而不是抖动

### 与其他Shader的区别
- 本Shader专门用于Sprite，不适用于3D模型
- 如果需要在UI上使用，需要修改Shader的Tags

---

## 🎨 效果展示

### 视觉效果描述

#### 默认效果（轻微抖动）
- 顶点位移：±1-2像素
- 速度：缓慢、有节奏
- 适合：Idle动画、呼吸效果

#### 中等强度
- 顶点位移：±2-4像素
- 速度：较快、不规则
- 适合：紧张状态、警觉

#### 强烈抖动
- 顶点位移：±4-8像素
- 速度：快速、混乱
- 适合：惊恐、受伤、濒死

---

## 🔍 调试技巧

### 问题1：看不到效果
**检查：**
- 材质是否正确分配
- Strength参数是否>0
- 物体是否在摄像机视野内

### 问题2：抖动太剧烈
**解决：**
- 降低Strength参数（0.005-0.01）
- 降低Frequency参数（5-10）
- 降低Speed参数（0.5-1.0）

### 问题3：抖动不够明显
**解决：**
- 增大Strength参数（0.02-0.03）
- 增大Frequency参数（20-30）
- 增大Speed参数（2.0-3.0）

### 问题4：抖动看起来不自然
**解决：**
- X和Y的Frequency设置不同值（产生非同步效果）
- 添加UV抖动（增强不稳定感）
- 调整Speed在0.8-1.5之间（避免过快或过慢）

---

## 📚 扩展阅读

### 相关概念
- **顶点着色器**：处理顶点位置和变换
- **正弦/余弦函数**：创造周期性动画
- **噪声函数**：生成随机但连续的值
- **Sprite Renderer**：Unity 2D渲染系统

### 进阶改进
1. **添加Perlin Noise**：更自然的抖动模式
2. **添加随机爆发**：偶尔的强烈抖动
3. **添加方向性**：根据力的方向抖动
4. **添加衰减**：抖动逐渐减弱

---

## ✅ 快速检查清单

使用前确认：
- [ ] Shader文件已导入（IdleJitterShader.shader）
- [ ] 材质已创建（IdleJitterMaterial.mat）
- [ ] 材质已分配到Sprite Renderer
- [ ] 物体有Sprite纹理
- [ ] 参数已调整到合适范围
- [ ] Game视图中可以看到效果

---

**祝你创作出独特的抖动动画效果！** 🎮✨

如有问题，检查：
1. Shader是否编译成功（无错误）
2. 材质Shader是否正确选择
3. 参数是否在推荐范围内

