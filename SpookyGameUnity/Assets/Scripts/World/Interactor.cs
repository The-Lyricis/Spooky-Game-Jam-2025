using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using SpookyGame.World;
using SpookyGame.Core;
using System.Collections.Generic;
using System.Linq;

namespace SpookyGame.World
{
    /// <summary>
    /// 点击式交互管理器
    /// 负责检测鼠标悬停和点击，触发可交互对象的交互
    /// 包含 UI 穿透防护、多层级排序、动态提示更新
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
        private string _currentPromptText = "";
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
            
            // 【修复 1】检查是否点击在 UI 上，避免穿透
            if (IsPointerOverUI())
            {
                // 鼠标在 UI 上，清除世界交互
                if (_hoveredInteractable != null)
                {
                    _hoveredInteractable = null;
                    UpdatePrompt();
                    UpdateCursor();
                }
                return;
            }
            
            // 获取鼠标世界坐标
            Vector3 mouseWorldPos = _mainCamera.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
            mouseWorldPos.z = 0f;
            
            // 【修复 2】使用 OverlapPointAll 获取所有碰撞体，支持多层级选择
            Collider2D[] hitColliders = Physics2D.OverlapPointAll(mouseWorldPos, interactableLayerMask);
            
            Interactable newInteractable = null;
            if (hitColliders.Length > 0)
            {
                // 【修复 3】根据渲染层级排序，选择最上层的可交互对象
                newInteractable = GetTopMostInteractable(hitColliders);
            }
            
            // 更新悬停对象
            if (_hoveredInteractable != newInteractable)
            {
                // 退出旧的悬停对象
                if (_hoveredInteractable != null)
                {
                    _hoveredInteractable.OnHoverExit();
                }
                
                // 进入新的悬停对象
                if (newInteractable != null)
                {
                    newInteractable.OnHoverEnter();
                }
                
                _hoveredInteractable = newInteractable;
                UpdatePrompt();
                UpdateCursor();
            }
            // 【修复 4】即使悬停对象没变，也检查提示文本是否变化（支持动态提示）
            else if (_hoveredInteractable != null)
            {
                string newPrompt = _hoveredInteractable.CurrentPrompt;
                if (newPrompt != _currentPromptText)
                {
                    UpdatePrompt();
                }
            }
        }
        
        /// <summary>
        /// 检查鼠标是否在 UI 上（防止 UI 穿透）
        /// </summary>
        private bool IsPointerOverUI()
        {
            // 检查是否有 EventSystem
            if (EventSystem.current == null)
            {
                return false;
            }
            
            // 使用 EventSystem 检测是否在 UI 上
            return EventSystem.current.IsPointerOverGameObject();
        }
        
        /// <summary>
        /// 从多个碰撞体中选择最上层的可交互对象
        /// 优先级：SortingGroup > SpriteRenderer > Z轴
        /// </summary>
        private Interactable GetTopMostInteractable(Collider2D[] colliders)
        {
            List<Interactable> interactables = new List<Interactable>();
            
            foreach (var collider in colliders)
            {
                Interactable interactable = collider.GetComponent<Interactable>();
                if (interactable != null && interactable.CanHover(gameObject))
                {
                    interactables.Add(interactable);
                }
            }
            
            if (interactables.Count == 0)
            {
                return null;
            }
            
            if (interactables.Count == 1)
            {
                return interactables[0];
            }
            
            // 使用改进的排序算法
            return interactables
                .OrderByDescending(i => GetSortingWeight(i))
                .FirstOrDefault();
        }
        
        /// <summary>
        /// 获取渲染排序权重
        /// 优先级：SortingGroup > SpriteRenderer > Z轴
        /// </summary>
        private long GetSortingWeight(Interactable interactable)
        {
            // 1. 优先检查 SortingGroup
            SortingGroup sortingGroup = interactable.GetComponentInParent<SortingGroup>();
            if (sortingGroup != null)
            {
                int layerValue = SortingLayer.GetLayerValueFromID(sortingGroup.sortingLayerID);
                return ((long)layerValue * 100000) + sortingGroup.sortingOrder;
            }
            
            // 2. 检查 SpriteRenderer
            SpriteRenderer spriteRenderer = interactable.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                int layerValue = SortingLayer.GetLayerValueFromID(spriteRenderer.sortingLayerID);
                return ((long)layerValue * 100000) + spriteRenderer.sortingOrder;
            }
            
            // 3. 最后使用 Z 轴（负值，因为 Z 轴越小越靠前）
            return (long)(-interactable.transform.position.z * 1000);
        }
        
        /// <summary>
        /// 检测鼠标点击
        /// </summary>
        private void DetectClick()
        {
            // 检测鼠标左键点击
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                // 【修复】点击时也要检查 UI 穿透
                if (IsPointerOverUI())
                {
                    if (showDebugInfo)
                    {
                        Debug.Log("[Interactor] Click blocked by UI");
                    }
                    return;
                }
                
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
        /// 更新交互提示（支持动态文本）
        /// </summary>
        private void UpdatePrompt()
        {
            // if (_hoveredInteractable != null)
            // {
            //     // 使用 CurrentPrompt 支持动态提示文本
            //     string newPrompt = _hoveredInteractable.CurrentPrompt;
                
            //     // 只有文本变化或首次显示时才发布事件
            //     if (newPrompt != _currentPromptText)
            //     {
            //         _currentPromptText = newPrompt;
            //         EventBus.Publish(new PromptEvent(newPrompt, true));
                    
            //         if (showDebugInfo)
            //         {
            //             Debug.Log($"[Interactor] Prompt updated: {newPrompt}");
            //         }
            //     }
            // }
            // else
            // {
            //     // 清空提示
            //     if (!string.IsNullOrEmpty(_currentPromptText))
            //     {
            //         _currentPromptText = "";
            //         EventBus.Publish(new PromptEvent("", false));
                    
            //         if (showDebugInfo)
            //         {
            //             Debug.Log("[Interactor] Prompt hidden");
            //         }
            //     }
            // }
        }
        
        /// <summary>
        /// 强制刷新提示（当 Flag 或手持物变化时调用）
        /// </summary>
        public void RefreshPrompt()
        {
            if (_hoveredInteractable != null)
            {
                UpdatePrompt();
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
