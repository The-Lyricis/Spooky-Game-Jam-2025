using UnityEngine;
using SpookyGame.UI;
using SpookyGame.Core;

namespace SpookyGame.D0Scene
{
    /// <summary>
    /// D0 场景 - 照片检视
    /// 点击后显示照片特写，并设置检视旗标
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class PhotoInspection : MonoBehaviour
    {
        [Header("Photo Settings")]
        [Tooltip("照片特写图")]
        [SerializeField] private Sprite photoCloseup;
        
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
                    Debug.LogError("[PhotoInspection] CloseupViewUI not found in scene!");
                }
            }
        }
        
        private void OnMouseDown()
        {
            // 检查是否已经检视过
            if (FlagService.GetFlag("inspected.photo"))
            {
                Debug.Log("[PhotoInspection] Already inspected.");
                // 仍然可以再次查看
            }
            
            // 显示特写
            if (closeupUI != null && photoCloseup != null)
            {
                closeupUI.ShowCloseup(photoCloseup);
                Debug.Log("[PhotoInspection] Showing photo closeup.");
            }
            
            // 设置检视旗标
            FlagService.SetFlag("inspected.photo", true);
            Debug.Log("[PhotoInspection] Set flag: inspected.photo = true");
        }
    }
}

