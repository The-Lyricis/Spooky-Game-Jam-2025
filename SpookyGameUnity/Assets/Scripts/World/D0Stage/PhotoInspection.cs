using UnityEngine;
using SpookyGame.World;
using SpookyGame.UI;
using SpookyGame.Core;
using System.Collections;

namespace SpookyGame.D0Scene
{
    /// <summary>
    /// D0 场景 - 照片检视
    /// 继承自 InspectableObject，点击后显示照片特写并设置检视旗标
    /// </summary>
    public class PhotoInspection : InspectableObject
    {
        private CloseupViewUI _closeupUI;
        
        protected override void Awake()
        {
            base.Awake();
            
            // 查找 CloseupViewUI
            _closeupUI = FindObjectOfType<CloseupViewUI>();
            if (_closeupUI == null)
            {
                Debug.LogError("[PhotoInspection] CloseupViewUI not found in scene!");
            }
        }
        
        protected override void OnInteract(GameObject actor)
        {
            // 调用父类显示特写
            base.OnInteract(actor);
            
            Debug.Log("[PhotoInspection] Showing photo closeup...");
            
            // 等待特写关闭后再设置旗标
            StartCoroutine(WaitForCloseupClose());
        }
        
        private IEnumerator WaitForCloseupClose()
        {
            // 等待一帧，确保特写已经显示
            yield return null;
            
            // 等待特写关闭
            while (_closeupUI != null && _closeupUI.IsVisible)
            {
                yield return null;
            }
            
            // 特写关闭后设置旗标
            FlagService.SetFlag("inspected.photo", true);
            Debug.Log("[PhotoInspection] Closeup closed. Set flag: inspected.photo = true");
        }
    }
}

