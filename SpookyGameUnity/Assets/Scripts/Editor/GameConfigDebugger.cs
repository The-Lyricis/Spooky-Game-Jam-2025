using UnityEngine;
using UnityEditor;
using SpookyGame.Core;

namespace SpookyGame.Editor
{
    /// <summary>
    /// GameConfig 调试工具
    /// </summary>
    public class GameConfigDebugger : EditorWindow
    {
        private GameConfig gameConfig;
        
        [MenuItem("Tools/GameConfig Debugger")]
        public static void ShowWindow()
        {
            GetWindow<GameConfigDebugger>("GameConfig 调试器");
        }
        
        private void OnGUI()
        {
            EditorGUILayout.LabelField("GameConfig 调试器", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);
            
            // 选择 GameConfig
            gameConfig = (GameConfig)EditorGUILayout.ObjectField(
                "GameConfig", 
                gameConfig, 
                typeof(GameConfig), 
                false
            );
            
            if (gameConfig == null)
            {
                EditorGUILayout.HelpBox("请先选择或拖入 GameConfig 资产", MessageType.Info);
                
                if (GUILayout.Button("自动查找 GameConfig"))
                {
                    string[] guids = AssetDatabase.FindAssets("t:GameConfig");
                    if (guids.Length > 0)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                        gameConfig = AssetDatabase.LoadAssetAtPath<GameConfig>(path);
                        Debug.Log($"[GameConfigDebugger] 找到 GameConfig: {path}");
                    }
                    else
                    {
                        Debug.LogWarning("[GameConfigDebugger] 未找到 GameConfig 资产！");
                    }
                }
                
                return;
            }
            
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("场景配置", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");
            
            // 显示场景配置
            EditorGUILayout.LabelField($"主菜单场景名: {gameConfig.MainMenuSceneName}");
            EditorGUILayout.LabelField($"游戏场景名: {gameConfig.GameSceneName}");
            
            EditorGUILayout.EndVertical();
            
            // 显示 SceneAsset 状态
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("SceneAsset 状态", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");
            
            SerializedObject serializedConfig = new SerializedObject(gameConfig);
            SerializedProperty mainMenuAssetProp = serializedConfig.FindProperty("mainMenuSceneAsset");
            SerializedProperty gameSceneAssetProp = serializedConfig.FindProperty("gameSceneAsset");
            
            if (mainMenuAssetProp != null)
            {
                if (mainMenuAssetProp.objectReferenceValue != null)
                {
                    EditorGUILayout.LabelField($"✓ 主菜单 SceneAsset: {mainMenuAssetProp.objectReferenceValue.name}");
                }
                else
                {
                    EditorGUILayout.LabelField("❌ 主菜单 SceneAsset: 未配置");
                }
            }
            
            if (gameSceneAssetProp != null)
            {
                if (gameSceneAssetProp.objectReferenceValue != null)
                {
                    EditorGUILayout.LabelField($"✓ 游戏 SceneAsset: {gameSceneAssetProp.objectReferenceValue.name}");
                }
                else
                {
                    EditorGUILayout.LabelField("❌ 游戏 SceneAsset: 未配置");
                }
            }
            
            EditorGUILayout.EndVertical();
            
            // 显示 Build Settings
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Build Settings 状态", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");
            
            EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;
            if (buildScenes.Length == 0)
            {
                EditorGUILayout.HelpBox("❌ Build Settings 中没有任何场景！", MessageType.Error);
            }
            else
            {
                EditorGUILayout.LabelField($"Build Settings 中的场景 ({buildScenes.Length}):");
                foreach (var scene in buildScenes)
                {
                    string sceneName = System.IO.Path.GetFileNameWithoutExtension(scene.path);
                    string status = scene.enabled ? "✓" : "✗";
                    EditorGUILayout.LabelField($"{status} {sceneName} ({scene.path})");
                }
            }
            
            EditorGUILayout.EndVertical();
            
            // 快速修复按钮
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("快速修复", EditorStyles.boldLabel);
            
            if (GUILayout.Button("自动配置 SceneAsset", GUILayout.Height(30)))
            {
                AutoConfigureSceneAssets();
            }
            
            if (GUILayout.Button("添加场景到 Build Settings", GUILayout.Height(30)))
            {
                AddScenesToBuildSettings();
            }
            
            if (GUILayout.Button("打开 Build Settings", GUILayout.Height(30)))
            {
                EditorWindow.GetWindow(System.Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
            }
        }
        
        private void AutoConfigureSceneAssets()
        {
            if (gameConfig == null) return;
            
            SerializedObject serializedConfig = new SerializedObject(gameConfig);
            SerializedProperty mainMenuAssetProp = serializedConfig.FindProperty("mainMenuSceneAsset");
            SerializedProperty gameSceneAssetProp = serializedConfig.FindProperty("gameSceneAsset");
            
            bool changed = false;
            
            // 查找 MainMenu.unity
            string[] mainMenuGuids = AssetDatabase.FindAssets("MainMenu t:Scene");
            if (mainMenuGuids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(mainMenuGuids[0]);
                SceneAsset mainMenuAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                if (mainMenuAsset != null)
                {
                    mainMenuAssetProp.objectReferenceValue = mainMenuAsset;
                    changed = true;
                    Debug.Log($"[GameConfigDebugger] 已设置主菜单场景: {path}");
                }
            }
            
            // 查找 Scene_0.unity
            string[] gameSceneGuids = AssetDatabase.FindAssets("Scene_0 t:Scene");
            if (gameSceneGuids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(gameSceneGuids[0]);
                SceneAsset gameSceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                if (gameSceneAsset != null)
                {
                    gameSceneAssetProp.objectReferenceValue = gameSceneAsset;
                    changed = true;
                    Debug.Log($"[GameConfigDebugger] 已设置游戏场景: {path}");
                }
            }
            
            if (changed)
            {
                serializedConfig.ApplyModifiedProperties();
                EditorUtility.SetDirty(gameConfig);
                AssetDatabase.SaveAssets();
                Debug.Log("[GameConfigDebugger] GameConfig 已更新并保存！");
                Repaint();
            }
            else
            {
                Debug.LogWarning("[GameConfigDebugger] 未找到场景文件！");
            }
        }
        
        private void AddScenesToBuildSettings()
        {
            if (gameConfig == null) return;
            
            var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            
            // 添加主菜单场景
            string mainMenuPath = $"Assets/Scenes/{gameConfig.MainMenuSceneName}.unity";
            if (System.IO.File.Exists(mainMenuPath))
            {
                if (!scenes.Exists(s => s.path == mainMenuPath))
                {
                    scenes.Add(new EditorBuildSettingsScene(mainMenuPath, true));
                    Debug.Log($"[GameConfigDebugger] 已添加: {mainMenuPath}");
                }
            }
            
            // 添加游戏场景
            string gameScenePath = $"Assets/Scenes/{gameConfig.GameSceneName}.unity";
            if (System.IO.File.Exists(gameScenePath))
            {
                if (!scenes.Exists(s => s.path == gameScenePath))
                {
                    scenes.Add(new EditorBuildSettingsScene(gameScenePath, true));
                    Debug.Log($"[GameConfigDebugger] 已添加: {gameScenePath}");
                }
            }
            
            EditorBuildSettings.scenes = scenes.ToArray();
            Debug.Log("[GameConfigDebugger] Build Settings 已更新！");
            Repaint();
        }
    }
}

