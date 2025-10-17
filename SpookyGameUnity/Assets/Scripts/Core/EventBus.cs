using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpookyGame.Core
{
    /// <summary>
    /// 全局事件中枢，负责事件的发布和订阅
    /// </summary>
    public static class EventBus
    {
        private static Dictionary<Type, List<IEventHandler>> _handlers = new Dictionary<Type, List<IEventHandler>>();
        private static bool _isInitialized = false;
        
        /// <summary>
        /// 是否已初始化
        /// </summary>
        public static bool IsInitialized => _isInitialized;
        
        /// <summary>
        /// 初始化事件总线
        /// </summary>
        public static void Initialize()
        {
            if (_isInitialized)
            {
                Debug.LogWarning("[EventBus] Already initialized");
                return;
            }
            
            _handlers.Clear();
            _isInitialized = true;
            Debug.Log("[EventBus] Initialized");
        }
        
        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="handler">事件处理器</param>
        public static void Subscribe<T>(IEventHandler<T> handler) where T : IEvent
        {
            if (!_isInitialized)
            {
                Debug.LogError("[EventBus] Not initialized. Call Initialize() first.");
                return;
            }
            
            Type eventType = typeof(T);
            if (!_handlers.ContainsKey(eventType))
            {
                _handlers[eventType] = new List<IEventHandler>();
            }
            
            _handlers[eventType].Add(handler);
            Debug.Log($"[EventBus] Subscribed to {eventType.Name}");
        }
        
        /// <summary>
        /// 取消订阅事件
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="handler">事件处理器</param>
        public static void Unsubscribe<T>(IEventHandler<T> handler) where T : IEvent
        {
            if (!_isInitialized) return;
            
            Type eventType = typeof(T);
            if (_handlers.ContainsKey(eventType))
            {
                _handlers[eventType].Remove(handler);
                Debug.Log($"[EventBus] Unsubscribed from {eventType.Name}");
            }
        }
        
        /// <summary>
        /// 发布事件
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <param name="eventData">事件数据</param>
        public static void Publish<T>(T eventData) where T : IEvent
        {
            if (!_isInitialized)
            {
                Debug.LogError("[EventBus] Not initialized. Call Initialize() first.");
                return;
            }
            
            Type eventType = typeof(T);
            if (_handlers.ContainsKey(eventType))
            {
                foreach (var handler in _handlers[eventType])
                {
                    try
                    {
                        ((IEventHandler<T>)handler).Handle(eventData);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[EventBus] Error handling event {eventType.Name}: {e.Message}");
                    }
                }
            }
            
            Debug.Log($"[EventBus] Published {eventType.Name}");
        }
        
        /// <summary>
        /// 清理所有订阅
        /// </summary>
        public static void Clear()
        {
            _handlers.Clear();
            Debug.Log("[EventBus] Cleared all subscriptions");
        }
    }
    
    /// <summary>
    /// 事件接口
    /// </summary>
    public interface IEvent { }
    
    /// <summary>
    /// 提示事件
    /// </summary>
    public class PromptEvent : IEvent
    {
        public string PromptText { get; }
        public bool IsVisible { get; }
        
        public PromptEvent(string promptText, bool isVisible)
        {
            PromptText = promptText;
            IsVisible = isVisible;
        }
    }
    
    /// <summary>
    /// 谜题解决事件
    /// </summary>
    public class PuzzleSolvedEvent : IEvent
    {
        public string PuzzleId { get; }
        public string PuzzleName { get; }
        
        public PuzzleSolvedEvent(string puzzleId, string puzzleName = "")
        {
            PuzzleId = puzzleId;
            PuzzleName = puzzleName;
        }
    }
    
    /// <summary>
    /// 事件处理器接口
    /// </summary>
    public interface IEventHandler { }
    
    /// <summary>
    /// 泛型事件处理器接口
    /// </summary>
    /// <typeparam name="T">事件类型</typeparam>
    public interface IEventHandler<T> : IEventHandler where T : IEvent
    {
        void Handle(T eventData);
    }
}
