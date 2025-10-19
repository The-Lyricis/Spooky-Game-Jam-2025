using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashingMonsters : MonoBehaviour
{
    [Header("Monster Objects")]
    [Tooltip("要闪动的怪物物体数组")]
    [SerializeField] private GameObject[] monsterObjects;
    
    [Header("Flash Settings")]
    [Tooltip("闪动次数")]
    [SerializeField] private int flashCount = 3;
    [Tooltip("单次闪动持续时间（秒）")]
    [SerializeField] private float flashDuration = 0.1f;
    [Tooltip("闪动间隔时间（秒）")]
    [SerializeField] private float flashInterval = 0.2f;
    [Tooltip("闪动时的透明度（0=完全透明，1=完全不透明）")]
    [SerializeField] private float flashAlpha = 1f;
    
    [Header("Animation Settings")]
    [Tooltip("是否在Start时自动开始闪动")]
    [SerializeField] private bool autoStart = true;
    [Tooltip("开始闪动的延迟时间")]
    [SerializeField] private float startDelay = 1f;
    [Tooltip("闪动时的音效")]
    [SerializeField] private AudioClip flashSound;
    [Tooltip("音频源")]
    [SerializeField] private AudioSource audioSource;
    
    [Header("Debug Settings")]
    [Tooltip("是否显示调试信息")]
    [SerializeField] private bool showDebugInfo = true;
    
    // 私有变量
    private List<SpriteRenderer> _monsterRenderers = new List<SpriteRenderer>();
    private List<float> _originalAlphas = new List<float>();
    private bool _isFlashing = false;
    private Coroutine _flashCoroutine;
    
    void Start()
    {
        InitializeMonsters();
        
        // 如果没有设置音频源，尝试获取
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        
        // 自动开始闪动
        if (autoStart)
        {
            StartFlashing();
        }
    }
    
    /// <summary>
    /// 初始化怪物对象
    /// </summary>
    private void InitializeMonsters()
    {
        _monsterRenderers.Clear();
        _originalAlphas.Clear();
        
        if (monsterObjects == null || monsterObjects.Length == 0)
        {
            Debug.LogWarning("[FlashingMonsters] 没有设置怪物对象！");
            return;
        }
        
        foreach (GameObject monster in monsterObjects)
        {
            if (monster != null)
            {
                SpriteRenderer renderer = monster.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    _monsterRenderers.Add(renderer);
                    _originalAlphas.Add(renderer.color.a);
                    
                    if (showDebugInfo)
                    {
                        Debug.Log($"[FlashingMonsters] 初始化怪物: {monster.name}, 原始透明度: {renderer.color.a}");
                    }
                }
                else
                {
                    Debug.LogWarning($"[FlashingMonsters] 怪物 {monster.name} 缺少 SpriteRenderer 组件！");
                }
            }
            else
            {
                Debug.LogWarning("[FlashingMonsters] 发现空的怪物对象引用！");
            }
        }
        
        if (showDebugInfo)
        {
            Debug.Log($"[FlashingMonsters] 初始化完成，共 {_monsterRenderers.Count} 个怪物");
        }
    }
    
    /// <summary>
    /// 开始闪动
    /// </summary>
    public void StartFlashing()
    {
        if (_isFlashing)
        {
            Debug.LogWarning("[FlashingMonsters] 已经在闪动中，无法重复开始");
            return;
        }
        
        if (_monsterRenderers.Count == 0)
        {
            Debug.LogError("[FlashingMonsters] 没有可用的怪物渲染器！");
            return;
        }
        
        if (showDebugInfo)
        {
            Debug.Log($"[FlashingMonsters] 开始闪动 {flashCount} 次");
        }
        
        _flashCoroutine = StartCoroutine(FlashCoroutine());
    }
    
    /// <summary>
    /// 停止闪动
    /// </summary>
    public void StopFlashing()
    {
        if (_flashCoroutine != null)
        {
            StopCoroutine(_flashCoroutine);
            _flashCoroutine = null;
        }
        
        // 恢复原始透明度
        RestoreOriginalAlphas();
        _isFlashing = false;
        
        if (showDebugInfo)
        {
            Debug.Log("[FlashingMonsters] 闪动已停止");
        }
    }
    
    /// <summary>
    /// 闪动协程
    /// </summary>
    private IEnumerator FlashCoroutine()
    {
        _isFlashing = true;
        
        // 开始延迟
        if (startDelay > 0)
        {
            yield return new WaitForSeconds(startDelay);
        }
        
        // 执行指定次数的闪动
        for (int i = 0; i < flashCount; i++)
        {
            if (showDebugInfo)
            {
                Debug.Log($"[FlashingMonsters] 第 {i + 1} 次闪动");
            }
            
            // 播放闪动音效
            PlayFlashSound();
            
            // 执行单次闪动
            yield return StartCoroutine(SingleFlash());
            
            // 闪动间隔（除了最后一次）
            if (i < flashCount - 1)
            {
                yield return new WaitForSeconds(flashInterval);
            }
        }
        
        _isFlashing = false;
        
        // 闪动完成后的日志
        OnFlashingComplete();
    }
    
    /// <summary>
    /// 单次闪动
    /// </summary>
    private IEnumerator SingleFlash()
    {
        // 设置为闪动透明度
        SetFlashAlpha();
        
        // 等待闪动持续时间
        yield return new WaitForSeconds(flashDuration);
        
        // 恢复原始透明度
        RestoreOriginalAlphas();
    }
    
    /// <summary>
    /// 设置闪动透明度
    /// </summary>
    private void SetFlashAlpha()
    {
        for (int i = 0; i < _monsterRenderers.Count; i++)
        {
            if (_monsterRenderers[i] != null)
            {
                Color currentColor = _monsterRenderers[i].color;
                currentColor.a = flashAlpha;
                _monsterRenderers[i].color = currentColor;
                
                if (showDebugInfo)
                {
                    Debug.Log($"[FlashingMonsters] 设置闪动透明度: {flashAlpha} (怪物: {_monsterRenderers[i].gameObject.name})");
                }
            }
        }
    }
    
    /// <summary>
    /// 恢复原始透明度
    /// </summary>
    private void RestoreOriginalAlphas()
    {
        for (int i = 0; i < _monsterRenderers.Count; i++)
        {
            if (_monsterRenderers[i] != null)
            {
                Color currentColor = _monsterRenderers[i].color;
                currentColor.a = _originalAlphas[i];
                _monsterRenderers[i].color = currentColor;
                
                if (showDebugInfo)
                {
                    Debug.Log($"[FlashingMonsters] 恢复原始透明度: {_originalAlphas[i]} (怪物: {_monsterRenderers[i].gameObject.name})");
                }
            }
        }
    }
    
    /// <summary>
    /// 播放闪动音效
    /// </summary>
    private void PlayFlashSound()
    {
        if (audioSource != null && flashSound != null)
        {
            audioSource.PlayOneShot(flashSound);
        }
    }
    
    /// <summary>
    /// 闪动完成后的回调
    /// </summary>
    private void OnFlashingComplete()
    {
        Debug.Log("[FlashingMonsters] 闪动完成！所有怪物已恢复正常状态");
        
        // 可以在这里添加其他完成后的逻辑
        // 例如：触发其他事件、播放其他音效等
    }
    
    /// <summary>
    /// 设置怪物对象数组
    /// </summary>
    public void SetMonsterObjects(GameObject[] monsters)
    {
        monsterObjects = monsters;
        InitializeMonsters();
    }
    
    /// <summary>
    /// 添加怪物对象
    /// </summary>
    public void AddMonsterObject(GameObject monster)
    {
        if (monster == null) return;
        
        // 添加到数组
        List<GameObject> monsterList = new List<GameObject>();
        if (monsterObjects != null)
        {
            monsterList.AddRange(monsterObjects);
        }
        monsterList.Add(monster);
        monsterObjects = monsterList.ToArray();
        
        // 重新初始化
        InitializeMonsters();
    }
    
    /// <summary>
    /// 获取当前怪物数量
    /// </summary>
    public int GetMonsterCount()
    {
        return _monsterRenderers.Count;
    }
    
    /// <summary>
    /// 是否正在闪动
    /// </summary>
    public bool IsFlashing => _isFlashing;
    
    /// <summary>
    /// 强制测试闪动（用于调试）
    /// </summary>
    public void ForceTestFlash()
    {
        Debug.Log("[FlashingMonsters] 强制测试闪动开始");
        
        // 重新初始化
        InitializeMonsters();
        
        if (_monsterRenderers.Count == 0)
        {
            Debug.LogError("[FlashingMonsters] 没有找到任何怪物渲染器！请检查怪物对象设置。");
            return;
        }
        
        // 强制设置闪动透明度
        SetFlashAlpha();
        
        // 1秒后恢复
        StartCoroutine(ForceTestRestore());
    }
    
    private IEnumerator ForceTestRestore()
    {
        yield return new WaitForSeconds(1f);
        RestoreOriginalAlphas();
        Debug.Log("[FlashingMonsters] 强制测试闪动结束");
    }
    
#if UNITY_EDITOR
    [ContextMenu("测试闪动")]
    private void TestFlashing()
    {
        if (Application.isPlaying)
        {
            StartFlashing();
        }
        else
        {
            Debug.LogWarning("请在运行时测试闪动功能");
        }
    }
    
    [ContextMenu("强制测试闪动")]
    private void TestForceFlash()
    {
        if (Application.isPlaying)
        {
            ForceTestFlash();
        }
        else
        {
            Debug.LogWarning("请在运行时测试闪动功能");
        }
    }
    
    [ContextMenu("停止闪动")]
    private void TestStopFlashing()
    {
        if (Application.isPlaying)
        {
            StopFlashing();
        }
    }
    
    [ContextMenu("预览闪动透明度")]
    private void PreviewFlashAlpha()
    {
        if (Application.isPlaying)
        {
            SetFlashAlpha();
        }
        else
        {
            Debug.LogWarning("请在运行时预览闪动透明度");
        }
    }
    
    [ContextMenu("恢复原始透明度")]
    private void TestRestoreAlphas()
    {
        if (Application.isPlaying)
        {
            RestoreOriginalAlphas();
        }
    }
    
    [ContextMenu("检查怪物设置")]
    private void CheckMonsterSetup()
    {
        Debug.Log($"[FlashingMonsters] 怪物对象数组长度: {monsterObjects?.Length ?? 0}");
        if (monsterObjects != null)
        {
            for (int i = 0; i < monsterObjects.Length; i++)
            {
                GameObject monster = monsterObjects[i];
                if (monster != null)
                {
                    SpriteRenderer renderer = monster.GetComponent<SpriteRenderer>();
                    Debug.Log($"[FlashingMonsters] 怪物 {i}: {monster.name}, SpriteRenderer: {(renderer != null ? "存在" : "缺失")}");
                }
                else
                {
                    Debug.LogWarning($"[FlashingMonsters] 怪物 {i}: 空引用");
                }
            }
        }
    }
#endif
}
