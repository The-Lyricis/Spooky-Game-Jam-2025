using UnityEngine;

namespace SpookyGame.World.InteractionEffects
{
    /// <summary>
    /// 播放动画效果
    /// 支持Animator和简单的动画效果
    /// </summary>
    public class PlayAnimationEffect : BaseInteractionEffect
    {
        [Header("Animation Settings")]
        [SerializeField] private Animator targetAnimator;
        [SerializeField] private string animationTrigger = "Play";
        [SerializeField] private string animationStateName = "";
        [SerializeField] private bool useTrigger = true;
        
        [Header("Simple Animation (如果没有Animator)")]
        [SerializeField] private bool useSimpleAnimation = false;
        [SerializeField] private AnimationType animationType = AnimationType.Bounce;
        [SerializeField] private float animationDuration = 0.5f;
        [SerializeField] private float animationStrength = 0.2f;
        
        private void Awake()
        {
            // 如果没有指定目标，尝试获取自身或子物体的Animator
            if (targetAnimator == null)
            {
                targetAnimator = GetComponentInChildren<Animator>();
            }
        }
        
        protected override void ExecuteImmediate(GameObject actor)
        {
            if (useSimpleAnimation)
            {
                PlaySimpleAnimation();
            }
            else if (targetAnimator != null)
            {
                PlayAnimatorAnimation();
            }
            else
            {
                Debug.LogWarning($"[PlayAnimationEffect] {gameObject.name}: 没有找到Animator且未启用简单动画", gameObject);
            }
        }
        
        private void PlayAnimatorAnimation()
        {
            if (useTrigger && !string.IsNullOrEmpty(animationTrigger))
            {
                targetAnimator.SetTrigger(animationTrigger);
                Debug.Log($"[PlayAnimationEffect] 触发动画: {animationTrigger}", gameObject);
            }
            else if (!string.IsNullOrEmpty(animationStateName))
            {
                targetAnimator.Play(animationStateName);
                Debug.Log($"[PlayAnimationEffect] 播放动画: {animationStateName}", gameObject);
            }
        }
        
        private void PlaySimpleAnimation()
        {
            switch (animationType)
            {
                case AnimationType.Bounce:
                    StartCoroutine(BounceAnimation());
                    break;
                case AnimationType.Shake:
                    StartCoroutine(ShakeAnimation());
                    break;
                case AnimationType.Pulse:
                    StartCoroutine(PulseAnimation());
                    break;
            }
        }
        
        private System.Collections.IEnumerator BounceAnimation()
        {
            Vector3 originalScale = transform.localScale;
            float elapsed = 0f;
            
            while (elapsed < animationDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / animationDuration;
                float bounce = Mathf.Sin(t * Mathf.PI);
                
                transform.localScale = originalScale * (1f + bounce * animationStrength);
                yield return null;
            }
            
            transform.localScale = originalScale;
        }
        
        private System.Collections.IEnumerator ShakeAnimation()
        {
            Vector3 originalPosition = transform.localPosition;
            float elapsed = 0f;
            
            while (elapsed < animationDuration)
            {
                elapsed += Time.deltaTime;
                
                float x = Random.Range(-1f, 1f) * animationStrength;
                float y = Random.Range(-1f, 1f) * animationStrength;
                
                transform.localPosition = originalPosition + new Vector3(x, y, 0);
                yield return null;
            }
            
            transform.localPosition = originalPosition;
        }
        
        private System.Collections.IEnumerator PulseAnimation()
        {
            Vector3 originalScale = transform.localScale;
            float elapsed = 0f;
            
            // 放大
            while (elapsed < animationDuration / 2)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (animationDuration / 2);
                transform.localScale = Vector3.Lerp(originalScale, originalScale * (1f + animationStrength), t);
                yield return null;
            }
            
            elapsed = 0f;
            // 缩小
            while (elapsed < animationDuration / 2)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (animationDuration / 2);
                transform.localScale = Vector3.Lerp(originalScale * (1f + animationStrength), originalScale, t);
                yield return null;
            }
            
            transform.localScale = originalScale;
        }
        
        public enum AnimationType
        {
            Bounce,
            Shake,
            Pulse
        }
    }
}

