using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Lando.Transitions.Editor
{
    public class BuildScenesWindow : EditorWindow
    {
        private ReorderableList _sceneReorderableList;
        private Texture _sceneIconTexture;
        private float _lastClickTime;
        private Vector2 _listScrollPosition;
        
        private const float DOUBLE_CLICK_TIME = 0.3f;
        private const float ICON_SIZE = 20f;

        [MenuItem("Tools/Transitions/Build Scenes Viewer")]
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

            ReorderableList sceneReorderableList = new(
                elements: scenesList,
                elementType: typeof(EditorBuildSettingsScene),
                draggable: true,
                displayHeader: false,
                displayAddButton: false,
                displayRemoveButton: false)
            {
                drawElementCallback = DrawElement,
                elementHeight = EditorGUIUtility.singleLineHeight + 4f,
                onReorderCallback = changedList =>
                {
                    EditorBuildSettings.scenes = changedList.list.Cast<EditorBuildSettingsScene>().ToArray();
                    Repaint();
                }
            };

            return sceneReorderableList;
        }

        private void OnGUI()
        {
            DrawWindowHeader();
            DrawSceneList();
            DrawGenerateClassSection();
            DrawAddOpenSceneButton();
            DrawOpenBuildSettingsButton();
        }
        
        private void DrawWindowHeader()
        {
            _sceneIconTexture = EditorGUIUtility.IconContent(name: "SceneAsset Icon").image;
            titleContent.image = _sceneIconTexture;
        
            HandleDragAndDrop(currentEvent: Event.current);
        
            GUIStyle headerStyle = new(EditorStyles.helpBox)
            {
                padding = new RectOffset(left: 10, right: 10, top: 10, bottom: 10),
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            GUILayout.Label(text: "Scenes in Build Settings", style: headerStyle, options: GUILayout.Height(32f));
            GUILayout.Space(pixels: 5f);
        }
        
        private void DrawSceneList()
        {
            _listScrollPosition = EditorGUILayout.BeginScrollView(scrollPosition: _listScrollPosition);
            _sceneReorderableList ??= BuildSceneReorderableList();
            _sceneReorderableList.DoLayoutList();
            EditorGUILayout.EndScrollView();
            GUILayout.Space(pixels: 6f);
        }
        
        private void DrawGenerateClassSection()
        {
            GUILayout.Label(text: "Generate Scenes Class", style: EditorStyles.boldLabel);
            GUILayout.Space(pixels: 4f);
            if (!GUILayout.Button(text: "Generate Scenes Class", options: GUILayout.Height(22f))) 
                return;
            
            string directory = TransitionsSettings.ScenePathClassGenerationSettings.Path;
            if(!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            
            GenerateScenesClass(TransitionsSettings.ScenePathClassGenerationSettings.FullPath);
        }
        
        private void GenerateScenesClass(string classPath)
        {
            List<EditorBuildSettingsScene> scenes = EditorBuildSettings.scenes.ToList();
            if (scenes.Count == 0)
            {
                Debug.LogWarning(message: "No scenes in Build Settings to generate class.");
                return;
            }

            using StreamWriter writer = new(classPath, append: false);
            writer.WriteLine($"namespace {TransitionsSettings.ScenePathClassGenerationSettings.Namespace}");
            writer.WriteLine("{");
            writer.WriteLine($"    public static class {TransitionsSettings.ScenePathClassGenerationSettings.ClassName}");
            writer.WriteLine("    {");

            foreach (EditorBuildSettingsScene scene in scenes)
            {
                string sceneName = Path.GetFileNameWithoutExtension(scene.path);
                sceneName = RemoveWhitespace(sceneName);
                writer.WriteLine($"        public static readonly string {sceneName} = \"{scene.path}\";");
            }
            
            writer.WriteLine("    }");
            writer.WriteLine("}");
            
            AssetDatabase.Refresh();
            Debug.Log(message: $"Scenes class generated at {classPath}");
        }
        
        private static string RemoveWhitespace(string input)
        {
            return string.Concat(input.Where(@char => !char.IsWhiteSpace(@char)));
        }
        
        private void DrawAddOpenSceneButton()
        {
            if (!GUILayout.Button(text: "Add Open Scene", options: GUILayout.Height(22f))) 
                return;

            string activeScenePath = SceneManager.GetActiveScene().path;
            if (string.IsNullOrEmpty(activeScenePath))
            {
                Debug.LogWarning(message: "No active scene to add to Build Settings.");
                return;
            }

            EditorBuildSettingsScene[] currentScenes = EditorBuildSettings.scenes;
            if (currentScenes.Any(predicate: scene => scene.path == activeScenePath))
            {
                Debug.LogWarning(message: "The active scene is already in Build Settings.");
                return;
            }

            List<EditorBuildSettingsScene> updatedScenes = currentScenes.ToList();
            updatedScenes.Add(new EditorBuildSettingsScene(activeScenePath, enabled: true));
            EditorBuildSettings.scenes = updatedScenes.ToArray();
            _sceneReorderableList = BuildSceneReorderableList();
        }
        
        private static void DrawOpenBuildSettingsButton()
        {
            if (!GUILayout.Button(text: "Open Build Settings", options: GUILayout.Height(22f))) 
                return;
            Type buildPlayerWindowType = Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor");
            GetWindow(buildPlayerWindowType);
        }

        private void DrawElement(Rect elementRectangle, int elementIndex, bool isActive, bool isFocused)
        {
            EditorBuildSettingsScene sceneItem = (EditorBuildSettingsScene)_sceneReorderableList.list[elementIndex];

            elementRectangle.y += 2f;
            elementRectangle.height = ICON_SIZE;

            const float buttonWidth = 18f;
            const float indexWidth = 16f;
            const float spacing = 2f;

            Rect buttonRectangle = new(
                x: elementRectangle.xMax - buttonWidth,
                y: elementRectangle.y + 1,
                width: buttonWidth,
                height: elementRectangle.height);

            Rect indexRectangle = new(
                x: buttonRectangle.x - spacing - indexWidth,
                y: elementRectangle.y,
                width: indexWidth,
                height: elementRectangle.height);

            Rect iconRectangle = new(
                x: elementRectangle.x + 2f,
                y: elementRectangle.y,
                width: ICON_SIZE,
                height: ICON_SIZE);
            GUI.DrawTexture(position: iconRectangle, image: _sceneIconTexture, scaleMode: ScaleMode.ScaleToFit);

            float nameStart = iconRectangle.xMax + spacing;
            float nameWidth = indexRectangle.x - spacing - nameStart;
            Rect nameRectangle = new Rect(
                x: nameStart,
                y: elementRectangle.y,
                width: nameWidth,
                height: elementRectangle.height);

            GUIContent sceneNameContent = new(text: Path.GetFileNameWithoutExtension(sceneItem.path));
            EditorGUI.LabelField(position: nameRectangle, label: sceneNameContent);

            GUIStyle rightAlignedStyle = new(EditorStyles.label) { alignment = TextAnchor.MiddleCenter };
            EditorGUI.LabelField(position: indexRectangle, label: elementIndex.ToString(), style: rightAlignedStyle);

            GUIContent removeContent = EditorGUIUtility.IconContent(name: "TreeEditor.Trash");
            removeContent.tooltip = "Remove from Build Settings";
            GUIStyle removeButtonStyle = new(EditorStyles.miniButton)
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

            switch (currentEvent.type)
            {
                case EventType.DragUpdated:
                    currentEvent.Use();
                    return;
                case EventType.DragPerform:
                    ProcessDragPerform(currentEvent: currentEvent, draggedSceneAssets: draggedSceneAssets);
                    break;
                case EventType.MouseDown:
                case EventType.MouseUp:
                case EventType.MouseMove:
                case EventType.MouseDrag:
                case EventType.KeyDown:
                case EventType.KeyUp:
                case EventType.ScrollWheel:
                case EventType.Repaint:
                case EventType.Layout:
                case EventType.DragExited:
                case EventType.Ignore:
                case EventType.Used:
                case EventType.ValidateCommand:
                case EventType.ExecuteCommand:
                case EventType.ContextClick:
                case EventType.MouseEnterWindow:
                case EventType.MouseLeaveWindow:
                case EventType.TouchDown:
                case EventType.TouchUp:
                case EventType.TouchMove:
                case EventType.TouchEnter:
                case EventType.TouchLeave:
                case EventType.TouchStationary:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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
            if (Time.realtimeSinceStartup - _lastClickTime < DOUBLE_CLICK_TIME)
            {
                _lastClickTime = 0f;
                return true;
            }
            _lastClickTime = Time.realtimeSinceStartup;
            return false;
        }
    }
}
