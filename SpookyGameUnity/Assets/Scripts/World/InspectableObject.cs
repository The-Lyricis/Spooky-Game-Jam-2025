using UnityEngine;
using SpookyGame.UI;

namespace SpookyGame.World
{
    /// <summary>
    /// 可查看特写的物体
    /// 继承自 Interactable，点击后显示物品的特写图片
    /// </summary>
    public class InspectableObject : Interactable
    {
        [Header("Closeup Settings")]
        [SerializeField] private Sprite closeupSprite;
        [SerializeField] private bool useObjectSprite = false;
        
        [Header("UI Reference")]
        [SerializeField] private CloseupViewUI closeupUI;
        [SerializeField] private bool autoFindUI = true;
        
        [Header("Debug")]
        [SerializeField] private bool useDebugLog = true;
        
        private SpriteRenderer _spriteRenderer;
        
        protected override void Awake()
        {
            base.Awake();
            
            // 获取 SpriteRenderer（如果需要使用物体自身的图片）
            if (useObjectSprite)
            {
                _spriteRenderer = GetComponent<SpriteRenderer>();
                if (_spriteRenderer == null)
                {
                    Debug.LogWarning($"[InspectableObject] {gameObject.name}: Use Object Sprite is enabled but no SpriteRenderer found", gameObject);
                }
            }
            
            // 自动查找 UI
            if (autoFindUI && closeupUI == null)
            {
                closeupUI = FindObjectOfType<CloseupViewUI>();
                if (closeupUI == null)
                {
                    Debug.LogError($"[InspectableObject] {gameObject.name}: CloseupViewUI not found in scene!", gameObject);
                }
            }
        }
        
        protected override void OnInteract(GameObject actor)
        {
            if (closeupUI == null)
            {
                Debug.LogError($"[InspectableObject] {gameObject.name}: CloseupViewUI is not assigned!", gameObject);
                return;
            }
            
            // 获取要显示的图片
            Sprite spriteToShow = GetSpriteToShow();
            
            if (spriteToShow == null)
            {
                Debug.LogWarning($"[InspectableObject] {gameObject.name}: No sprite to show!", gameObject);
                return;
            }
            
            // 显示特写
            closeupUI.ShowCloseup(spriteToShow);
            
            if (useDebugLog)
            {
                Debug.Log($"<color=cyan>[InspectableObject] {gameObject.name}:</color> Showing closeup of {spriteToShow.name}", gameObject);
            }
        }
        
        /// <summary>
        /// 获取要显示的图片
        /// </summary>
        private Sprite GetSpriteToShow()
        {
            if (useObjectSprite && _spriteRenderer != null)
            {
                return _spriteRenderer.sprite;
            }
            
            return closeupSprite;
        }
        
        /// <summary>
        /// 设置特写图片
        /// </summary>
        public void SetCloseupSprite(Sprite sprite)
        {
            closeupSprite = sprite;
        }
        
        /// <summary>
        /// 设置 UI 引用
        /// </summary>
        public void SetCloseupUI(CloseupViewUI ui)
        {
            closeupUI = ui;
        }
        
#if UNITY_EDITOR
        /// <summary>
        /// 在 Inspector 中验证设置
        /// </summary>
        private void OnValidate()
        {
            // 如果启用了使用物体图片，检查是否有 SpriteRenderer
            if (useObjectSprite)
            {
                SpriteRenderer sr = GetComponent<SpriteRenderer>();
                if (sr == null)
                {
                    Debug.LogWarning($"[InspectableObject] {gameObject.name}: Use Object Sprite is enabled but no SpriteRenderer found", gameObject);
                }
            }
            
            // 检查是否设置了特写图片
            if (!useObjectSprite && closeupSprite == null)
            {
                Debug.LogWarning($"[InspectableObject] {gameObject.name}: Closeup Sprite is not assigned!", gameObject);
            }
        }
#endif
    }
}

