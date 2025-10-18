using UnityEngine;

namespace SpookyGame.World.InteractionEffects
{
    /// <summary>
    /// 解锁交互物体效果
    /// 点击后解锁其他物体的交互功能
    /// </summary>
    public class UnlockInteractableEffect : BaseInteractionEffect
    {
        [Header("Unlock Settings")]
        [SerializeField] private Interactable[] interactablesToUnlock;
        [SerializeField] private bool unlockState = true;
        [SerializeField] private bool showDebugInfo = true;
        
        protected override void ExecuteImmediate(GameObject actor)
        {
            if (interactablesToUnlock == null || interactablesToUnlock.Length == 0)
            {
                Debug.LogWarning($"[UnlockInteractableEffect] {gameObject.name}: 没有设置要解锁的物体", gameObject);
                return;
            }
            
            foreach (var interactable in interactablesToUnlock)
            {
                if (interactable != null)
                {
                    interactable.SetInteractionEnabled(unlockState);
                    
                    if (showDebugInfo)
                    {
                        string action = unlockState ? "解锁" : "锁定";
                        Debug.Log($"[UnlockInteractableEffect] {action}物体: {interactable.gameObject.name}", interactable.gameObject);
                    }
                }
            }
        }
    }
}

