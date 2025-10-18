namespace SpookyGame.World
{
    /// <summary>
    /// 指针语义类型
    /// 用于 HoverHighlighter 显示不同的交互图标
    /// </summary>
    public enum CursorType
    {
        Default,   // 默认指针
        Hand,      // 手型（拿取/点击）
        Inspect,   // 放大镜（查看）
        // Door,      // 门（进入/切换）
        // Talk,      // 对话泡泡
        // Combine,   // 合成/使用
        Locked     // 锁定/禁止
    }
}

