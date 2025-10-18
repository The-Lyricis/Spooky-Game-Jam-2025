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
        [SerializeField] private Text clickPromptText;
        
        [Header("Typewriter Settings")]
        [SerializeField] private float typingSpeed = 0.05f;
        [SerializeField] private bool skipOnClick = true;
        
        [Header("Animation Settings")]
        [SerializeField] private float fadeInDuration = 0.5f;
        [SerializeField] private float fadeOutDuration = 0.5f;
        
        [Header("Prompt Settings")]
        [SerializeField] private string clickPromptMessage = "点击继续...";
        [SerializeField] private float promptBlinkSpeed = 1f;
        
        private CanvasGroup _canvasGroup;
        private bool _isVisible = false;
        private bool _isTyping = false;
        private bool _isWaitingForClick = false;
        private Coroutine _typewriterCoroutine;
        private Coroutine _blinkCoroutine;
        private System.Action _onComplete;
        
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
            
            // 隐藏点击提示
            if (clickPromptText != null)
            {
                clickPromptText.gameObject.SetActive(false);
            }
        }
        
        private void Start()
        {
            // 订阅过渡文字事件
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
            
            // 如果正在打字且允许跳过
            if (_isTyping && skipOnClick && UnityEngine.Input.GetMouseButtonDown(0))
            {
                SkipTypewriter();
            }
            // 如果等待点击
            else if (_isWaitingForClick && UnityEngine.Input.GetMouseButtonDown(0))
            {
                OnClickToContinue();
            }
        }
        
        /// <summary>
        /// 处理过渡文字事件
        /// </summary>
        public void Handle(TransitionTextEvent eventData)
        {
            if (eventData.Show)
            {
                ShowTransitionText(eventData.Text, eventData.OnComplete);
            }
            else
            {
                HideTransitionText();
            }
        }
        
        /// <summary>
        /// 显示过渡文字
        /// </summary>
        public void ShowTransitionText(string text, System.Action onComplete = null)
        {
            if (string.IsNullOrEmpty(text))
            {
                Debug.LogWarning("[TransitionTextUI] Text is null or empty");
                onComplete?.Invoke();
                return;
            }
            
            _onComplete = onComplete;
            _isVisible = true;
            _isWaitingForClick = false;
            
            // 显示面板
            transitionPanel.SetActive(true);
            
            // 隐藏点击提示
            if (clickPromptText != null)
            {
                clickPromptText.gameObject.SetActive(false);
            }
            
            // 清空文字
            if (transitionText != null)
            {
                transitionText.text = "";
            }
            
            // 播放淡入动画
            StartCoroutine(ShowSequence(text));
            
            Debug.Log($"[TransitionTextUI] Showing transition text: {text}");
        }
        
        /// <summary>
        /// 隐藏过渡文字
        /// </summary>
        public void HideTransitionText()
        {
            if (!_isVisible) return;
            
            _isVisible = false;
            _isWaitingForClick = false;
            
            // 停止所有协程
            if (_typewriterCoroutine != null)
            {
                StopCoroutine(_typewriterCoroutine);
                _typewriterCoroutine = null;
            }
            
            if (_blinkCoroutine != null)
            {
                StopCoroutine(_blinkCoroutine);
                _blinkCoroutine = null;
            }
            
            // 播放淡出动画
            StartCoroutine(FadeOut());
            
            Debug.Log("[TransitionTextUI] Hiding transition text");
        }
        
        /// <summary>
        /// 显示序列：淡入 -> 打字 -> 等待点击
        /// </summary>
        private IEnumerator ShowSequence(string text)
        {
            // 淡入
            yield return StartCoroutine(FadeIn());
            
            // 打字机效果
            _typewriterCoroutine = StartCoroutine(TypewriterEffect(text));
            yield return _typewriterCoroutine;
            
            // 显示点击提示
            ShowClickPrompt();
            
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
        /// 显示点击提示
        /// </summary>
        private void ShowClickPrompt()
        {
            if (clickPromptText != null)
            {
                clickPromptText.text = clickPromptMessage;
                clickPromptText.gameObject.SetActive(true);
                
                // 开始闪烁效果
                _blinkCoroutine = StartCoroutine(BlinkPrompt());
            }
        }
        
        /// <summary>
        /// 点击提示闪烁效果
        /// </summary>
        private IEnumerator BlinkPrompt()
        {
            if (clickPromptText == null) yield break;
            
            CanvasGroup promptGroup = clickPromptText.GetComponent<CanvasGroup>();
            if (promptGroup == null)
            {
                promptGroup = clickPromptText.gameObject.AddComponent<CanvasGroup>();
            }
            
            while (_isWaitingForClick)
            {
                // 淡出
                float elapsed = 0f;
                while (elapsed < promptBlinkSpeed / 2f)
                {
                    elapsed += Time.deltaTime;
                    promptGroup.alpha = Mathf.Lerp(1f, 0.3f, elapsed / (promptBlinkSpeed / 2f));
                    yield return null;
                }
                
                // 淡入
                elapsed = 0f;
                while (elapsed < promptBlinkSpeed / 2f)
                {
                    elapsed += Time.deltaTime;
                    promptGroup.alpha = Mathf.Lerp(0.3f, 1f, elapsed / (promptBlinkSpeed / 2f));
                    yield return null;
                }
            }
        }
        
        /// <summary>
        /// 玩家点击继续
        /// </summary>
        private void OnClickToContinue()
        {
            _isWaitingForClick = false;
            
            // 停止闪烁
            if (_blinkCoroutine != null)
            {
                StopCoroutine(_blinkCoroutine);
                _blinkCoroutine = null;
            }
            
            Debug.Log("[TransitionTextUI] Player clicked to continue");
            
            // 淡出并调用完成回调
            StartCoroutine(FadeOutAndComplete());
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
        /// 淡出并执行完成回调
        /// </summary>
        private IEnumerator FadeOutAndComplete()
        {
            yield return StartCoroutine(FadeOut());
            
            _isVisible = false;
            
            // 执行完成回调
            _onComplete?.Invoke();
            _onComplete = null;
        }
        
        /// <summary>
        /// 获取当前是否显示
        /// </summary>
        public bool IsVisible => _isVisible;
    }
}

