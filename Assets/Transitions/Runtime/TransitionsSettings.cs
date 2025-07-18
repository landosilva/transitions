using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Lando.Transitions
{
    public static class TransitionsSettings
    {
#if UNITY_EDITOR
        private const string PREF_DEFAULT_TRANSITION_GUID = "Transitions.DefaultTransitionGuid";
        private const string PREF_NAMESPACE = "Transitions.ScenePathNamespace";
        private const string PREF_PATH = "Transitions.ScenePathPath";
        private const string PREF_CLASSNAME = "Transitions.ScenePathClassName";
#endif

        private static ITransitionView _defaultTransition;

        public static ScenePathClassGenerationSettings ScenePathClassGenerationSettings
        {
            get
            {
#if UNITY_EDITOR
                return new ScenePathClassGenerationSettings
                {
                    Namespace = EditorPrefs.GetString(PREF_NAMESPACE, "Generated"),
                    Path = EditorPrefs.GetString(PREF_PATH, "Assets/Scripts/Generated/"),
                    ClassName = EditorPrefs.GetString(PREF_CLASSNAME, "ScenePath")
                };
#else
                return new ScenePathClassGenerationSettings();
#endif
            }
        }

        public static ITransitionView GetDefaultTransitionView() => _defaultTransition ??= CreateDefaultTransition();

        private static ITransitionView CreateDefaultTransition()
        {
#if UNITY_EDITOR
            string guid = EditorPrefs.GetString(PREF_DEFAULT_TRANSITION_GUID, string.Empty);
            if (!string.IsNullOrEmpty(guid))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = AssetDatabase.LoadAssetAtPath<TransitionViewMonoBehaviour>(path);
                if (prefab != null)
                    return Object.Instantiate(prefab);
            }
#endif
            return new LazyBlackFadeInFadeOut();
        }
    }
}