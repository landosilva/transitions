using System;
using System.IO;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Lando.Transitions
{
    public class TransitionsSettings : ScriptableObject
    {
        [field: SerializeField] public TransitionViewMonoBehaviour DefaultTransitionPrefab { get; private set; }
        [SerializeField] private ScenePathClassGenerationSettings _scenePathClassGenerationSettings;

        [NonSerialized] private static ITransitionView _defaultTransition;
        public static ScenePathClassGenerationSettings ScenePathClassGenerationSettings 
            => Instance._scenePathClassGenerationSettings;
        
        private static TransitionsSettings _instance;
        private static TransitionsSettings Instance
        {
            get
            {
#if UNITY_EDITOR
                _instance = LoadOrCreateSettings();
#else
                string scriptName = nameof(TransitionsSettings);
                _instance = Resources.Load<TransitionsSettings>(scriptName);
#endif
                return _instance;
            }
        }
        
        public static ITransitionView GetDefaultTransitionView() => _defaultTransition ??= CreateDefaultTransition();
        private static ITransitionView CreateDefaultTransition()
        {
            return Instance.DefaultTransitionPrefab == null
                ? new LazyBlackFadeInFadeOut()
                : Instantiate(Instance.DefaultTransitionPrefab);
        }

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        private static void EnsureSettingsExist() => LoadOrCreateSettings();
        
        private static TransitionsSettings LoadOrCreateSettings()
        {
            const string scriptName = nameof(TransitionsSettings);
            _instance ??= Resources.Load<TransitionsSettings>(scriptName) ?? CreateSettings();
            return _instance;
        }
        
        private static TransitionsSettings CreateSettings()
        {
            const string scriptName = nameof(TransitionsSettings);
            string scriptableObjectName = $"{scriptName}.asset";
            
            string filter = $"t:MonoScript {scriptName}";
            string scriptPath = AssetDatabase.FindAssets(filter).Select(AssetDatabase.GUIDToAssetPath).ElementAt(0);
            string directory = Path.GetDirectoryName(scriptPath) ?? string.Empty;
            string path = Path.Combine(directory, "../Resources", scriptableObjectName);
            
            TransitionsSettings settings = CreateInstance<TransitionsSettings>();
            AssetDatabase.CreateAsset(settings, path);
            AssetDatabase.SaveAssets();

            Debug.Log("Transition Settings Created at: " + path);
            return settings;
        }
#endif
    }
}