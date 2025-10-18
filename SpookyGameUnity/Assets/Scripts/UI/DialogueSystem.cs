using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace SpookyGame.UI
{
    /// <summary>
    /// 通用对话系统
    /// 管理对话面板、文本显示、点击推进、完成回调
    /// 包含 UI 穿透防护
    /// </summary>
    public class DialogueSystem : MonoBehaviour
    {
        [Header("UI References")]
        [Tooltip("对话面板")]
        [SerializeField] private GameObject dialoguePanel;
        
        [Tooltip("对话文本")]
        [SerializeField] private Text dialogueText;
        
        [Header("Settings")]
        [Tooltip("自动查找 UI（如果未手动赋值）")]
        [SerializeField] private bool autoFindUI = true;
        
        [Tooltip("是否启用调试日志")]
        [SerializeField] private bool enableDebugLog = true;
        
        [Header("Typewriter Settings")]
        [Tooltip("是否启用打字机效果")]
        [SerializeField] private bool enableTypewriter = true;
        
        [Tooltip("基础打字速度（字符/秒）")]
        [SerializeField] private float typewriterSpeed = 30f;
        
        [Tooltip("点击可跳过打字机效果")]
        [SerializeField] private bool canSkipTypewriter = true;
        
        [Header("Punctuation Delays")]
        [Tooltip("句末标点停顿倍数（。！？…）")]
        [SerializeField] private float sentenceEndDelayMultiplier = 3f;
        
        [Tooltip("逗号停顿倍数（，、）")]
        [SerializeField] private float commaDelayMultiplier = 1.5f;
        
        [Tooltip("空格是否停顿")]
        [SerializeField] private bool pauseOnSpace = false;
        
        [Tooltip("空格停顿倍数")]
        [SerializeField] private float spaceDelayMultiplier = 0.5f;
        
        // 对话状态
        private string[] _currentDialogueLines;
        private int _currentLineIndex = 0;
        private bool _isDialogueActive = false;
        private bool _skipNextClick = false; // 防止启动对话时的点击被处理
        
        // 打字机状态
        private Coroutine _typewriterCoroutine;
        private bool _isTyping = false;
        private string _fullText = "";
        
        // 回调
        private Action _onDialogueComplete;
        
        private void Awake()
        {
            // 自动查找 UI
            if (autoFindUI)
            {
                if (dialoguePanel == null)
                {
                    dialoguePanel = GameObject.Find("DialoguePanel");
                    if (dialoguePanel != null && enableDebugLog)
                    {
                        Debug.Log($"[DialogueSystem] Auto-found DialoguePanel: {dialoguePanel.name}");
                    }
                }
                
                if (dialogueText == null && dialoguePanel != null)
                {
                    dialogueText = dialoguePanel.GetComponentInChildren<Text>();
                    if (dialogueText != null && enableDebugLog)
                    {
                        Debug.Log("[DialogueSystem] Auto-found DialogueText");
                    }
                }
            }
            
            // 验证配置
            if (dialoguePanel == null)
            {
                Debug.LogError("[DialogueSystem] DialoguePanel is not assigned! Dialogue will not work.", this);
            }
            
            if (dialogueText == null)
            {
                Debug.LogError("[DialogueSystem] DialogueText is not assigned! Text will not display.", this);
            }
            
            // 初始隐藏对话面板
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }
        }
        
        private void Update()
        {
            // 对话推进 - 跳过启动对话的那一帧
            if (_isDialogueActive && !_skipNextClick && UnityEngine.Input.GetMouseButtonDown(0))
            {
                // 【防止 UI 穿透】只有点击在对话面板上时才推进对话
                // 这样可以避免点击对话时误触场景中的可交互物体
                if (IsPointerOverDialoguePanel())
                {
                    // 如果正在打字，先跳过打字机效果
                    if (_isTyping && canSkipTypewriter)
                    {
                        SkipTypewriter();
                    }
                    else if (!_isTyping)
                    {
                        // 打字完成后，点击推进到下一行
                        NextLine();
                    }
                }
            }
            
            // 重置跳过标记（等到鼠标释放）
            if (_skipNextClick && !UnityEngine.Input.GetMouseButton(0))
            {
                _skipNextClick = false;
            }
        }
        
        /// <summary>
        /// 检查鼠标是否在对话面板上
        /// </summary>
        private bool IsPointerOverDialoguePanel()
        {
            if (!_isDialogueActive || dialoguePanel == null || !dialoguePanel.activeSelf)
            {
                return false;
            }
            
            // 检查是否在 UI 上
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                // 进一步检查是否在对话面板上（而非其他 UI）
                PointerEventData pointerData = new PointerEventData(EventSystem.current)
                {
                    position = UnityEngine.Input.mousePosition
                };
                
                var results = new System.Collections.Generic.List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, results);
                
                // 检查是否点击在 DialoguePanel 或其子物体上
                foreach (var result in results)
                {
                    if (result.gameObject == dialoguePanel || 
                        result.gameObject.transform.IsChildOf(dialoguePanel.transform))
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }
        
        /// <summary>
        /// 开始对话
        /// </summary>
        /// <param name="lines">对话内容数组</param>
        /// <param name="onComplete">对话完成时的回调（可选）</param>
        public void StartDialogue(string[] lines, Action onComplete = null)
        {
            if (lines == null || lines.Length == 0)
            {
                Debug.LogWarning("[DialogueSystem] Dialogue lines are empty!");
                onComplete?.Invoke();
                return;
            }
            
            if (dialoguePanel == null || dialogueText == null)
            {
                Debug.LogError("[DialogueSystem] UI not set up! Cannot start dialogue.", this);
                onComplete?.Invoke();
                return;
            }
            
            // 如果已有对话在进行，先结束
            if (_isDialogueActive)
            {
                if (enableDebugLog)
                {
                    Debug.LogWarning("[DialogueSystem] Another dialogue is already active. Ending it first.");
                }
                EndDialogue(false); // 不触发回调
            }
            
            // 初始化对话
            _currentDialogueLines = lines;
            _currentLineIndex = 0;
            _isDialogueActive = true;
            _skipNextClick = true; // 跳过启动对话时的点击
            _onDialogueComplete = onComplete;
            
            // 显示对话面板
            dialoguePanel.SetActive(true);
            
            // 显示第一行
            ShowCurrentLine();
            
            if (enableDebugLog)
            {
                Debug.Log($"[DialogueSystem] Dialogue started with {lines.Length} lines");
            }
        }
        
        /// <summary>
        /// 推进到下一行对话
        /// </summary>
        public void NextLine()
        {
            if (!_isDialogueActive)
            {
                return;
            }
            
            _currentLineIndex++;
            
            if (_currentLineIndex < _currentDialogueLines.Length)
            {
                // 还有对话，显示下一行
                ShowCurrentLine();
            }
            else
            {
                // 对话结束
                EndDialogue(true); // 触发回调
            }
        }
        
        /// <summary>
        /// 显示当前行对话
        /// </summary>
        private void ShowCurrentLine()
        {
            if (_currentLineIndex < _currentDialogueLines.Length && dialogueText != null)
            {
                string line = _currentDialogueLines[_currentLineIndex];
                _fullText = line;
                
                // 停止之前的打字机效果
                if (_typewriterCoroutine != null)
                {
                    StopCoroutine(_typewriterCoroutine);
                    _typewriterCoroutine = null;
                }
                
                if (enableTypewriter && !string.IsNullOrEmpty(line))
                {
                    // 启动打字机效果
                    _typewriterCoroutine = StartCoroutine(TypewriterEffect(line));
                }
                else
                {
                    // 直接显示全部文字
                    dialogueText.text = line;
                    _isTyping = false;
                }
                
                if (enableDebugLog)
                {
                    Debug.Log($"[DialogueSystem] Line {_currentLineIndex + 1}/{_currentDialogueLines.Length}: {line}");
                }
            }
        }
        
        /// <summary>
        /// 打字机效果协程
        /// </summary>
        private System.Collections.IEnumerator TypewriterEffect(string fullText)
        {
            _isTyping = true;
            dialogueText.text = "";
            
            float baseDelay = 1f / typewriterSpeed;
            
            for (int i = 0; i < fullText.Length; i++)
            {
                char c = fullText[i];
                dialogueText.text += c;
                
                // 根据字符类型计算延迟
                float delay = baseDelay;
                
                // 句末标点：。！？…
                if (c == '。' || c == '！' || c == '？' || c == '…' || 
                    c == '.' || c == '!' || c == '?')
                {
                    delay *= sentenceEndDelayMultiplier;
                }
                // 逗号：，、
                else if (c == '，' || c == '、' || c == ',')
                {
                    delay *= commaDelayMultiplier;
                }
                // 空格
                else if (c == ' ' && pauseOnSpace)
                {
                    delay *= spaceDelayMultiplier;
                }
                
                yield return new UnityEngine.WaitForSeconds(delay);
            }
            
            _isTyping = false;
            _typewriterCoroutine = null;
            
            if (enableDebugLog)
            {
                Debug.Log("[DialogueSystem] Typewriter effect completed.");
            }
        }
        
        /// <summary>
        /// 跳过打字机效果，立即显示全部文字
        /// </summary>
        private void SkipTypewriter()
        {
            if (_typewriterCoroutine != null)
            {
                StopCoroutine(_typewriterCoroutine);
                _typewriterCoroutine = null;
            }
            
            dialogueText.text = _fullText;
            _isTyping = false;
            
            if (enableDebugLog)
            {
                Debug.Log("[DialogueSystem] Typewriter effect skipped.");
            }
        }
        
        /// <summary>
        /// 结束对话
        /// </summary>
        /// <param name="invokeCallback">是否触发完成回调</param>
        private void EndDialogue(bool invokeCallback)
        {
            if (enableDebugLog)
            {
                Debug.Log("[DialogueSystem] Dialogue ended.");
            }
            
            _isDialogueActive = false;
            
            // 停止打字机效果
            if (_typewriterCoroutine != null)
            {
                StopCoroutine(_typewriterCoroutine);
                _typewriterCoroutine = null;
            }
            _isTyping = false;
            
            // 隐藏对话面板
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }
            
            // 触发回调
            if (invokeCallback && _onDialogueComplete != null)
            {
                if (enableDebugLog)
                {
                    Debug.Log("[DialogueSystem] Invoking completion callback.");
                }
                _onDialogueComplete.Invoke();
            }
            
            // 清理
            _currentDialogueLines = null;
            _onDialogueComplete = null;
            _fullText = "";
        }
        
        /// <summary>
        /// 强制结束当前对话（不触发回调）
        /// </summary>
        public void CancelDialogue()
        {
            if (_isDialogueActive)
            {
                EndDialogue(false);
            }
        }
        
        /// <summary>
        /// 是否有对话正在进行
        /// </summary>
        public bool IsDialogueActive => _isDialogueActive;
        
        /// <summary>
        /// 设置对话面板引用（运行时配置）
        /// </summary>
        public void SetDialoguePanel(GameObject panel)
        {
            dialoguePanel = panel;
            if (panel != null && dialogueText == null)
            {
                dialogueText = panel.GetComponentInChildren<Text>();
            }
        }
        
        /// <summary>
        /// 设置对话文本引用（运行时配置）
        /// </summary>
        public void SetDialogueText(Text text)
        {
            dialogueText = text;
        }
    }
}

