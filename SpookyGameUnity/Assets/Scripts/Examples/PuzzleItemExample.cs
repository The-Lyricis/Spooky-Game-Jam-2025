using UnityEngine;
using SpookyGame.World;

namespace SpookyGame.Examples
{
    /// <summary>
    /// 解谜物品示例
    /// 展示如何使用 CompositeInteractable 和各种效果组件
    /// 
    /// 使用方法：
    /// 1. 将此脚本添加到一个2D物体上
    /// 2. 添加需要的效果组件（如 ShowMessageEffect, UnlockInteractableEffect 等）
    /// 3. 在Inspector中配置效果参数
    /// 4. 运行游戏，点击物体查看效果
    /// </summary>
    public class PuzzleItemExample : CompositeInteractable
    {
        // 这个类不需要任何额外代码
        // 所有功能都通过添加效果组件实现
        
        // 示例场景设置：
        // 【场景1：点击按钮解锁门】
        // 1. 在按钮物体上添加 CompositeInteractable
        // 2. 添加 ShowMessageEffect，设置消息为 "按钮已按下"
        // 3. 添加 PlayAnimationEffect，播放按下动画
        // 4. 添加 UnlockInteractableEffect，解锁门物体
        // 5. 添加 SetFlagEffect，设置旗标 "button_pressed" = true
        
        // 【场景2：点击钥匙改变外观并激活新物体】
        // 1. 在钥匙物体上添加 CompositeInteractable
        // 2. 添加 ShowMessageEffect，显示 "获得钥匙"
        // 3. 添加 ChangeAppearanceEffect，改变颜色为灰色（表示已拾取）
        // 4. 添加 ActivateGameObjectEffect，激活宝箱物体
        // 5. 添加 PlaySoundEffect，播放拾取音效
        
        // 【场景3：点击画框触发复杂交互】
        // 1. 添加 ShowMessageEffect（延迟0秒），显示提示文字
        // 2. 添加 PlayAnimationEffect（延迟0.5秒），播放画框晃动动画
        // 3. 添加 ActivateGameObjectEffect（延迟1秒），显示隐藏的密码
        // 4. 添加 UnlockInteractableEffect（延迟1.5秒），解锁保险柜
    }
}

