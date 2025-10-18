using UnityEngine;
using SpookyGame.UI;
using SpookyGame.Core;

namespace SpookyGame.D0Scene
{
    /// <summary>
    /// D0 场景 - 窗户检视
    /// 点击后显示窗户特写，并设置检视旗标
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class WindowInspection : MonoBehaviour
    {
        [Header("Window Settings")]
        [Tooltip("窗户特写图")]
        [SerializeField] private Sprite windowCloseup;
        
        [Header("UI Reference")]
        [SerializeField] private CloseupViewUI closeupUI;
        [SerializeField] private bool autoFindUI = true;
        
        private void Start()
        {
            // 自动查找特写 UI
            if (autoFindUI && closeupUI == null)
            {
                closeupUI = FindObjectOfType<CloseupViewUI>();
                if (closeupUI == null)
                {
                    Debug.LogError("[WindowInspection] CloseupViewUI not found in scene!");
                }
            }
        }
        
        private void OnMouseDown()
        {
            // 检查是否已经检视过
            if (FlagService.GetFlag("inspected.window"))
            {
                Debug.Log("[WindowInspection] Already inspected.");
                // 仍然可以再次查看
            }
            
            // 显示特写
            if (closeupUI != null && windowCloseup != null)
            {
                closeupUI.ShowCloseup(windowCloseup);
                Debug.Log("[WindowInspection] Showing window closeup.");
            }
            
            // 设置检视旗标
            FlagService.SetFlag("inspected.window", true);
            Debug.Log("[WindowInspection] Set flag: inspected.window = true");
        }
    }
}

