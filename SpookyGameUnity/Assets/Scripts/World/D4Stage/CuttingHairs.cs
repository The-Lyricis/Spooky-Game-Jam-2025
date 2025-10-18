using System.Collections;
using System.Collections.Generic;
using SpookyGame.Core;
using SpookyGame.World;
using UnityEngine;

public class CuttingHairs : Interactable
{
    [Header("Flower Settings")]
    [Tooltip("交互音效")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip interactSfx;
    
    [Header("Sprite Settings")]
    [Tooltip("头发的不同状态图片")]
    [SerializeField] private GameObject[] hairGameObject;
    
    private SpriteRenderer _spriteRenderer;
    private int _currentSpriteIndex = 0;
    private int Index = 0;
    
    protected override void Awake()
    {
        base.Awake();
        
        // 获取 SpriteRenderer 组件
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    
    /// <summary>
    /// 检查是否可以交互
    /// </summary>
    public override bool CanInteract(GameObject actor)
    {
        if (!base.CanInteract(actor)) return false;
        string held = FlagService.GetHeldItem();
        if (held != "Hairs")
        {
            return false;
        }
        
        
        return true;
    }
    
    /// <summary>
    /// 与花交互 - 变换图片
    /// </summary>
    protected override void OnInteract(GameObject actor)
    {
        if(Index<=4)
        {
            ChangeSprite();
            // 播放音效
            if (audioSource != null && interactSfx != null)
            {
                audioSource.PlayOneShot(interactSfx);
            }
        }
        else
        {
            FlagService.SetHeldItem("Hairs");
        }
    }
    
    /// <summary>
    /// 切换图片
    /// </summary>
    private void ChangeSprite()
    {
        foreach (var hair in hairGameObject)
        {
            hair.SetActive(false);
        }
        hairGameObject[++Index].SetActive(true);
    }
}
