using UnityEngine;
using UnityEngine.Events;
using SpookyGame.World.InteractionEffects;

namespace SpookyGame.World
{
    /// <summary>
    /// 可组合的交互对象
    /// 支持添加多个效果组件，点击时依次执行
    /// </summary>
    public class CompositeInteractable : Interactable
    {
        [Header("Composite Settings")]
        [SerializeField] private bool autoCollectEffects = true;
        [SerializeField] private IInteractionEffect[] customEffects;
        
        [Header("Unity Events")]
        [SerializeField] private UnityEvent onInteractEvent;
        [SerializeField] private UnityEvent<GameObject> onInteractWithActorEvent;
        
        private IInteractionEffect[] _effects;
        
        protected override void Awake()
        {
            base.Awake();
            
            // 自动收集子物体上的所有效果组件
            if (autoCollectEffects)
            {
                _effects = GetComponents<IInteractionEffect>();
            }
            else if (customEffects != null)
            {
                _effects = customEffects;
            }
        }
        
        protected override void OnInteract(GameObject actor)
        {
            // 执行所有效果
            if (_effects != null)
            {
                foreach (var effect in _effects)
                {
                    if (effect != null && effect.CanExecute(actor))
                    {
                        effect.Execute(actor);
                    }
                }
            }
            
            // 触发Unity事件
            onInteractEvent?.Invoke();
            onInteractWithActorEvent?.Invoke(actor);
        }
        
        /// <summary>
        /// 手动添加效果
        /// </summary>
        public void AddEffect(IInteractionEffect effect)
        {
            if (effect == null) return;
            
            var effectsList = new System.Collections.Generic.List<IInteractionEffect>();
            if (_effects != null)
            {
                effectsList.AddRange(_effects);
            }
            
            effectsList.Add(effect);
            _effects = effectsList.ToArray();
        }
        
        /// <summary>
        /// 清空所有效果
        /// </summary>
        public void ClearEffects()
        {
            _effects = null;
        }
        
        /// <summary>
        /// 重新收集效果组件
        /// </summary>
        public void RefreshEffects()
        {
            _effects = GetComponents<IInteractionEffect>();
        }
    }
}

