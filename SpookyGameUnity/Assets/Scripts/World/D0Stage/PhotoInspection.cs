using UnityEngine;
using SpookyGame.World;
using SpookyGame.UI;
using SpookyGame.Core;
using System.Collections;

namespace SpookyGame.D0Scene
{
    /// <summary>
    /// D0 场景 - 照片检视
    /// 点击后显示照片特写，同时显示对话，对话完成后可点击退出特写
    /// </summary>
    public class PhotoInspection : InspectableObject
    {
        [Header("Dialogue Settings")]
        [Tooltip("对话系统引用")]
        [SerializeField] private DialogueSystem dialogueSystem;
        
        [Tooltip("自动查找对话系统")]
        [SerializeField] private bool autoFindDialogueSystem = true;
        
        [Tooltip("检视时的对话内容")]
        [TextArea(2, 5)]
        [SerializeField] private string[] dialogueLines = new string[]
        {
            "Faces...I see."
        };
        
        protected override void Awake()
        {
            base.Awake();
            
            if (autoFindDialogueSystem && dialogueSystem == null)
            {
                dialogueSystem = FindObjectOfType<DialogueSystem>();
            }
        }
        
        protected override void OnInteract(GameObject actor)
        {
            base.OnInteract(actor);
            
            if (dialogueSystem != null && dialogueLines != null && dialogueLines.Length > 0)
            {
                StartCoroutine(StartDialogueAfterFrame());
            }
            
            StartCoroutine(WaitForCloseupClose());
        }
        
        private IEnumerator StartDialogueAfterFrame()
        {
            // 等待鼠标释放，避免点击照片的操作被对话系统检测到
            while (UnityEngine.Input.GetMouseButton(0))
            {
                yield return null;
            }
            
            // 再等待一帧确保状态稳定
            yield return null;
            
            dialogueSystem.StartDialogue(dialogueLines, OnDialogueComplete);
        }
        
        private IEnumerator WaitForCloseupClose()
        {
            yield return null;
            
            while (closeupUI != null && closeupUI.IsVisible)
            {
                yield return null;
            }
            
            FlagService.SetFlag("inspected.photo", true);
        }
        
        private void OnDialogueComplete()
        {
            // 对话完成后不做任何事，等待用户点击照片外退出特写
        }
    }
}

