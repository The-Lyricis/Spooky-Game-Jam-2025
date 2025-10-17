using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SpookyGame.Core;

namespace SpookyGame.UI
{
    /// <summary>
    /// 交互提示 UI，显示"按 E 交互"等文案
    /// </summary>
    public class PromptUI : MonoBehaviour, IEventHandler<PromptEvent>
    {
        [Header("UI References")]
        [SerializeField] private GameObject promptPanel;
        [SerializeField] private TextMeshProUGUI promptText;
        [SerializeField] private Image promptIcon;
        
        [Header("Animation Settings")]
        [SerializeField] private float fadeInDuration = 0.2f;
        [SerializeField] private float fadeOutDuration = 0.2f;
        
        private CanvasGroup _canvasGroup;
        private bool _isVisible = false;
        
        private void Awake()
        {
            // 获取或添加 CanvasGroup 组件
            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
            {
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            
            // 初始化状态
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            
            if (promptPanel != null)
            {
                promptPanel.SetActive(false);
            }
        }
        
        private void Start()
        {
            // 订阅提示事件
            EventBus.Subscribe<PromptEvent>(this);
        }
        
        private void OnDestroy()
        {
            // 取消订阅提示事件
            EventBus.Unsubscribe<PromptEvent>(this);
        }
        
        /// <summary>
        /// 处理提示事件
        /// </summary>
        /// <param name="eventData">提示事件数据</param>
        public void Handle(PromptEvent eventData)
        {
            if (eventData.IsVisible && !string.IsNullOrEmpty(eventData.PromptText))
            {
                ShowPrompt(eventData.PromptText);
            }
            else
            {
                HidePrompt();
            }
        }
        
        /// <summary>
        /// 显示提示
        /// </summary>
        /// <param name="text">提示文本</param>
        private void ShowPrompt(string text)
        {
            if (_isVisible) return;
            
            _isVisible = true;
            
            // 设置提示文本
            if (promptText != null)
            {
                promptText.text = text;
            }
            
            // 显示面板
            if (promptPanel != null)
            {
                promptPanel.SetActive(true);
            }
            
            // 播放淡入动画
            StartCoroutine(FadeIn());
            
            Debug.Log($"[PromptUI] Showing prompt: {text}");
        }
        
        /// <summary>
        /// 隐藏提示
        /// </summary>
        private void HidePrompt()
        {
            if (!_isVisible) return;
            
            _isVisible = false;
            
            // 播放淡出动画
            StartCoroutine(FadeOut());
            
            Debug.Log("[PromptUI] Hiding prompt");
        }
        
        /// <summary>
        /// 淡入动画协程
        /// </summary>
        private System.Collections.IEnumerator FadeIn()
        {
            float elapsedTime = 0f;
            
            while (elapsedTime < fadeInDuration)
            {
                elapsedTime += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeInDuration);
                yield return null;
            }
            
            _canvasGroup.alpha = 1f;
        }
        
        /// <summary>
        /// 淡出动画协程
        /// </summary>
        private System.Collections.IEnumerator FadeOut()
        {
            float elapsedTime = 0f;
            float startAlpha = _canvasGroup.alpha;
            
            while (elapsedTime < fadeOutDuration)
            {
                elapsedTime += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeOutDuration);
                yield return null;
            }
            
            _canvasGroup.alpha = 0f;
            
            // 隐藏面板
            if (promptPanel != null)
            {
                promptPanel.SetActive(false);
            }
        }
        
        /// <summary>
        /// 设置提示文本
        /// </summary>
        /// <param name="text">新的提示文本</param>
        public void SetPromptText(string text)
        {
            if (promptText != null)
            {
                promptText.text = text;
            }
        }
        
        /// <summary>
        /// 设置提示图标
        /// </summary>
        /// <param name="sprite">新的图标精灵</param>
        public void SetPromptIcon(Sprite sprite)
        {
            if (promptIcon != null)
            {
                promptIcon.sprite = sprite;
                promptIcon.gameObject.SetActive(sprite != null);
            }
        }
    }
}
