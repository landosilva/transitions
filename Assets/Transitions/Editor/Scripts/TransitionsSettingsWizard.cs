using UnityEditor;

namespace Lando.Transitions.Editor
{
    public class TransitionsSettingsWizard : ScriptableWizard
    {
        private const string PREF_DEFAULT_TRANSITION_GUID = "Transitions.DefaultTransitionGuid";
        private const string DEFAULT_BLACK_FADE_GUID = "df50ac26ac8724da69e3440d60573cf0";
        private const string PREF_NAMESPACE = "Transitions.ScenePathNamespace";
        private const string PREF_PATH = "Transitions.ScenePathPath";
        private const string PREF_CLASSNAME = "Transitions.ScenePathClassName";

        public TransitionViewMonoBehaviour defaultTransitionPrefab;
        public string scenePathNamespace = "Generated";
        public string scenePathPath = "Assets/Scripts/Generated/";
        public string scenePathClassName = "ScenePath";

        [MenuItem("Tools/Transitions/Settings")]
        private static void Open()
        {
            DisplayWizard<TransitionsSettingsWizard>("Transitions Settings", "Save");
        }

        private void OnEnable()
        {
            LoadPrefs();
        }

        private void OnWizardCreate()
        {
            SavePrefs();
        }

        private void LoadPrefs()
        {
            string guid = EditorPrefs.GetString(PREF_DEFAULT_TRANSITION_GUID, DEFAULT_BLACK_FADE_GUID);
            if (!string.IsNullOrEmpty(guid))
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                defaultTransitionPrefab = AssetDatabase.LoadAssetAtPath<TransitionViewMonoBehaviour>(assetPath);
            }

            scenePathNamespace = EditorPrefs.GetString(PREF_NAMESPACE, scenePathNamespace);
            scenePathPath = EditorPrefs.GetString(PREF_PATH, scenePathPath);
            scenePathClassName = EditorPrefs.GetString(PREF_CLASSNAME, scenePathClassName);
        }

        private void SavePrefs()
        {
            string guid = defaultTransitionPrefab != null
                ? AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(defaultTransitionPrefab))
                : string.Empty;

            EditorPrefs.SetString(PREF_DEFAULT_TRANSITION_GUID, guid);
            EditorPrefs.SetString(PREF_NAMESPACE, scenePathNamespace);
            EditorPrefs.SetString(PREF_PATH, scenePathPath);
            EditorPrefs.SetString(PREF_CLASSNAME, scenePathClassName);
        }
    }
}
