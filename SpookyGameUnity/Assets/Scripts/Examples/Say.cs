using UnityEngine;

namespace SpookyGame.World
{
    /// <summary>
    /// 可交互对象示例：点击后在控制台输出信息
    /// </summary>
    public class Say : Interactable
    {
        [Header("Say Settings")]
        [SerializeField] private string message = "Hello World!";
        
        protected override void OnInteract(GameObject actor)
        {
            Debug.Log($"[Say] {gameObject.name} says: {message}");
        }
    }
}