using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpookyGame.Core
{
    /// <summary>
    /// 场景服务，负责关卡加载与过渡
    /// </summary>
    public static class SceneService
    {
        private static bool _isInitialized = false;
        private static string _currentSceneName;
        
        /// <summary>
        /// 当前场景名称
        /// </summary>
        public static string CurrentSceneName => _currentSceneName;
        
        /// <summary>
        /// 初始化场景服务
        /// </summary>
        public static void Initialize()
        {
            if (_isInitialized)
            {
                Debug.LogWarning("[SceneService] Already initialized");
                return;
            }
            
            _currentSceneName = SceneManager.GetActiveScene().name;
            _isInitialized = true;
            Debug.Log($"[SceneService] Initialized with current scene: {_currentSceneName}");
        }
        
        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="onComplete">加载完成回调</param>
        public static void LoadScene(string sceneName, Action onComplete = null)
        {
            if (!_isInitialized)
            {
                Debug.LogError("[SceneService] Not initialized. Call Initialize() first.");
                return;
            }
            
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("[SceneService] Scene name is null or empty");
                return;
            }
            
            Debug.Log($"[SceneService] Loading scene: {sceneName}");
            
            // 发布场景开始加载事件
            EventBus.Publish(new SceneLoadStartedEvent(sceneName));
            
            // 异步加载场景
            var operation = SceneManager.LoadSceneAsync(sceneName);
            operation.completed += (op) =>
            {
                _currentSceneName = sceneName;
                Debug.Log($"[SceneService] Scene loaded: {sceneName}");
                
                // 发布场景加载完成事件
                EventBus.Publish(new SceneLoadCompletedEvent(sceneName));
                
                // 执行完成回调
                onComplete?.Invoke();
            };
        }
        
        /// <summary>
        /// 重新加载当前场景
        /// </summary>
        /// <param name="onComplete">加载完成回调</param>
        public static void ReloadCurrentScene(Action onComplete = null)
        {
            if (string.IsNullOrEmpty(_currentSceneName))
            {
                Debug.LogError("[SceneService] No current scene to reload");
                return;
            }
            
            LoadScene(_currentSceneName, onComplete);
        }
        
        /// <summary>
        /// 检查场景是否存在
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <returns>是否存在</returns>
        public static bool SceneExists(string sceneName)
        {
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                string sceneNameFromPath = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                
                if (sceneNameFromPath == sceneName)
                {
                    return true;
                }
            }
            
            return false;
        }
    }
    
    /// <summary>
    /// 场景开始加载事件
    /// </summary>
    public class SceneLoadStartedEvent : IEvent
    {
        public string SceneName { get; }
        
        public SceneLoadStartedEvent(string sceneName)
        {
            SceneName = sceneName;
        }
    }
    
    /// <summary>
    /// 场景加载完成事件
    /// </summary>
    public class SceneLoadCompletedEvent : IEvent
    {
        public string SceneName { get; }
        
        public SceneLoadCompletedEvent(string sceneName)
        {
            SceneName = sceneName;
        }
    }
}
