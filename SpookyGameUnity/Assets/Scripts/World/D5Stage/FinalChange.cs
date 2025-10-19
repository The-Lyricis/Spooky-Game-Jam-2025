using System.Collections;
using System.Collections.Generic;
using SpookyGame.Core;
using SpookyGame.World;
using UnityEngine;

public class FinalChange : Interactable
{
    [Header("Flower Settings")]
    [Tooltip("交互音效")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip interactSfx;
    
    [Header("Sprite Settings")]
    [SerializeField] private GameObject[] MeGameObject;
    
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

        
        
        return true;
    }
    
    /// <summary>
    /// 与花交互 - 变换图片
    /// </summary>
    protected override void OnInteract(GameObject actor)
    {
        if(Index<MeGameObject.Length)
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
            
        }
    }
    
    /// <summary>
    /// 切换图片
    /// </summary>
    private void ChangeSprite()
    {
        Debug.Log(Index);
        foreach (var me in MeGameObject)
        {
            me.SetActive(false);
        }
        MeGameObject[Index++].SetActive(true);
    }
}
