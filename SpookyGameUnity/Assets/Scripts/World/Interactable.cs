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
        /// 交互提示文本
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
        /// 检查是否可以交互（点击式游戏不需要距离检测）
        /// </summary>
        /// <param name="actor">交互者（通常是 Interactor）</param>
        /// <returns>是否可以交互</returns>
        public virtual bool CanInteract(GameObject actor)
        {
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
                Gizmos.color = new Color(0, 1, 0, 0.3f); // 半透明绿色
                Gizmos.DrawCube(col.bounds.center, col.bounds.size);
            }
        }
    }
}
