using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpookyGame.Core
{
    /// <summary>
    /// 全局旗标服务
    /// 支持 bool 旗标和 string 变量
    /// </summary>
    public static class FlagService
    {
        private static bool _isInitialized = false;
        private static Dictionary<string, bool> _flags = new Dictionary<string, bool>();
        private static Dictionary<string, string> _vars = new Dictionary<string, string>();
        
        public static void Initialize()
        {
            if (_isInitialized) return;
            
            _flags.Clear();
            _vars.Clear();
            _isInitialized = true;
            
            Debug.Log("[FlagService] Initialized");
        }
        
        #region Bool Flags
        
        /// <summary>
        /// 获取布尔旗标（默认 false）
        /// </summary>
        public static bool GetFlag(string flagName)
        {
            if (string.IsNullOrEmpty(flagName)) return false;
            return _flags.ContainsKey(flagName) ? _flags[flagName] : false;
        }
        
        /// <summary>
        /// 设置布尔旗标
        /// </summary>
        public static void SetFlag(string flagName, bool value)
        {
            if (string.IsNullOrEmpty(flagName)) return;
            
            bool previousValue = GetFlag(flagName);
            _flags[flagName] = value;
            
            if (EventBus.IsInitialized)
            {
                EventBus.Publish(new FlagChangedEvent(flagName, value, previousValue));
            }
            
            Debug.Log($"[FlagService] Flag '{flagName}' set to {value}");
        }
        
        #endregion
        
        #region String Variables
        
        /// <summary>
        /// 获取字符串变量（默认空字符串）
        /// </summary>
        public static string GetVar(string varName)
        {
            if (string.IsNullOrEmpty(varName)) return "";
            return _vars.ContainsKey(varName) ? _vars[varName] : "";
        }
        
        /// <summary>
        /// 设置字符串变量
        /// </summary>
        public static void SetVar(string varName, string value)
        {
            if (string.IsNullOrEmpty(varName)) return;
            
            string previousValue = GetVar(varName);
            _vars[varName] = value ?? "";
            
            if (EventBus.IsInitialized)
            {
                EventBus.Publish(new VarChangedEvent(varName, value, previousValue));
            }
            
            // 特殊变量：held 变化时刷新交互提示
            if (varName == "held")
            {
                RefreshInteractionPrompts();
            }
            
            Debug.Log($"[FlagService] Var '{varName}' set to '{value}'");
        }
        
        /// <summary>
        /// 获取整数变量（如 day）
        /// </summary>
        public static int GetInt(string varName)
        {
            string value = GetVar(varName);
            return int.TryParse(value, out int result) ? result : 0;
        }
        
        /// <summary>
        /// 设置整数变量
        /// </summary>
        public static void SetInt(string varName, int value)
        {
            SetVar(varName, value.ToString());
        }
        
        #endregion
        
        #region Shortcuts
        
        /// <summary>
        /// 获取当前手持物品
        /// </summary>
        public static string GetHeldItem()
        {
            return GetVar("held");
        }
        
        /// <summary>
        /// 设置当前手持物品
        /// </summary>
        public static void SetHeldItem(string itemId)
        {
            SetVar("held", itemId);
        }
        
        /// <summary>
        /// 清空手持物品
        /// </summary>
        public static void ClearHeldItem()
        {
            SetVar("held", "");
        }
        
        /// <summary>
        /// 获取抽屉物品
        /// </summary>
        public static string GetSlot(int slotIndex)
        {
            return GetVar($"slot.{slotIndex}");
        }
        
        /// <summary>
        /// 设置抽屉物品
        /// </summary>
        public static void SetSlot(int slotIndex, string itemId)
        {
            SetVar($"slot.{slotIndex}", itemId);
        }
        
        /// <summary>
        /// 获取当前天数
        /// </summary>
        public static int GetDay()
        {
            return GetInt("day");
        }
        
        /// <summary>
        /// 设置当前天数
        /// </summary>
        public static void SetDay(int day)
        {
            SetInt("day", day);
        }
        
        #endregion
        
        #region Utility
        
        /// <summary>
        /// 清空所有旗标和变量
        /// </summary>
        public static void ClearAllFlags()
        {
            _flags.Clear();
            _vars.Clear();
            Debug.Log("[FlagService] All flags and vars cleared");
        }
        
        /// <summary>
        /// 刷新交互提示（当 held 变化时）
        /// </summary>
        private static void RefreshInteractionPrompts()
        {
            var interactor = UnityEngine.Object.FindObjectOfType<SpookyGame.World.Interactor>();
            if (interactor != null)
            {
                interactor.RefreshPrompt();
            }
        }
        
        /// <summary>
        /// 调试：打印所有旗标和变量
        /// </summary>
        public static void DebugPrintAll()
        {
            Debug.Log("=== Flags ===");
            foreach (var kvp in _flags)
            {
                Debug.Log($"  {kvp.Key} = {kvp.Value}");
            }
            
            Debug.Log("=== Vars ===");
            foreach (var kvp in _vars)
            {
                Debug.Log($"  {kvp.Key} = '{kvp.Value}'");
            }
        }
        
        #endregion
    }
    
    /// <summary>
    /// 旗标变化事件
    /// </summary>
    public class FlagChangedEvent : IEvent
    {
        public string FlagName { get; }
        public bool NewValue { get; }
        public bool OldValue { get; }
        
        public FlagChangedEvent(string flagName, bool newValue, bool oldValue)
        {
            FlagName = flagName;
            NewValue = newValue;
            OldValue = oldValue;
        }
    }
    
    /// <summary>
    /// 变量变化事件
    /// </summary>
    public class VarChangedEvent : IEvent
    {
        public string VarName { get; }
        public string NewValue { get; }
        public string OldValue { get; }
        
        public VarChangedEvent(string varName, string newValue, string oldValue)
        {
            VarName = varName;
            NewValue = newValue;
            OldValue = oldValue;
        }
    }
}
