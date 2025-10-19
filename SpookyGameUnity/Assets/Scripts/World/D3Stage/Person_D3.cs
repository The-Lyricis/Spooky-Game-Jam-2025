using UnityEngine;
using SpookyGame.World;
using SpookyGame.Core;
using SpookyGame.UI;

namespace SpookyGame.D3Scene
{
    /// <summary>
    /// D3 场景 - 人
    /// 用剪刀点击后激活指甲
    /// </summary>
    public class Person_D3 : Interactable
    {
        [Header("Audio")]
        [Tooltip("剪指甲音效")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip cutSfx;
        
        [Header("Animation")]
        [Tooltip("动画控制器")]
        [SerializeField] private Animator animator;
        [SerializeField] private string animTrigger = "Cut";
        
        [Header("Dialogue Settings")]
        [Tooltip("对话系统引用")]
        [SerializeField] private DialogueSystem dialogueSystem;
        
        [Tooltip("自动查找对话系统")]
        [SerializeField] private bool autoFindDialogueSystem = true;
        
        [Tooltip("没有剪刀时的对话")]
        [TextArea(2, 5)]
        [SerializeField] private string[] noScissorsDialogueLines = new string[]
        {
            "I need scissors to cut my nails."
        };
        
        private bool _hasBeenCut = false;
        
        protected override void Awake()
        {
            base.Awake();
            
            if (autoFindDialogueSystem && dialogueSystem == null)
            {
                dialogueSystem = FindObjectOfType<DialogueSystem>();
            }
        }
        
        public override string CurrentPrompt
        {
            get
            {
                string held = FlagService.GetHeldItem();
                if (held == "scissors")
                {
                    return "";
                }
                else
                {
                    return "";
                }
            }
        }
        
        public override bool CanInteract(GameObject actor)
        {
            if (!base.CanInteract(actor)) return false;
            return !_hasBeenCut;
        }
        
        protected override void OnInteract(GameObject actor)
        {
            if (_hasBeenCut) return;
            
            string held = FlagService.GetHeldItem();
            
            if (held == "scissors")
            {
                // 有剪刀：剪指甲
                _hasBeenCut = true;
                
                // 播放动画
                if (animator != null && !string.IsNullOrEmpty(animTrigger))
                {
                    animator.SetTrigger(animTrigger);
                }
                
                // 播放音效
                if (audioSource != null && cutSfx != null)
                {
                    audioSource.PlayOneShot(cutSfx);
                }
                
                // 设置 flag，激活指甲
                FlagService.SetFlag("d3.person_cut", true);
                
                // 清除手持物品
                FlagService.ClearHeldItem();
            }
            else
            {
                // 没有剪刀：显示提示对话
                if (dialogueSystem != null && noScissorsDialogueLines != null && noScissorsDialogueLines.Length > 0)
                {
                    dialogueSystem.StartDialogue(noScissorsDialogueLines, null);
                }
            }
        }
    }
}


