using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeGenerate : MonoBehaviour
{
    [Header("Eye Prefab Settings")]
    [Tooltip("眼睛预制体")]
    [SerializeField] private GameObject eyePrefab;
    [Tooltip("要生成的眼睛数量")]
    [SerializeField] private int eyeCount = 20;
    
    [Header("Generation Area")]
    [Tooltip("生成区域的中心点")]
    [SerializeField] private Transform generationCenter;
    [Tooltip("生成区域的宽度")]
    [SerializeField] private float areaWidth = 10f;
    [Tooltip("生成区域的高度")]
    [SerializeField] private float areaHeight = 8f;
    [Tooltip("是否使用圆形区域（false为矩形区域）")]
    [SerializeField] private bool useCircularArea = false;
    [Tooltip("圆形区域半径（仅在useCircularArea为true时有效）")]
    [SerializeField] private float circularRadius = 5f;
    
    [Header("Eye Properties")]
    [Tooltip("眼睛大小范围（最小倍数）")]
    [SerializeField] private float minScale = 0.3f;
    [Tooltip("眼睛大小范围（最大倍数）")]
    [SerializeField] private float maxScale = 1.5f;
    [Tooltip("是否随机旋转")]
    [SerializeField] private bool randomRotation = true;
    [Tooltip("最小旋转角度")]
    [SerializeField] private float minRotation = 0f;
    [Tooltip("最大旋转角度")]
    [SerializeField] private float maxRotation = 360f;
    
    [Header("Generation Timing")]
    [Tooltip("生成间隔时间（秒）")]
    [SerializeField] private float generationInterval = 0.2f;
    [Tooltip("是否在Start时自动开始生成")]
    [SerializeField] private bool autoStart = true;
    [Tooltip("生成延迟时间（开始生成前的等待时间）")]
    [SerializeField] private float startDelay = 1f;
    [Tooltip("是否持续生成（true=无限生成，false=生成指定数量后停止）")]
    [SerializeField] private bool continuousGeneration = true;
    
    [Header("Eye Lifetime")]
    [Tooltip("眼睛存在时间（秒）")]
    [SerializeField] private float eyeLifetime = 5f;
    [Tooltip("眼睛存在时间随机范围（在基础时间上加减）")]
    [SerializeField] private float lifetimeVariation = 1f;
    [Tooltip("眼睛消失时的淡出动画时长")]
    [SerializeField] private float fadeOutDuration = 0.5f;
    
    [Header("Layer Settings")]
    [Tooltip("生成的眼睛的排序层级")]
    [SerializeField] private int sortingOrder = 0;
    [Tooltip("是否随机排序层级")]
    [SerializeField] private bool randomSortingOrder = true;
    [Tooltip("排序层级范围")]
    [SerializeField] private Vector2Int sortingOrderRange = new Vector2Int(-5, 5);
    
    [Header("Animation Settings")]
    [Tooltip("生成时的淡入动画时长")]
    [SerializeField] private float fadeInDuration = 0.5f;
    [Tooltip("是否播放生成音效")]
    [SerializeField] private bool playSoundOnGenerate = true;
    [Tooltip("生成音效")]
    [SerializeField] private AudioClip generateSound;
    [Tooltip("音频源")]
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private GameObject bigbigMonster;
    [SerializeField] private GameObject zhezhao;
    [SerializeField] private GameObject yanjing;
    
    // 私有变量
    private List<GameObject> _generatedEyes = new List<GameObject>();
    private List<Coroutine> _eyeLifetimeCoroutines = new List<Coroutine>();
    private bool _isGenerating = false;
    private int _currentEyeIndex = 0;
    private Coroutine _generationCoroutine;
    [SerializeField] private GameObject EndPanel;
    
    void Start()
    {
        // 如果没有设置生成中心，使用当前对象的位置
        if (generationCenter == null)
        {
            generationCenter = transform;
        }
        
        // 如果没有设置音频源，尝试获取
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        
        // 自动开始生成
        if (autoStart)
        {
            StartGeneration();
            StartCoroutine(EndTimeWait());
        }
    }
    IEnumerator EndTimeWait()
    {
        yield return new WaitForSeconds(5f);
        StopGeneration();
        yield return new WaitForSeconds(5f);
        bigbigMonster.SetActive(false);
        zhezhao.SetActive(false);
        
        yield return new WaitForSeconds(1f);
        //
        EndPanel.SetActive(true);
        //

    }
    
    /// <summary>
    /// 开始生成眼睛
    /// </summary>
    public void StartGeneration()
    {
        if (_isGenerating)
        {
            Debug.LogWarning("[EyeGenerate] 已经在生成中，无法重复开始");
            return;
        }
        
        if (eyePrefab == null)
        {
            Debug.LogError("[EyeGenerate] 没有设置眼睛预制体！");
            return;
        }
        
        Debug.Log($"[EyeGenerate] 开始生成 {eyeCount} 个眼睛");
        _generationCoroutine = StartCoroutine(GenerateEyesCoroutine());
    }
    
    /// <summary>
    /// 停止生成眼睛
    /// </summary>
    public void StopGeneration()
    {
        if (_generationCoroutine != null)
        {
            StopCoroutine(_generationCoroutine);
            _generationCoroutine = null;
        }
        _isGenerating = false;
        Debug.Log("[EyeGenerate] 停止生成眼睛");
    }
    
    /// <summary>
    /// 设置持续生成模式
    /// </summary>
    public void SetContinuousGeneration(bool continuous)
    {
        continuousGeneration = continuous;
        Debug.Log($"[EyeGenerate] 持续生成模式: {continuous}");
    }
    
    /// <summary>
    /// 生成眼睛的协程
    /// </summary>
    private IEnumerator GenerateEyesCoroutine()
    {
        _isGenerating = true;
        _currentEyeIndex = 0;
        
        // 开始延迟
        if (startDelay > 0)
        {
            yield return new WaitForSeconds(startDelay);
        }
        
        // 持续生成或生成指定数量
        while (continuousGeneration || _currentEyeIndex < eyeCount)
        {
            GenerateSingleEye();
            _currentEyeIndex++;
            
            // 生成间隔
            if (generationInterval > 0)
            {
                yield return new WaitForSeconds(generationInterval);
            }
        }
        
        _isGenerating = false;
        Debug.Log($"[EyeGenerate] 完成生成 {_generatedEyes.Count} 个眼睛");
        
        // 生成完成后的回调
        OnGenerationComplete();
    }
    
    /// <summary>
    /// 生成单个眼睛
    /// </summary>
    private void GenerateSingleEye()
    {
        // 计算生成位置
        Vector3 spawnPosition = GetRandomSpawnPosition();
        
        // 实例化眼睛
        GameObject newEye = Instantiate(eyePrefab, spawnPosition, Quaternion.identity, transform);
        
        // 设置随机大小
        float randomScale = Random.Range(minScale, maxScale);
        newEye.transform.localScale = Vector3.one * randomScale;
        
        // 设置随机旋转
        if (randomRotation)
        {
            float randomRotationZ = Random.Range(minRotation, maxRotation);
            newEye.transform.rotation = Quaternion.Euler(0, 0, randomRotationZ);
        }
        
        // 设置排序层级
        SpriteRenderer eyeRenderer = newEye.GetComponent<SpriteRenderer>();
        if (eyeRenderer != null)
        {
            if (randomSortingOrder)
            {
                int randomOrder = Random.Range(sortingOrderRange.x, sortingOrderRange.y + 1);
                eyeRenderer.sortingOrder = randomOrder;
            }
            else
            {
                eyeRenderer.sortingOrder = sortingOrder;
            }
        }
        
        // 添加到列表
        _generatedEyes.Add(newEye);
        
        // 播放生成音效
        if (playSoundOnGenerate && audioSource != null && generateSound != null)
        {
            audioSource.PlayOneShot(generateSound);
        }
        
        // 播放淡入动画
        if (fadeInDuration > 0)
        {
            StartCoroutine(FadeInEye(newEye));
        }
        
        // 启动眼睛生命周期
        Coroutine lifetimeCoroutine = StartCoroutine(HandleEyeLifetime(newEye));
        _eyeLifetimeCoroutines.Add(lifetimeCoroutine);
        
        Debug.Log($"[EyeGenerate] 生成第 {_currentEyeIndex + 1} 个眼睛，位置: {spawnPosition}");
    }
    
    /// <summary>
    /// 获取随机生成位置
    /// </summary>
    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 centerPos = generationCenter.position;
        
        if (useCircularArea)
        {
            // 圆形区域
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float radius = Random.Range(0f, circularRadius);
            
            float x = centerPos.x + Mathf.Cos(angle) * radius;
            float y = centerPos.y + Mathf.Sin(angle) * radius;
            
            return new Vector3(x, y, centerPos.z);
        }
        else
        {
            // 矩形区域
            float x = centerPos.x + Random.Range(-areaWidth * 0.5f, areaWidth * 0.5f);
            float y = centerPos.y + Random.Range(-areaHeight * 0.5f, areaHeight * 0.5f);
            
            return new Vector3(x, y, centerPos.z);
        }
    }
    
    /// <summary>
    /// 眼睛淡入动画
    /// </summary>
    private IEnumerator FadeInEye(GameObject eye)
    {
        SpriteRenderer renderer = eye.GetComponent<SpriteRenderer>();
        if (renderer == null) yield break;
        
        Color originalColor = renderer.color;
        Color transparentColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        
        // 设置为透明
        renderer.color = transparentColor;
        
        // 淡入动画
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, originalColor.a, elapsed / fadeInDuration);
            renderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
        
        // 确保最终颜色正确
        renderer.color = originalColor;
    }
    
    /// <summary>
    /// 处理眼睛生命周期
    /// </summary>
    private IEnumerator HandleEyeLifetime(GameObject eye)
    {
        // 计算随机存在时间
        float randomLifetime = eyeLifetime + Random.Range(-lifetimeVariation, lifetimeVariation);
        randomLifetime = Mathf.Max(0.1f, randomLifetime); // 确保至少存在0.1秒
        
        // 等待存在时间
        yield return new WaitForSeconds(randomLifetime);
        
        // 播放淡出动画并销毁
        if (eye != null)
        {
            yield return StartCoroutine(FadeOutEye(eye));
            DestroyEye(eye);
        }
    }
    
    /// <summary>
    /// 眼睛淡出动画
    /// </summary>
    private IEnumerator FadeOutEye(GameObject eye)
    {
        SpriteRenderer renderer = eye.GetComponent<SpriteRenderer>();
        if (renderer == null) yield break;
        
        Color originalColor = renderer.color;
        
        // 淡出动画
        float elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(originalColor.a, 0f, elapsed / fadeOutDuration);
            renderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
        
        // 确保完全透明
        renderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
    }
    
    /// <summary>
    /// 销毁眼睛
    /// </summary>
    private void DestroyEye(GameObject eye)
    {
        if (eye != null)
        {
            // 从列表中移除
            _generatedEyes.Remove(eye);
            
            // 销毁对象
            Destroy(eye);
            
            Debug.Log("[EyeGenerate] 眼睛已销毁");
        }
    }
    
    /// <summary>
    /// 生成完成后的回调（可重写）
    /// </summary>
    protected virtual void OnGenerationComplete()
    {
        Debug.Log("[EyeGenerate] 所有眼睛生成完成！");
        // 子类可以重写此方法添加自定义逻辑
    }
    
    /// <summary>
    /// 清除所有生成的眼睛
    /// </summary>
    public void ClearAllEyes()
    {
        // 停止所有生命周期协程
        foreach (Coroutine coroutine in _eyeLifetimeCoroutines)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }
        _eyeLifetimeCoroutines.Clear();
        
        // 销毁所有眼睛
        foreach (GameObject eye in _generatedEyes)
        {
            if (eye != null)
            {
                Destroy(eye);
            }
        }
        _generatedEyes.Clear();
        _currentEyeIndex = 0;
        Debug.Log("[EyeGenerate] 已清除所有眼睛");
    }
    
    /// <summary>
    /// 重新生成所有眼睛
    /// </summary>
    public void RegenerateAllEyes()
    {
        ClearAllEyes();
        StartGeneration();
    }
    
    /// <summary>
    /// 获取当前生成的眼睛数量
    /// </summary>
    public int GetCurrentEyeCount()
    {
        return _generatedEyes.Count;
    }
    
    /// <summary>
    /// 是否正在生成
    /// </summary>
    public bool IsGenerating => _isGenerating;
    
    /// <summary>
    /// 在Scene视图中绘制生成区域
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (generationCenter == null) return;
        
        Gizmos.color = Color.yellow;
        Vector3 center = generationCenter.position;
        
        if (useCircularArea)
        {
            // 绘制圆形区域
            Gizmos.DrawWireSphere(center, circularRadius);
        }
        else
        {
            // 绘制矩形区域
            Vector3 size = new Vector3(areaWidth, areaHeight, 0);
            Gizmos.DrawWireCube(center, size);
        }
    }
    
#if UNITY_EDITOR
    [ContextMenu("测试生成眼睛")]
    private void TestGenerateEyes()
    {
        if (Application.isPlaying)
        {
            StartGeneration();
        }
        else
        {
            Debug.LogWarning("请在运行时测试生成功能");
        }
    }
    
    [ContextMenu("清除所有眼睛")]
    private void TestClearEyes()
    {
        if (Application.isPlaying)
        {
            ClearAllEyes();
        }
    }
    
    [ContextMenu("重新生成")]
    private void TestRegenerate()
    {
        if (Application.isPlaying)
        {
            RegenerateAllEyes();
        }
    }
#endif
}
