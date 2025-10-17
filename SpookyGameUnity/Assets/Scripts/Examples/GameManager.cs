using UnityEngine;
using SpookyGame.Core;
using SpookyGame.Input;

namespace SpookyGame.Examples
{
    /// <summary>
    /// 游戏管理器，负责更新输入服务和处理游戏状态
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        private void Start()
        {
            // 确保输入服务已初始化
            if (!InputService.IsInitialized)
            {
                Debug.LogError("[GameManager] InputService not initialized!");
                return;
            }
        }
        
        private void Update()
        {
            // 更新输入服务
            InputService.Update();
        }
        
        private void OnApplicationPause(bool pauseStatus)
        {
            // 检查 EventBus 是否已初始化
            if (!EventBus.IsInitialized) return;
            
            // 处理应用暂停
            if (pauseStatus)
            {
                EventBus.Publish(new GameStateChangedEvent(GameState.Paused, GameState.Exploring));
            }
            else
            {
                EventBus.Publish(new GameStateChangedEvent(GameState.Exploring, GameState.Paused));
            }
        }
        
        private void OnApplicationFocus(bool hasFocus)
        {
            // 检查 EventBus 是否已初始化
            if (!EventBus.IsInitialized) return;
            
            // 处理应用失去焦点
            if (!hasFocus)
            {
                EventBus.Publish(new GameStateChangedEvent(GameState.Paused, GameState.Exploring));
            }
            else
            {
                EventBus.Publish(new GameStateChangedEvent(GameState.Exploring, GameState.Paused));
            }
        }
    }
}
