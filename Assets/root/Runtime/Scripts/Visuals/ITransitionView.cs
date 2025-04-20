using System.Collections;

namespace Lando.Transitions
{
    public interface ITransitionView
    {
        IEnumerator In(float duration);
        IEnumerator Out(float duration);
    }
}