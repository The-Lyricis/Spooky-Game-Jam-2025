using UnityEngine;

namespace SpookyGame.Core
{
    /// <summary>
    /// 游戏全局配置（ScriptableObject）
    /// 集中管理所有场景名、关卡ID等配置
    /// </summary>
    [CreateAssetMenu(fileName = "GameConfig", menuName = "SpookyGame/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Scene Names")]
        [Tooltip("主菜单场景名称")]
        public string mainMenuSceneName = "MainMenu";
        
        [Tooltip("游戏场景名称")]
        public string gameSceneName = "GameScene";
        
        [Header("Stage Configuration")]
        [Tooltip("首个关卡ID")]
        public string firstStageId = "D1";
        
        [Tooltip("所有关卡配置")]
        public StageConfig[] stages = new StageConfig[]
        {
            new StageConfig { stageId = "D1", displayName = "DAY 1" },
            new StageConfig { stageId = "D2", displayName = "DAY 2" },
            new StageConfig { stageId = "D3", displayName = "DAY 3" },
            new StageConfig { stageId = "D4", displayName = "DAY 4" },
        };
        
        [Header("Game Settings")]
        [Tooltip("是否使用主菜单")]
        public bool useMainMenu = true;
        
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
            return new StageConfig { stageId = stageId, displayName = stageId };
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

