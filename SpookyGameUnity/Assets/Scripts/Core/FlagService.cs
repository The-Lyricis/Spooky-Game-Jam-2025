using System.Collections.Generic;
using UnityEngine;

namespace SpookyGame.Core
{
    /// <summary>
    /// 旗标服务，负责全局旗标的读写管理
    /// </summary>
    public static class FlagService
    {
        private static bool _isInitialized = false;
        private static Dictionary<string, bool> _flags = new Dictionary<string, bool>();
        
        /// <summary>
        /// 初始化旗标服务
        /// </summary>
        public static void Initialize()
        {
            if (_isInitialized)
            {
                Debug.LogWarning("[FlagService] Already initialized");
                return;
            }
            
            _flags.Clear();
            _isInitialized = true;
            Debug.Log("[FlagService] Initialized");
        }
        
        /// <summary>
        /// 设置旗标值
        /// </summary>
        /// <param name="flagName">旗标名称</param>
        /// <param name="value">旗标值</param>
        public static void SetFlag(string flagName, bool value)
        {
            if (!_isInitialized)
            {
                Debug.LogError("[FlagService] Not initialized. Call Initialize() first.");
                return;
            }
            
            if (string.IsNullOrEmpty(flagName))
            {
                Debug.LogError("[FlagService] Flag name cannot be null or empty");
                return;
            }
            
            bool previousValue = _flags.ContainsKey(flagName) ? _flags[flagName] : false;
            _flags[flagName] = value;
            
            Debug.Log($"[FlagService] Flag '{flagName}' set to {value}");
            
            // 发布旗标变化事件
            EventBus.Publish(new FlagChangedEvent(flagName, value, previousValue));
            
            // 如果是手持物变化，触发提示刷新
            if (flagName.StartsWith("held_") || flagName == "held")
            {
                RefreshInteractionPrompts();
            }
        }
        
        /// <summary>
        /// 触发交互提示刷新
        /// </summary>
        private static void RefreshInteractionPrompts()
        {
            // 查找 Interactor 并刷新提示
            var interactor = UnityEngine.Object.FindObjectOfType<SpookyGame.Player.Interactor>();
            if (interactor != null)
            {
                interactor.RefreshPrompt();
            }
        }
        
        /// <summary>
        /// 获取旗标值
        /// </summary>
        /// <param name="flagName">旗标名称</param>
        /// <returns>旗标值，如果不存在则返回 false</returns>
        public static bool GetFlag(string flagName)
        {
            if (!_isInitialized)
            {
                Debug.LogError("[FlagService] Not initialized. Call Initialize() first.");
                return false;
            }
            
            if (string.IsNullOrEmpty(flagName))
            {
                Debug.LogError("[FlagService] Flag name cannot be null or empty");
                return false;
            }
            
            return _flags.ContainsKey(flagName) ? _flags[flagName] : false;
        }
        
        /// <summary>
        /// 检查旗标是否存在
        /// </summary>
        /// <param name="flagName">旗标名称</param>
        /// <returns>是否存在</returns>
        public static bool HasFlag(string flagName)
        {
            if (!_isInitialized) return false;
            return _flags.ContainsKey(flagName);
        }
        
        /// <summary>
        /// 删除旗标
        /// </summary>
        /// <param name="flagName">旗标名称</param>
        public static void RemoveFlag(string flagName)
        {
            if (!_isInitialized)
            {
                Debug.LogError("[FlagService] Not initialized. Call Initialize() first.");
                return;
            }
            
            if (_flags.ContainsKey(flagName))
            {
                bool previousValue = _flags[flagName];
                _flags.Remove(flagName);
                
                Debug.Log($"[FlagService] Flag '{flagName}' removed");
                
                // 发布旗标删除事件
                EventBus.Publish(new FlagRemovedEvent(flagName, previousValue));
            }
        }
        
        /// <summary>
        /// 获取所有旗标
        /// </summary>
        /// <returns>旗标字典的副本</returns>
        public static Dictionary<string, bool> GetAllFlags()
        {
            if (!_isInitialized)
            {
                Debug.LogError("[FlagService] Not initialized. Call Initialize() first.");
                return new Dictionary<string, bool>();
            }
            
            return new Dictionary<string, bool>(_flags);
        }
        
        /// <summary>
        /// 设置多个旗标
        /// </summary>
        /// <param name="flags">旗标字典</param>
        public static void SetFlags(Dictionary<string, bool> flags)
        {
            if (!_isInitialized)
            {
                Debug.LogError("[FlagService] Not initialized. Call Initialize() first.");
                return;
            }
            
            if (flags == null) return;
            
            foreach (var kvp in flags)
            {
                SetFlag(kvp.Key, kvp.Value);
            }
        }
        
        /// <summary>
        /// 清空所有旗标
        /// </summary>
        public static void ClearAllFlags()
        {
            if (!_isInitialized)
            {
                Debug.LogError("[FlagService] Not initialized. Call Initialize() first.");
                return;
            }
            
            _flags.Clear();
            Debug.Log("[FlagService] All flags cleared");
        }
    }
    
    /// <summary>
    /// 旗标变化事件
    /// </summary>
    public class FlagChangedEvent : IEvent
    {
        public string FlagName { get; }
        public bool NewValue { get; }
        public bool PreviousValue { get; }
        
        public FlagChangedEvent(string flagName, bool newValue, bool previousValue)
        {
            FlagName = flagName;
            NewValue = newValue;
            PreviousValue = previousValue;
        }
    }
    
    /// <summary>
    /// 旗标删除事件
    /// </summary>
    public class FlagRemovedEvent : IEvent
    {
        public string FlagName { get; }
        public bool PreviousValue { get; }
        
        public FlagRemovedEvent(string flagName, bool previousValue)
        {
            FlagName = flagName;
            PreviousValue = previousValue;
        }
    }
}
