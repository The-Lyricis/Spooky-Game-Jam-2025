using UnityEngine;
using SpookyGame.Core;

namespace SpookyGame.World.InteractionEffects
{
    /// <summary>
    /// 设置游戏旗标效果
    /// 用于标记游戏进度、解谜状态等
    /// </summary>
    public class SetFlagEffect : BaseInteractionEffect
    {
        [Header("Flag Settings")]
        [SerializeField] private string flagKey = "";
        [SerializeField] private bool flagValue = true;
        [SerializeField] private bool showDebugInfo = true;
        
        protected override void ExecuteImmediate(GameObject actor)
        {
            if (string.IsNullOrEmpty(flagKey))
            {
                Debug.LogWarning($"[SetFlagEffect] {gameObject.name}: 旗标键值为空", gameObject);
                return;
            }
            
            FlagService.SetFlag(flagKey, flagValue);
            
            if (showDebugInfo)
            {
                Debug.Log($"[SetFlagEffect] 设置旗标: {flagKey} = {flagValue}", gameObject);
            }
        }
    }
}

