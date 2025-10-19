using UnityEngine;
using SpookyGame.Core;
using SpookyGame.UI;

namespace SpookyGame.World
{
    /// <summary>
    /// 胡子 - 需要剃刀，点击后设置 flag
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class Beard : Interactable
    {
        [Header("Beard Settings")]
        [Tooltip("交互音效")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip shaveSfx;
        
        [Tooltip("交互动画")]
        [SerializeField] private Animator animator;
        [SerializeField] private string animTrigger = "Shave";
        
        [Header("Progress Flag")]
        [Tooltip("完成后设置的 flag")]
        [SerializeField] private string completionFlag = "d1.beard_shaved";
        
        private bool _hasInteracted = false;
        
        /// <summary>
        /// 动态提示
        /// </summary>
        public override string CurrentPrompt
        {
            get
            {
                string held = FlagService.GetHeldItem();
                if (held == "razor")
                {
                    return "刮胡子";
                }
                else
                {
                    return "需要剃刀";
                }
            }
        }
        
        /// <summary>
        /// 检查是否可以交互
        /// </summary>
        public override bool CanInteract(GameObject actor)
        {
            if (!base.CanInteract(actor)) return false;
            
            if (_hasInteracted) return false;
            
            string held = FlagService.GetHeldItem();
            return held == "razor";
        }
        
        /// <summary>
        /// 刮胡子
        /// </summary>
        protected override void OnInteract(GameObject actor)
        {
            if (_hasInteracted) return;
            
            _hasInteracted = true;
            
            if (animator != null && !string.IsNullOrEmpty(animTrigger))
            {
                animator.SetTrigger(animTrigger);
            }
            
            if (audioSource != null && shaveSfx != null)
            {
                audioSource.PlayOneShot(shaveSfx);
            }
            
            // 设置 flag，由 D1ProgressManager 处理后续对话和场景切换
            FlagService.SetFlag(completionFlag, true);
        }
    }
}

