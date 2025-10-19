using UnityEngine;
using SpookyGame.Core;
using SpookyGame.UI;

namespace SpookyGame.World
{
    /// <summary>
    /// 胡子 - 需要剃刀，点击后设置 flag
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class Beard : Interactable
    {
        [Header("Beard Settings")]
        [Tooltip("交互音效")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip shaveSfx;
        
        [Tooltip("交互动画")]
        [SerializeField] private Animator animator;
        [SerializeField] private string animTrigger = "Shave";
        
        [Header("Dialogue Settings")]
        [Tooltip("对话系统引用")]
        [SerializeField] private DialogueSystem dialogueSystem;
        
        [Tooltip("自动查找对话系统")]
        [SerializeField] private bool autoFindDialogueSystem = true;
        
        [Tooltip("没有剃刀时的对话")]
        [TextArea(2, 5)]
        [SerializeField] private string[] noRazorDialogueLines = new string[]
        {
            "需要剃刀才能刮胡子。"
        };
        
        [Header("Progress Flag")]
        [Tooltip("完成后设置的 flag")]
        [SerializeField] private string completionFlag = "d1.beard_shaved";
        
        [Header("Fade Out Settings")]
        [Tooltip("刮胡子后淡出时长")]
        [SerializeField] private float fadeOutDuration = 1f;
        
        private bool _hasInteracted = false;
        
        /// <summary>
        /// 动态提示
        /// </summary>
        public override string CurrentPrompt
        {
            get
            {
                string held = FlagService.GetHeldItem();
                if (held == "razor")
                {
                    return "刮胡子";
                }
                else
                {
                    return "需要剃刀";
                }
            }
        }
        protected override void Awake()
        {
            base.Awake();
            
            if (autoFindDialogueSystem && dialogueSystem == null)
            {
                dialogueSystem = FindObjectOfType<DialogueSystem>();
            }
            
            // 淡入效果
            StartCoroutine(FadeInCoroutine());
        }
        
        /// <summary>
        /// 淡入协程
        /// </summary>
        private System.Collections.IEnumerator FadeInCoroutine()
        {
            // 获取 SpriteRenderer 组件
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) yield break;
            
            // 设置初始透明度为 0
            Color startColor = spriteRenderer.color;
            startColor.a = 0f;
            spriteRenderer.color = startColor;
            
            // 淡入到完全不透明
            float duration = 1f; // 淡入时长
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                
                Color currentColor = spriteRenderer.color;
                currentColor.a = Mathf.Lerp(0f, 1f, t);
                spriteRenderer.color = currentColor;
                
                yield return null;
            }
            
            // 确保最终完全不透明
            Color finalColor = spriteRenderer.color;
            finalColor.a = 1f;
            spriteRenderer.color = finalColor;
        }
        
        /// <summary>
        /// 淡出协程
        /// </summary>
        private System.Collections.IEnumerator FadeOutCoroutine()
        {
            // 获取 SpriteRenderer 组件
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) yield break;
            
            // 从当前透明度淡出到完全透明
            Color startColor = spriteRenderer.color;
            float startAlpha = startColor.a;
            
            float elapsed = 0f;
            
            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / fadeOutDuration);
                
                Color currentColor = spriteRenderer.color;
                currentColor.a = Mathf.Lerp(startAlpha, 0f, t);
                spriteRenderer.color = currentColor;
                
                yield return null;
            }
            
            // 确保最终完全透明
            Color finalColor = spriteRenderer.color;
            finalColor.a = 0f;
            spriteRenderer.color = finalColor;
            
            // 隐藏游戏对象
            gameObject.SetActive(false);
        }
        
        /// <summary>
        /// 检查是否可以交互
        /// </summary>
        public override bool CanInteract(GameObject actor)
        {
            if (!base.CanInteract(actor)) return false;
            
            // 已经刮过胡子就不能再交互
            if (_hasInteracted) return false;
            
            // 无论是否有剃刀都可以点击（会显示不同的对话）
            return true;
        }
        
        /// <summary>
        /// 交互：刮胡子或提示需要剃刀
        /// </summary>
        protected override void OnInteract(GameObject actor)
        {
            if (_hasInteracted) return;
            
            string held = FlagService.GetHeldItem();
            
            // 检查是否持有剃刀
            if (held == "razor")
            {
                // 有剃刀：刮胡子
                _hasInteracted = true;
                
                if (animator != null && !string.IsNullOrEmpty(animTrigger))
                {
                    animator.SetTrigger(animTrigger);
                }
                
                if (audioSource != null && shaveSfx != null)
                {
                    audioSource.PlayOneShot(shaveSfx);
                }
                
                // 设置 flag，由 D1ProgressManager 处理后续对话和场景切换
                FlagService.SetFlag(completionFlag, true);
                
                // 开始淡出效果
                StartCoroutine(FadeOutCoroutine());
            }
            else
            {
                // 没有剃刀：显示提示对话
                if (dialogueSystem != null && noRazorDialogueLines != null && noRazorDialogueLines.Length > 0)
                {
                    dialogueSystem.StartDialogue(noRazorDialogueLines, null);
                }
            }
        }
    }
}

