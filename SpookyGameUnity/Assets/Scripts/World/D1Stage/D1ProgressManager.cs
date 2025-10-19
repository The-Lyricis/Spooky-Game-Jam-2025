using UnityEngine;
using SpookyGame.Core;
using SpookyGame.UI;

namespace SpookyGame.D1Scene
{
    /// <summary>
    /// D1 场景管理器
    /// 进入场景显示开场对话，刮胡子后显示结束对话并切换场景
    /// </summary>
    public class D1ProgressManager : MonoBehaviour
    {
        [Header("Scene Object")]
        [Tooltip("刮胡子后显示的物体")]
        [SerializeField] private GameObject objectA;
        
        [Header("Dialogue Settings")]
        [Tooltip("对话系统引用")]
        [SerializeField] private DialogueSystem dialogueSystem;
        
        [Tooltip("自动查找对话系统")]
        [SerializeField] private bool autoFindDialogueSystem = true;
        
        [Tooltip("开场对话")]
        [TextArea(2, 5)]
        [SerializeField] private string[] openingDialogueLines = new string[]
        {
            "新的一天开始了...",
            "该刮刮胡子了。"
        };
        
        [Tooltip("结束对话（刮胡子后）")]
        [TextArea(2, 5)]
        [SerializeField] private string[] endingDialogueLines = new string[]
        {
            "你刮了胡子，感觉清爽多了。",
            "可以开始新的一天了。"
        };
        
        [Header("Scene Transition")]
        [Tooltip("对话结束后切换的关卡")]
        [SerializeField] private string nextStageId = "D2";
        
        [Tooltip("场景切换提示文字")]
        [SerializeField] private string stageChangeStr = "Day 2";
        
        private bool _hasShaved = false;
        
        private void Start()
        {
            if (autoFindDialogueSystem && dialogueSystem == null)
            {
                dialogueSystem = FindObjectOfType<DialogueSystem>();
            }
            
            // 初始隐藏 ObjectA
            if (objectA != null)
            {
                objectA.SetActive(false);
            }
            
            // 显示开场对话
            ShowOpeningDialogue();
        }
        
        private void Update()
        {
            // 检查是否刮了胡子
            if (!_hasShaved && FlagService.GetFlag("d1.beard_shaved"))
            {
                _hasShaved = true;
                OnBeardShaved();
            }
        }
        
        /// <summary>
        /// 显示开场对话
        /// </summary>
        private void ShowOpeningDialogue()
        {
            if (dialogueSystem != null && openingDialogueLines != null && openingDialogueLines.Length > 0)
            {
                dialogueSystem.StartDialogue(openingDialogueLines, null);
            }
        }
        
        /// <summary>
        /// 刮胡子后的处理
        /// </summary>
        private void OnBeardShaved()
        {
            // 显示 ObjectA
            if (objectA != null)
            {
                objectA.SetActive(true);
            }
            
            // 显示结束对话
            if (dialogueSystem != null && endingDialogueLines != null && endingDialogueLines.Length > 0)
            {
                dialogueSystem.StartDialogue(endingDialogueLines, OnEndingDialogueComplete);
            }
            else
            {
                OnEndingDialogueComplete();
            }
        }
        
        /// <summary>
        /// 结束对话完成后切换场景
        /// </summary>
        private void OnEndingDialogueComplete()
        {
            SceneService.FadeToStage(nextStageId, stageChangeStr, 1f, stageChangeStr);
        }
    }
}

