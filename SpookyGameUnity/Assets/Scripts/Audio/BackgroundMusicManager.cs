using UnityEngine;
using System.Collections;

namespace SpookyGame.Audio
{
    /// <summary>
    /// 背景音乐管理器
    /// 单例模式，支持跨场景持久化、淡入淡出、音量控制等功能
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class BackgroundMusicManager : MonoBehaviour
    {
        private static BackgroundMusicManager _instance;
        
        /// <summary>
        /// 单例实例
        /// </summary>
        public static BackgroundMusicManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("BackgroundMusicManager");
                    _instance = go.AddComponent<BackgroundMusicManager>();
                }
                return _instance;
            }
        }
        
        [Header("音乐设置")]
        [SerializeField] private AudioClip defaultMusic;
        [SerializeField] private bool playOnAwake = true;
        [SerializeField] private bool loop = true;
        
        [Header("音量设置")]
        [Range(0f, 1f)]
        [SerializeField] private float volume = 0.5f;
        [Range(0f, 1f)]
        [SerializeField] private float minVolume = 0f;
        [Range(0f, 1f)]
        [SerializeField] private float maxVolume = 1f;
        
        [Header("淡入淡出设置")]
        [SerializeField] private float fadeInDuration = 2f;
        [SerializeField] private float fadeOutDuration = 2f;
        
        private AudioSource _audioSource;
        private Coroutine _fadeCoroutine;
        private float _targetVolume;
        
        private void Awake()
        {
            // 单例模式
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = this;
            DontDestroyOnLoad(gameObject);
            
            // 初始化 AudioSource
            _audioSource = GetComponent<AudioSource>();
            _audioSource.loop = loop;
            _audioSource.volume = 0f; // 初始音量为0，准备淡入
            _audioSource.playOnAwake = false;
            
            _targetVolume = volume;
            
            Debug.Log("[BackgroundMusicManager] Initialized");
        }
        
        private void Start()
        {
            // 如果设置了默认音乐且启用自动播放
            if (playOnAwake && defaultMusic != null)
            {
                PlayMusic(defaultMusic, fadeInDuration);
            }
        }
        
        /// <summary>
        /// 播放音乐（带淡入效果）
        /// </summary>
        /// <param name="clip">音乐剪辑</param>
        /// <param name="fadeDuration">淡入时长（秒），-1 表示使用默认值</param>
        public void PlayMusic(AudioClip clip, float fadeDuration = -1f)
        {
            if (clip == null)
            {
                Debug.LogWarning("[BackgroundMusicManager] AudioClip is null");
                return;
            }
            
            // 如果正在播放相同的音乐，不做任何操作
            if (_audioSource.isPlaying && _audioSource.clip == clip)
            {
                Debug.Log($"[BackgroundMusicManager] Already playing: {clip.name}");
                return;
            }
            
            float duration = fadeDuration < 0 ? fadeInDuration : fadeDuration;
            
            // 如果当前有音乐在播放，先淡出再切换
            if (_audioSource.isPlaying)
            {
                StartCoroutine(CrossfadeMusic(clip, duration));
            }
            else
            {
                _audioSource.clip = clip;
                _audioSource.Play();
                FadeIn(duration);
            }
            
            Debug.Log($"[BackgroundMusicManager] Playing: {clip.name}");
        }
        
        /// <summary>
        /// 停止音乐（带淡出效果）
        /// </summary>
        /// <param name="fadeDuration">淡出时长（秒），-1 表示使用默认值</param>
        public void StopMusic(float fadeDuration = -1f)
        {
            if (!_audioSource.isPlaying)
            {
                return;
            }
            
            float duration = fadeDuration < 0 ? fadeOutDuration : fadeDuration;
            FadeOut(duration, () => _audioSource.Stop());
            
            Debug.Log("[BackgroundMusicManager] Stopping music");
        }
        
        /// <summary>
        /// 暂停音乐
        /// </summary>
        public void PauseMusic()
        {
            if (_audioSource.isPlaying)
            {
                _audioSource.Pause();
                Debug.Log("[BackgroundMusicManager] Paused");
            }
        }
        
        /// <summary>
        /// 恢复播放
        /// </summary>
        public void ResumeMusic()
        {
            if (!_audioSource.isPlaying)
            {
                _audioSource.UnPause();
                Debug.Log("[BackgroundMusicManager] Resumed");
            }
        }
        
        /// <summary>
        /// 淡入
        /// </summary>
        /// <param name="duration">淡入时长（秒）</param>
        public void FadeIn(float duration = -1f)
        {
            float fadeDuration = duration < 0 ? fadeInDuration : duration;
            StartFade(_targetVolume, fadeDuration);
        }
        
        /// <summary>
        /// 淡出
        /// </summary>
        /// <param name="duration">淡出时长（秒）</param>
        /// <param name="onComplete">完成回调</param>
        public void FadeOut(float duration = -1f, System.Action onComplete = null)
        {
            float fadeDuration = duration < 0 ? fadeOutDuration : duration;
            StartFade(0f, fadeDuration, onComplete);
        }
        
        /// <summary>
        /// 设置音量
        /// </summary>
        /// <param name="newVolume">新音量（0-1）</param>
        /// <param name="smoothTransition">是否平滑过渡</param>
        public void SetVolume(float newVolume, bool smoothTransition = false)
        {
            newVolume = Mathf.Clamp(newVolume, minVolume, maxVolume);
            _targetVolume = newVolume;
            
            if (smoothTransition)
            {
                StartFade(newVolume, 0.5f);
            }
            else
            {
                _audioSource.volume = newVolume;
            }
            
            volume = newVolume;
        }
        
        /// <summary>
        /// 获取当前音量
        /// </summary>
        public float GetVolume()
        {
            return _audioSource.volume;
        }
        
        /// <summary>
        /// 是否正在播放
        /// </summary>
        public bool IsPlaying()
        {
            return _audioSource.isPlaying;
        }
        
        /// <summary>
        /// 获取当前播放的音乐
        /// </summary>
        public AudioClip GetCurrentClip()
        {
            return _audioSource.clip;
        }
        
        /// <summary>
        /// 设置循环播放
        /// </summary>
        public void SetLoop(bool shouldLoop)
        {
            loop = shouldLoop;
            _audioSource.loop = shouldLoop;
        }
        
        #region 私有方法
        
        /// <summary>
        /// 开始音量淡变
        /// </summary>
        private void StartFade(float targetVolume, float duration, System.Action onComplete = null)
        {
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }
            
            _fadeCoroutine = StartCoroutine(FadeCoroutine(targetVolume, duration, onComplete));
        }
        
        /// <summary>
        /// 音量淡变协程
        /// </summary>
        private IEnumerator FadeCoroutine(float targetVolume, float duration, System.Action onComplete = null)
        {
            float startVolume = _audioSource.volume;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                _audioSource.volume = Mathf.Lerp(startVolume, targetVolume, t);
                yield return null;
            }
            
            _audioSource.volume = targetVolume;
            _fadeCoroutine = null;
            
            onComplete?.Invoke();
        }
        
        /// <summary>
        /// 交叉淡变音乐
        /// </summary>
        private IEnumerator CrossfadeMusic(AudioClip newClip, float duration)
        {
            // 淡出当前音乐
            float halfDuration = duration / 2f;
            yield return StartCoroutine(FadeCoroutine(0f, halfDuration));
            
            // 切换音乐
            _audioSource.clip = newClip;
            _audioSource.Play();
            
            // 淡入新音乐
            yield return StartCoroutine(FadeCoroutine(_targetVolume, halfDuration));
        }
        
        #endregion
        
        #region Unity 生命周期
        
        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
        
        #endregion
        
        #region 编辑器辅助方法
        
#if UNITY_EDITOR
        [ContextMenu("播放测试音乐")]
        private void TestPlay()
        {
            if (defaultMusic != null)
            {
                PlayMusic(defaultMusic);
            }
            else
            {
                Debug.LogWarning("请先设置 Default Music");
            }
        }
        
        [ContextMenu("停止音乐")]
        private void TestStop()
        {
            StopMusic();
        }
        
        [ContextMenu("暂停音乐")]
        private void TestPause()
        {
            PauseMusic();
        }
        
        [ContextMenu("恢复播放")]
        private void TestResume()
        {
            ResumeMusic();
        }
#endif
        
        #endregion
    }
}

