using UnityEngine;
using System.Collections;

namespace SpookyGame.World
{
    /// <summary>
    /// 人物循环行走脚本
    /// 在起点和终点之间来回行走，到达终点后瞬移回起点，可在起点随机停顿
    /// </summary>
    public class PatrolWalker : MonoBehaviour
    {
        [Header("Path Settings")]
        [Tooltip("起点位置（相对于物体自身位置的偏移）")]
        [SerializeField] private Vector3 startPoint = new Vector3(-2f, 0f, 0f);
        
        [Tooltip("终点位置（相对于物体自身位置的偏移）")]
        [SerializeField] private Vector3 endPoint = new Vector3(2f, 0f, 0f);
        
        [Header("Movement Settings")]
        [Tooltip("移动速度（单位/秒）")]
        [SerializeField] private float moveSpeed = 2f;
        
        [Tooltip("是否在游戏开始时自动行走")]
        [SerializeField] private bool autoStart = true;
        
        [Tooltip("是否朝向移动方向（翻转 Sprite）")]
        [SerializeField] private bool faceDirection = true;
        
        [Header("Fade Settings")]
        [Tooltip("是否启用淡入淡出效果")]
        [SerializeField] private bool enableFade = true;
        
        [Tooltip("在起点隐藏等待的最小时间（秒）")]
        [SerializeField] private float minWaitTime = 0.5f;
        
        [Tooltip("在起点隐藏等待的最大时间（秒）")]
        [SerializeField] private float maxWaitTime = 2f;
        
        [Tooltip("淡入持续时间（秒）")]
        [SerializeField] private float fadeInDuration = 1f;
        
        [Tooltip("最大透明度（淡入目标值）")]
        [SerializeField] [Range(0f, 1f)] private float maxAlpha = 1f;
        
        [Header("Gizmos Settings")]
        [Tooltip("路径线颜色")]
        [SerializeField] private Color pathColor = Color.yellow;
        
        [Tooltip("起点终点球体颜色")]
        [SerializeField] private Color pointColor = Color.red;
        
        [Tooltip("球体半径")]
        [SerializeField] private float gizmoRadius = 0.2f;
        
        [Header("Debug")]
        [Tooltip("是否显示调试信息")]
        [SerializeField] private bool showDebugLog = false;
        
        // 运行时状态
        private Vector3 _worldStartPoint;
        private Vector3 _worldEndPoint;
        private Vector3 _targetPosition;
        private bool _isWalking = false;
        private bool _isWaiting = false;
        private bool _isFadingIn = false;
        private Vector3 _basePosition;
        private float _currentAlpha = 0f;
        private Coroutine _fadeRoutine;
        
        // 组件引用
        private SpriteRenderer _spriteRenderer;
        
        private void Awake()
        {
            // 记录基础位置
            _basePosition = transform.position;
            
            // 计算世界坐标
            UpdateWorldPoints();
            
            // 获取 SpriteRenderer（用于翻转和透明度）
            _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_spriteRenderer == null)
            {
                Debug.LogError($"[PatrolWalker] {gameObject.name}: SpriteRenderer not found! Fade effect will not work.", gameObject);
            }
        }
        
        private void Start()
        {
            if (autoStart)
            {
                StartWalking();
            }
        }
        
        private void OnDisable()
        {
            if (_fadeRoutine != null)
            {
                StopCoroutine(_fadeRoutine);
                _fadeRoutine = null;
            }
            _isWaiting = false;
            _isFadingIn = false;
        }
        
        private void Update()
        {
            if (!_isWalking) return;
            
            // 如果正在等待或淡入，不移动
            if (_isWaiting || _isFadingIn) return;
            
            // 向目标移动
            float step = moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, step);
            
            // 检查是否到达目标
            if (Vector3.Distance(transform.position, _targetPosition) < 0.01f)
            {
                OnReachedTarget();
            }
        }
        
        /// <summary>
        /// 开始行走循环
        /// </summary>
        public void StartWalking()
        {
            if (_isWalking) return;
            
            transform.position = _worldStartPoint;
            _targetPosition = _worldEndPoint;
            _isWalking = true;
            
            if (enableFade)
            {
                if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
                _fadeRoutine = StartCoroutine(FadeInCycle_Unscaled());
            }
            else
            {
                SetAlpha(maxAlpha);
                UpdateFacing();
            }
        }
        
        /// <summary>
        /// 停止行走
        /// </summary>
        public void StopWalking()
        {
            _isWalking = false;
            _isWaiting = false;
            _isFadingIn = false;
            
            if (_fadeRoutine != null)
            {
                StopCoroutine(_fadeRoutine);
                _fadeRoutine = null;
            }
        }
        
        /// <summary>
        /// 到达目标点时的处理
        /// </summary>
        private void OnReachedTarget()
        {
            transform.position = _worldStartPoint;
            _targetPosition = _worldEndPoint;
            
            if (enableFade)
            {
                if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
                _fadeRoutine = StartCoroutine(FadeInCycle_Unscaled());
            }
            else
            {
                UpdateFacing();
            }
        }
        
        /// <summary>
        /// 淡入循环：等待 → 淡入 → 行走（使用非缩放时间，不受 timeScale 影响）
        /// </summary>
        private IEnumerator FadeInCycle_Unscaled()
        {
            _isWaiting = true;
            _isFadingIn = false;
            SetAlpha(0f);
            
            float waitDuration = Mathf.Clamp(Random.Range(minWaitTime, maxWaitTime), 0f, 9999f);
            yield return new WaitForSecondsRealtime(waitDuration);
            
            _isWaiting = false;
            _isFadingIn = true;
            
            float elapsed = 0f;
            float dur = Mathf.Max(0.0001f, fadeInDuration);
            while (elapsed < dur)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / dur);
                SetAlpha(Mathf.Lerp(0f, maxAlpha, t));
                yield return null;
            }
            
            SetAlpha(maxAlpha);
            _isFadingIn = false;
            _fadeRoutine = null;
            
            UpdateFacing();
        }
        
        /// <summary>
        /// 设置透明度
        /// </summary>
        private void SetAlpha(float alpha)
        {
            _currentAlpha = alpha;
            
            if (_spriteRenderer != null)
            {
                Color color = _spriteRenderer.color;
                color.a = alpha;
                _spriteRenderer.color = color;
            }
        }
        
        /// <summary>
        /// 更新朝向（翻转 Sprite）
        /// </summary>
        private void UpdateFacing()
        {
            if (!faceDirection || _spriteRenderer == null) return;
            
            // 根据目标方向翻转
            float direction = _targetPosition.x - transform.position.x;
            if (Mathf.Abs(direction) > 0.01f)
            {
                _spriteRenderer.flipX = direction < 0;
            }
        }
        
        /// <summary>
        /// 更新世界坐标点
        /// </summary>
        private void UpdateWorldPoints()
        {
            _worldStartPoint = _basePosition + startPoint;
            _worldEndPoint = _basePosition + endPoint;
        }
        
        /// <summary>
        /// 设置移动速度
        /// </summary>
        public void SetSpeed(float speed)
        {
            moveSpeed = Mathf.Max(0f, speed);
        }
        
        /// <summary>
        /// 设置起点（相对偏移）
        /// </summary>
        public void SetStartPoint(Vector3 offset)
        {
            startPoint = offset;
            UpdateWorldPoints();
        }
        
        /// <summary>
        /// 设置终点（相对偏移）
        /// </summary>
        public void SetEndPoint(Vector3 offset)
        {
            endPoint = offset;
            UpdateWorldPoints();
        }
        
        /// <summary>
        /// 设置等待时间范围
        /// </summary>
        public void SetWaitTimeRange(float min, float max)
        {
            minWaitTime = Mathf.Max(0f, min);
            maxWaitTime = Mathf.Max(minWaitTime, max);
        }
        
        /// <summary>
        /// 设置淡入持续时间
        /// </summary>
        public void SetFadeInDuration(float duration)
        {
            fadeInDuration = Mathf.Max(0.1f, duration);
        }
        
        /// <summary>
        /// 设置最大透明度
        /// </summary>
        public void SetMaxAlpha(float alpha)
        {
            maxAlpha = Mathf.Clamp01(alpha);
        }
        
        /// <summary>
        /// 启用/禁用淡入淡出
        /// </summary>
        public void SetFadeEnabled(bool enabled)
        {
            enableFade = enabled;
        }
        
        /// <summary>
        /// 是否正在行走
        /// </summary>
        public bool IsWalking => _isWalking;
        
        /// <summary>
        /// 是否正在等待显现
        /// </summary>
        public bool IsWaiting => _isWaiting;
        
        /// <summary>
        /// 是否正在淡入
        /// </summary>
        public bool IsFadingIn => _isFadingIn;
        
        /// <summary>
        /// 当前透明度
        /// </summary>
        public float CurrentAlpha => _currentAlpha;
        
        /// <summary>
        /// 当前目标点
        /// </summary>
        public Vector3 CurrentTarget => _targetPosition;
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            // 计算世界坐标（编辑器模式下使用当前位置）
            Vector3 basePos = Application.isPlaying ? _basePosition : transform.position;
            Vector3 worldStart = basePos + startPoint;
            Vector3 worldEnd = basePos + endPoint;
            
            // 绘制路径线
            Gizmos.color = pathColor;
            Gizmos.DrawLine(worldStart, worldEnd);
            
            // 绘制起点
            Gizmos.color = pointColor;
            Gizmos.DrawWireSphere(worldStart, gizmoRadius);
            
            // 绘制终点
            Gizmos.color = pointColor;
            Gizmos.DrawWireSphere(worldEnd, gizmoRadius);
            
            // 绘制方向箭头
            Vector3 direction = (worldEnd - worldStart).normalized;
            Vector3 arrowPos = worldStart + direction * Vector3.Distance(worldStart, worldEnd) * 0.5f;
            DrawArrow(arrowPos, direction, pathColor, 0.3f);
            
            // 如果启用淡入，在起点绘制淡入标记
            if (enableFade)
            {
                // 绘制淡入效果标记（渐变圆圈）
                Gizmos.color = new Color(0.5f, 0.5f, 1f, 0.3f); // 蓝色半透明
                Gizmos.DrawSphere(worldStart, gizmoRadius * 0.5f);
                
                Gizmos.color = new Color(0.5f, 0.5f, 1f, 0.1f); // 更淡的蓝色
                Gizmos.DrawSphere(worldStart, gizmoRadius * 0.8f);
            }
        }
        
        private void OnDrawGizmosSelected()
        {
            // 选中时绘制填充球体
            Vector3 basePos = Application.isPlaying ? _basePosition : transform.position;
            Vector3 worldStart = basePos + startPoint;
            Vector3 worldEnd = basePos + endPoint;
            
            Gizmos.color = new Color(pointColor.r, pointColor.g, pointColor.b, 0.3f);
            Gizmos.DrawSphere(worldStart, gizmoRadius);
            Gizmos.DrawSphere(worldEnd, gizmoRadius);
            
            // 绘制当前位置和状态
            if (Application.isPlaying && _isWalking)
            {
                if (_isWaiting)
                {
                    // 等待中：灰色（隐形状态）
                    Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.3f);
                    Gizmos.DrawWireSphere(transform.position, gizmoRadius * 0.8f);
                }
                else if (_isFadingIn)
                {
                    // 淡入中：蓝色
                    Gizmos.color = new Color(0.5f, 0.5f, 1f, _currentAlpha);
                    Gizmos.DrawSphere(transform.position, gizmoRadius * 0.7f);
                }
                else
                {
                    // 行走中：绿色
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere(transform.position, gizmoRadius * 0.7f);
                }
            }
        }
        
        /// <summary>
        /// 绘制箭头 Gizmo
        /// </summary>
        private void DrawArrow(Vector3 pos, Vector3 direction, Color color, float arrowSize)
        {
            Gizmos.color = color;
            
            // 箭头主线
            Vector3 arrowHead = pos + direction * arrowSize;
            Gizmos.DrawLine(pos, arrowHead);
            
            // 箭头两侧
            Vector3 right = Quaternion.Euler(0, 0, 150) * direction * arrowSize * 0.5f;
            Vector3 left = Quaternion.Euler(0, 0, -150) * direction * arrowSize * 0.5f;
            
            Gizmos.DrawLine(arrowHead, arrowHead + right);
            Gizmos.DrawLine(arrowHead, arrowHead + left);
        }
#endif
        
        /// <summary>
        /// 验证设置
        /// </summary>
        private void OnValidate()
        {
            moveSpeed = Mathf.Max(0f, moveSpeed);
            gizmoRadius = Mathf.Max(0.05f, gizmoRadius);
            minWaitTime = Mathf.Max(0f, minWaitTime);
            maxWaitTime = Mathf.Max(minWaitTime, maxWaitTime);
            fadeInDuration = Mathf.Max(0.1f, fadeInDuration);
            maxAlpha = Mathf.Clamp01(maxAlpha);
            
            if (enableFade && maxAlpha < 0.01f)
            {
                Debug.LogWarning($"[PatrolWalker] {name}: maxAlpha ~= 0, 将不可见", this);
            }
            
            if (Vector3.Distance(startPoint, endPoint) < 0.01f)
            {
                Debug.LogWarning($"[PatrolWalker] {name}: Start point and end point are too close!", this);
            }
        }
    }
}

