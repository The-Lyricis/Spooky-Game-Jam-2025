using UnityEngine;
using SpookyGame.World;
using SpookyGame.Core;

namespace SpookyGame.D3Scene
{
    /// <summary>
    /// D3 场景 - 指甲
    /// 剪完指甲后激活，点击后触发最终对话和转场
    /// </summary>
    public class Nail_D3 : Interactable
    {
        [Header("Audio")]
        [Tooltip("点击音效")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip clickSfx;
        
        private bool _hasBeenClicked = false;
        
        public override string CurrentPrompt => "查看指甲";
        
        public override bool CanInteract(GameObject actor)
        {
            if (!base.CanInteract(actor)) return false;
            return !_hasBeenClicked;
        }
        
        protected override void OnInteract(GameObject actor)
        {
            if (_hasBeenClicked) return;
            
            _hasBeenClicked = true;
            
            // 播放音效
            if (audioSource != null && clickSfx != null)
            {
                audioSource.PlayOneShot(clickSfx);
            }
            
            // 设置 flag，触发最终对话和转场
            FlagService.SetFlag("d3.nail_clicked", true);
        }
    }
}

