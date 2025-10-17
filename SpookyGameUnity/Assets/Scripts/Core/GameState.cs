namespace SpookyGame.Core
{
    /// <summary>
    /// 游戏状态枚举
    /// </summary>
    public enum GameState
    {
        /// <summary>
        /// 启动状态
        /// </summary>
        Boot,
        
        /// <summary>
        /// 加载状态
        /// </summary>
        Loading,
        
        /// <summary>
        /// 探索状态
        /// </summary>
        Exploring,
        
        /// <summary>
        /// 谜题状态
        /// </summary>
        InPuzzle,
        
        /// <summary>
        /// 暂停状态
        /// </summary>
        Paused
    }
    
    /// <summary>
    /// 游戏状态变化事件
    /// </summary>
    public class GameStateChangedEvent : IEvent
    {
        public GameState NewState { get; }
        public GameState PreviousState { get; }
        
        public GameStateChangedEvent(GameState newState, GameState previousState = GameState.Boot)
        {
            NewState = newState;
            PreviousState = previousState;
        }
    }
}
