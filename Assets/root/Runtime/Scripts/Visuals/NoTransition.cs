using System.Threading;
using System.Threading.Tasks;

namespace Lando.Patterns.Transitions
{
    public class NoTransition : ITransitionView
    {
        public async Task In(float duration, CancellationToken token)
        {
            await Task.CompletedTask;
        }

        public async Task Out(float duration, CancellationToken token)
        {
            await Task.CompletedTask;
        }
    }
}