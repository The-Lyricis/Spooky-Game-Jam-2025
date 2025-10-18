using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SpookyGame.Core
{
    /// <summary>
    /// 游戏全局配置（ScriptableObject）
    /// 集中管理所有场景名、关卡ID等配置
    /// </summary>
    [CreateAssetMenu(fileName = "GameConfig", menuName = "SpookyGame/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Scene References")]
        [Tooltip("主菜单场景文件")]
        #if UNITY_EDITOR
        [SerializeField] private SceneAsset mainMenuSceneAsset;
        #endif
        
        [Tooltip("游戏场景文件")]
        #if UNITY_EDITOR
        [SerializeField] private SceneAsset gameSceneAsset;
        #endif
        
        [Header("Scene Names (Auto-generated)")]
        [Tooltip("主菜单场景名称（自动从 SceneAsset 获取）")]
        [SerializeField] private string mainMenuSceneName = "MainMenu";
        
        [Tooltip("游戏场景名称（自动从 SceneAsset 获取）")]
        [SerializeField] private string gameSceneName = "GameScene";
        
        /// <summary>
        /// 获取主菜单场景名称
        /// </summary>
        public string MainMenuSceneName
        {
            get
            {
                #if UNITY_EDITOR
                if (mainMenuSceneAsset != null)
                {
                    return mainMenuSceneAsset.name;
                }
                #endif
                return mainMenuSceneName;
            }
        }
        
        /// <summary>
        /// 获取游戏场景名称
        /// </summary>
        public string GameSceneName
        {
            get
            {
                #if UNITY_EDITOR
                if (gameSceneAsset != null)
                {
                    return gameSceneAsset.name;
                }
                #endif
                return gameSceneName;
            }
        }
        
        [Header("Stage Configuration")]
        [Tooltip("首个关卡ID")]
        public string firstStageId = "D1";
        
        [Tooltip("所有关卡配置")]
        public StageConfig[] stages = new StageConfig[]
        {
            new StageConfig { stageId = "D1", displayName = "DAY 1", intertitle = "DAY 1 · 第一天" },
            new StageConfig { stageId = "D2", displayName = "DAY 2", intertitle = "DAY 2 · 第二天" },
            new StageConfig { stageId = "D3", displayName = "DAY 3", intertitle = "DAY 3 · 第三天" },
            new StageConfig { stageId = "D4", displayName = "DAY 4", intertitle = "DAY 4 · 最终日" },
        };
        
        [Header("Transition Settings")]
        [Tooltip("默认转场时长")]
        public float defaultFadeDuration = 1f;
        
        [Tooltip("标题显示时长")]
        public float intertitleDuration = 2f;
        
        [Header("Game Settings")]
        [Tooltip("是否使用主菜单")]
        public bool useMainMenu = true;
        
        [Tooltip("是否在启动时显示首个关卡标题")]
        public bool showFirstStageTitle = true;
        
        /// <summary>
        /// 获取关卡配置
        /// </summary>
        public StageConfig GetStageConfig(string stageId)
        {
            foreach (var stage in stages)
            {
                if (stage.stageId == stageId)
                {
                    return stage;
                }
            }
            
            Debug.LogWarning($"[GameConfig] Stage {stageId} not found in config");
            return new StageConfig { stageId = stageId, displayName = stageId, intertitle = stageId };
        }
        
        /// <summary>
        /// 获取下一个关卡ID
        /// </summary>
        public string GetNextStageId(string currentStageId)
        {
            for (int i = 0; i < stages.Length - 1; i++)
            {
                if (stages[i].stageId == currentStageId)
                {
                    return stages[i + 1].stageId;
                }
            }
            
            return null; // 没有下一关了
        }
        
        /// <summary>
        /// 检查是否是最后一关
        /// </summary>
        public bool IsLastStage(string stageId)
        {
            return stages.Length > 0 && stages[stages.Length - 1].stageId == stageId;
        }
    }
    
    /// <summary>
    /// 关卡配置
    /// </summary>
    [System.Serializable]
    public class StageConfig
    {
        [Tooltip("关卡ID（如 D1, D2）")]
        public string stageId;
        
        [Tooltip("显示名称")]
        public string displayName;
        
        [Tooltip("转场标题")]
        public string intertitle;
        
        [Tooltip("关卡初始旗标")]
        public FlagEntry[] initialFlags = new FlagEntry[0];
    }
    
    /// <summary>
    /// 旗标条目
    /// </summary>
    [System.Serializable]
    public class FlagEntry
    {
        public string key;
        public bool value;
    }
}

