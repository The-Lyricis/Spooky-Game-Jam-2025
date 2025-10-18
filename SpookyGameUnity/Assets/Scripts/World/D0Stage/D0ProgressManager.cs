using UnityEngine;
using SpookyGame.Core;
using SpookyGame.UI;

namespace SpookyGame.D0Scene
{
    /// <summary>
    /// D0 场景进度管理器
    /// 追踪照片和窗户的检视状态，切换物体显示，触发对话
    /// </summary>
    public class D0ProgressManager : MonoBehaviour
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
        
        [Tooltip("对话内容")]
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
        
        private int _lastInspectionCount = 0;
        private bool _dialogueTriggered = false;
        
        private void Start()
        {
            // 自动查找对话系统
            if (autoFindDialogueSystem && dialogueSystem == null)
            {
                dialogueSystem = FindObjectOfType<DialogueSystem>();
                if (dialogueSystem == null)
                {
                    Debug.LogError("[D0ProgressManager] DialogueSystem not found in scene! Please add a DialogueSystem component.");
                }
                else
                {
                    Debug.Log("[D0ProgressManager] Auto-found DialogueSystem");
                }
            }
            
            // 初始化：显示 ObjectA
            UpdateObjects();
            
            Debug.Log("[D0ProgressManager] Initialized. Waiting for inspections...");
        }
        
        private void Update()
        {
            // 检查检视进度
            int currentCount = GetInspectionCount();
            
            if (currentCount != _lastInspectionCount)
            {
                Debug.Log($"[D0ProgressManager] Inspection count: {_lastInspectionCount} → {currentCount}");
                _lastInspectionCount = currentCount;
                UpdateObjects();
                
                // 检视完 2 个后触发对话
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
            
            // 先隐藏所有物体
            if (objectA != null) objectA.SetActive(false);
            if (objectB != null) objectB.SetActive(false);
            if (objectC != null) objectC.SetActive(false);
            
            // 再根据检视数量显示对应物体
            switch (count)
            {
                case 0:
                    if (objectA != null) objectA.SetActive(true);
                    Debug.Log($"[D0ProgressManager] Showing ObjectA");
                    break;
                
                case 1:
                    if (objectB != null) objectB.SetActive(true);
                    Debug.Log($"[D0ProgressManager] A hidden → B enabled");
                    break;
                
                case 2:
                    if (objectC != null) objectC.SetActive(true);
                    Debug.Log($"[D0ProgressManager] B hidden → C enabled");
                    break;
            }
        }
        
        /// <summary>
        /// 触发最终对话
        /// </summary>
        private void TriggerFinalDialogue()
        {
            if (dialogueSystem == null)
            {
                Debug.LogError("[D0ProgressManager] DialogueSystem is not assigned! Cannot start dialogue.");
                return;
            }
            
            Debug.Log("[D0ProgressManager] Starting final dialogue...");
            
            // 使用对话系统，传入完成回调
            dialogueSystem.StartDialogue(dialogueLines, OnDialogueComplete);
        }
        [SerializeField] private string StagechangeStr = "第一关完成，即将进入第二关";
        /// <summary>
        /// 对话完成时的回调
        /// </summary>
        private void OnDialogueComplete()
        {
            Debug.Log("[D0ProgressManager] Dialogue complete. Switching to next stage...");
            
            // 切换到下一关
            SceneService.FadeToStage(nextStageId,"",1f,StagechangeStr);
        }
    }
}

