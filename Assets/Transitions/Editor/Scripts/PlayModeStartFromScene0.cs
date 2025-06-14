using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Transitions.Editor
{
    [InitializeOnLoad]
    public static class PlayFromFirstScene
    {
        private const string MENU_PATH = "Tools/Lando/Transitions/Play From First Scene";
        private const string PREF_KEY = "PlayFromFirstScene_Enabled";
        private const string SAVED_SCENE_KEY = "PlayFromFirstScene_LastScene";
        private const string PREFAB_PATH_KEY = "PlayFromFirstScene_PrefabPath";

        static PlayFromFirstScene()
        {
            Menu.SetChecked(MENU_PATH, isChecked: EditorPrefs.GetBool(PREF_KEY, false));
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        [MenuItem(MENU_PATH)]
        private static void ToggleMenu()
        {
            bool enabled = !EditorPrefs.GetBool(PREF_KEY, false);
            EditorPrefs.SetBool(PREF_KEY, enabled);
            Menu.SetChecked(MENU_PATH, enabled);
        }

        [MenuItem(MENU_PATH, isValidateFunction: true)]
        private static bool ToggleMenuValidate()
        {
            Menu.SetChecked(MENU_PATH, isChecked: EditorPrefs.GetBool(PREF_KEY, false));
            return true;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (!EditorPrefs.GetBool(PREF_KEY, false)) 
                return;

            switch (state)
            {
                case PlayModeStateChange.ExitingEditMode:
                    CacheSceneAndPrefab();
                    if (!PromptToSaveDirtyScenes())
                    {
                        EditorApplication.isPlaying = false;
                        return;
                    }

                    string scene0 = EditorBuildSettings.scenes.Length > 0 ? EditorBuildSettings.scenes[0].path : null;
                    if (!string.IsNullOrEmpty(scene0)) 
                        EditorSceneManager.OpenScene(scene0);
                    break;

                case PlayModeStateChange.EnteredEditMode:
                    EditorApplication.delayCall += RestoreSceneAndPrefab;
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                case PlayModeStateChange.ExitingPlayMode:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private static void CacheSceneAndPrefab()
        {
            string currentScene = SceneManager.GetActiveScene().path;
            EditorPrefs.SetString(SAVED_SCENE_KEY, currentScene);

            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null)
                EditorPrefs.SetString(PREFAB_PATH_KEY, prefabStage.assetPath);
            else
                EditorPrefs.DeleteKey(PREFAB_PATH_KEY);
        }

        private static void RestoreSceneAndPrefab()
        {
            string previousScene = EditorPrefs.GetString(SAVED_SCENE_KEY, null);
            if (!string.IsNullOrEmpty(previousScene))
            {
                EditorSceneManager.OpenScene(previousScene);
                EditorPrefs.DeleteKey(SAVED_SCENE_KEY);
            }

            if (!EditorPrefs.HasKey(PREFAB_PATH_KEY)) 
                return;
            
            string prefabPath = EditorPrefs.GetString(PREFAB_PATH_KEY);
            Object prefab = AssetDatabase.LoadAssetAtPath<Object>(prefabPath);
            if (prefab != null) 
                AssetDatabase.OpenAsset(prefab);
            EditorPrefs.DeleteKey(PREFAB_PATH_KEY);
        }

        private static bool PromptToSaveDirtyScenes() => EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
    }
}
