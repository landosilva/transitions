using UnityEditor;
using UnityEngine;

namespace Lando.Attributes.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(SceneAttribute))]
    public class SceneAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.PropertyField(position, property, label);
                EditorGUI.LabelField(position, "Error: Use [SceneOnly] on a string field.");
                return;
            }
            
            EditorGUI.BeginProperty(position, label, property);
            SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(property.stringValue);
            SceneAsset selectedScene = EditorGUI.ObjectField(position, label, sceneAsset, typeof(SceneAsset), false) as SceneAsset;
            
            if (selectedScene != null)
            {
                string scenePath = AssetDatabase.GetAssetPath(selectedScene);
                property.stringValue = scenePath;
            }
            
            EditorGUI.EndProperty();
        }
    }
}