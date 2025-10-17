using UnityEngine;

namespace SpookyGame.Input
{
    /// <summary>
    /// 简化输入服务，用于点击式冒险游戏
    /// </summary>
    public static class InputService
    {
        private static bool _isInitialized = false;
        
        /// <summary>
        /// 是否已初始化
        /// </summary>
        public static bool IsInitialized => _isInitialized;
        
        /// <summary>
        /// 初始化输入服务
        /// </summary>
        public static void Initialize()
        {
            if (_isInitialized)
            {
                Debug.LogWarning("[InputService] Already initialized");
                return;
            }
            
            _isInitialized = true;
            Debug.Log("[InputService] Initialized (Click-based adventure game mode)");
        }
        
        /// <summary>
        /// 更新输入状态（可选，点击式游戏可能不需要）
        /// </summary>
        public static void Update()
        {
            if (!_isInitialized) return;
            // 点击式游戏主要通过 Interactor 处理输入
        }
        
        /// <summary>
        /// 启用/禁用输入
        /// </summary>
        /// <param name="enabled">是否启用</param>
        public static void SetInputEnabled(bool enabled)
        {
            if (!_isInitialized) return;
            Debug.Log($"[InputService] Input {(enabled ? "enabled" : "disabled")}");
        }
        
        /// <summary>
        /// 清理输入服务
        /// </summary>
        public static void Cleanup()
        {
            _isInitialized = false;
            Debug.Log("[InputService] Cleaned up");
        }
    }
}
