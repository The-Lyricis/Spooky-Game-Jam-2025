using UnityEngine;
using SpookyGame.World;

namespace SpookyGame.Examples
{
    /// <summary>
    /// 点击式交互示例：点击后在控制台输出信息
    /// 适用于点击式冒险游戏
    /// </summary>
    public class Say : Interactable
    {
        [Header("Say Settings")]
        [SerializeField] private string message = "Hello World!";
        [SerializeField] private bool playOnce = false;
        
        protected override void OnInteract(GameObject actor)
        {
            Debug.Log($"<color=cyan>[Say] {gameObject.name}:</color> {message}", gameObject);
            
            // 如果设置为只播放一次，交互后禁用
            if (playOnce)
            {
                interactionPrompt = "（已查看）";
            }
        }
        
        public override bool CanInteract(GameObject actor)
        {
            // 如果设置为只播放一次且已经交互过，则不允许再次交互
            if (playOnce && HasInteracted)
            {
                return false;
            }
            
            return base.CanInteract(actor);
        }
    }
}
