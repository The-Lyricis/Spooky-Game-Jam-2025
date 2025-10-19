using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SpookyGame.Core;

namespace SpookyGame.UI
{
    /// <summary>
    /// 场景切换过渡文字 UI
    /// 显示打字机效果的过渡文字，等待玩家点击后继续
    /// </summary>
    public class TransitionTextUI : MonoBehaviour, IEventHandler<TransitionTextEvent>
    {
        [Header("UI References")]
        [SerializeField] private GameObject transitionPanel;
        [SerializeField] private Text transitionText;
        
        [Header("Typewriter Settings")]
        [SerializeField] private float typingSpeed = 0.05f;
        [SerializeField] private bool skipOnClick = true;
        
        [Header("Animation Settings")]
        [SerializeField] private float fadeInDuration = 0.5f;
        [SerializeField] private float fadeOutDuration = 0.5f;
        
        
        private CanvasGroup _canvasGroup;
        private bool _isVisible = false;
        private bool _isTyping = false;
        private bool _isWaitingForClick = false;
        private Coroutine _typewriterCoroutine;
        private System.Action _onComplete;
        
        // 多文字支持
        private string[] _texts;
        private int _currentTextIndex = 0;
        
        // 防重复点击保护
        private bool _isProcessingClick = false;
        private float _lastClickTime = 0f;
        private const float CLICK_COOLDOWN = 0.3f; // 点击冷却时间
        
        private void Awake()
        {
            // 获取或添加 CanvasGroup
            _canvasGroup = transitionPanel.GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
            {
                _canvasGroup = transitionPanel.AddComponent<CanvasGroup>();
            }
            
            // 初始化状态
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            transitionPanel.SetActive(false);
        }
        
        private void Start()
        {
            // 订阅过渡文字事件
            Debug.Log("[TransitionTextUI] Subscribing to TransitionTextEvent");
            EventBus.Subscribe<TransitionTextEvent>(this);
        }
        
        private void OnDestroy()
        {
            // 取消订阅
            EventBus.Unsubscribe<TransitionTextEvent>(this);
        }
        
        private void Update()
        {
            if (!_isVisible) return;
            
            // 防止重复点击 - 添加点击冷却和状态保护
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                float currentTime = Time.unscaledTime;
                
                // 检查点击冷却时间
                if (currentTime - _lastClickTime < CLICK_COOLDOWN)
                {
                    Debug.Log("[TransitionTextUI] Click ignored - too fast");
                    return;
                }
                
                // 检查是否正在处理点击
                if (_isProcessingClick)
                {
                    Debug.Log("[TransitionTextUI] Click ignored - already processing");
                    return;
                }
                
                _lastClickTime = currentTime;
                
                // 如果等待点击（转场文字不使用打字机效果）
                if (_isWaitingForClick)
                {
                    OnClickToContinue();
                }
            }
        }
        
        /// <summary>
        /// 处理过渡文字事件
        /// </summary>
        public void Handle(TransitionTextEvent eventData)
        {
            Debug.Log($"[TransitionTextUI] Received TransitionTextEvent: Show={eventData.Show}, Texts={eventData.Texts?.Length ?? 0}");
            
            if (eventData.Texts != null)
            {
                for (int i = 0; i < eventData.Texts.Length; i++)
                {
                    Debug.Log($"[TransitionTextUI] Text {i}: '{eventData.Texts[i]}'");
                }
            }
            
            if (eventData.Show)
            {
                ShowTransitionText(eventData.Texts, eventData.OnComplete);
            }
            else
            {
                HideTransitionText();
            }
        }
        
        /// <summary>
        /// 显示过渡文字（单个文字，兼容性方法）
        /// </summary>
        public void ShowTransitionText(string text, System.Action onComplete = null)
        {
            ShowTransitionText(new string[] { text }, onComplete);
        }
        
        /// <summary>
        /// 显示过渡文字（多个文字）
        /// </summary>
        public void ShowTransitionText(string[] texts, System.Action onComplete = null)
        {
            if (texts == null || texts.Length == 0)
            {
                Debug.LogWarning("[TransitionTextUI] Texts is null or empty");
                onComplete?.Invoke();
                return;
            }
            
            _texts = texts;
            _currentTextIndex = 0;
            _onComplete = onComplete;
            _isVisible = true;
            _isWaitingForClick = false;
            
            // 显示面板
            transitionPanel.SetActive(true);
            
            // 清空文字
            if (transitionText != null)
            {
                transitionText.text = "";
            }
            
            // 显示第一个文字
            ShowCurrentText();
            
            Debug.Log($"[TransitionTextUI] Showing transition texts: {texts.Length} texts");
        }
        
        /// <summary>
        /// 隐藏过渡文字
        /// </summary>
        public void HideTransitionText()
        {
            if (!_isVisible) return;
            
            _isVisible = false;
            _isWaitingForClick = false;
            
            // 重置多文字状态
            _texts = null;
            _currentTextIndex = 0;
            
            // 停止所有协程
            if (_typewriterCoroutine != null)
            {
                StopCoroutine(_typewriterCoroutine);
                _typewriterCoroutine = null;
            }
            
            // 播放淡出动画
            StartCoroutine(FadeOut());
            
            Debug.Log("[TransitionTextUI] Hiding transition text");
        }
        
        /// <summary>
        /// 显示序列：淡入 -> 直接显示文字 -> 等待点击
        /// </summary>
        private IEnumerator ShowSequence(string text)
        {
            // 淡入
            yield return StartCoroutine(FadeIn());
            
            // 直接设置完整文字（不使用打字机效果）
            if (transitionText != null)
            {
                transitionText.text = text;
            }
            
            // 淡入文字
            yield return StartCoroutine(FadeInText());
            
            // 等待玩家点击
            _isWaitingForClick = true;
        }
        
        /// <summary>
        /// 打字机效果
        /// </summary>
        private IEnumerator TypewriterEffect(string text)
        {
            _isTyping = true;
            
            if (transitionText == null)
            {
                Debug.LogError("[TransitionTextUI] Transition Text is not assigned!");
                _isTyping = false;
                yield break;
            }
            
            transitionText.text = "";
            
            foreach (char c in text)
            {
                transitionText.text += c;
                yield return new WaitForSeconds(typingSpeed);
            }
            
            _isTyping = false;
            _typewriterCoroutine = null;
        }
        
        /// <summary>
        /// 跳过打字机效果
        /// </summary>
        private void SkipTypewriter()
        {
            if (_typewriterCoroutine != null)
            {
                StopCoroutine(_typewriterCoroutine);
                _typewriterCoroutine = null;
            }
            
            _isTyping = false;
            
            // 立即显示完整文字
            // 文字已经在 transitionText.text 中累积，无需额外操作
            
            Debug.Log("[TransitionTextUI] Typewriter skipped");
        }
        
        /// <summary>
        /// 玩家点击继续
        /// </summary>
        private void OnClickToContinue()
        {
            if (_isProcessingClick)
            {
                Debug.LogWarning("[TransitionTextUI] Already processing click, ignoring");
                return;
            }
            
            _isProcessingClick = true;
            _isWaitingForClick = false;
            
            Debug.Log("[TransitionTextUI] Player clicked to continue");
            
            // 检查是否还有下一个文字
            if (_texts != null && _currentTextIndex < _texts.Length - 1)
            {
                // 切换到下一个文字（淡出当前文字，淡入下一个文字）
                _currentTextIndex++;
                StartCoroutine(TransitionToNextText());
            }
            else
            {
                // 所有文字显示完毕，只淡出文字，不淡出黑幕，立即调用完成回调
                StartCoroutine(FadeOutTextAndComplete());
            }
        }
        
        /// <summary>
        /// 显示当前文字
        /// </summary>
        private void ShowCurrentText()
        {
            if (_texts == null || _currentTextIndex >= _texts.Length)
            {
                Debug.LogWarning("[TransitionTextUI] No more texts to show");
                StartCoroutine(FadeOutAndComplete());
                return;
            }
            
            string currentText = _texts[_currentTextIndex];
            Debug.Log($"[TransitionTextUI] Showing text {_currentTextIndex + 1}/{_texts.Length}: {currentText}");
            
            // 如果是第一个文字，需要先淡入黑幕
            if (_currentTextIndex == 0)
            {
                StartCoroutine(ShowSequence(currentText));
            }
            else
            {
                // 后续文字直接显示（黑幕已经存在）
                StartCoroutine(ShowTextOnly(currentText));
            }
        }
        
        /// <summary>
        /// 切换到下一个文字（淡出当前文字，淡入下一个文字）
        /// </summary>
        private IEnumerator TransitionToNextText()
        {
            // 淡出当前文字
            if (transitionText != null)
            {
                yield return StartCoroutine(FadeOutText());
            }
            
            // 显示下一个文字
            ShowCurrentText();
            
            // 重置处理状态，允许下次点击
            _isProcessingClick = false;
        }
        
        /// <summary>
        /// 只显示文字（不包含黑幕淡入，不使用打字机效果）
        /// </summary>
        private IEnumerator ShowTextOnly(string text)
        {
            // 直接设置完整文字内容
            if (transitionText != null)
            {
                transitionText.text = text;
            }
            
            // 淡入文字
            yield return StartCoroutine(FadeInText());
            
            // 等待玩家点击
            _isWaitingForClick = true;
            
            // 重置处理状态，允许下次点击
            _isProcessingClick = false;
        }
        
        /// <summary>
        /// 淡出文字
        /// </summary>
        private IEnumerator FadeOutText()
        {
            if (transitionText == null) yield break;
            
            float elapsed = 0f;
            float duration = 0.3f; // 文字淡出时间
            
            Color startColor = transitionText.color;
            Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);
            
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                transitionText.color = Color.Lerp(startColor, endColor, t);
                yield return null;
            }
            
            transitionText.color = endColor;
        }
        
        /// <summary>
        /// 淡入文字
        /// </summary>
        private IEnumerator FadeInText()
        {
            if (transitionText == null) yield break;
            
            float elapsed = 0f;
            float duration = 0.3f; // 文字淡入时间
            
            Color startColor = new Color(transitionText.color.r, transitionText.color.g, transitionText.color.b, 0f);
            Color endColor = new Color(startColor.r, startColor.g, startColor.b, 1f);
            
            transitionText.color = startColor;
            
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                transitionText.color = Color.Lerp(startColor, endColor, t);
                yield return null;
            }
            
            transitionText.color = endColor;
        }
        
        /// <summary>
        /// 淡入动画
        /// </summary>
        private IEnumerator FadeIn()
        {
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            
            float elapsed = 0f;
            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
                yield return null;
            }
            
            _canvasGroup.alpha = 1f;
        }
        
        /// <summary>
        /// 淡出动画
        /// </summary>
        private IEnumerator FadeOut()
        {
            float elapsed = 0f;
            float startAlpha = _canvasGroup.alpha;
            
            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / fadeOutDuration);
                yield return null;
            }
            
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            
            transitionPanel.SetActive(false);
        }
        
        /// <summary>
        /// 只淡出文字并执行完成回调（不淡出黑幕）
        /// </summary>
        private IEnumerator FadeOutTextAndComplete()
        {
            // 只淡出文字
            if (transitionText != null)
            {
                yield return StartCoroutine(FadeOutText());
            }
            
            // 执行完成回调（让 SceneService 切换场景）
            _onComplete?.Invoke();
            _onComplete = null;
            
            // 等待一小段时间让场景切换完成，然后淡出黑幕
            yield return new WaitForSeconds(0.1f);
            
            // 淡出黑幕
            yield return StartCoroutine(FadeOut());
            
            _isVisible = false;
            
            // 重置处理状态
            _isProcessingClick = false;
            
            // 发布转场淡出完成事件
            EventBus.Publish(new TransitionFadeOutCompleteEvent(SceneService.CurrentStageId));
        }
        
        /// <summary>
        /// 淡出并执行完成回调
        /// </summary>
        private IEnumerator FadeOutAndComplete()
        {
            // 先淡出文字
            if (transitionText != null)
            {
                yield return StartCoroutine(FadeOutText());
            }
            
            // 再淡出黑幕
            yield return StartCoroutine(FadeOut());
            
            _isVisible = false;
            
            // 发布转场淡出完成事件
            EventBus.Publish(new TransitionFadeOutCompleteEvent(SceneService.CurrentStageId));
            
            // 执行完成回调
            _onComplete?.Invoke();
            _onComplete = null;
        }
        
        /// <summary>
        /// 获取当前是否显示
        /// </summary>
        public bool IsVisible => _isVisible;
        
        /// <summary>
        /// 测试方法：手动显示文字（用于调试）
        /// </summary>
        [ContextMenu("Test Show Text")]
        public void TestShowText()
        {
            ShowTransitionText(new string[] { "第一段文字", "第二段文字", "第三段文字", "最后一段文字" }, () => Debug.Log("所有文字显示完成"));
        }
    }
}

