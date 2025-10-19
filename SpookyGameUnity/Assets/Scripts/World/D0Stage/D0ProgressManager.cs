using UnityEngine;
using SpookyGame.Core;
using SpookyGame.UI;

namespace SpookyGame.D0Scene
{
    /// <summary>
    /// D0 场景进度管理器
    /// 追踪照片和窗户的检视状态，切换物体显示，触发对话
    /// </summary>
    public class D0ProgressManager : MonoBehaviour, IEventHandler<TransitionFadeOutCompleteEvent>
    {
        [Header("Progress Objects")]
        [Tooltip("物体 A（0 个检视）")]
        [SerializeField] private GameObject objectA;
        
        [Tooltip("物体 B（1 个检视）")]
        [SerializeField] private GameObject objectB;
        
        [Tooltip("物体 C（2 个检视）")]
        [SerializeField] private GameObject objectC;
        
        [Header("Dialogue Settings")]
        [Tooltip("对话系统引用")]
        [SerializeField] private DialogueSystem dialogueSystem;
        
        [Tooltip("自动查找对话系统")]
        [SerializeField] private bool autoFindDialogueSystem = true;
        
        [Tooltip("开场对话")]
        [TextArea(2, 5)]
        [SerializeField] private string[] openingDialogueLines = new string[]
        {
            "Myself, I do not recognise."
        };
        
        [Tooltip("最终对话内容")]
        [TextArea(2, 5)]
        [SerializeField] private string[] dialogueLines = new string[]
        {
            "你注意到了一些奇怪的细节...",
            "这些线索似乎暗示着什么...",
            "是时候开始调查了。"
        };
        
        [Header("Scene Transition")]
        [Tooltip("对话结束后切换的关卡")]
        [SerializeField] private string nextStageId = "D1";
        
        [Tooltip("场景切换提示文字")]
        [SerializeField] private  // 使用多个过渡文字
            string[] transitionTexts = new string[]
            {
                "Day 1"
            };
        
        private int _lastInspectionCount = 0;
        private bool _dialogueTriggered = false;
        private bool _openingDialogueShown = false;
        
        private void Start()
        {
            if (autoFindDialogueSystem && dialogueSystem == null)
            {
                dialogueSystem = FindObjectOfType<DialogueSystem>();
            }
            
            // 订阅转场淡出完成事件，等待黑幕淡出结束
            EventBus.Subscribe<TransitionFadeOutCompleteEvent>(this);
            
            UpdateObjects();
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
            // 当转场淡出完成且当前在 D0 场景时，显示开场对话
            if (eventData.StageId == "D0" && !_openingDialogueShown)
            {
                _openingDialogueShown = true;
                ShowOpeningDialogue();
            }
        }
        
        private void Update()
        {
            int currentCount = GetInspectionCount();
            
            if (currentCount != _lastInspectionCount)
            {
                _lastInspectionCount = currentCount;
                UpdateObjects();
                
                if (currentCount == 2 && !_dialogueTriggered)
                {
                    _dialogueTriggered = true;
                    TriggerFinalDialogue();
                }
            }
        }
        
        /// <summary>
        /// 获取检视数量
        /// </summary>
        private int GetInspectionCount()
        {
            int count = 0;
            
            if (FlagService.GetFlag("inspected.photo"))
            {
                count++;
            }
            
            if (FlagService.GetFlag("inspected.window"))
            {
                count++;
            }
            
            return count;
        }
        
        /// <summary>
        /// 更新物体显示
        /// 确保先隐藏旧物体，再启用新物体
        /// </summary>
        private void UpdateObjects()
        {
            int count = _lastInspectionCount;
            
            if (objectA != null) objectA.SetActive(false);
            if (objectB != null) objectB.SetActive(false);
            if (objectC != null) objectC.SetActive(false);
            
            switch (count)
            {
                case 0:
                    if (objectA != null) objectA.SetActive(true);
                    break;
                
                case 1:
                    if (objectB != null) objectB.SetActive(true);
                    break;
                
                case 2:
                    if (objectC != null) objectC.SetActive(true);
                    break;
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
        /// 触发最终对话
        /// </summary>
        private void TriggerFinalDialogue()
        {
            if (dialogueSystem != null)
            {
                dialogueSystem.StartDialogue(dialogueLines, OnDialogueComplete);
            }
        }
        
        /// <summary>
        /// 对话完成时的回调
        /// </summary>
        private void OnDialogueComplete()
        {
           
            
            SceneService.FadeToStage(nextStageId, 1f, transitionTexts);
        }
    }
}

