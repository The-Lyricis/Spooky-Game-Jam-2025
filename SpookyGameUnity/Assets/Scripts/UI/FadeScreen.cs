using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SpookyGame.Core;
using System.Collections;

namespace SpookyGame.UI
{
    /// <summary>
    /// 黑幕转场 UI
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class FadeScreen : MonoBehaviour, IEventHandler<FadeStartedEvent>, IEventHandler<IntertitleEvent>
    {
        [Header("UI References")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image fadeImage;
        [SerializeField] private TextMeshProUGUI intertitleText;
        
        [Header("Settings")]
        [SerializeField] private Color fadeColor = Color.black;
        
        private Coroutine _fadeCoroutine;
        
        private void Awake()
        {
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }
            
            // 初始化为完全透明
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            
            if (fadeImage != null)
            {
                fadeImage.color = fadeColor;
            }
            
            if (intertitleText != null)
            {
                intertitleText.gameObject.SetActive(false);
            }
        }
        
        private void Start()
        {
            // 订阅事件
            EventBus.Subscribe<FadeStartedEvent>(this);
            EventBus.Subscribe<IntertitleEvent>(this);
        }
        
        private void OnDestroy()
        {
            // 取消订阅
            EventBus.Unsubscribe<FadeStartedEvent>(this);
            EventBus.Unsubscribe<IntertitleEvent>(this);
        }
        
        public void Handle(FadeStartedEvent eventData)
        {
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }
            
            _fadeCoroutine = StartCoroutine(FadeCoroutine(eventData.FadeIn, eventData.Duration));
        }
        
        public void Handle(IntertitleEvent eventData)
        {
            if (intertitleText != null)
            {
                if (eventData.Show)
                {
                    intertitleText.text = eventData.Text;
                    intertitleText.gameObject.SetActive(true);
                }
                else
                {
                    intertitleText.gameObject.SetActive(false);
                }
            }
        }
        
        private IEnumerator FadeCoroutine(bool fadeIn, float duration)
        {
            float startAlpha = canvasGroup.alpha;
            float targetAlpha = fadeIn ? 1f : 0f;
            
            // 淡入时阻挡射线
            if (fadeIn)
            {
                canvasGroup.blocksRaycasts = true;
            }
            
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
                yield return null;
            }
            
            canvasGroup.alpha = targetAlpha;
            
            // 淡出后不阻挡射线
            if (!fadeIn)
            {
                canvasGroup.blocksRaycasts = false;
            }
            
            _fadeCoroutine = null;
        }
        
        /// <summary>
        /// 立即设置为黑屏（用于开场）
        /// </summary>
        [ContextMenu("Fade In Immediate")]
        public void FadeInImmediate()
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }
        
        /// <summary>
        /// 立即设置为透明（用于调试）
        /// </summary>
        [ContextMenu("Fade Out Immediate")]
        public void FadeOutImmediate()
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
        }
    }
}

