using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SpookyGame.UI
{
    /// <summary>
    /// 物品特写查看 UI 控制器
    /// 支持缩放、拖动、点击外部关闭等功能
    /// </summary>
    public class CloseupViewUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject closeupPanel;
        [SerializeField] private Image closeupImage;
        [SerializeField] private RectTransform imageTransform;
        
        [Header("Zoom Settings")]
        [SerializeField] private float minZoom = 0.5f;
        [SerializeField] private float maxZoom = 3f;
        [SerializeField] private float zoomSpeed = 0.1f;
        
        [Header("Drag Settings")]
        [SerializeField] private bool enableDrag = true;
        [SerializeField] private float dragSpeed = 1f;
        
        [Header("Animation Settings")]
        [SerializeField] private float fadeInDuration = 0.2f;
        [SerializeField] private float fadeOutDuration = 0.2f;
        
        [Header("Click Outside Settings")]
        [SerializeField] private float clickOutsideDelay = 0.3f; // 延迟检测点击外部关闭的时间
        
        private CanvasGroup _canvasGroup;
        private bool _isVisible = false;
        private bool _isDragging = false;
        private Vector3 _dragStartPos;
        private Vector3 _imageStartPos;
        private float _currentZoom = 1f;
        private Sprite _originalSprite;
        private float _showTime = 0f; // 记录显示的时间
        
        private void Awake()
        {
            // 获取或添加 CanvasGroup
            _canvasGroup = closeupPanel.GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
            {
                _canvasGroup = closeupPanel.AddComponent<CanvasGroup>();
            }
            
            // 初始化状态
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            closeupPanel.SetActive(false);
            
            // 保存 ImageTransform 引用
            if (imageTransform == null && closeupImage != null)
            {
                imageTransform = closeupImage.GetComponent<RectTransform>();
            }
        }
        
        private void Update()
        {
            if (!_isVisible) return;
            
            // 处理鼠标滚轮缩放
            HandleZoom();
            
            // 处理鼠标拖动
            HandleDrag();
            
            // 处理点击外部关闭
            HandleClickOutside();
        }
        
        /// <summary>
        /// 显示特写
        /// </summary>
        /// <param name="sprite">要显示的图片</param>
        public void ShowCloseup(Sprite sprite)
        {
            if (sprite == null)
            {
                Debug.LogWarning("[CloseupViewUI] Sprite is null");
                return;
            }
            
            _originalSprite = sprite;
            closeupImage.sprite = sprite;
            
            // 重置变换
            ResetTransform();
            
            // 显示面板
            closeupPanel.SetActive(true);
            _isVisible = true;
            _showTime = Time.time; // 记录显示时间
            
            // 播放淡入动画
            StartCoroutine(FadeIn());
            
            Debug.Log($"[CloseupViewUI] Showing closeup: {sprite.name}");
        }
        
        /// <summary>
        /// 隐藏特写
        /// </summary>
        public void HideCloseup()
        {
            if (!_isVisible) return;
            
            _isVisible = false;
            
            // 播放淡出动画
            StartCoroutine(FadeOut());
            
            Debug.Log("[CloseupViewUI] Hiding closeup");
        }
        
        /// <summary>
        /// 处理缩放
        /// </summary>
        private void HandleZoom()
        {
            float scroll = UnityEngine.Input.mouseScrollDelta.y;
            if (Mathf.Abs(scroll) > 0.01f)
            {
                _currentZoom += scroll * zoomSpeed;
                _currentZoom = Mathf.Clamp(_currentZoom, minZoom, maxZoom);
                
                imageTransform.localScale = Vector3.one * _currentZoom;
                
                Debug.Log($"[CloseupViewUI] Zoom: {_currentZoom:F2}");
            }
        }
        
        /// <summary>
        /// 处理拖动
        /// </summary>
        private void HandleDrag()
        {
            if (!enableDrag) return;
            
            // 开始拖动
            if (UnityEngine.Input.GetMouseButtonDown(0) && IsPointerOverImage())
            {
                _isDragging = true;
                _dragStartPos = UnityEngine.Input.mousePosition;
                _imageStartPos = imageTransform.anchoredPosition;
            }
            
            // 拖动中
            if (_isDragging && UnityEngine.Input.GetMouseButton(0))
            {
                Vector3 delta = (UnityEngine.Input.mousePosition - _dragStartPos) * dragSpeed;
                imageTransform.anchoredPosition = _imageStartPos + new Vector3(delta.x, delta.y, 0);
            }
            
            // 结束拖动
            if (UnityEngine.Input.GetMouseButtonUp(0))
            {
                _isDragging = false;
            }
        }
        
        /// <summary>
        /// 处理点击外部关闭
        /// </summary>
        private void HandleClickOutside()
        {
            // 检查是否已经过了延迟时间
            if (Time.time - _showTime < clickOutsideDelay)
            {
                return; // 还在延迟时间内，不处理点击外部关闭
            }
            
            if (UnityEngine.Input.GetMouseButtonDown(0) && !_isDragging)
            {
                // 如果点击的不是图片，则关闭
                if (!IsPointerOverImage())
                {
                    HideCloseup();
                }
            }
        }
        
        /// <summary>
        /// 检查鼠标是否在图片上
        /// </summary>
        private bool IsPointerOverImage()
        {
            if (closeupImage == null) return false;
            
            return RectTransformUtility.RectangleContainsScreenPoint(
                imageTransform,
                UnityEngine.Input.mousePosition,
                null // 使用 Screen Space - Overlay Canvas
            );
        }
        
        /// <summary>
        /// 重置变换
        /// </summary>
        private void ResetTransform()
        {
            _currentZoom = 1f;
            imageTransform.localScale = Vector3.one;
            imageTransform.anchoredPosition = Vector2.zero;
            _isDragging = false;
        }
        
        /// <summary>
        /// 淡入动画
        /// </summary>
        private System.Collections.IEnumerator FadeIn()
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
        private System.Collections.IEnumerator FadeOut()
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
            
            closeupPanel.SetActive(false);
        }
        
        /// <summary>
        /// 获取当前是否显示
        /// </summary>
        public bool IsVisible => _isVisible;
        
        /// <summary>
        /// 设置缩放范围
        /// </summary>
        public void SetZoomRange(float min, float max)
        {
            minZoom = min;
            maxZoom = max;
            _currentZoom = Mathf.Clamp(_currentZoom, minZoom, maxZoom);
        }
        
        #region 编辑器辅助
        
#if UNITY_EDITOR
        [ContextMenu("测试显示特写")]
        private void TestShowCloseup()
        {
            if (closeupImage.sprite != null)
            {
                ShowCloseup(closeupImage.sprite);
            }
            else
            {
                Debug.LogWarning("请先设置一个测试用的 Sprite");
            }
        }
        
        [ContextMenu("隐藏特写")]
        private void TestHideCloseup()
        {
            HideCloseup();
        }
#endif
        
        #endregion
    }
}

