using UnityEngine;

namespace SpookyGame.Input
{
    /// <summary>
    /// 简化输入服务，只处理交互和暂停输入
    /// </summary>
    public static class InputService
    {
        private static bool _isInitialized = false;
        
        // 输入状态
        private static bool _interactPressed;
        private static bool _pausePressed;
        
        // 输入事件
        public static System.Action OnInteractPressed;
        public static System.Action OnPausePressed;
        
        /// <summary>
        /// 是否按下交互键
        /// </summary>
        public static bool InteractPressed => _interactPressed;
        
        /// <summary>
        /// 是否按下暂停键
        /// </summary>
        public static bool PausePressed => _pausePressed;
        
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
            Debug.Log("[InputService] Initialized");
        }
        
        /// <summary>
        /// 更新输入状态（每帧调用）
        /// </summary>
        public static void Update()
        {
            if (!_isInitialized) return;
            
            // 检查交互输入 (E 键)
            if (UnityEngine.Input.GetKeyDown(KeyCode.E))
            {
                _interactPressed = true;
                OnInteractPressed?.Invoke();
            }
            else
            {
                _interactPressed = false;
            }
            
            // 检查暂停输入 (ESC 键) - 保留接口但不实现
            _pausePressed = false;
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
