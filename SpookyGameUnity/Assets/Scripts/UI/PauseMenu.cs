using UnityEngine;
using UnityEngine.UI;
using SpookyGame.Core;
using SpookyGame.Input;

namespace SpookyGame.UI
{
    /// <summary>
    /// 暂停菜单，处理游戏暂停和恢复
    /// </summary>
    public class PauseMenu : MonoBehaviour, IEventHandler<GameStateChangedEvent>
    {
        [Header("UI References")]
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button quitButton;
        
        [Header("Settings")]
        [SerializeField] private string mainMenuSceneName = "MainMenu";
        
        private bool _isPaused = false;
        
        private void Awake()
        {
            // 初始化状态
            if (pausePanel != null)
            {
                pausePanel.SetActive(false);
            }
        }
        
        private void Start()
        {
            // 订阅游戏状态变化事件
            EventBus.Subscribe<GameStateChangedEvent>(this);
            
            // 暂停功能已移除，保留接口
            
            // 绑定按钮事件
            if (resumeButton != null)
            {
                resumeButton.onClick.AddListener(ResumeGame);
            }
            
            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.AddListener(GoToMainMenu);
            }
            
            if (quitButton != null)
            {
                quitButton.onClick.AddListener(QuitGame);
            }
        }
        
        private void OnDestroy()
        {
            // 取消订阅事件
            EventBus.Unsubscribe<GameStateChangedEvent>(this);
            
            // 暂停功能已移除，保留接口
            
            // 解绑按钮事件
            if (resumeButton != null)
            {
                resumeButton.onClick.RemoveListener(ResumeGame);
            }
            
            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.RemoveListener(GoToMainMenu);
            }
            
            if (quitButton != null)
            {
                quitButton.onClick.RemoveListener(QuitGame);
            }
        }
        
        /// <summary>
        /// 处理游戏状态变化事件
        /// </summary>
        /// <param name="eventData">游戏状态变化事件数据</param>
        public void Handle(GameStateChangedEvent eventData)
        {
            // 暂停功能已移除，保留接口
        }
        
        /// <summary>
        /// 处理暂停键输入
        /// </summary>
        private void HandlePausePressed()
        {
            // 暂停功能已移除，保留接口
        }
        
        /// <summary>
        /// 暂停游戏
        /// </summary>
        public void PauseGame()
        {
            // 暂停功能已移除，保留接口
        }
        
        /// <summary>
        /// 恢复游戏
        /// </summary>
        public void ResumeGame()
        {
            // 暂停功能已移除，保留接口
        }
        
        /// <summary>
        /// 显示暂停菜单
        /// </summary>
        private void ShowPauseMenu()
        {
            // 暂停功能已移除，保留接口
        }
        
        /// <summary>
        /// 隐藏暂停菜单
        /// </summary>
        private void HidePauseMenu()
        {
            // 暂停功能已移除，保留接口
        }
        
        /// <summary>
        /// 返回主菜单
        /// </summary>
        private void GoToMainMenu()
        {
            // 恢复时间缩放
            Time.timeScale = 1f;
            
            // 加载主菜单场景
            SceneService.LoadScene(mainMenuSceneName);
            
            Debug.Log("[PauseMenu] Going to main menu");
        }
        
        /// <summary>
        /// 退出游戏
        /// </summary>
        private void QuitGame()
        {
            Debug.Log("[PauseMenu] Quitting game");
            
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
        
        /// <summary>
        /// 检查游戏是否暂停
        /// </summary>
        /// <returns>是否暂停</returns>
        public bool IsPaused()
        {
            return _isPaused;
        }
    }
}
