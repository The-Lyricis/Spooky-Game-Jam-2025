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
    [Tooltip("牙齿掉落距离范围")]
    [SerializeField] private Vector2 fallDistanceRange = new Vector2(2f, 4f);
    [Tooltip("牙齿掉落持续时间范围")]
    [SerializeField] private Vector2 fallDurationRange = new Vector2(0.6f, 1.2f);
    [Tooltip("牙齿掉落时的旋转角度范围")]
    [SerializeField] private Vector2 rotationRange = new Vector2(180f, 720f);
    [Tooltip("每颗牙齿之间的延迟时间范围")]
    [SerializeField] private Vector2 delayRange = new Vector2(0.05f, 0.2f);
    [Tooltip("弹跳效果强度范围")]
    [SerializeField] private Vector2 bounceStrengthRange = new Vector2(0.1f, 0.4f);
    [Tooltip("弹跳次数范围")]
    [SerializeField] private Vector2Int bounceCountRange = new Vector2Int(1, 3);
    [Tooltip("重力影响强度")]
    [SerializeField] private float gravityStrength = 1.5f;
    
    [Header("Stage Transition")]
    [Tooltip("目标场景ID")]
    [SerializeField] private string targetStageId = "D3";
    [Tooltip("场景切换延迟（动画完成后等待时间）")]
    [SerializeField] private float transitionDelay = 0.5f;
    [Tooltip("场景切换提示文字")]
    [SerializeField] private string[] transitionTexts = new string[]
    {
        "Day 3"
    };
    
    private SpriteRenderer _spriteRenderer;
    private bool _isAnimating = false;
    
    protected override void Awake()
    {
        base.Awake();
        
        // 获取 SpriteRenderer 组件
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        // 确保所有牙齿一开始都是隐藏的
        HideAllTeeth();
    }
    
    /// <summary>
    /// 隐藏所有牙齿对象
    /// </summary>
    private void HideAllTeeth()
    {
        if (teethObjects != null)
        {
            foreach (var tooth in teethObjects)
            {
                if (tooth != null)
                {
                    tooth.SetActive(false);
                }
            }
        }
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
    /// 播放牙齿掉落动画 - 模拟真实石头掉落过程
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
            
            // 随机化参数
            float fallDistance = Random.Range(fallDistanceRange.x, fallDistanceRange.y);
            float fallDuration = Random.Range(fallDurationRange.x, fallDurationRange.y);
            float rotation = Random.Range(rotationRange.x, rotationRange.y);
            float delay = Random.Range(delayRange.x, delayRange.y);
            float bounceStrength = Random.Range(bounceStrengthRange.x, bounceStrengthRange.y);
            int bounceCount = Random.Range(bounceCountRange.x, bounceCountRange.y + 1);
            
            // 随机旋转方向
            rotation *= (Random.value > 0.5f ? 1f : -1f);
            
            // 添加水平方向的随机偏移，模拟牙齿散落
            float horizontalOffset = Random.Range(-1f, 1f);
            Vector3 endPos = startPos + Vector3.down * fallDistance + Vector3.right * horizontalOffset;
            
            // 创建单个牙齿的掉落动画
            int index = i;
            teethSequence.Insert(index * delay, 
                tooth.transform.DOMove(endPos, fallDuration)
                    .SetEase(Ease.InQuart) // 使用更符合重力的缓动
            );
            
            // 添加旋转动画 - 模拟石头在空中旋转
            teethSequence.Insert(index * delay,
                tooth.transform.DORotate(new Vector3(0, 0, rotation), fallDuration, RotateMode.FastBeyond360)
                    .SetEase(Ease.Linear) // 旋转保持匀速
            );
            
            // 添加弹跳效果 - 模拟石头落地后的弹跳
            if (bounceStrength > 0 && bounceCount > 0)
            {
                float bounceDuration = fallDuration * 0.1f; // 弹跳时间很短
                float bounceDelay = index * delay + fallDuration * 0.9f; // 在落地前开始弹跳
                
                for (int bounce = 0; bounce < bounceCount; bounce++)
                {
                    float currentBounceStrength = bounceStrength * Mathf.Pow(0.6f, bounce); // 每次弹跳强度递减
                    float currentBounceDuration = bounceDuration * Mathf.Pow(0.8f, bounce); // 每次弹跳时间递减
                    
                    teethSequence.Insert(bounceDelay + bounce * bounceDuration * 2f,
                        tooth.transform.DOMoveY(endPos.y + currentBounceStrength, currentBounceDuration)
                            .SetEase(Ease.OutQuad)
                            .SetLoops(2, LoopType.Yoyo)
                    );
                }
            }
            
            // 添加重力影响 - 让掉落更真实
            teethSequence.Insert(index * delay + fallDuration * 0.3f,
                tooth.transform.DOScale(0.95f, fallDuration * 0.7f)
                    .SetEase(Ease.InQuart)
            );
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
        
        // 重置动画状态
        _isAnimating = false;
        
        // 隐藏所有牙齿
        HideAllTeeth();
        
        // 切换到下一个场景
        SceneService.FadeToStage(targetStageId, 1f, transitionTexts);
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
