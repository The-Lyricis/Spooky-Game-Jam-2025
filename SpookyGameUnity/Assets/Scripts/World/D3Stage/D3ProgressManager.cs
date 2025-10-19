using UnityEngine;
using SpookyGame.Core;
using SpookyGame.UI;
using System.Collections;

namespace SpookyGame.D3Scene
{
    /// <summary>
    /// D3 场景管理器
    /// 管理复杂的物品合成和激活流程
    /// </summary>
    public class D3ProgressManager : MonoBehaviour
    {
        [Header("Scene Objects")]
        [Tooltip("剪刀物体（初始隐藏）")]
        [SerializeField] private GameObject scissors;
        
        [Tooltip("指甲物体（初始隐藏）")]
        [SerializeField] private GameObject nail;
        
        [Header("Dialogue Settings")]
        [Tooltip("对话系统引用")]
        [SerializeField] private DialogueSystem dialogueSystem;
        
        [Tooltip("自动查找对话系统")]
        [SerializeField] private bool autoFindDialogueSystem = true;
        
        [Tooltip("开场对话延迟时间（秒）")]
        [SerializeField] private float openingDialogueDelay = 2f;
        
        [Tooltip("开场对话")]
        [TextArea(2, 5)]
        [SerializeField] private string[] openingDialogueLines = new string[]
        {
            "需要剪指甲。"
        };
        
        [Tooltip("最终对话")]
        [TextArea(2, 5)]
        [SerializeField] private string[] finalDialogueLines = new string[]
        {
            "指甲剪好了。",
            "可以继续了。"
        };
        
        [Header("Scene Transition")]
        [Tooltip("对话结束后切换的关卡")]
        [SerializeField] private string nextStageId = "D4";
        
        [Tooltip("场景切换提示文字")]
        [SerializeField] private string stageChangeStr = "Day 4";
        
        [Tooltip("黑幕淡入淡出时长（秒）")]
        [SerializeField] private float fadeDuration = 1f;
        
        private bool _openingDialogueShown = false;
        private bool _scissorsActivated = false;
        private bool _nailActivated = false;
        private bool _finalDialogueTriggered = false;
        
        private void Start()
        {
            if (autoFindDialogueSystem && dialogueSystem == null)
            {
                dialogueSystem = FindObjectOfType<DialogueSystem>();
            }
            
            // 初始隐藏剪刀和指甲
            if (scissors != null) scissors.SetActive(false);
            if (nail != null) nail.SetActive(false);
            
            // 延迟显示开场对话
            StartCoroutine(WaitAndShowOpeningDialogue());
        }
        
        private void Update()
        {
            // 检查是否应该激活剪刀（牙齿和剃刀都被使用）
            if (!_scissorsActivated && 
                FlagService.GetFlag("d3.tooth_used") && 
                FlagService.GetFlag("d3.razor_used"))
            {
                _scissorsActivated = true;
                ActivateScissors();
            }
            
            // 检查是否应该激活指甲（剪刀被用在人身上）
            if (!_nailActivated && FlagService.GetFlag("d3.person_cut"))
            {
                _nailActivated = true;
                ActivateNail();
            }
            
            // 检查是否应该触发最终对话（指甲被点击）
            if (!_finalDialogueTriggered && FlagService.GetFlag("d3.nail_clicked"))
            {
                _finalDialogueTriggered = true;
                TriggerFinalDialogue();
            }
        }
        
        private IEnumerator WaitAndShowOpeningDialogue()
        {
            yield return new WaitForSeconds(openingDialogueDelay);
            
            if (!_openingDialogueShown)
            {
                _openingDialogueShown = true;
                ShowOpeningDialogue();
            }
        }
        
        private void ShowOpeningDialogue()
        {
            if (dialogueSystem != null && openingDialogueLines != null && openingDialogueLines.Length > 0)
            {
                dialogueSystem.StartDialogue(openingDialogueLines, OnOpeningDialogueComplete);
            }
        }
        
        private void OnOpeningDialogueComplete()
        {
            // 开场对话完成后，允许花播放动画
            FlagService.SetFlag("d3.opening_complete", true);
        }
        
        private void ActivateScissors()
        {
            if (scissors != null)
            {
                scissors.SetActive(true);
            }
        }
        
        private void ActivateNail()
        {
            if (nail != null)
            {
                nail.SetActive(true);
            }
        }
        
        private void TriggerFinalDialogue()
        {
            if (dialogueSystem != null && finalDialogueLines != null && finalDialogueLines.Length > 0)
            {
                dialogueSystem.StartDialogue(finalDialogueLines, OnFinalDialogueComplete);
            }
            else
            {
                OnFinalDialogueComplete();
            }
        }
        
        private void OnFinalDialogueComplete()
        {
            // 对话完成后，黑幕转场到下一关
            SceneService.FadeToStage(nextStageId, stageChangeStr, fadeDuration, stageChangeStr);
        }
    }
}

