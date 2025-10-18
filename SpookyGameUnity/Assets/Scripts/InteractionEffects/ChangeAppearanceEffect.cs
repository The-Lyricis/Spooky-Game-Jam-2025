using UnityEngine;

namespace SpookyGame.World.InteractionEffects
{
    /// <summary>
    /// 改变物体外观效果
    /// 支持切换精灵图、改变颜色、显示/隐藏等
    /// </summary>
    public class ChangeAppearanceEffect : BaseInteractionEffect
    {
        [Header("Appearance Settings")]
        [SerializeField] private SpriteRenderer targetRenderer;
        
        [Header("Change Type")]
        [SerializeField] private bool changeSprite = false;
        [SerializeField] private Sprite newSprite;
        
        [SerializeField] private bool changeColor = false;
        [SerializeField] private Color newColor = Color.white;
        
        [SerializeField] private bool toggleVisibility = false;
        [SerializeField] private bool newVisibilityState = false;
        
        [SerializeField] private bool changeScale = false;
        [SerializeField] private Vector3 newScale = Vector3.one;
        
        [Header("Animation")]
        [SerializeField] private bool useAnimation = false;
        [SerializeField] private float animationDuration = 0.3f;
        
        private void Awake()
        {
            // 如果没有指定目标，使用自身的 SpriteRenderer
            if (targetRenderer == null)
            {
                targetRenderer = GetComponent<SpriteRenderer>();
            }
        }
        
        protected override void ExecuteImmediate(GameObject actor)
        {
            if (targetRenderer == null)
            {
                Debug.LogWarning($"[ChangeAppearanceEffect] {gameObject.name}: 没有找到 SpriteRenderer", gameObject);
                return;
            }
            
            if (useAnimation)
            {
                StartCoroutine(AnimateChange());
            }
            else
            {
                ApplyChanges();
            }
        }
        
        private void ApplyChanges()
        {
            if (changeSprite && newSprite != null)
            {
                targetRenderer.sprite = newSprite;
            }
            
            if (changeColor)
            {
                targetRenderer.color = newColor;
            }
            
            if (toggleVisibility)
            {
                targetRenderer.enabled = newVisibilityState;
            }
            
            if (changeScale)
            {
                targetRenderer.transform.localScale = newScale;
            }
        }
        
        private System.Collections.IEnumerator AnimateChange()
        {
            float elapsed = 0f;
            
            Color startColor = targetRenderer.color;
            Vector3 startScale = targetRenderer.transform.localScale;
            
            while (elapsed < animationDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / animationDuration;
                
                if (changeColor)
                {
                    targetRenderer.color = Color.Lerp(startColor, newColor, t);
                }
                
                if (changeScale)
                {
                    targetRenderer.transform.localScale = Vector3.Lerp(startScale, newScale, t);
                }
                
                yield return null;
            }
            
            // 确保最终状态准确
            ApplyChanges();
        }
    }
}

