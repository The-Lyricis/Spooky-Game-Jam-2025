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
        [Header("Game Settings")]
        [SerializeField] private string firstSceneName = "Scene_0";
        
        private void Awake()
        {
            // 确保只有一个 Bootstrap 实例
            if (FindObjectOfType<Bootstrap>() != this)
            {
                Destroy(gameObject);
                return;
            }
            
            DontDestroyOnLoad(gameObject);
        }
        
        private void Start()
        {
            InitializeServices();
            LoadFirstScene();
        }
        
        /// <summary>
        /// 初始化所有核心服务
        /// </summary>
        private void InitializeServices()
        {
            // 初始化事件总线
            EventBus.Initialize();
            
            // 初始化场景服务
            SceneService.Initialize();
            
            
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
            Debug.Log($"[Bootstrap] Loading first scene: {firstSceneName}");
            
            // 发布游戏状态变化事件
            EventBus.Publish(new GameStateChangedEvent(GameState.Loading));
            
            // 加载首关
            SceneService.LoadScene(firstSceneName, () =>
            {
                // 切换到探索状态
                EventBus.Publish(new GameStateChangedEvent(GameState.Exploring));
                
                Debug.Log("[Bootstrap] First scene loaded");
            });
        }
    }
}
