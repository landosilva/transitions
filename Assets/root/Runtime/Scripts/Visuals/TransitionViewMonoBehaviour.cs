using System.Collections;
using UnityEngine;

namespace Lando.Transitions
{
    public abstract class TransitionViewMonoBehaviour : MonoBehaviour, ITransitionView
    {
        public abstract void Initialize();
        public abstract IEnumerator In(float duration);
        public abstract IEnumerator Out(float duration);

        private void Awake()
        {
            Initialize();
            DontDestroyOnLoad(gameObject);
        }
    }
}