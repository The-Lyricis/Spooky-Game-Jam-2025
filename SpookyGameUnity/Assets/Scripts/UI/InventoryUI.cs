using UnityEngine;
using UnityEngine.UI;
using SpookyGame.Core;

namespace SpookyGame.UI
{
    /// <summary>
    /// 物品栏 UI - 显示当前持有的工具图标
    /// </summary>
    public class InventoryUI : MonoBehaviour, IEventHandler<VarChangedEvent>
    {
        [Header("UI References")]
        [Tooltip("工具图标的 Image 组件")]
        [SerializeField] private Image toolIconImage;
        
        [Tooltip("空槽位时显示的占位图（可选）")]
        [SerializeField] private Sprite emptySlotSprite;
        
        [Header("Tool Icons")]
        [Tooltip("剃刀图标")]
        [SerializeField] private Sprite razorIcon;
        [SerializeField] private Sprite BirdIcon;
        [SerializeField] private Sprite BirdSkullIcon;
        [SerializeField] private Sprite TeethIcon;
        [SerializeField] private Sprite HairsIcon;
        [SerializeField] private Sprite ScissorIcon;
        [SerializeField] private Sprite chuchou;
        
        // 未来可以添加更多工具图标
        // [SerializeField] private Sprite scissorsIcon;
        // [SerializeField] private Sprite keyIcon;
        
        [Header("Settings")]
        [Tooltip("空槽位时是否隐藏图标")]
        [SerializeField] private bool hideWhenEmpty = true;
        
        private void Start()
        {
            // 订阅变量变化事件
            EventBus.Subscribe<VarChangedEvent>(this);
            
            // 初始化显示
            RefreshDisplay();
        }
        
        private void OnDestroy()
        {
            // 取消订阅
            EventBus.Unsubscribe<VarChangedEvent>(this);
        }
        
        /// <summary>
        /// 处理变量变化事件
        /// </summary>
        public void Handle(VarChangedEvent eventData)
        {
            // 只关心 "held" 变量的变化
            if (eventData.VarName == "held")
            {
                RefreshDisplay();
            }
        }
        
        /// <summary>
        /// 刷新显示
        /// </summary>
        private void RefreshDisplay()
        {
            if (toolIconImage == null)
            {
                Debug.LogWarning("[InventoryUI] Tool Icon Image is not assigned!");
                return;
            }
            
            // 获取当前持有的工具
            string heldTool = FlagService.GetHeldItem();
            
            if (string.IsNullOrEmpty(heldTool))
            {
                // 空槽位
                if (hideWhenEmpty)
                {
                    toolIconImage.enabled = false;
                }
                else if (emptySlotSprite != null)
                {
                    toolIconImage.enabled = true;
                    toolIconImage.sprite = emptySlotSprite;
                }
                else
                {
                    toolIconImage.enabled = false;
                }
                
                Debug.Log("[InventoryUI] Slot is empty");
            }
            else
            {
                // 根据工具 ID 显示对应图标
                Sprite icon = GetToolIcon(heldTool);
                
                if (icon != null)
                {
                    toolIconImage.enabled = true;
                    toolIconImage.sprite = icon;
                    Debug.Log($"[InventoryUI] Displaying icon for: {heldTool}");
                }
                else
                {
                    Debug.LogWarning($"[InventoryUI] No icon found for tool: {heldTool}");
                    toolIconImage.enabled = false;
                }
            }
        }
        
        /// <summary>
        /// 根据工具 ID 获取对应的图标
        /// </summary>
        private Sprite GetToolIcon(string toolId)
        {
            switch (toolId)
            {
                case "razor":
                    return razorIcon;
                case "birdSkull":
                    return BirdSkullIcon;
                case "bird":
                    return BirdIcon;
                case "tooth":
                    return TeethIcon;
                case "Hairs":
                    return HairsIcon;
                case "scissors":
                    return ScissorIcon;
                case "chuchou":
                    return chuchou;
                // 未来添加更多工具
                // case "scissors":
                //     return scissorsIcon;
                // case "key":
                //     return keyIcon;
                
                default:
                    return null;
            }
        }
        
        /// <summary>
        /// 手动刷新显示（用于调试）
        /// </summary>
        [ContextMenu("Refresh Display")]
        public void ForceRefresh()
        {
            RefreshDisplay();
        }
    }
}


