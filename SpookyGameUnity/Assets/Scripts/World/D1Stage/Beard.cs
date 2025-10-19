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
        
        [Header("Dialogue Settings")]
        [Tooltip("对话系统引用")]
        [SerializeField] private DialogueSystem dialogueSystem;
        
        [Tooltip("自动查找对话系统")]
        [SerializeField] private bool autoFindDialogueSystem = true;
        
        [Tooltip("没有剃刀时的对话")]
        [TextArea(2, 5)]
        [SerializeField] private string[] noRazorDialogueLines = new string[]
        {
            "需要剃刀才能刮胡子。"
        };
        
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
        protected override void Awake()
        {
            base.Awake();
            
            if (autoFindDialogueSystem && dialogueSystem == null)
            {
                dialogueSystem = FindObjectOfType<DialogueSystem>();
            }
        }
        
        /// <summary>
        /// 检查是否可以交互
        /// </summary>
        public override bool CanInteract(GameObject actor)
        {
            if (!base.CanInteract(actor)) return false;
            
            // 已经刮过胡子就不能再交互
            if (_hasInteracted) return false;
            
            // 无论是否有剃刀都可以点击（会显示不同的对话）
            return true;
        }
        
        /// <summary>
        /// 交互：刮胡子或提示需要剃刀
        /// </summary>
        protected override void OnInteract(GameObject actor)
        {
            if (_hasInteracted) return;
            
            string held = FlagService.GetHeldItem();
            
            // 检查是否持有剃刀
            if (held == "razor")
            {
                // 有剃刀：刮胡子
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
            else
            {
                // 没有剃刀：显示提示对话
                if (dialogueSystem != null && noRazorDialogueLines != null && noRazorDialogueLines.Length > 0)
                {
                    dialogueSystem.StartDialogue(noRazorDialogueLines, null);
                }
            }
        }
    }
}

