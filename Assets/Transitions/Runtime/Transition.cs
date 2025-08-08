using UnityEngine;

namespace Lando.Transitions
{
    public static class Transition
    {
        private class TransitionRunner : MonoBehaviour { }
        private static TransitionRunner _runner;
        
        private static ITransitionView _defaultView;

        public static TransitionHandler To(string sceneName)
        {
            _defaultView ??= new LazyBlackFadeInFadeOut();
            return To(sceneName, _defaultView);
        }

        public static TransitionHandler To(string sceneName, ITransitionView transitionView)
        {
            TransitionHandler handler = new(transitionView);
            GetRunner().StartCoroutine(handler.StartTransitionRoutine(sceneName));
            return handler;
        }

        private static TransitionRunner GetRunner()
        {
            if (_runner != null) 
                return _runner;
            GameObject transitionRunner = new(name: "TransitionRunner");
            Object.DontDestroyOnLoad(transitionRunner);
            _runner = transitionRunner.AddComponent<TransitionRunner>();
            return _runner;
        }
    }
}
