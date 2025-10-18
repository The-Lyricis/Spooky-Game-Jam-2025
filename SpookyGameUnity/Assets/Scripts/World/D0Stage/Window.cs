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
            
            // 自动查找对话系统
            if (autoFindDialogueSystem && dialogueSystem == null)
            {
                dialogueSystem = FindObjectOfType<DialogueSystem>();
                if (dialogueSystem == null)
                {
                    Debug.LogError("[Window] DialogueSystem not found in scene! Please add a DialogueSystem component.", gameObject);
                }
                else
                {
                    Debug.Log($"[Window] Auto-found DialogueSystem");
                }
            }
            
            // 验证配置
            if (dialogueSystem == null)
            {
                Debug.LogError("[Window] DialogueSystem is not assigned! Window dialogue will not work.", gameObject);
            }
        }
        
        protected override void OnInteract(GameObject actor)
        {
            if (_hasInteracted)
            {
                Debug.Log("[Window] Already interacted.");
                return;
            }
            
            if (dialogueSystem == null)
            {
                Debug.LogError("[Window] DialogueSystem is not assigned!", gameObject);
                return;
            }
            
            Debug.Log("[Window] Starting dialogue...");
            
            // 使用对话系统，传入完成回调
            dialogueSystem.StartDialogue(dialogueLines, OnDialogueComplete);
            
            _hasInteracted = true;
        }
        
        /// <summary>
        /// 对话完成时的回调
        /// </summary>
        private void OnDialogueComplete()
        {
            Debug.Log("[Window] Dialogue complete.");
            
            // 设置交互旗标（用于触发进度）
            FlagService.SetFlag("inspected.window", true);
            Debug.Log("[Window] Set flag: inspected.window = true");
        }
    }
}

