using UnityEngine;
using SpookyGame.World;
using SpookyGame.Core;

namespace SpookyGame.Examples
{
    /// <summary>
    /// 示例可交互对象，展示如何使用 Interactable 基类
    /// </summary>
    public class ExampleInteractable : Interactable
    {
        [Header("Example Settings")]
        [SerializeField] private string puzzleId = "example_puzzle";
        [SerializeField] private bool isSolved = false;
        
        protected override void OnInteract(GameObject actor)
        {
            if (isSolved)
            {
                Debug.Log($"[ExampleInteractable] Puzzle {puzzleId} is already solved!");
                return;
            }
            
            // 解决谜题
            isSolved = true;
            
            // 发布谜题解决事件
            EventBus.Publish(new PuzzleSolvedEvent(puzzleId, "Example Puzzle"));
            
            // 设置旗标
            FlagService.SetFlag($"puzzle_{puzzleId}_solved", true);
            
            // 更新交互提示
            SetInteractionPrompt("已解决");
            
            Debug.Log($"[ExampleInteractable] Puzzle {puzzleId} solved by {actor.name}!");
        }
        
        public override bool CanInteract(GameObject actor)
        {
            // 如果已经解决，不允许再次交互
            if (isSolved)
            {
                return false;
            }
            
            // 调用基类的检查逻辑
            return base.CanInteract(actor);
        }
    }
}
