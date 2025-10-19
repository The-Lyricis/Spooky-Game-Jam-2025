using System.Collections;
using System.Collections.Generic;
using SpookyGame.Core;
using SpookyGame.World;
using UnityEngine;

public class FlowerGrow : Interactable
{
    
    [Header("Flower Settings")]
    [Tooltip("交互音效")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip interactSfx;
    
    [Header("Sprite Settings")]
    [Tooltip("花的不同状态图片")]
    [SerializeField] private Sprite flowerSprite;
    [SerializeField] private GameObject flowerGameObject;
    
    private SpriteRenderer _spriteRenderer;
    private int _currentSpriteIndex = 0;
    [SerializeField] private string targetStageId = "D5";
    
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
        
        // 检查是否持有鸟
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
        ChangeSprite();
        // 播放音效
        if (audioSource != null && interactSfx != null)
        {
            audioSource.PlayOneShot(interactSfx);
        }
        StartCoroutine(ChangeStage());
    }
    IEnumerator ChangeStage()
    {
        yield return new WaitForSeconds(3f);
        SceneService.FadeToStage(targetStageId,"",1f,"123");
    }
    
    /// <summary>
    /// 切换图片
    /// </summary>
    private void ChangeSprite()
    {
        flowerGameObject.SetActive(true);
        this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        // _spriteRenderer.sprite = flowerSprite;
        // SceneService.FadeToStage(targetStageId,"",1f,"123");

    }
}
