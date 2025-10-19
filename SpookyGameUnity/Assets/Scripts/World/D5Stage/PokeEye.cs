using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpookyGame.Core;
using SpookyGame.World;

public class PokeEye : MonoBehaviour
{
    [Header("Eye Settings")]
    [Tooltip("左眼对象")]
    [SerializeField] private GameObject leftEye;
    [Tooltip("右眼对象")]
    [SerializeField] private GameObject rightEye;
    
    [Header("Mist Objects")]
    [Tooltip("左侧迷雾物体")]
    [SerializeField] private GameObject leftMist;
    [Tooltip("右侧迷雾物体")]
    [SerializeField] private GameObject rightMist;
    
    [Header("Animation Settings")]
    [Tooltip("眼睛点击音效")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip eyeClickSfx;
    [Tooltip("眼睛点击后的动画时间")]
    [SerializeField] private float eyeAnimationDuration = 0.5f;
    [Tooltip("等待时间（两个眼睛都被点击后）")]
    [SerializeField] private float waitTimeAfterBothClicked = 2f;
    
    [Header("Event Settings")]
    [Tooltip("后续事件触发时播放的音效")]
    [SerializeField] private AudioClip finalEventSfx;
    [Tooltip("是否在后续事件后切换场景")]
    [SerializeField] private bool switchSceneAfterEvent = true;
    [Tooltip("目标场景ID")]
    [SerializeField] private string targetSceneId = "End";
    
    // 状态跟踪
    private bool _leftEyeClicked = false;
    private bool _rightEyeClicked = false;
    private bool _isProcessing = false;
    
    // 组件引用
    private Collider2D _leftEyeCollider;
    private Collider2D _rightEyeCollider;
    private SpriteRenderer _leftEyeRenderer;
    private SpriteRenderer _rightEyeRenderer;
    [SerializeField] private GameObject EyePanel;
    
    void Start()
    {
        InitializeComponents();
        SetupInitialState();
    }
    
    /// <summary>
    /// 初始化组件引用
    /// </summary>
    private void InitializeComponents()
    {
        // 获取左眼组件
        if (leftEye != null)
        {
            _leftEyeCollider = leftEye.GetComponent<Collider2D>();
            _leftEyeRenderer = leftEye.GetComponent<SpriteRenderer>();
            
            if (_leftEyeCollider == null)
            {
                Debug.LogWarning("[PokeEye] 左眼缺少 Collider2D 组件");
            }
        }
        
        // 获取右眼组件
        if (rightEye != null)
        {
            _rightEyeCollider = rightEye.GetComponent<Collider2D>();
            _rightEyeRenderer = rightEye.GetComponent<SpriteRenderer>();
            
            if (_rightEyeCollider == null)
            {
                Debug.LogWarning("[PokeEye] 右眼缺少 Collider2D 组件");
            }
        }
    }
    
    /// <summary>
    /// 设置初始状态
    /// </summary>
    private void SetupInitialState()
    {
        // 确保迷雾物体初始状态为禁用
        if (leftMist != null)
        {
            leftMist.SetActive(false);
        }
        
        if (rightMist != null)
        {
            rightMist.SetActive(false);
        }
    }
    
    void Update()
    {
        HandleInput();
    }
    
    /// <summary>
    /// 处理输入
    /// </summary>
    private void HandleInput()
    {
        if (_isProcessing) return;
        
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            
            // 检查左眼点击
            if (!_leftEyeClicked && _leftEyeCollider != null && _leftEyeCollider.OverlapPoint(mousePos2D))
            {
                ClickLeftEye();
            }
            // 检查右眼点击
            else if (!_rightEyeClicked && _rightEyeCollider != null && _rightEyeCollider.OverlapPoint(mousePos2D))
            {
                ClickRightEye();
            }
        }
    }
    
    /// <summary>
    /// 点击左眼
    /// </summary>
    private void ClickLeftEye()
    {
        Debug.Log("[PokeEye] 左眼被点击");
        _leftEyeClicked = true;
        
        // 播放音效
        PlayEyeClickSound();
        
        // 激活左侧迷雾
        if (leftMist != null)
        {
            leftMist.SetActive(true);
            Debug.Log("[PokeEye] 左侧迷雾已激活");
        }
        
        // 播放眼睛动画
        StartCoroutine(PlayEyeAnimation(_leftEyeRenderer, true));
        
        // 检查是否两个眼睛都被点击
        CheckBothEyesClicked();
    }
    
    /// <summary>
    /// 点击右眼
    /// </summary>
    private void ClickRightEye()
    {
        Debug.Log("[PokeEye] 右眼被点击");
        _rightEyeClicked = true;
        
        // 播放音效
        PlayEyeClickSound();
        
        // 激活右侧迷雾
        if (rightMist != null)
        {
            rightMist.SetActive(true);
            Debug.Log("[PokeEye] 右侧迷雾已激活");
        }
        
        // 播放眼睛动画
        StartCoroutine(PlayEyeAnimation(_rightEyeRenderer, false));
        
        // 检查是否两个眼睛都被点击
        CheckBothEyesClicked();
    }
    
    /// <summary>
    /// 检查两个眼睛是否都被点击
    /// </summary>
    private void CheckBothEyesClicked()
    {
        if (_leftEyeClicked && _rightEyeClicked)
        {
            Debug.Log("[PokeEye] 两个眼睛都被点击，开始后续事件");
            StartCoroutine(TriggerFinalEvent());
        }
    }
    
    /// <summary>
    /// 触发后续事件
    /// </summary>
    private IEnumerator TriggerFinalEvent()
    {
        _isProcessing = true;
        
        // 等待指定时间
        yield return new WaitForSeconds(waitTimeAfterBothClicked);
        
        Debug.Log("[PokeEye] 触发后续事件");
        
        // 播放最终事件音效
        if (audioSource != null && finalEventSfx != null)
        {
            audioSource.PlayOneShot(finalEventSfx);
        }
        
        // 执行后续事件逻辑
        OnFinalEventTriggered();
        
        // 如果需要切换场景
        if (switchSceneAfterEvent)
        {
            yield return new WaitForSeconds(1f); // 给音效播放时间
            SwitchToNextScene();
        }
    }
    
    /// <summary>
    /// 后续事件触发时的逻辑（子类可重写）
    /// </summary>
    protected virtual void OnFinalEventTriggered()
    {
        Debug.Log("[PokeEye] 后续事件已触发");
        EyePanel.SetActive(true);
        // 这里可以添加具体的后续事件逻辑
        // 例如：播放特殊动画、显示文本、触发其他游戏机制等
    }
    
    /// <summary>
    /// 切换到下一个场景
    /// </summary>
    private void SwitchToNextScene()
    {
        // Debug.Log($"[PokeEye] 切换到场景: {targetSceneId}");
        // SceneService.FadeToStage(targetSceneId, 1f, null);
    }
    
    /// <summary>
    /// 播放眼睛点击音效
    /// </summary>
    private void PlayEyeClickSound()
    {
        if (audioSource != null && eyeClickSfx != null)
        {
            audioSource.PlayOneShot(eyeClickSfx);
        }
    }
    
    /// <summary>
    /// 播放眼睛动画
    /// </summary>
    private IEnumerator PlayEyeAnimation(SpriteRenderer eyeRenderer, bool isLeftEye)
    {
        if (eyeRenderer == null) yield break;
        
        // 简单的闪烁效果
        Color originalColor = eyeRenderer.color;
        Color targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0.3f);
        
        // 淡出
        float elapsed = 0f;
        while (elapsed < eyeAnimationDuration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0.3f, elapsed / (eyeAnimationDuration * 0.5f));
            eyeRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
        
        // 淡入
        elapsed = 0f;
        while (elapsed < eyeAnimationDuration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0.3f, 1f, elapsed / (eyeAnimationDuration * 0.5f));
            eyeRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
        
        // 恢复原始颜色
        eyeRenderer.color = originalColor;
    }
    
    /// <summary>
    /// 重置状态（用于测试或重新开始）
    /// </summary>
    [ContextMenu("重置眼睛状态")]
    public void ResetEyeState()
    {
        _leftEyeClicked = false;
        _rightEyeClicked = false;
        _isProcessing = false;
        
        // 禁用迷雾
        if (leftMist != null) leftMist.SetActive(false);
        if (rightMist != null) rightMist.SetActive(false);
        
        Debug.Log("[PokeEye] 眼睛状态已重置");
    }
    
    /// <summary>
    /// 获取当前状态信息
    /// </summary>
    public string GetStatusInfo()
    {
        return $"左眼: {(_leftEyeClicked ? "已点击" : "未点击")}, 右眼: {(_rightEyeClicked ? "已点击" : "未点击")}";
    }
    
#if UNITY_EDITOR
    [ContextMenu("测试左眼点击")]
    private void TestLeftEyeClick()
    {
        if (Application.isPlaying && !_leftEyeClicked)
        {
            ClickLeftEye();
        }
    }
    
    [ContextMenu("测试右眼点击")]
    private void TestRightEyeClick()
    {
        if (Application.isPlaying && !_rightEyeClicked)
        {
            ClickRightEye();
        }
    }
#endif
}
