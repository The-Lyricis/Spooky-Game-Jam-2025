using UnityEngine;
using SpookyGame.Core;

namespace SpookyGame.World
{
    /// <summary>
    /// 花 - 需要鸟，点击后变换图片
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Flower : Interactable
    {
        [Header("Flower Settings")]
        [Tooltip("交互音效")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip interactSfx;
        
        [Header("Sprite Settings")]
        [Tooltip("花的不同状态图片")]
        [SerializeField] private Sprite flowerSprite;
        
        private SpriteRenderer _spriteRenderer;
        private int _currentSpriteIndex = 0;
        
        protected override void Awake()
        {
            base.Awake();
            
            // 获取 SpriteRenderer 组件
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        /// <summary>
        /// 动态提示
        /// </summary>
        public override string CurrentPrompt
        {
            get
            {
                string held = FlagService.GetHeldItem();
                if (held == "bird")
                {
                    return "使用鸟";
                }
                else
                {
                    return "需要鸟";
                }
            }
        }
        
        /// <summary>
        /// 检查是否可以交互
        /// </summary>
        public override bool CanInteract(GameObject actor)
        {
            if (!base.CanInteract(actor)) return false;
            
            // 检查是否持有鸟
            string held = FlagService.GetHeldItem();
            if (held != "bird")
            {
                Debug.Log("[Flower] 需要鸟才能与花交互");
                return false;
            }
            
            
            return true;
        }
        
        /// <summary>
        /// 与花交互 - 变换图片
        /// </summary>
        protected override void OnInteract(GameObject actor)
        {
            Debug.Log("[Flower] 使用鸟与花交互");
            
            // 切换到下一张图片
            ChangeSprite();
           FlagService.SetHeldItem("birdSkull"); 
            
            // 播放音效
            if (audioSource != null && interactSfx != null)
            {
                audioSource.PlayOneShot(interactSfx);
            }
        }
        
        /// <summary>
        /// 切换图片
        /// </summary>
        private void ChangeSprite()
        {
            _spriteRenderer.sprite = flowerSprite;

        }
    
    }
}

