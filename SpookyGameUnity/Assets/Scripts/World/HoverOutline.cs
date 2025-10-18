using UnityEngine;

namespace SpookyGame.World
{
    /// <summary>
    /// Hover 时显示发光描边效果
    /// 需要配合 SpriteOutline Shader 使用
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class HoverOutline : MonoBehaviour
    {
        [Header("Outline Settings")]
        [Tooltip("描边颜色")]
        [SerializeField] private Color outlineColor = new Color(1f, 1f, 0.3f, 1f); // 黄色发光
        
        [Tooltip("描边宽度")]
        [SerializeField] private float outlineWidth = 0.03f;
        
        [Header("Animation")]
        [Tooltip("是否启用脉动动画")]
        [SerializeField] private bool enablePulse = true;
        
        [Tooltip("脉动速度")]
        [SerializeField] private float pulseSpeed = 2f;
        
        [Tooltip("脉动最小宽度倍数")]
        [SerializeField] private float pulseMin = 0.7f;
        
        [Tooltip("脉动最大宽度倍数")]
        [SerializeField] private float pulseMax = 1.3f;
        
        [Header("Transition")]
        [Tooltip("淡入淡出速度")]
        [SerializeField] private float transitionSpeed = 10f;
        
        [Header("Debug")]
        [Tooltip("是否显示调试信息")]
        [SerializeField] private bool showDebug = false;
        
        // 组件引用
        private SpriteRenderer _spriteRenderer;
        private Material _material;
        
        // 状态
        private bool _isGlowing = false;
        private float _currentWidth = 0f;
        private float _pulseTime = 0f;
        
        // Shader 属性 ID（缓存以提高性能）
        private static readonly int OutlineColorID = Shader.PropertyToID("_OutlineColor");
        private static readonly int OutlineWidthID = Shader.PropertyToID("_OutlineWidth");
        
        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            
            // 检查是否使用了正确的 Shader
            if (_spriteRenderer.material == null || !_spriteRenderer.material.shader.name.Contains("SpriteOutline"))
            {
                Debug.LogWarning($"[HoverOutline] {gameObject.name}: SpriteRenderer is not using SpriteOutline shader! Please assign the correct material.", gameObject);
            }
            
            // 创建材质实例（避免修改共享材质）
            _material = new Material(_spriteRenderer.material);
            _spriteRenderer.material = _material;
            
            // 初始隐藏描边
            SetOutlineWidth(0f);
            _material.SetColor(OutlineColorID, outlineColor);
            
            if (showDebug)
            {
                Debug.Log($"[HoverOutline] {gameObject.name}: Initialized with outline color {outlineColor}, width {outlineWidth}");
            }
        }
        
        private void Update()
        {
            // 计算目标宽度
            float targetWidth = _isGlowing ? outlineWidth : 0f;
            
            // 如果启用脉动且正在发光
            if (_isGlowing && enablePulse)
            {
                _pulseTime += Time.deltaTime * pulseSpeed;
                float pulse = Mathf.Lerp(pulseMin, pulseMax, (Mathf.Sin(_pulseTime) + 1f) * 0.5f);
                targetWidth *= pulse;
            }
            
            // 平滑过渡
            _currentWidth = Mathf.Lerp(_currentWidth, targetWidth, Time.deltaTime * transitionSpeed);
            SetOutlineWidth(_currentWidth);
        }
        
        /// <summary>
        /// 显示发光描边
        /// </summary>
        public void ShowGlow()
        {
            _isGlowing = true;
            
            if (showDebug)
            {
                Debug.Log($"[HoverOutline] {gameObject.name}: Glow started");
            }
        }
        
        /// <summary>
        /// 隐藏发光描边
        /// </summary>
        public void HideGlow()
        {
            _isGlowing = false;
            _pulseTime = 0f;
            
            if (showDebug)
            {
                Debug.Log($"[HoverOutline] {gameObject.name}: Glow stopped");
            }
        }
        
        /// <summary>
        /// 立即显示描边（无过渡）
        /// </summary>
        public void ShowGlowImmediate()
        {
            _isGlowing = true;
            _currentWidth = outlineWidth;
            SetOutlineWidth(_currentWidth);
        }
        
        /// <summary>
        /// 立即隐藏描边（无过渡）
        /// </summary>
        public void HideGlowImmediate()
        {
            _isGlowing = false;
            _currentWidth = 0f;
            _pulseTime = 0f;
            SetOutlineWidth(0f);
        }
        
        /// <summary>
        /// 设置描边颜色
        /// </summary>
        public void SetOutlineColor(Color color)
        {
            outlineColor = color;
            if (_material != null)
            {
                _material.SetColor(OutlineColorID, color);
            }
        }
        
        /// <summary>
        /// 设置描边宽度
        /// </summary>
        public void SetOutlineWidthValue(float width)
        {
            outlineWidth = width;
        }
        
        private void SetOutlineWidth(float width)
        {
            if (_material != null && _material.HasProperty(OutlineWidthID))
            {
                _material.SetFloat(OutlineWidthID, width);
            }
        }
        
        private void OnDestroy()
        {
            // 清理材质实例
            if (_material != null)
            {
                Destroy(_material);
            }
        }
        
        /// <summary>
        /// 是否正在发光
        /// </summary>
        public bool IsGlowing => _isGlowing;
    }
}

