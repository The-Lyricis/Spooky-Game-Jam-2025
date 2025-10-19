using UnityEngine;
using SpookyGame.World;
using SpookyGame.Core;
using SpookyGame.UI;

namespace SpookyGame.D0Scene
{
    /// <summary>
    /// D0 场景 - 窗户对话
    /// 点击后显示对话，对话结束后设置交互旗标
    /// </summary>
    public class Window : Interactable
    {
        [Header("Dialogue Settings")]
        [Tooltip("对话内容")]
        [TextArea(2, 5)]
        [SerializeField] private string[] dialogueLines = new string[]
        {
            "窗外的景色很平静...",
            "但总感觉有什么不对劲。"
        };
        
        [Tooltip("对话系统引用")]
        [SerializeField] private DialogueSystem dialogueSystem;
        
        [Tooltip("自动查找对话系统")]
        [SerializeField] private bool autoFindDialogueSystem = true;
        
        private bool _hasInteracted = false;
        
        protected override void Awake()
        {
            base.Awake();
            
            if (autoFindDialogueSystem && dialogueSystem == null)
            {
                dialogueSystem = FindObjectOfType<DialogueSystem>();
            }
        }
        
        protected override void OnInteract(GameObject actor)
        {
            if (_hasInteracted || dialogueSystem == null)
            {
                return;
            }
            
            dialogueSystem.StartDialogue(dialogueLines, OnDialogueComplete);
            _hasInteracted = true;
        }
        
        private void OnDialogueComplete()
        {
            FlagService.SetFlag("inspected.window", true);
        }
    }
}

