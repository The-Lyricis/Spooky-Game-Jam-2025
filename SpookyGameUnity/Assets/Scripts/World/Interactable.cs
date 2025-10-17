using UnityEngine;

namespace SpookyGame.World
{
    /// <summary>
    /// 可交互对象的抽象基类
    /// </summary>
    public abstract class Interactable : MonoBehaviour
    {
        [Header("Interaction Settings")]
        [SerializeField] protected string interactionPrompt = "按 E 交互";
        [SerializeField] protected float interactionRange = 2f;
        [SerializeField] protected bool canInteractMultipleTimes = true;
        [SerializeField] protected float interactionCooldown = 0.5f;
        
        protected bool _hasInteracted = false;
        protected float _lastInteractionTime = 0f;
        
        /// <summary>
        /// 交互提示文本
        /// </summary>
        public string InteractionPrompt => interactionPrompt;
        
        /// <summary>
        /// 交互范围
        /// </summary>
        public float InteractionRange => interactionRange;
        
        /// <summary>
        /// 是否可以多次交互
        /// </summary>
        public bool CanInteractMultipleTimes => canInteractMultipleTimes;
        
        /// <summary>
        /// 是否已经交互过
        /// </summary>
        public bool HasInteracted => _hasInteracted;
        
        /// <summary>
        /// 检查是否可以与指定对象交互
        /// </summary>
        /// <param name="actor">交互者</param>
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
            
            // 检查距离
            float distance = Vector2.Distance(transform.position, actor.transform.position);
            if (distance > interactionRange)
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
        /// 设置交互范围
        /// </summary>
        /// <param name="range">新的交互范围</param>
        public virtual void SetInteractionRange(float range)
        {
            interactionRange = range;
        }
        
        /// <summary>
        /// 在 Scene 视图中绘制交互范围
        /// </summary>
        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionRange);
        }
    }
}
