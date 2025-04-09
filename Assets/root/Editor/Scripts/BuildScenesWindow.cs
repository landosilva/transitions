using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Lando.Patterns.Transitions.Editor
{
    public class BuildScenesWindow : EditorWindow
    {
        private float _lastClickTime;
        private const float DoubleClickTime = 0.3f;

        [MenuItem("Tools/Lando/Build Scenes Viewer")]
        public static void ShowWindow()
        {
            GetWindow<BuildScenesWindow>("Build Scenes");
        }

        private void OnGUI()
        {
            GUILayout.Label("Scenes in Build Settings", EditorStyles.boldLabel);
            GUILayout.Space(5);

            var scenes = EditorBuildSettings.scenes;

            if (scenes.Length == 0)
            {
                EditorGUILayout.HelpBox("No scenes added to Build Settings.", MessageType.Warning);
                return;
            }

            foreach (var scene in scenes)
            {
                if (scene.enabled)
                {
                    DrawSceneItem(scene.path);
                }
            }
        }

        private void DrawSceneItem(string scenePath)
        {
            GUIContent sceneContent = new GUIContent(Path.GetFileNameWithoutExtension(scenePath), EditorGUIUtility.IconContent("SceneAsset Icon").image);
        
            Rect rect = GUILayoutUtility.GetRect(0, 20);

            // Detect click events
            if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
            {
                if (IsDoubleClick())
                {
                    OpenScene(scenePath);
                }
                else
                {
                    SelectScene(scenePath);
                }

                Event.current.Use(); // Mark the event as used
            }

            GUI.Button(rect, sceneContent, EditorStyles.objectField);
        }

        private void SelectScene(string scenePath)
        {
            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
            if (sceneAsset != null)
            {
                // Ping the asset in the Project window
                EditorGUIUtility.PingObject(sceneAsset);
                Selection.activeObject = sceneAsset;
            }
            else
            {
                Debug.LogWarning($"Scene not found: {scenePath}");
            }
        }

        private void OpenScene(string scenePath)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(scenePath);
            }
        }

        private bool IsDoubleClick()
        {
            if (Time.realtimeSinceStartup - _lastClickTime < DoubleClickTime)
            {
                _lastClickTime = 0;
                return true;
            }
            _lastClickTime = Time.realtimeSinceStartup;
            return false;
        }
    }
}
