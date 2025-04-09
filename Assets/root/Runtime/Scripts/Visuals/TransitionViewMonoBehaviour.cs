using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Lando.Patterns.Transitions
{
    public abstract class TransitionViewMonoBehaviour : MonoBehaviour, ITransitionView
    {
        public abstract void Initialize();
        public abstract Task In(float duration, CancellationToken token);
        public abstract Task Out(float duration, CancellationToken token);

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
    }
}