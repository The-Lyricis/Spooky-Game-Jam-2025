using UnityEngine;
using SpookyGame.Core;

namespace SpookyGame.World
{
    /// <summary>
    /// 剃刀 - 点击拾取，鼠标指针变化
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class Razor : Interactable
    {
        [Header("Razor Settings")]
        [Tooltip("剃刀的鼠标指针图标")]
        [SerializeField] private Texture2D razorCursor;
        
        [Tooltip("指针热点偏移")]
        [SerializeField] private Vector2 cursorHotspot = new Vector2(16, 16);
        
        [Tooltip("拾取音效")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip pickupSfx;
        
        public override string CurrentPrompt => "拾取剃刀";
        
        // 公开属性，供 Interactor 读取
        public Texture2D RazorCursor => razorCursor;
        public Vector2 CursorHotspot => cursorHotspot;
        
        /// <summary>
        /// 检查是否可以悬停
        /// </summary>
        public override bool CanHover(GameObject actor)
        {
            if (!base.CanHover(actor)) return false;
            
            // 如果已经持有剃刀，不显示提示
            string held = FlagService.GetHeldItem();
            if (held == "razor")
            {
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// 检查是否可以交互
        /// </summary>
        public override bool CanInteract(GameObject actor)
        {
            if (!base.CanInteract(actor)) return false;
            
            // 确保没有持有其他物品
            string held = FlagService.GetHeldItem();
            if (!string.IsNullOrEmpty(held))
            {
                Debug.Log("[Razor] 已经持有物品，无法拾取剃刀");
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// 拾取剃刀
        /// </summary>
        protected override void OnInteract(GameObject actor)
        {
            Debug.Log("[Razor] 拾取剃刀");
            
            // 设置手持物品
            FlagService.SetHeldItem("razor");
            
            // 更新鼠标指针
            if (razorCursor != null)
            {
                Cursor.SetCursor(razorCursor, cursorHotspot, CursorMode.Auto);
                Debug.Log("[Razor] 鼠标指针已更新为剃刀图标");
            }
            
            // 播放音效
            if (audioSource != null && pickupSfx != null)
            {
                audioSource.PlayOneShot(pickupSfx);
            }
            
            // 隐藏剃刀
            gameObject.SetActive(false);
        }
    }
}

