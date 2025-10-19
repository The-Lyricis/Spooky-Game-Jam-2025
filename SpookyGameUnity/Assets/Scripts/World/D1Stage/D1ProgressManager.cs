using UnityEngine;
using SpookyGame.Core;
using SpookyGame.UI;
using System.Collections;

namespace SpookyGame.D1Scene
{
    /// <summary>
    /// D1 场景管理器
    /// 进入场景显示开场对话，刮胡子后显示结束对话并切换场景
    /// </summary>
    public class D1ProgressManager : MonoBehaviour, IEventHandler<TransitionFadeOutCompleteEvent>
    {
        [Header("Scene Object")]
        [Tooltip("刮胡子后显示的物体")]
        [SerializeField] private GameObject objectA;
        
        [Header("Dialogue Settings")]
        [Tooltip("对话系统引用")]
        [SerializeField] private DialogueSystem dialogueSystem;
        
        [Tooltip("自动查找对话系统")]
        [SerializeField] private bool autoFindDialogueSystem = true;
        
        [Tooltip("开场对话延迟时间（秒）- 等待场景切换动画完成")]
        [SerializeField] private float openingDialogueDelay = 2f;
        
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
        
        private bool _hasShaved = false;
        private bool _openingDialogueShown = false;
        
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
            
            // 订阅转场淡出完成事件，等待黑幕淡出结束
            EventBus.Subscribe<TransitionFadeOutCompleteEvent>(this);
        }
        
        private void OnDestroy()
        {
            // 取消订阅
            EventBus.Unsubscribe<TransitionFadeOutCompleteEvent>(this);
        }
        
        /// <summary>
        /// 处理转场淡出完成事件
        /// </summary>
        public void Handle(TransitionFadeOutCompleteEvent eventData)
        {
            // 当转场淡出完成且当前在 D1 场景时，显示开场对话
            if (eventData.StageId == "D1" && !_openingDialogueShown)
            {
                _openingDialogueShown = true;
                ShowOpeningDialogue();
            }
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
            FlagService.ClearHeldItem();
        }

        [SerializeField] private  // 使用多个过渡文字
            string[] transitionTexts = new string[]
            {
                "Day 2"
            };
        
        /// <summary>
        /// 结束对话完成后切换场景
        /// </summary>
        private void OnEndingDialogueComplete()
        {
            SceneService.FadeToStage(nextStageId, 1f, transitionTexts);
        }
    }
}

