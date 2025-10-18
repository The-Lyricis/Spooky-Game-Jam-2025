using UnityEngine;
using SpookyGame.Audio;

namespace SpookyGame.World.InteractionEffects
{
    /// <summary>
    /// 播放背景音乐效果
    /// 通过 BackgroundMusicManager 播放或切换背景音乐
    /// </summary>
    public class PlayMusicEffect : BaseInteractionEffect
    {
        [Header("Music Settings")]
        [SerializeField] private AudioClip musicClip;
        [SerializeField] private float fadeDuration = 2f;
        [SerializeField] private bool stopMusic = false;
        
        [Header("Debug")]
        [SerializeField] private bool useDebugLog = true;
        
        protected override void ExecuteImmediate(GameObject actor)
        {
            if (stopMusic)
            {
                // 停止音乐
                BackgroundMusicManager.Instance.StopMusic(fadeDuration);
                
                if (useDebugLog)
                {
                    Debug.Log($"<color=cyan>[PlayMusicEffect] {gameObject.name}:</color> Stopping music", gameObject);
                }
            }
            else
            {
                // 播放音乐
                if (musicClip == null)
                {
                    Debug.LogWarning($"[PlayMusicEffect] {gameObject.name}: Music clip is not assigned", gameObject);
                    return;
                }
                
                BackgroundMusicManager.Instance.PlayMusic(musicClip, fadeDuration);
                
                if (useDebugLog)
                {
                    Debug.Log($"<color=cyan>[PlayMusicEffect] {gameObject.name}:</color> Playing {musicClip.name}", gameObject);
                }
            }
        }
    }
}

