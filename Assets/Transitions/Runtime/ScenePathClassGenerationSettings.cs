using System;
using UnityEngine;

namespace Lando.Transitions
{
    [Serializable]
    public class ScenePathClassGenerationSettings
    {
        [field: SerializeField] public string Namespace { get; private set; } = "Generated";
        [field: SerializeField] public string Path { get; private set; } = "Assets/Scripts/Generated/";
        [field: SerializeField] public string ClassName { get; private set; } = "ScenePath";
        
        public string FullPath => System.IO.Path.Combine(Path, $"{ClassName}.cs");
    }
}