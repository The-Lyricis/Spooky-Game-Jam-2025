using UnityEngine;
using SpookyGame.Core;
using UnityEngine.UI;
using System.Collections;

namespace SpookyGame.D3Scene
{
    /// <summary>
    /// D3 场景 - 花
    /// 开场对话结束后可点击，播放动画并显示头顶对话框
    /// </summary>
    public class Flower_D3 : MonoBehaviour
    {
        [Header("Animation Settings")]
        [Tooltip("动画控制器")]
        [SerializeField] private Animator animator;
        
        [Tooltip("动画触发器名称")]
        [SerializeField] private string animTrigger = "Transform";
        
        [Header("Overhead Dialogue")]
        [Tooltip("头顶对话框 Canvas")]
        [SerializeField] private GameObject overheadDialogueCanvas;
        
        [Tooltip("头顶对话框文本")]
        [SerializeField] private Text overheadDialogueText;
        
        [Tooltip("头顶对话内容")]
        [SerializeField] private string overheadMessage = "Hungry~";
        
        [Tooltip("头顶对话显示时长（秒）")]
        [SerializeField] private float dialogueDuration = 3f;
        
        [Tooltip("头顶对话框淡入淡出时长（秒）")]
        [SerializeField] private float fadeDuration = 0.5f;
        
        private bool _hasTransformed = false;
        private CanvasGroup _canvasGroup;
        
        private void Awake()
        {
            // 初始隐藏头顶对话框
            if (overheadDialogueCanvas != null)
            {
                _canvasGroup = overheadDialogueCanvas.GetComponent<CanvasGroup>();
                if (_canvasGroup == null)
                {
                    _canvasGroup = overheadDialogueCanvas.AddComponent<CanvasGroup>();
                }
                _canvasGroup.alpha = 0f;
                overheadDialogueCanvas.SetActive(false);
            }
        }
        
        private void OnMouseDown()
        {
            // 检查开场对话是否完成
            if (!FlagService.GetFlag("d3.opening_complete"))
            {
                return;
            }
            
            // 只能变形一次
            if (_hasTransformed)
            {
                return;
            }
            
            _hasTransformed = true;
            Transform();
        }
        
        private void Transform()
        {
            // 播放动画
            if (animator != null && !string.IsNullOrEmpty(animTrigger))
            {
                animator.SetTrigger(animTrigger);
            }
            
            // 显示头顶对话框
            StartCoroutine(ShowOverheadDialogue());
        }
        
        private IEnumerator ShowOverheadDialogue()
        {
            if (overheadDialogueCanvas == null) yield break;
            
            // 设置文本内容
            if (overheadDialogueText != null)
            {
                overheadDialogueText.text = overheadMessage;
            }
            
            // 激活 Canvas
            overheadDialogueCanvas.SetActive(true);
            
            // 淡入
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
                yield return null;
            }
            _canvasGroup.alpha = 1f;
            
            // 显示一段时间
            yield return new WaitForSeconds(dialogueDuration);
            
            // 淡出
            elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
                yield return null;
            }
            _canvasGroup.alpha = 0f;
            
            // 隐藏 Canvas
            overheadDialogueCanvas.SetActive(false);
        }
    }
}
