using UnityEngine;
using SpookyGame.World;
using SpookyGame.Core;

namespace SpookyGame.D3Scene
{
    /// <summary>
    /// D3 场景 - 牙齿
    /// 点击后装备，如果已有剃刀则合成剪刀
    /// </summary>
    public class Tooth_D3 : Interactable
    {
        [Header("Audio")]
        [Tooltip("拾取音效")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip pickupSfx;
        
        private bool _hasBeenUsed = false;
        
        public override string CurrentPrompt => "拾取牙齿";
        
        public override bool CanInteract(GameObject actor)
        {
            if (!base.CanInteract(actor)) return false;
            return !_hasBeenUsed;
        }
        
        protected override void OnInteract(GameObject actor)
        {
            if (_hasBeenUsed) return;
            
            _hasBeenUsed = true;
            
            // 播放音效
            if (audioSource != null && pickupSfx != null)
            {
                audioSource.PlayOneShot(pickupSfx);
            }
            
            // 检查是否已经有剃刀
            if (FlagService.GetFlag("d3.razor_used"))
            {
                // 已有剃刀，合成剪刀
                FlagService.SetFlag("d3.tooth_used", true);
                FlagService.ClearHeldItem(); // 清除之前持有的剃刀
                gameObject.SetActive(false);
            }
            else
            {
                // 还没有剃刀，先装备牙齿
                FlagService.SetFlag("d3.tooth_used", true);
                FlagService.SetHeldItem("tooth");
                gameObject.SetActive(false);
            }
        }
    }
}

