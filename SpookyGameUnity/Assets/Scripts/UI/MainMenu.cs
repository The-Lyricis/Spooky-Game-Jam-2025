using UnityEngine;
using UnityEngine.UI;
using SpookyGame.Core;

namespace SpookyGame.UI
{
    /// <summary>
    /// 主菜单 UI
    /// </summary>
    public class MainMenu : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button startButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;
        
        [Header("Game Configuration")]
        [SerializeField] private GameConfig gameConfig;
        
        private void Start()
        {
            // 绑定按钮事件
            if (startButton != null)
            {
                startButton.onClick.AddListener(OnStartGameClicked);
            }
            
            if (continueButton != null)
            {
                continueButton.onClick.AddListener(OnContinueClicked);
                // 检查是否有存档
                // continueButton.interactable = SaveService.HasSaveData();
            }
            
            if (settingsButton != null)
            {
                settingsButton.onClick.AddListener(OnSettingsClicked);
            }
            
            if (quitButton != null)
            {
                quitButton.onClick.AddListener(OnQuitClicked);
            }
        }
        
        private void OnDestroy()
        {
            // 解绑按钮事件
            if (startButton != null)
            {
                startButton.onClick.RemoveListener(OnStartGameClicked);
            }
            
            if (continueButton != null)
            {
                continueButton.onClick.RemoveListener(OnContinueClicked);
            }
            
            if (settingsButton != null)
            {
                settingsButton.onClick.RemoveListener(OnSettingsClicked);
            }
            
            if (quitButton != null)
            {
                quitButton.onClick.RemoveListener(OnQuitClicked);
            }
        }
        
        /// <summary>
        /// 开始新游戏
        /// </summary>
        private void OnStartGameClicked()
        {
            if (gameConfig == null)
            {
                Debug.LogError("[MainMenu] GameConfig is not assigned!");
                return;
            }
            
            Debug.Log("[MainMenu] Starting new game");
            
            // 清除旧的游戏状态
            FlagService.ClearAllFlags();
            
            // 获取首关配置
            StageConfig firstStage = gameConfig.GetStageConfig(gameConfig.firstStageId);
            
            // 加载游戏场景
            SceneService.LoadScene(gameConfig.gameSceneName, () =>
            {
                // 场景加载完成后，使用转场效果启动第一关
                SceneService.FadeToStage(
                    gameConfig.firstStageId, 
                    firstStage.intertitle, 
                    gameConfig.defaultFadeDuration
                );
                
                // 切换到探索状态
                EventBus.Publish(new GameStateChangedEvent(GameState.Exploring));
            });
        }
        
        /// <summary>
        /// 继续游戏
        /// </summary>
        private void OnContinueClicked()
        {
            if (gameConfig == null)
            {
                Debug.LogError("[MainMenu] GameConfig is not assigned!");
                return;
            }
            
            Debug.Log("[MainMenu] Continuing game");
            
            // 如果你有存档系统，这里加载存档
            // SaveData saveData = SaveService.LoadGame();
            // string savedStage = saveData.currentStageId;
            
            // 加载游戏场景
            SceneService.LoadScene(gameConfig.gameSceneName, () =>
            {
                // 加载到存档的关卡
                // SceneService.ActivateStage(savedStage);
                
                // 临时：加载第一关
                SceneService.ActivateStage(gameConfig.firstStageId);
                
                EventBus.Publish(new GameStateChangedEvent(GameState.Exploring));
            });
        }
        
        /// <summary>
        /// 打开设置
        /// </summary>
        private void OnSettingsClicked()
        {
            Debug.Log("[MainMenu] Opening settings");
            // TODO: 打开设置面板
        }
        
        /// <summary>
        /// 退出游戏
        /// </summary>
        private void OnQuitClicked()
        {
            Debug.Log("[MainMenu] Quitting game");
            
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}

