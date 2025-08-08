using UnityEditor;
using UnityEngine;

namespace Lando.Transitions.Editor
{
    public static class TransitionsSettings
    {
        private const string PREF_NAMESPACE = "Transitions.ScenePathNamespace";
        private const string PREF_PATH = "Transitions.ScenePathPath";
        private const string PREF_CLASSNAME = "Transitions.ScenePathClassName";
        
        public static ScenePathClassGenerationSettings ScenePathClassGenerationSettings =>
            new()
            {
                Namespace = EditorPrefs.GetString(PREF_NAMESPACE, "Generated"),
                Path = EditorPrefs.GetString(PREF_PATH, "Assets/Scripts/Generated/"),
                ClassName = EditorPrefs.GetString(PREF_CLASSNAME, "ScenePath")
            };
    }
}