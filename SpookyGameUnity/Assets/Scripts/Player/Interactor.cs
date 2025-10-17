using UnityEngine;
using SpookyGame.Input;
using SpookyGame.World;
using SpookyGame.Core;

namespace SpookyGame.Player
{
    /// <summary>
    /// 交互器，负责处理鼠标点击交互
    /// </summary>
    public class Interactor : MonoBehaviour
    {
        [Header("Interaction Settings")]
        [SerializeField] private LayerMask interactableLayerMask = -1;
        
        private Interactable _currentInteractable;
        private bool _isInteractionPromptVisible = false;
        
        /// <summary>
        /// 当前可交互对象
        /// </summary>
        public Interactable CurrentInteractable => _currentInteractable;
        
        /// <summary>
        /// 是否显示交互提示
        /// </summary>
        public bool IsInteractionPromptVisible => _isInteractionPromptVisible;
        
        private void Start()
        {
            // 订阅输入事件
            InputService.OnInteractPressed += HandleInteractPressed;
        }
        
        private void OnDestroy()
        {
            // 取消订阅输入事件
            InputService.OnInteractPressed -= HandleInteractPressed;
        }
        
        private void Update()
        {
            DetectInteractables();
        }
        
        /// <summary>
        /// 检测鼠标悬停的可交互对象
        /// </summary>
        private void DetectInteractables()
        {
            // 获取鼠标世界坐标
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
            mouseWorldPos.z = 0f;
            
            // 使用点检测查找鼠标位置的可交互对象
            Collider2D hitCollider = Physics2D.OverlapPoint(mouseWorldPos, interactableLayerMask);
            
            Interactable newInteractable = null;
            if (hitCollider != null)
            {
                newInteractable = hitCollider.GetComponent<Interactable>();
                if (newInteractable != null && !newInteractable.CanInteract(gameObject))
                {
                    newInteractable = null;
                }
            }
            
            // 更新当前可交互对象
            if (_currentInteractable != newInteractable)
            {
                _currentInteractable = newInteractable;
                UpdateInteractionPrompt();
            }
        }
        
        /// <summary>
        /// 更新交互提示显示
        /// </summary>
        private void UpdateInteractionPrompt()
        {
            if (_currentInteractable != null)
            {
                if (!_isInteractionPromptVisible)
                {
                    ShowInteractionPrompt();
                }
            }
            else
            {
                if (_isInteractionPromptVisible)
                {
                    HideInteractionPrompt();
                }
            }
        }
        
        /// <summary>
        /// 显示交互提示
        /// </summary>
        private void ShowInteractionPrompt()
        {
            if (_currentInteractable != null)
            {
                _isInteractionPromptVisible = true;
                EventBus.Publish(new PromptEvent(_currentInteractable.InteractionPrompt, true));
                Debug.Log($"[Interactor] Showing prompt: {_currentInteractable.InteractionPrompt}");
            }
        }
        
        /// <summary>
        /// 隐藏交互提示
        /// </summary>
        private void HideInteractionPrompt()
        {
            _isInteractionPromptVisible = false;
            EventBus.Publish(new PromptEvent("", false));
            Debug.Log("[Interactor] Hiding prompt");
        }
        
        /// <summary>
        /// 处理交互输入
        /// </summary>
        private void HandleInteractPressed()
        {
            if (_currentInteractable != null)
            {
                _currentInteractable.Interact(gameObject);
            }
        }
    }
}
