using UnityEngine;

namespace SpookyGame.World.InteractionEffects
{
    /// <summary>
    /// 播放音效效果
    /// 支持AudioClip播放
    /// </summary>
    public class PlaySoundEffect : BaseInteractionEffect
    {
        [Header("Sound Settings")]
        [SerializeField] private AudioClip audioClip;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private bool createTemporarySource = true;
        
        [Range(0f, 1f)]
        [SerializeField] private float volume = 1f;
        
        [Range(0f, 3f)]
        [SerializeField] private float pitch = 1f;
        
        private void Awake()
        {
            // 如果没有指定AudioSource，尝试获取
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }
        }
        
        protected override void ExecuteImmediate(GameObject actor)
        {
            if (audioClip == null)
            {
                Debug.LogWarning($"[PlaySoundEffect] {gameObject.name}: 没有设置音频剪辑", gameObject);
                return;
            }
            
            if (createTemporarySource)
            {
                // 创建临时AudioSource播放一次性音效
                AudioSource.PlayClipAtPoint(audioClip, transform.position, volume);
            }
            else if (audioSource != null)
            {
                audioSource.pitch = pitch;
                audioSource.volume = volume;
                audioSource.PlayOneShot(audioClip);
            }
            else
            {
                Debug.LogWarning($"[PlaySoundEffect] {gameObject.name}: 没有找到AudioSource", gameObject);
            }
        }
    }
}

