using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Lando.Transitions.Editor
{
    public class BuildScenesWindow : EditorWindow
    {
        private ReorderableList _sceneReorderableList;
        private Texture _sceneIconTexture;
        private float _lastClickTime;
        private Vector2 _listScrollPosition;
        
        private const float DoubleClickTime = 0.3f;
        private const float IconSize = 20f;

        [MenuItem("Tools/Lando/Build Scenes Viewer")]
        public static void ShowWindow()
        {
            GetWindow<BuildScenesWindow>(title: "Build Scenes");
        }

        private void OnEnable()
        {
            _sceneReorderableList = BuildSceneReorderableList();
        }

        private ReorderableList BuildSceneReorderableList()
        {
            List<EditorBuildSettingsScene> scenesList = EditorBuildSettings.scenes.ToList();

            ReorderableList sceneReorderableList = new ReorderableList(
                elements: scenesList,
                elementType: typeof(EditorBuildSettingsScene),
                draggable: true,
                displayHeader: false,
                displayAddButton: false,
                displayRemoveButton: false);

            sceneReorderableList.drawElementCallback = DrawElement;
            sceneReorderableList.elementHeight = EditorGUIUtility.singleLineHeight + 4f;

            sceneReorderableList.onReorderCallback = changedList =>
            {
                EditorBuildSettings.scenes = changedList.list.Cast<EditorBuildSettingsScene>().ToArray();
                Repaint();
            };

            return sceneReorderableList;
        }

        private void OnGUI()
        {
            _sceneIconTexture = EditorGUIUtility.IconContent(name: "SceneAsset Icon").image;
            titleContent.image = _sceneIconTexture;
            
            HandleDragAndDrop(currentEvent: Event.current);

            GUIStyle headerStyle = new GUIStyle(EditorStyles.helpBox)
            {
                padding = new RectOffset(left: 10, right: 10, top: 10, bottom: 10),
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            GUILayout.Label(text: "Scenes in Build Settings", style: headerStyle, options: GUILayout.Height(32f));
            GUILayout.Space(pixels: 5f);

            _listScrollPosition = EditorGUILayout.BeginScrollView(scrollPosition: _listScrollPosition);
            _sceneReorderableList ??= BuildSceneReorderableList();
            _sceneReorderableList.DoLayoutList();
            EditorGUILayout.EndScrollView();

            GUILayout.Space(pixels: 6f);
            if (GUILayout.Button(text: "Open Build Settings", options: GUILayout.Height(22f)))
            {
                Type buildPlayerWindowType = Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor");
                GetWindow(buildPlayerWindowType);
            }
        }

        private void DrawElement(Rect elementRectangle, int elementIndex, bool isActive, bool isFocused)
        {
            EditorBuildSettingsScene sceneItem = (EditorBuildSettingsScene)_sceneReorderableList.list[elementIndex];

            elementRectangle.y += 2f;
            elementRectangle.height = IconSize;

            const float buttonWidth = 18f;
            const float indexWidth = 16f;
            const float spacing = 2f;

            Rect buttonRectangle = new Rect(
                x: elementRectangle.xMax - buttonWidth,
                y: elementRectangle.y + 1,
                width: buttonWidth,
                height: elementRectangle.height);

            Rect indexRectangle = new Rect(
                x: buttonRectangle.x - spacing - indexWidth,
                y: elementRectangle.y,
                width: indexWidth,
                height: elementRectangle.height);

            Rect iconRectangle = new Rect(
                x: elementRectangle.x + 2f,
                y: elementRectangle.y,
                width: IconSize,
                height: IconSize);
            GUI.DrawTexture(position: iconRectangle, image: _sceneIconTexture, scaleMode: ScaleMode.ScaleToFit);

            float nameStart = iconRectangle.xMax + spacing;
            float nameWidth = indexRectangle.x - spacing - nameStart;
            Rect nameRectangle = new Rect(
                x: nameStart,
                y: elementRectangle.y,
                width: nameWidth,
                height: elementRectangle.height);

            GUIContent sceneNameContent = new GUIContent(text: Path.GetFileNameWithoutExtension(sceneItem.path));
            EditorGUI.LabelField(position: nameRectangle, label: sceneNameContent);

            GUIStyle rightAlignedStyle = new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleCenter };
            EditorGUI.LabelField(position: indexRectangle, label: elementIndex.ToString(), style: rightAlignedStyle);

            GUIContent removeContent = EditorGUIUtility.IconContent(name: "TreeEditor.Trash");
            removeContent.tooltip = "Remove from Build Settings";
            GUIStyle removeButtonStyle = new GUIStyle(EditorStyles.miniButton)
            {
                padding = new RectOffset(left: 1, right: 1, top: 1, bottom: 1),
            };
            if (GUI.Button(position: buttonRectangle, removeContent, removeButtonStyle))
            {
                RemoveSceneAt(index: elementIndex);
                return;
            }

            if (Event.current.type == EventType.MouseDown && elementRectangle.Contains(Event.current.mousePosition))
            {
                if (IsDoubleClick())
                    OpenScene(scenePath: sceneItem.path);
                else
                    PingScene(scenePath: sceneItem.path);

                Event.current.Use();
            }

            if (Event.current.type != EventType.MouseDrag ||
                !elementRectangle.Contains(Event.current.mousePosition)) return;
            SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(assetPath: sceneItem.path);
            if (sceneAsset == null) 
                return;
            DragAndDrop.PrepareStartDrag();
            DragAndDrop.objectReferences = new Object[] { sceneAsset };
            DragAndDrop.StartDrag(sceneItem.path);
            Event.current.Use();
        }

        private static void PingScene(string scenePath)
        {
            SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(assetPath: scenePath);
            if (sceneAsset == null)
                return;
            EditorGUIUtility.PingObject(sceneAsset);
            Selection.activeObject = sceneAsset;
        }

        private static void OpenScene(string scenePath)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) 
                EditorSceneManager.OpenScene(scenePath: scenePath);
        }

        private void RemoveSceneAt(int index)
        {
            List<EditorBuildSettingsScene> currentScenes = EditorBuildSettings.scenes.ToList();
            if (index < 0 || index >= currentScenes.Count)
                return;
            currentScenes.RemoveAt(index: index);
            EditorBuildSettings.scenes = currentScenes.ToArray();
            _sceneReorderableList = BuildSceneReorderableList();
            Repaint();
        }

        private void HandleDragAndDrop(Event currentEvent)
        {
            if (!IsDragEvent(currentEvent: currentEvent))
                return;

            SceneAsset[] draggedSceneAssets = GetDraggedSceneAssets();
            if (draggedSceneAssets.Length == 0)
                return;

            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            if (currentEvent.type == EventType.DragUpdated)
            {
                currentEvent.Use();
                return;
            }

            if (currentEvent.type == EventType.DragPerform) 
                ProcessDragPerform(currentEvent: currentEvent, draggedSceneAssets: draggedSceneAssets);
        }

        private static bool IsDragEvent(Event currentEvent)
        {
            if (currentEvent.type != EventType.DragUpdated && currentEvent.type != EventType.DragPerform)
                return false;
            return DragAndDrop.objectReferences.Length > 0;
        }

        private static SceneAsset[] GetDraggedSceneAssets() 
            => DragAndDrop.objectReferences.OfType<SceneAsset>().ToArray();

        private void ProcessDragPerform(Event currentEvent, SceneAsset[] draggedSceneAssets)
        {
            DragAndDrop.AcceptDrag();
            currentEvent.Use();

            List<EditorBuildSettingsScene> existingScenes = EditorBuildSettings.scenes.ToList();

            foreach (SceneAsset draggedSceneAsset in draggedSceneAssets)
            {
                string assetPath = AssetDatabase.GetAssetPath(draggedSceneAsset);
                bool scenePresent = existingScenes.Any(predicate: scene => scene.path == assetPath);
                if (!scenePresent)
                {
                    existingScenes.Add(new EditorBuildSettingsScene(assetPath, enabled: true));
                }
            }

            EditorBuildSettings.scenes = existingScenes.ToArray();
            _sceneReorderableList = BuildSceneReorderableList();
            Repaint();
        }

        private bool IsDoubleClick()
        {
            if (Time.realtimeSinceStartup - _lastClickTime < DoubleClickTime)
            {
                _lastClickTime = 0f;
                return true;
            }
            _lastClickTime = Time.realtimeSinceStartup;
            return false;
        }
    }
}
