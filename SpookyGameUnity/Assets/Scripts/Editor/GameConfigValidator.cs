using UnityEngine;
using UnityEditor;
using SpookyGame.Core;
using System.Reflection;

namespace SpookyGame.Editor
{
    /// <summary>
    /// GameConfig 验证工具
    /// 在 Inspector 中显示配置状态
    /// </summary>
    [CustomEditor(typeof(GameConfig))]
    public class GameConfigValidator : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            // 自动同步场景名称
            SyncSceneNames();
            
            DrawDefaultInspector();
            
            GameConfig config = (GameConfig)target;
            
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("配置验证", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");
            
            // 验证场景名称
            ValidateSceneName("主菜单场景", config.MainMenuSceneName);
            ValidateSceneName("游戏场景", config.GameSceneName);
            
            // 验证关卡配置
            EditorGUILayout.Space(5);
            if (config.stages.Length == 0)
            {
                EditorGUILayout.HelpBox("❌ 未配置任何关卡！", MessageType.Error);
            }
            else
            {
                EditorGUILayout.LabelField($"✓ 已配置 {config.stages.Length} 个关卡", EditorStyles.label);
                
                // 验证首关ID
                bool firstStageExists = false;
                foreach (var stage in config.stages)
                {
                    if (stage.stageId == config.firstStageId)
                    {
                        firstStageExists = true;
                        break;
                    }
                }
                
                if (firstStageExists)
                {
                    EditorGUILayout.LabelField($"✓ 首关 '{config.firstStageId}' 配置正确", EditorStyles.label);
                }
                else
                {
                    EditorGUILayout.HelpBox($"❌ 首关 '{config.firstStageId}' 在 Stages 数组中不存在！", MessageType.Error);
                }
            }
            
            EditorGUILayout.EndVertical();
            
            // Build Settings 验证
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Build Settings 验证", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");
            
            ValidateSceneInBuildSettings(config.MainMenuSceneName);
            ValidateSceneInBuildSettings(config.GameSceneName);
            
            EditorGUILayout.EndVertical();
            
            // 快速操作按钮
            EditorGUILayout.Space(10);
            if (GUILayout.Button("打开 Build Settings", GUILayout.Height(30)))
            {
                EditorWindow.GetWindow(System.Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
            }
        }
        
        private void ValidateSceneName(string label, string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                EditorGUILayout.HelpBox($"❌ {label} 未配置！", MessageType.Error);
                return;
            }
            
            // 检查场景是否存在
            string scenePath = $"Assets/Scenes/{sceneName}.unity";
            if (System.IO.File.Exists(scenePath))
            {
                EditorGUILayout.LabelField($"✓ {label}: {sceneName}", EditorStyles.label);
            }
            else
            {
                EditorGUILayout.HelpBox($"❌ {label} '{sceneName}' 不存在于 Assets/Scenes/", MessageType.Warning);
            }
        }
        
        private void ValidateSceneInBuildSettings(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName)) return;
            
            string scenePath = $"Assets/Scenes/{sceneName}.unity";
            bool foundInBuildSettings = false;
            
            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (scene.path == scenePath)
                {
                    foundInBuildSettings = true;
                    if (scene.enabled)
                    {
                        EditorGUILayout.LabelField($"✓ '{sceneName}' 已添加到 Build Settings", EditorStyles.label);
                    }
                    else
                    {
                        EditorGUILayout.HelpBox($"⚠️ '{sceneName}' 在 Build Settings 中被禁用", MessageType.Warning);
                    }
                    break;
                }
            }
            
            if (!foundInBuildSettings)
            {
                EditorGUILayout.HelpBox($"❌ '{sceneName}' 未添加到 Build Settings！", MessageType.Error);
                if (GUILayout.Button($"添加 '{sceneName}' 到 Build Settings"))
                {
                    AddSceneToBuildSettings(scenePath);
                }
            }
        }
        
        private void AddSceneToBuildSettings(string scenePath)
        {
            var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            scenes.Add(new EditorBuildSettingsScene(scenePath, true));
            EditorBuildSettings.scenes = scenes.ToArray();
            Debug.Log($"[GameConfigValidator] 已添加场景到 Build Settings: {scenePath}");
        }
        
        /// <summary>
        /// 自动同步场景名称
        /// </summary>
        private void SyncSceneNames()
        {
            SerializedProperty mainMenuAssetProp = serializedObject.FindProperty("mainMenuSceneAsset");
            SerializedProperty gameSceneAssetProp = serializedObject.FindProperty("gameSceneAsset");
            SerializedProperty mainMenuNameProp = serializedObject.FindProperty("mainMenuSceneName");
            SerializedProperty gameSceneNameProp = serializedObject.FindProperty("gameSceneName");
            
            bool changed = false;
            
            // 同步主菜单场景名
            if (mainMenuAssetProp != null && mainMenuAssetProp.objectReferenceValue != null)
            {
                SceneAsset sceneAsset = mainMenuAssetProp.objectReferenceValue as SceneAsset;
                if (sceneAsset != null && mainMenuNameProp.stringValue != sceneAsset.name)
                {
                    mainMenuNameProp.stringValue = sceneAsset.name;
                    changed = true;
                }
            }
            
            // 同步游戏场景名
            if (gameSceneAssetProp != null && gameSceneAssetProp.objectReferenceValue != null)
            {
                SceneAsset sceneAsset = gameSceneAssetProp.objectReferenceValue as SceneAsset;
                if (sceneAsset != null && gameSceneNameProp.stringValue != sceneAsset.name)
                {
                    gameSceneNameProp.stringValue = sceneAsset.name;
                    changed = true;
                }
            }
            
            if (changed)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
        }
    }
}

