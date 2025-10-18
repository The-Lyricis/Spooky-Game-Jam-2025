using UnityEngine;

namespace SpookyGame.World
{
    /// <summary>
    /// 可交互对象的抽象基类（点击式交互）
    /// 会自动添加 Collider2D 组件用于鼠标检测
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public abstract class Interactable : MonoBehaviour
    {
        [Header("Interaction Settings")]
        [SerializeField] protected string interactionPrompt = "点击交互";
        [SerializeField] protected bool canInteractMultipleTimes = true;
        [SerializeField] protected float interactionCooldown = 0.5f;
        
        protected bool _hasInteracted = false;
        protected float _lastInteractionTime = 0f;
        protected Collider2D _collider;
        
        /// <summary>
        /// 当前交互提示文本（支持动态修改，例如根据手持物显示不同提示）
        /// </summary>
        public virtual string CurrentPrompt => interactionPrompt;
        
        /// <summary>
        /// 交互提示文本（静态字段）
        /// </summary>
        public string InteractionPrompt => interactionPrompt;
        
        /// <summary>
        /// 是否可以多次交互
        /// </summary>
        public bool CanInteractMultipleTimes => canInteractMultipleTimes;
        
        /// <summary>
        /// 是否已经交互过
        /// </summary>
        public bool HasInteracted => _hasInteracted;
        
        /// <summary>
        /// 是否启用交互（基于 Collider 状态）
        /// </summary>
        public bool InteractionEnabled => _collider != null && _collider.enabled;
        
        protected virtual void Awake()
        {
            // 获取 Collider2D 组件
            _collider = GetComponent<Collider2D>();
            if (_collider == null)
            {
                Debug.LogError($"[Interactable] {gameObject.name} 缺少 Collider2D 组件！", gameObject);
            }
        }
        
        /// <summary>
        /// 检查是否可以悬停显示提示（不包含冷却和一次性判断）
        /// </summary>
        /// <param name="actor">交互者（通常是 Interactor）</param>
        /// <returns>是否可以悬停</returns>
        public virtual bool CanHover(GameObject actor)
        {
            // 默认只要启用就可以悬停显示提示
            return InteractionEnabled;
        }
        
        /// <summary>
        /// 检查是否可以交互（点击时调用，包含冷却和一次性判断）
        /// </summary>
        /// <param name="actor">交互者（通常是 Interactor）</param>
        /// <returns>是否可以交互</returns>
        public virtual bool CanInteract(GameObject actor)
        {
            // 先检查基础悬停条件
            if (!CanHover(actor))
            {
                return false;
            }
            
            // 检查是否在冷却时间内
            if (Time.time - _lastInteractionTime < interactionCooldown)
            {
                return false;
            }
            
            // 检查是否已经交互过且不允许多次交互
            if (_hasInteracted && !canInteractMultipleTimes)
            {
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// 执行交互
        /// </summary>
        /// <param name="actor">交互者</param>
        public virtual void Interact(GameObject actor)
        {
            if (!CanInteract(actor))
            {
                return;
            }
            
            _lastInteractionTime = Time.time;
            _hasInteracted = true;
            
            // 执行具体的交互逻辑
            OnInteract(actor);
            
            Debug.Log($"[Interactable] {gameObject.name} interacted by {actor.name}");
        }
        
        /// <summary>
        /// 子类需要实现的交互逻辑
        /// </summary>
        /// <param name="actor">交互者</param>
        protected abstract void OnInteract(GameObject actor);
        
        /// <summary>
        /// 鼠标悬停进入时调用（用于高亮等视觉反馈）
        /// </summary>
        public virtual void OnHoverEnter()
        {
            // 自动查找并触发 HoverOutline
            var outline = GetComponent<HoverOutline>();
            if (outline != null)
            {
                outline.ShowGlow();
            }
        }
        
        /// <summary>
        /// 鼠标悬停离开时调用
        /// </summary>
        public virtual void OnHoverExit()
        {
            // 自动查找并停止 HoverOutline
            var outline = GetComponent<HoverOutline>();
            if (outline != null)
            {
                outline.HideGlow();
            }
        }
        
        /// <summary>
        /// 重置交互状态
        /// </summary>
        public virtual void ResetInteraction()
        {
            _hasInteracted = false;
            _lastInteractionTime = 0f;
        }
        
        /// <summary>
        /// 设置交互提示文本
        /// </summary>
        /// <param name="prompt">新的提示文本</param>
        public virtual void SetInteractionPrompt(string prompt)
        {
            interactionPrompt = prompt;
        }
        
        /// <summary>
        /// 启用/禁用交互
        /// </summary>
        /// <param name="enabled">是否启用</param>
        public virtual void SetInteractionEnabled(bool enabled)
        {
            if (_collider != null)
            {
                _collider.enabled = enabled;
            }
        }
        
        /// <summary>
        /// 在 Scene 视图中绘制 Collider 边界
        /// </summary>
        protected virtual void OnDrawGizmosSelected()
        {
            Collider2D col = GetComponent<Collider2D>();
            if (col != null)
            {
                Gizmos.color = new Color(1, 1, 0, 0.3f); // 半透明黄色
                Gizmos.DrawCube(col.bounds.center, col.bounds.size);
            }
        }
    }
}
