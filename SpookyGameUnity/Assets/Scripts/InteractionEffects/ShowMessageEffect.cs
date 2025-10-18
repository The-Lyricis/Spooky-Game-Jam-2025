using UnityEngine;
using SpookyGame.Core;

namespace SpookyGame.World.InteractionEffects
{
    /// <summary>
    /// 显示文字提示效果
    /// 通过EventBus发送消息事件
    /// </summary>
    public class ShowMessageEffect : BaseInteractionEffect
    {
        [Header("Message Settings")]
        [TextArea(3, 10)]
        [SerializeField] private string message = "Hello!";
        [SerializeField] private float displayDuration = 3f;
        [SerializeField] private bool useDebugLog = true;
        
        protected override void ExecuteImmediate(GameObject actor)
        {
            if (string.IsNullOrEmpty(message))
            {
                Debug.LogWarning($"[ShowMessageEffect] {gameObject.name}: 消息内容为空", gameObject);
                return;
            }
            
            // 发送消息事件
            EventBus.Publish(new MessageEvent(message, displayDuration));
            
            if (useDebugLog)
            {
                Debug.Log($"<color=cyan>[ShowMessageEffect] {gameObject.name}:</color> {message}", gameObject);
            }
        }
    }
    
    /// <summary>
    /// 消息事件（需要在EventBus中使用）
    /// </summary>
    public class MessageEvent : IEvent
    {
        public string Message { get; }
        public float Duration { get; }
        
        public MessageEvent(string message, float duration = 3f)
        {
            Message = message;
            Duration = duration;
        }
    }
}

