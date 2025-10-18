using UnityEngine;

namespace SpookyGame.World.InteractionEffects
{
    /// <summary>
    /// 链式反应效果
    /// 点击一个物体后，触发另一个物体的交互
    /// 用于创建连锁反应和多米诺骨牌效果
    /// </summary>
    public class ChainReactionEffect : BaseInteractionEffect
    {
        [Header("Chain Settings")]
        [SerializeField] private CompositeInteractable[] targetInteractables;
        [SerializeField] private bool triggerSequentially = false;
        [SerializeField] private float sequenceDelay = 0.5f;
        [SerializeField] private bool showDebugInfo = true;
        
        protected override void ExecuteImmediate(GameObject actor)
        {
            if (targetInteractables == null || targetInteractables.Length == 0)
            {
                Debug.LogWarning($"[ChainReactionEffect] {gameObject.name}: 没有设置目标物体", gameObject);
                return;
            }
            
            if (triggerSequentially)
            {
                StartCoroutine(TriggerSequentially(actor));
            }
            else
            {
                TriggerAll(actor);
            }
        }
        
        private void TriggerAll(GameObject actor)
        {
            foreach (var interactable in targetInteractables)
            {
                if (interactable != null)
                {
                    interactable.Interact(actor);
                    
                    if (showDebugInfo)
                    {
                        Debug.Log($"[ChainReactionEffect] 触发: {interactable.gameObject.name}", interactable.gameObject);
                    }
                }
            }
        }
        
        private System.Collections.IEnumerator TriggerSequentially(GameObject actor)
        {
            foreach (var interactable in targetInteractables)
            {
                if (interactable != null)
                {
                    interactable.Interact(actor);
                    
                    if (showDebugInfo)
                    {
                        Debug.Log($"[ChainReactionEffect] 顺序触发: {interactable.gameObject.name}", interactable.gameObject);
                    }
                    
                    yield return new UnityEngine.WaitForSeconds(sequenceDelay);
                }
            }
        }
    }
}

