using UnityEngine;

namespace SpookyGame.World.InteractionEffects
{
    /// <summary>
    /// 交互效果接口
    /// 所有交互效果都需要实现此接口
    /// </summary>
    public interface IInteractionEffect
    {
        /// <summary>
        /// 执行效果
        /// </summary>
        /// <param name="actor">交互者</param>
        void Execute(GameObject actor);
        
        /// <summary>
        /// 效果是否可以执行（用于条件判断）
        /// </summary>
        /// <param name="actor">交互者</param>
        /// <returns>是否可以执行</returns>
        bool CanExecute(GameObject actor);
    }
}

