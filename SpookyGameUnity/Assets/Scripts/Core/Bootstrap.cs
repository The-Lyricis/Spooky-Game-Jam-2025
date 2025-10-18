using UnityEngine;
using UnityEngine.SceneManagement;
using SpookyGame.Input;

namespace SpookyGame.Core
{
    /// <summary>
    /// 游戏启动入口，负责初始化所有服务和加载首关
    /// </summary>
    public class Bootstrap : MonoBehaviour
    {
        [Header("Game Configuration")]
        [SerializeField] private GameConfig gameConfig;
        
        [Header("Runtime Info (Debug)")]
        [SerializeField] private bool showDebugInfo = true;
        
        private void Awake()
        {
            // 确保只有一个 Bootstrap 实例
            if (FindObjectOfType<Bootstrap>() != this)
            {
                Destroy(gameObject);
                return;
            }
            
            DontDestroyOnLoad(gameObject);
            
            // 在 Awake 中初始化服务，确保在所有 Start() 之前完成
            InitializeServices();
        }
        
        private void Start()
        {
            LoadFirstScene();
        }
        
        /// <summary>
        /// 初始化所有核心服务
        /// </summary>
        private void InitializeServices()
        {
            // 初始化事件总线
            EventBus.Initialize();
            
            // 初始化场景服务（传入 this 作为协程运行器）
            SceneService.Initialize(this);
            
            // 初始化旗标服务
            FlagService.Initialize();
            
            // 初始化输入服务
            InputService.Initialize();
            
            Debug.Log("[Bootstrap] All services initialized");
        }
        
        /// <summary>
        /// 加载首关
        /// </summary>
        private void LoadFirstScene()
        {
            if (gameConfig == null)
            {
                Debug.LogError("[Bootstrap] GameConfig is not assigned!");
                return;
            }
            
            string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            
            // 检查当前场景类型
            if (SceneService.GetAllStageIds().Count > 0)
            {
                // ========== 情况 1: 在游戏场景（有 Stage 节点）==========
                LogDebug($"In game scene, starting first stage: {gameConfig.firstStageId}");
                
                // 获取首关配置
                StageConfig firstStage = gameConfig.GetStageConfig(gameConfig.firstStageId);
                
                // 直接启动第一关（带转场效果）
                if (gameConfig.showFirstStageTitle)
                {
                    SceneService.FadeToStage(
                        gameConfig.firstStageId, 
                        firstStage.intertitle, 
                        gameConfig.defaultFadeDuration
                    );
                }
                else
                {
                    SceneService.ActivateStage(gameConfig.firstStageId);
                }
                
                EventBus.Publish(new GameStateChangedEvent(GameState.Exploring));
            }
            else if (gameConfig.useMainMenu && currentSceneName != gameConfig.mainMenuSceneName)
            {
                // ========== 情况 2: 不在主菜单，且启用了主菜单 ==========
                LogDebug($"Loading main menu: {gameConfig.mainMenuSceneName}");
                
                // 先淡入黑幕
                EventBus.Publish(new FadeStartedEvent(true, 0.5f));
                
                // 延迟加载主菜单
                StartCoroutine(LoadMainMenuDelayed(0.5f));
            }
            else
            {
                // ========== 情况 3: 已经在主菜单 ==========
                LogDebug("Already in main menu");
                
                // 淡出黑幕，显示主菜单
                EventBus.Publish(new FadeStartedEvent(false, gameConfig.defaultFadeDuration));
            }
        }
        
        /// <summary>
        /// 延迟加载主菜单
        /// </summary>
        private System.Collections.IEnumerator LoadMainMenuDelayed(float delay)
        {
            yield return new WaitForSeconds(delay);
            
            SceneService.LoadScene(gameConfig.mainMenuSceneName, () =>
            {
                // 主菜单加载完成，淡出黑幕
                EventBus.Publish(new FadeStartedEvent(false, gameConfig.defaultFadeDuration));
            });
        }
        
        /// <summary>
        /// 调试日志
        /// </summary>
        private void LogDebug(string message)
        {
            if (showDebugInfo)
            {
                Debug.Log($"[Bootstrap] {message}");
            }
        }
    }
}
