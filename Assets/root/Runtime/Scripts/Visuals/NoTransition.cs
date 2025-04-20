using System.Collections;

namespace Lando.Patterns.Transitions
{
    public class NoTransition : ITransitionView
    {
        public IEnumerator In(float duration)
        {
            yield return null;
        }

        public IEnumerator Out(float duration)
        {
            yield return null;
        }
    }
}