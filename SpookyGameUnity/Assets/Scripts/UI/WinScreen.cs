using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SpookyGame.Core;
using SpookyGame.Input;

namespace SpookyGame.UI
{
    /// <summary>
    /// 通关界面，显示通关信息并提供返回主菜单或退出选项
    /// </summary>
    public class WinScreen : MonoBehaviour, IEventHandler<PuzzleSolvedEvent>
    {
        [Header("UI References")]
        [SerializeField] private GameObject winPanel;
        [SerializeField] private Text winTitleText;
        [SerializeField] private Text winMessageText;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private Button playAgainButton;
        
        [Header("Settings")]
        [SerializeField] private string mainMenuSceneName = "MainMenu";
        [SerializeField] private string firstLevelSceneName = "Scene_0";
        [SerializeField] private string winTitle = "恭喜通关！";
        [SerializeField] private string winMessage = "你成功解开了所有谜题！";
        
        private void Awake()
        {
            // 初始化状态
            if (winPanel != null)
            {
                winPanel.SetActive(false);
            }
        }
        
        private void Start()
        {
            // 订阅谜题解决事件
            EventBus.Subscribe<PuzzleSolvedEvent>(this);
            
            // 绑定按钮事件
            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.AddListener(GoToMainMenu);
            }
            
            if (quitButton != null)
            {
                quitButton.onClick.AddListener(QuitGame);
            }
            
            if (playAgainButton != null)
            {
                playAgainButton.onClick.AddListener(PlayAgain);
            }
        }
        
        private void OnDestroy()
        {
            // 取消订阅事件
            EventBus.Unsubscribe<PuzzleSolvedEvent>(this);
            
            // 解绑按钮事件
            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.RemoveListener(GoToMainMenu);
            }
            
            if (quitButton != null)
            {
                quitButton.onClick.RemoveListener(QuitGame);
            }
            
            if (playAgainButton != null)
            {
                playAgainButton.onClick.RemoveListener(PlayAgain);
            }
        }
        
        /// <summary>
        /// 处理谜题解决事件
        /// </summary>
        /// <param name="eventData">谜题解决事件数据</param>
        public void Handle(PuzzleSolvedEvent eventData)
        {
            // 检查是否是最后一个谜题
            if (IsLastPuzzle(eventData.PuzzleId))
            {
                ShowWinScreen();
            }
        }
        
        /// <summary>
        /// 显示通关界面
        /// </summary>
        public void ShowWinScreen()
        {
            if (winPanel != null)
            {
                winPanel.SetActive(true);
            }
            
            // 设置文本内容
            if (winTitleText != null)
            {
                winTitleText.text = winTitle;
            }
            
            if (winMessageText != null)
            {
                winMessageText.text = winMessage;
            }
            
            // 暂停游戏
            Time.timeScale = 0f;
            
            // 禁用输入
            InputService.SetInputEnabled(false);
            
            Debug.Log("[WinScreen] Win screen shown");
        }
        
        /// <summary>
        /// 隐藏通关界面
        /// </summary>
        public void HideWinScreen()
        {
            if (winPanel != null)
            {
                winPanel.SetActive(false);
            }
            
            // 恢复游戏
            Time.timeScale = 1f;
            
            // 启用输入
            InputService.SetInputEnabled(true);
            
            Debug.Log("[WinScreen] Win screen hidden");
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
            
            Debug.Log("[WinScreen] Going to main menu");
        }
        
        /// <summary>
        /// 退出游戏
        /// </summary>
        private void QuitGame()
        {
            Debug.Log("[WinScreen] Quitting game");
            
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
        
        /// <summary>
        /// 重新开始游戏
        /// </summary>
        private void PlayAgain()
        {
            // 恢复时间缩放
            Time.timeScale = 1f;
            
            // 重新加载第一关
            SceneService.LoadScene(firstLevelSceneName);
            
            Debug.Log("[WinScreen] Playing again");
        }
        
        /// <summary>
        /// 检查是否是最后一个谜题
        /// </summary>
        /// <param name="puzzleId">谜题ID</param>
        /// <returns>是否是最后一个谜题</returns>
        private bool IsLastPuzzle(string puzzleId)
        {
            // 这里可以根据实际游戏逻辑来判断
            // 例如检查所有谜题是否都已解决
            return true; // 临时返回 true，实际应该根据游戏逻辑判断
        }
        
        /// <summary>
        /// 设置通关标题
        /// </summary>
        /// <param name="title">新的标题</param>
        public void SetWinTitle(string title)
        {
            winTitle = title;
            if (winTitleText != null)
            {
                winTitleText.text = title;
            }
        }
        
        /// <summary>
        /// 设置通关消息
        /// </summary>
        /// <param name="message">新的消息</param>
        public void SetWinMessage(string message)
        {
            winMessage = message;
            if (winMessageText != null)
            {
                winMessageText.text = message;
            }
        }
    }
}
