using UnityEngine;
using SpookyGame.World;
using SpookyGame.Core;

namespace SpookyGame.Player
{
    /// <summary>
    /// 点击式交互管理器（无需玩家角色）
    /// 负责检测鼠标悬停和点击，触发可交互对象的交互
    /// 类似于《绣湖》等点击式冒险游戏
    /// </summary>
    public class Interactor : MonoBehaviour
    {
        [Header("Interaction Settings")]
        [SerializeField] private LayerMask interactableLayerMask = -1;
        [SerializeField] private bool showDebugInfo = true;
        
        [Header("Cursor Settings (Optional)")]
        [SerializeField] private Texture2D normalCursor;
        [SerializeField] private Texture2D hoverCursor;
        [SerializeField] private Vector2 cursorHotspot = Vector2.zero;
        
        private Interactable _hoveredInteractable;
        private bool _isPromptVisible = false;
        private Camera _mainCamera;
        
        /// <summary>
        /// 当前悬停的可交互对象
        /// </summary>
        public Interactable HoveredInteractable => _hoveredInteractable;
        
        private void Start()
        {
            _mainCamera = Camera.main;
            if (_mainCamera == null)
            {
                Debug.LogError("[Interactor] 找不到主摄像机！请确保摄像机有 MainCamera 标签");
            }
        }
        
        private void Update()
        {
            DetectHover();
            DetectClick();
        }
        
        /// <summary>
        /// 检测鼠标悬停的可交互对象
        /// </summary>
        private void DetectHover()
        {
            if (_mainCamera == null) return;
            
            // 获取鼠标世界坐标
            Vector3 mouseWorldPos = _mainCamera.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
            mouseWorldPos.z = 0f;
            
            // 使用点检测查找鼠标位置的可交互对象
            Collider2D hitCollider = Physics2D.OverlapPoint(mouseWorldPos, interactableLayerMask);
            
            Interactable newInteractable = null;
            if (hitCollider != null)
            {
                newInteractable = hitCollider.GetComponent<Interactable>();
                // 注意：这里传入的是 Interactor 自己，而不是"玩家"
                if (newInteractable != null && !newInteractable.CanInteract(gameObject))
                {
                    newInteractable = null;
                }
            }
            
            // 更新悬停对象
            if (_hoveredInteractable != newInteractable)
            {
                _hoveredInteractable = newInteractable;
                UpdatePrompt();
                UpdateCursor();
            }
        }
        
        /// <summary>
        /// 检测鼠标点击
        /// </summary>
        private void DetectClick()
        {
            // 检测鼠标左键点击
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                if (_hoveredInteractable != null)
                {
                    if (showDebugInfo)
                    {
                        Debug.Log($"[Interactor] Clicked on: {_hoveredInteractable.gameObject.name}");
                    }
                    _hoveredInteractable.Interact(gameObject);
                }
            }
        }
        
        /// <summary>
        /// 更新交互提示
        /// </summary>
        private void UpdatePrompt()
        {
            if (_hoveredInteractable != null)
            {
                if (!_isPromptVisible)
                {
                    _isPromptVisible = true;
                    EventBus.Publish(new PromptEvent(_hoveredInteractable.InteractionPrompt, true));
                    
                    if (showDebugInfo)
                    {
                        Debug.Log($"[Interactor] Hovering: {_hoveredInteractable.InteractionPrompt}");
                    }
                }
            }
            else
            {
                if (_isPromptVisible)
                {
                    _isPromptVisible = false;
                    EventBus.Publish(new PromptEvent("", false));
                }
            }
        }
        
        /// <summary>
        /// 更新鼠标光标样式
        /// </summary>
        private void UpdateCursor()
        {
            // 悬停在可交互对象上时显示特殊光标
            if (_hoveredInteractable != null && hoverCursor != null)
            {
                Cursor.SetCursor(hoverCursor, cursorHotspot, CursorMode.Auto);
            }
            else if (normalCursor != null)
            {
                Cursor.SetCursor(normalCursor, cursorHotspot, CursorMode.Auto);
            }
            else
            {
                // 恢复默认光标
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }
        }
        
        private void OnDisable()
        {
            // 恢复默认光标
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }
}
