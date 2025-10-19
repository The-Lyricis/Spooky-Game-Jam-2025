using System.Collections;
using System.Collections.Generic;
using SpookyGame.Core;
using SpookyGame.World;
using UnityEngine;
using DG.Tweening;

public class FallingTeeth : Interactable
{
    [Header("Interaction Settings")]
    [Tooltip("交互音效")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip interactSfx;
    
    [Header("Sprite Settings")]
    [Tooltip("人的不同状态图片（无牙齿）")]
    [SerializeField] private Sprite NoTeethSprite;
    
    [Header("Teeth Animation Settings")]
    [Tooltip("掉落的牙齿对象数组")]
    [SerializeField] private GameObject[] teethObjects;
    [Tooltip("牙齿掉落距离")]
    [SerializeField] private float fallDistance = 3f;
    [Tooltip("牙齿掉落持续时间")]
    [SerializeField] private float fallDuration = 0.8f;
    [Tooltip("牙齿掉落时的旋转角度")]
    [SerializeField] private float rotationAmount = 360f;
    [Tooltip("是否随机旋转方向")]
    [SerializeField] private bool randomRotation = true;
    [Tooltip("每颗牙齿之间的延迟时间")]
    [SerializeField] private float delayBetweenTeeth = 0.1f;
    [Tooltip("弹跳效果强度（0-1）")]
    [SerializeField] private float bounceStrength = 0.3f;
    
    [Header("Stage Transition")]
    [Tooltip("目标场景ID")]
    [SerializeField] private string targetStageId = "D3";
    [Tooltip("场景切换延迟（动画完成后等待时间）")]
    [SerializeField] private float transitionDelay = 0.5f;
    
    private SpriteRenderer _spriteRenderer;
    private bool _isAnimating = false;
    
    protected override void Awake()
    {
        base.Awake();
        
        // 获取 SpriteRenderer 组件
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    public override bool CanInteract(GameObject actor)
    {
        if (!base.CanInteract(actor)) return false;
        
        // 如果正在播放动画，不能再次交互
        if (_isAnimating)
        {
            return false;
        }
        
        // 检查是否持有鸟头骨
        string held = FlagService.GetHeldItem();
        if (held != "birdSkull")
        {
            Debug.Log("[FallingTeeth] 需要鸟头骨才能交互");
            return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// 动态提示
    /// </summary>
    public override string CurrentPrompt
    {
        get
        {
            string held = FlagService.GetHeldItem();
            if (held == "birdSkull")
            {
                return "使用鸟头骨";
            }
            else
            {
                return "需要鸟头骨";
            }
        }
    }
    
    /// <summary>
    /// 交互 - 触发牙齿掉落动画和场景切换
    /// </summary>
    protected override void OnInteract(GameObject actor)
    {
        Debug.Log("[FallingTeeth] 开始牙齿掉落动画");
        
        _isAnimating = true;
        
        // 播放音效
        if (audioSource != null && interactSfx != null)
        {
            audioSource.PlayOneShot(interactSfx);
        }
        
        // 切换到无牙齿图片
        ChangeSprite();
        
        // 给予牙齿物品
        FlagService.SetHeldItem("Teeth");
        
        // 开始牙齿掉落动画
        StartCoroutine(PlayTeethFallingAnimation());
    }
    
    /// <summary>
    /// 切换图片为无牙齿状态
    /// </summary>
    private void ChangeSprite()
    {
        if (_spriteRenderer != null && NoTeethSprite != null)
        {
            _spriteRenderer.sprite = NoTeethSprite;
            Debug.Log("[FallingTeeth] 切换到无牙齿图片");
        }
    }
    
    /// <summary>
    /// 播放牙齿掉落动画
    /// </summary>
    private IEnumerator PlayTeethFallingAnimation()
    {
        if (teethObjects == null || teethObjects.Length == 0)
        {
            Debug.LogWarning("[FallingTeeth] 没有配置牙齿对象，跳过动画");
            yield return new WaitForSeconds(transitionDelay);
            TransitionToNextStage();
            yield break;
        }
        
        // 记录每颗牙齿的初始位置
        Vector3[] initialPositions = new Vector3[teethObjects.Length];
        for (int i = 0; i < teethObjects.Length; i++)
        {
            if (teethObjects[i] != null)
            {
                initialPositions[i] = teethObjects[i].transform.position;
                teethObjects[i].SetActive(true);
            }
        }
        
        // 创建动画序列
        Sequence teethSequence = DOTween.Sequence();
        
        for (int i = 0; i < teethObjects.Length; i++)
        {
            if (teethObjects[i] == null) continue;
            
            GameObject tooth = teethObjects[i];
            Vector3 startPos = initialPositions[i];
            Vector3 endPos = startPos + Vector3.down * fallDistance;
            
            // 计算旋转方向
            float rotation = rotationAmount;
            if (randomRotation)
            {
                rotation *= (Random.value > 0.5f ? 1f : -1f);
            }
            
            // 创建单个牙齿的动画
            int index = i; // 捕获索引用于延迟
            teethSequence.Insert(index * delayBetweenTeeth, 
                tooth.transform.DOMove(endPos, fallDuration)
                    .SetEase(Ease.InBounce)
            );
            
            // 添加旋转动画
            teethSequence.Insert(index * delayBetweenTeeth,
                tooth.transform.DORotate(new Vector3(0, 0, rotation), fallDuration, RotateMode.FastBeyond360)
                    .SetEase(Ease.OutQuad)
            );
            
            // 添加轻微的弹跳效果
            if (bounceStrength > 0)
            {
                teethSequence.Insert(index * delayBetweenTeeth + fallDuration * 0.7f,
                    tooth.transform.DOMoveY(endPos.y + bounceStrength, fallDuration * 0.15f)
                        .SetEase(Ease.OutQuad)
                        .SetLoops(2, LoopType.Yoyo)
                );
            }
        }
        
        // 等待所有动画完成
        yield return teethSequence.WaitForCompletion();
        
        Debug.Log("[FallingTeeth] 牙齿掉落动画完成");
        
        // 额外延迟后切换场景
        yield return new WaitForSeconds(transitionDelay);
        
        // 切换到下一个场景
        TransitionToNextStage();
    }
    
    /// <summary>
    /// 切换到下一个场景
    /// </summary>
    private void TransitionToNextStage()
    {
        Debug.Log($"[FallingTeeth] 切换到场景: {targetStageId}");
        SceneService.FadeToStage(targetStageId, 1f, new string[] { "Day 3" });
    }
    
    /// <summary>
    /// 清理 DOTween 动画
    /// </summary>
    private void OnDestroy()
    {
        // 清理所有牙齿对象的动画
        if (teethObjects != null)
        {
            foreach (var tooth in teethObjects)
            {
                if (tooth != null)
                {
                    tooth.transform.DOKill();
                }
            }
        }
    }
    
#if UNITY_EDITOR
    [ContextMenu("测试牙齿掉落动画")]
    private void TestAnimation()
    {
        if (Application.isPlaying)
        {
            StartCoroutine(PlayTeethFallingAnimation());
        }
        else
        {
            Debug.LogWarning("请在运行时测试动画");
        }
    }
#endif
}
