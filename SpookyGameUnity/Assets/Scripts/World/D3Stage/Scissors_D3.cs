using UnityEngine;
using SpookyGame.World;
using SpookyGame.Core;

namespace SpookyGame.D3Scene
{
    /// <summary>
    /// D3 场景 - 剪刀
    /// 由牙齿+剃刀合成激活，可拾取
    /// </summary>
    public class Scissors_D3 : Interactable
    {
        [Header("Audio")]
        [Tooltip("拾取音效")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip pickupSfx;
        
        private bool _hasBeenPickedUp = false;
        
        public override string CurrentPrompt => "拾取剪刀";
        
        public override bool CanInteract(GameObject actor)
        {
            if (!base.CanInteract(actor)) return false;
            return !_hasBeenPickedUp;
        }
        
        protected override void OnInteract(GameObject actor)
        {
            if (_hasBeenPickedUp) return;
            
            _hasBeenPickedUp = true;
            
            // 播放音效
            if (audioSource != null && pickupSfx != null)
            {
                audioSource.PlayOneShot(pickupSfx);
            }
            
            // 设置持有剪刀
            FlagService.SetHeldItem("scissors");
            
            // 隐藏剪刀
            gameObject.SetActive(false);
        }
    }
}

