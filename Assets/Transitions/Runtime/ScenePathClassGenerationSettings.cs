using System;
using UnityEngine;

namespace Lando.Transitions
{
    [Serializable]
    public class ScenePathClassGenerationSettings
    {
        [field: SerializeField] public string Namespace { get; set; } = "Generated";
        [field: SerializeField] public string Path { get; set; } = "Assets/Scripts/Generated/";
        [field: SerializeField] public string ClassName { get; set; } = "ScenePath";
        
        public string FullPath => System.IO.Path.Combine(Path, $"{ClassName}.cs");
    }
}