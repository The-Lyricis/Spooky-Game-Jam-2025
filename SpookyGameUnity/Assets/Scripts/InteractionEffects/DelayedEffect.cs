using UnityEngine;
using UnityEngine.Events;

namespace SpookyGame.World.InteractionEffects
{
    /// <summary>
    /// 延迟效果包装器
    /// 允许在指定延迟后触发 UnityEvent
    /// 可用于在 Inspector 中配置任意延迟逻辑
    /// </summary>
    public class DelayedEffect : BaseInteractionEffect
    {
        [Header("Delayed Event")]
        [SerializeField] private UnityEvent onExecute;
        [SerializeField] private UnityEvent<GameObject> onExecuteWithActor;
        
        private GameObject _cachedActor;
        
        protected override void ExecuteImmediate(GameObject actor)
        {
            _cachedActor = actor;
            onExecute?.Invoke();
            onExecuteWithActor?.Invoke(actor);
        }
        
        // 以下方法可以在 UnityEvent 中调用
        
        /// <summary>
        /// 启用指定游戏物体
        /// </summary>
        public void ActivateObject(GameObject obj)
        {
            if (obj != null) obj.SetActive(true);
        }
        
        /// <summary>
        /// 禁用指定游戏物体
        /// </summary>
        public void DeactivateObject(GameObject obj)
        {
            if (obj != null) obj.SetActive(false);
        }
        
        /// <summary>
        /// 打印日志
        /// </summary>
        public void LogMessage(string message)
        {
            Debug.Log($"[DelayedEffect] {message}", gameObject);
        }
    }
}

