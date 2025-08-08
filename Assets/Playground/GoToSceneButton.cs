using System;
using Lando.Transitions;
using UnityEngine;
using UnityEngine.UI;

namespace Playground
{
    public class GoToSceneButton : MonoBehaviour
    {
        [SerializeField, Scene] private string _sceneName;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            Transition.To(_sceneName);
        }
    }
}
