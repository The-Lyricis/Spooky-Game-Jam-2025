using UnityEngine;

namespace SpookyGame.World.InteractionEffects
{
    /// <summary>
    /// 激活/禁用游戏物体效果
    /// 用于显示/隐藏物体
    /// </summary>
    public class ActivateGameObjectEffect : BaseInteractionEffect
    {
        [Header("Activation Settings")]
        [SerializeField] private GameObject[] targetObjects;
        [SerializeField] private bool activateState = true;
        [SerializeField] private bool showDebugInfo = true;
        
        protected override void ExecuteImmediate(GameObject actor)
        {
            if (targetObjects == null || targetObjects.Length == 0)
            {
                Debug.LogWarning($"[ActivateGameObjectEffect] {gameObject.name}: 没有设置目标物体", gameObject);
                return;
            }
            
            foreach (var obj in targetObjects)
            {
                if (obj != null)
                {
                    obj.SetActive(activateState);
                    
                    if (showDebugInfo)
                    {
                        string action = activateState ? "激活" : "禁用";
                        Debug.Log($"[ActivateGameObjectEffect] {action}物体: {obj.name}", obj);
                    }
                }
            }
        }
    }
}

