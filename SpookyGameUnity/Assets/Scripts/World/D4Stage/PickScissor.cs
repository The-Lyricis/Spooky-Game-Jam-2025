using UnityEngine;
using SpookyGame.Core;

namespace SpookyGame.World
{
    /// <summary>
    /// 剃刀 - 点击拾取
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class PickScissor : Interactable
    {
        [Tooltip("拾取音效")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip pickupSfx;
        
        
        /// <summary>
        /// 检查是否可以悬停
        /// </summary>
        public override bool CanHover(GameObject actor)
        {
            if (!base.CanHover(actor)) return false;
            
            // 如果已经持有剃刀，不显示提示
            string held = FlagService.GetHeldItem();
            if (held == "Scissor")
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
            
            // 允许交互，即使手上有其他物品（会直接替换）
            return true;
        }
        
        /// <summary>
        /// 拾取剃刀
        /// </summary>
        protected override void OnInteract(GameObject actor)
        {
            // 检查是否已经持有其他物品
            string currentHeld = FlagService.GetHeldItem();
            
            // 设置手持物品（直接替换）
            FlagService.SetHeldItem("Scissor");
            
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

