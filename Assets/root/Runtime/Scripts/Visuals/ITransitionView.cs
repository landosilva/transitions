using System.Threading;
using System.Threading.Tasks;

namespace Lando.Patterns.Transitions
{
    public interface ITransitionView
    {
        Task In(float duration, CancellationToken token);
        Task Out(float duration, CancellationToken token);
    }
}