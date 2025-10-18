using UnityEngine;

namespace SpookyGame.World.InteractionEffects
{
    /// <summary>
    /// 交互效果的抽象基类
    /// 提供基础的MonoBehaviour支持和通用功能
    /// </summary>
    public abstract class BaseInteractionEffect : MonoBehaviour, IInteractionEffect
    {
        [Header("Effect Settings")]
        [SerializeField] protected float executeDelay = 0f;
        [SerializeField] protected bool executeOnce = false;
        
        protected bool _hasExecuted = false;
        
        /// <summary>
        /// 执行效果（带延迟支持）
        /// </summary>
        public void Execute(GameObject actor)
        {
            if (!CanExecute(actor))
            {
                return;
            }
            
            if (executeDelay > 0)
            {
                StartCoroutine(ExecuteDelayed(actor));
            }
            else
            {
                ExecuteImmediate(actor);
            }
            
            _hasExecuted = true;
        }
        
        /// <summary>
        /// 检查是否可以执行
        /// </summary>
        public virtual bool CanExecute(GameObject actor)
        {
            if (executeOnce && _hasExecuted)
            {
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// 立即执行效果（子类实现）
        /// </summary>
        protected abstract void ExecuteImmediate(GameObject actor);
        
        /// <summary>
        /// 延迟执行效果
        /// </summary>
        private System.Collections.IEnumerator ExecuteDelayed(GameObject actor)
        {
            yield return new UnityEngine.WaitForSeconds(executeDelay);
            ExecuteImmediate(actor);
        }
        
        /// <summary>
        /// 重置效果状态
        /// </summary>
        public virtual void ResetEffect()
        {
            _hasExecuted = false;
        }
    }
}

