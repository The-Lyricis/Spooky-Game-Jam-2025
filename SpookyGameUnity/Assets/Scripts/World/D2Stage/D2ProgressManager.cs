using UnityEngine;
using SpookyGame.Core;
using SpookyGame.UI;

namespace SpookyGame.D2Scene
{
    /// <summary>
    /// D2 场景管理器
    /// 管理 D2 场景的进度和转场
    /// </summary>
    public class D2ProgressManager : MonoBehaviour
    {
        [Header("Scene Transition")]
        [Tooltip("对话结束后切换的关卡")]
        [SerializeField] private string nextStageId = "D3";
        
        [Tooltip("转场时长（秒）")]
        [SerializeField] private float transitionDuration = 1f;
        
        [Tooltip("场景切换提示文字")]
        [SerializeField] private string[] transitionTexts = new string[]
        {
            "Day 3"
        };
        
        /// <summary>
        /// 手动触发转场（供其他脚本调用）
        /// </summary>
        public void TriggerTransition()
        {
            SceneService.FadeToStage(nextStageId, transitionDuration, transitionTexts);
        }
        
        /// <summary>
        /// 手动触发转场（带自定义文字）
        /// </summary>
        /// <param name="customText">自定义转场文字</param>
        public void TriggerTransition(string customText)
        {
            SceneService.FadeToStage(nextStageId, transitionDuration, new string[] { customText });
        }



    }
}
