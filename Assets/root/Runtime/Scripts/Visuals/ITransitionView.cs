using System.Collections;

namespace Lando.Patterns.Transitions
{
    public interface ITransitionView
    {
        IEnumerator In(float duration);
        IEnumerator Out(float duration);
    }
}