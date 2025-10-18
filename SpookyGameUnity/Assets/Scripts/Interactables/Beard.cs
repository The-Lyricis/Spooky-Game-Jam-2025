using UnityEngine;
using SpookyGame.Core;

namespace SpookyGame.World
{
    /// <summary>
    /// 胡子 - 需要剃刀，点击后切换场景
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
        
        [Header("Stage Transition")]
        [Tooltip("目标场景")]
        [SerializeField] private string targetStageId = "D2";
        
        [Tooltip("转场文字")]
        [SerializeField] private string intertitle = "第二关";
        
        [Tooltip("转场时长")]
        [SerializeField] private float fadeDuration = 1f;
        
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
            
            // 检查是否持有剃刀
            string held = FlagService.GetHeldItem();
            if (held != "razor")
            {
                Debug.Log("[Beard] 需要剃刀才能刮胡子");
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// 刮胡子
        /// </summary>
        protected override void OnInteract(GameObject actor)
        {
            Debug.Log("[Beard] 刮胡子");
            
            // 播放动画
            if (animator != null && !string.IsNullOrEmpty(animTrigger))
            {
                animator.SetTrigger(animTrigger);
            }
            
            // 播放音效
            if (audioSource != null && shaveSfx != null)
            {
                audioSource.PlayOneShot(shaveSfx);
            }
            
            // 切换场景
            Debug.Log($"[Beard] 切换到场景: {targetStageId}");
            SceneService.FadeToStage(targetStageId, intertitle, fadeDuration);
        }
    }
}

