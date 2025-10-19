using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpookyGame.Core
{
    /// <summary>
    /// 场景服务，负责场景加载与单场景多关卡切换
    /// </summary>
    public static class SceneService
    {
        private static bool _isInitialized = false;
        private static string _currentSceneName;
        private static string _currentStageId;
        private static Dictionary<string, StageEntry> _stages = new Dictionary<string, StageEntry>();
        private static MonoBehaviour _coroutineRunner;
        
        /// <summary>
        /// 当前场景名称
        /// </summary>
        public static string CurrentSceneName => _currentSceneName;
        
        /// <summary>
        /// 当前关卡 ID
        /// </summary>
        public static string CurrentStageId => _currentStageId;
        
        /// <summary>
        /// 初始化场景服务
        /// </summary>
        /// <param name="coroutineRunner">用于运行协程的 MonoBehaviour（通常是 Bootstrap）</param>
        public static void Initialize(MonoBehaviour coroutineRunner = null)
        {
            if (_isInitialized)
            {
                Debug.LogWarning("[SceneService] Already initialized");
                return;
            }
            
            _currentSceneName = SceneManager.GetActiveScene().name;
            _coroutineRunner = coroutineRunner;
            
            // 自动发现场景中的所有 Stage
            DiscoverStages();
            
            _isInitialized = true;
            Debug.Log($"[SceneService] Initialized with current scene: {_currentSceneName}, found {_stages.Count} stages");
        }
        
        /// <summary>
        /// 自动发现场景中的所有 Stage 根节点
        /// </summary>
        private static void DiscoverStages()
        {
            _stages.Clear();
            
            // 查找所有以 "Stage_" 开头的根节点
            GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
            
            foreach (GameObject obj in rootObjects)
            {
                if (obj.name.StartsWith("Stage_"))
                {
                    string stageId = obj.name.Replace("Stage_", "");
                    RegisterStage(stageId, obj);
                }
            }
        }
        
        /// <summary>
        /// 注册关卡
        /// </summary>
        /// <param name="stageId">关卡 ID（如 D1, D2）</param>
        /// <param name="stageRoot">关卡根节点</param>
        /// <param name="initialFlags">进入时设置的旗标</param>
        public static void RegisterStage(string stageId, GameObject stageRoot, Dictionary<string, bool> initialFlags = null)
        {
            if (_stages.ContainsKey(stageId))
            {
                Debug.LogWarning($"[SceneService] Stage {stageId} already registered, overwriting");
            }
            
            _stages[stageId] = new StageEntry
            {
                stageId = stageId,
                stageRoot = stageRoot,
                initialFlags = initialFlags ?? new Dictionary<string, bool>()
            };
            
            Debug.Log($"[SceneService] Registered stage: {stageId}");
        }
        
        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="onComplete">加载完成回调</param>
        /// <param name="transitionText">过渡文字（可选）</param>
        public static void LoadScene(string sceneName, Action onComplete = null, string transitionText = null)
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
            
            // 如果有过渡文字，使用协程处理
            if (!string.IsNullOrEmpty(transitionText))
            {
                if (_coroutineRunner == null)
                {
                    Debug.LogWarning("[SceneService] No coroutine runner, loading scene without transition text");
                    LoadSceneInternal(sceneName, onComplete);
                }
                else
                {
                    _coroutineRunner.StartCoroutine(LoadSceneWithTransitionCoroutine(sceneName, transitionText, onComplete));
                }
            }
            else
            {
                LoadSceneInternal(sceneName, onComplete);
            }
        }
        
        /// <summary>
        /// 内部场景加载方法
        /// </summary>
        private static void LoadSceneInternal(string sceneName, Action onComplete = null)
        {
            Debug.Log($"[SceneService] Loading scene: {sceneName}");
            
            // 发布场景开始加载事件
            EventBus.Publish(new SceneLoadStartedEvent(sceneName));
            
            // 异步加载场景
            var operation = SceneManager.LoadSceneAsync(sceneName);
            operation.completed += (op) =>
            {
                _currentSceneName = sceneName;
                Debug.Log($"[SceneService] Scene loaded: {sceneName}");
                
                // 重新发现新场景中的 Stage 节点
                DiscoverStages();
                
                // 发布场景加载完成事件
                EventBus.Publish(new SceneLoadCompletedEvent(sceneName));
                
                // 执行完成回调
                onComplete?.Invoke();
            };
        }
        
        /// <summary>
        /// 带过渡文字的场景加载协程
        /// </summary>
        private static IEnumerator LoadSceneWithTransitionCoroutine(string sceneName, string transitionText, Action onComplete)
        {
            Debug.Log($"[SceneService] Loading scene with transition text: {sceneName}");
            
            bool transitionComplete = false;
            
            // 显示过渡文字
            EventBus.Publish(new TransitionTextEvent(transitionText, true, () =>
            {
                transitionComplete = true;
            }));
            
            // 等待过渡文字淡入完成后开始加载场景
            yield return new WaitForSeconds(0.5f); // 等待过渡UI淡入
            
            // 在后台加载场景
            Debug.Log($"[SceneService] Starting scene load: {sceneName}");
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName);
            loadOperation.allowSceneActivation = false; // 先不激活场景
            
            // 等待场景加载完成（但不激活）
            while (loadOperation.progress < 0.9f)
            {
                yield return null;
            }
            
            Debug.Log($"[SceneService] Scene loaded (waiting for player click): {sceneName}");
            
            // 等待玩家点击完成
            yield return new WaitUntil(() => transitionComplete);
            
            // 激活场景
            loadOperation.allowSceneActivation = true;
            
            // 等待场景真正激活
            yield return new WaitUntil(() => loadOperation.isDone);
            
            _currentSceneName = sceneName;
            Debug.Log($"[SceneService] Scene activated: {sceneName}");
            
            // 重新发现新场景中的 Stage 节点
            DiscoverStages();
            
            // 发布场景加载完成事件
            EventBus.Publish(new SceneLoadCompletedEvent(sceneName));
            
            // 执行完成回调
            onComplete?.Invoke();
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
        
        /// <summary>
        /// 激活指定关卡（无转场动画，直接切换）
        /// </summary>
        /// <param name="stageId">关卡 ID</param>
        public static void ActivateStage(string stageId)
        {
            if (!_isInitialized)
            {
                Debug.LogError("[SceneService] Not initialized. Call Initialize() first.");
                return;
            }
            
            if (!_stages.ContainsKey(stageId))
            {
                Debug.LogError($"[SceneService] Stage {stageId} not found!");
                return;
            }
            
            // 清空手持物品和鼠标指针（新关卡重新开始）
            ClearStageState();
            
            // 关闭所有关卡
            foreach (var stage in _stages.Values)
            {
                if (stage.stageRoot != null)
                {
                    stage.stageRoot.SetActive(false);
                }
            }
            
            // 激活目标关卡
            StageEntry targetStage = _stages[stageId];
            if (targetStage.stageRoot != null)
            {
                targetStage.stageRoot.SetActive(true);
            }
            
            // 设置初始旗标
            ApplyStageFlags(targetStage);
            
            _currentStageId = stageId;
            
            Debug.Log($"[SceneService] Activated stage: {stageId}");
            
            // 发布关卡切换事件
            EventBus.Publish(new StageChangedEvent(stageId));
        }
        
        /// <summary>
        /// 使用过渡文字转场切换关卡
        /// </summary>
        /// <param name="stageId">关卡 ID</param>
        /// <param name="fadeDuration">淡入淡出时长</param>
        /// <param name="transitionTexts">过渡文字数组（可选）</param>
        public static void FadeToStage(string stageId, float fadeDuration = 1f, string[] transitionTexts = null)
        {
            if (!_isInitialized)
            {
                Debug.LogError("[SceneService] Not initialized. Call Initialize() first.");
                return;
            }

            if (_coroutineRunner == null)
            {
                Debug.LogWarning("[SceneService] No coroutine runner, falling back to direct activation");
                ActivateStage(stageId);
                return;
            }

            // 强制要求有文字（按你需求去掉无文字写法）
            if (transitionTexts == null || transitionTexts.Length == 0)
            {
                transitionTexts = new[] { "" }; // 或者给个默认标题，比如 "Day 1"
            }

            _coroutineRunner.StartCoroutine(FadeToStageCoroutine(stageId, fadeDuration, transitionTexts));
        }
        
        /// <summary>
        /// 黑幕转场协程（支持多个过渡文字）
        /// </summary>
        private static IEnumerator FadeToStageCoroutine(string stageId, float fadeDuration, string[] transitionTexts)
        {
            Debug.Log($"[SceneService] Fading to stage (text-first): {stageId}, texts: {transitionTexts.Length}");

            // 1) 先淡入到黑屏（保持老场景，不暴露新场景）
            EventBus.Publish(new FadeStartedEvent(true, fadeDuration));
            yield return new WaitForSeconds(fadeDuration);

            // 2) 黑屏上显示过渡文字，等待玩家点击
            bool transitionComplete = false;
            EventBus.Publish(new TransitionTextEvent(transitionTexts, true, () => { 
                // 在玩家点击完成时立即切换场景
                ActivateStage(stageId);
                Debug.Log($"[SceneService] Stage switched to {stageId} after transition text confirmed");
                transitionComplete = true; 
            }));
            yield return new WaitUntil(() => transitionComplete);

            // 3) TransitionTextUI 会自己处理黑幕淡出，我们不需要额外操作
            Debug.Log($"[SceneService] Fade transition to {stageId} completed");
        }
        
        /// <summary>
        /// 清空关卡状态（切换关卡时调用）
        /// </summary>
        private static void ClearStageState()
        {
            // 清空手持物品
            FlagService.ClearHeldItem();
            
            // 重置鼠标指针为系统默认
            UnityEngine.Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            
            Debug.Log("[SceneService] Stage state cleared (held item and cursor reset)");
        }
        
        /// <summary>
        /// 应用关卡初始旗标
        /// </summary>
        private static void ApplyStageFlags(StageEntry stage)
        {
            if (stage.initialFlags == null || stage.initialFlags.Count == 0)
            {
                return;
            }
            
            foreach (var kvp in stage.initialFlags)
            {
                FlagService.SetFlag(kvp.Key, kvp.Value);
            }
            
            Debug.Log($"[SceneService] Applied {stage.initialFlags.Count} initial flags for stage {stage.stageId}");
        }
        
        /// <summary>
        /// 获取所有注册的关卡 ID
        /// </summary>
        public static List<string> GetAllStageIds()
        {
            return new List<string>(_stages.Keys);
        }
        
        /// <summary>
        /// 检查关卡是否存在
        /// </summary>
        public static bool StageExists(string stageId)
        {
            return _stages.ContainsKey(stageId);
        }
    }
    
    /// <summary>
    /// 关卡条目
    /// </summary>
    public class StageEntry
    {
        public string stageId;
        public GameObject stageRoot;
        public Dictionary<string, bool> initialFlags;
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
    
    /// <summary>
    /// 关卡切换事件
    /// </summary>
    public class StageChangedEvent : IEvent
    {
        public string StageId { get; }
        
        public StageChangedEvent(string stageId)
        {
            StageId = stageId;
        }
    }
    
    /// <summary>
    /// 转场淡入淡出事件
    /// </summary>
    public class FadeStartedEvent : IEvent
    {
        public bool FadeIn { get; } // true = 淡入黑幕, false = 淡出黑幕
        public float Duration { get; }
        
        public FadeStartedEvent(bool fadeIn, float duration)
        {
            FadeIn = fadeIn;
            Duration = duration;
        }
    }
    
    /// <summary>
    /// 转场标题事件
    /// </summary>
    public class IntertitleEvent : IEvent
    {
        public string Text { get; }
        public bool Show { get; }
        
        public IntertitleEvent(string text, bool show)
        {
            Text = text;
            Show = show;
        }
    }
    
    /// <summary>
    /// 过渡文字事件
    /// </summary>
    public class TransitionTextEvent : IEvent
    {
        public string[] Texts { get; }
        public bool Show { get; }
        public System.Action OnComplete { get; }
        
        public TransitionTextEvent(string[] texts, bool show, System.Action onComplete = null)
        {
            Texts = texts;
            Show = show;
            OnComplete = onComplete;
        }
        
        // 兼容性构造函数：单个文字
        public TransitionTextEvent(string text, bool show, System.Action onComplete = null)
        {
            Texts = new string[] { text };
            Show = show;
            OnComplete = onComplete;
        }
    }
    
    /// <summary>
    /// 转场淡出完成事件
    /// </summary>
    public class TransitionFadeOutCompleteEvent : IEvent
    {
        public string StageId { get; }
        
        public TransitionFadeOutCompleteEvent(string stageId)
        {
            StageId = stageId;
        }
    }
}
