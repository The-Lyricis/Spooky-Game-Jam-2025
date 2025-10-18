using UnityEngine;
using SpookyGame.Core;
using UnityEngine.UI;

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
        [Tooltip("对话面板")]
        [SerializeField] private GameObject dialoguePanel;
        
        [Tooltip("对话文本")]
        [SerializeField] private Text dialogueText;
        
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
        private int _currentDialogueIndex = 0;
        private bool _isDialogueActive = false;
        private bool _dialogueTriggered = false;
        
        private void Start()
        {
            // 初始化：显示 ObjectA
            UpdateObjects();
            
            // 隐藏对话面板
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }
            
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
                    StartDialogue();
                }
            }
            
            // 对话推进
            if (_isDialogueActive && UnityEngine.Input.GetMouseButtonDown(0))
            {
                NextDialogueLine();
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
        /// 开始对话
        /// </summary>
        private void StartDialogue()
        {
            Debug.Log("[D0ProgressManager] Starting dialogue...");
            
            _currentDialogueIndex = 0;
            _isDialogueActive = true;
            
            // 显示对话面板
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(true);
            }
            
            // 显示第一行对话
            ShowCurrentDialogueLine();
        }
        
        /// <summary>
        /// 显示当前对话行
        /// </summary>
        private void ShowCurrentDialogueLine()
        {
            if (_currentDialogueIndex < dialogueLines.Length && dialogueText != null)
            {
                dialogueText.text = dialogueLines[_currentDialogueIndex];
                Debug.Log($"[D0ProgressManager] Dialogue {_currentDialogueIndex + 1}/{dialogueLines.Length}: {dialogueLines[_currentDialogueIndex]}");
            }
        }
        
        /// <summary>
        /// 下一行对话
        /// </summary>
        private void NextDialogueLine()
        {
            _currentDialogueIndex++;
            
            if (_currentDialogueIndex < dialogueLines.Length)
            {
                // 还有对话，显示下一行
                ShowCurrentDialogueLine();
            }
            else
            {
                // 对话结束
                EndDialogue();
            }
        }
        
        /// <summary>
        /// 结束对话
        /// </summary>
        private void EndDialogue()
        {
            Debug.Log("[D0ProgressManager] Dialogue complete. Switching to next stage...");
            
            _isDialogueActive = false;
            
            // 隐藏对话面板
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }
            
            // 切换到下一关
            SceneService.FadeToStage(nextStageId,"",1f,"123");
        }
    }
}

