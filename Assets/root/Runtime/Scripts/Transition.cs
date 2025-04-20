using UnityEngine;

namespace Lando.Transitions
{
    public static class Transition
    {
        private class TransitionRunner : MonoBehaviour { }
        private static TransitionRunner _runner;

        public static TransitionHandler To(string sceneName)
        {
            var defaultView = TransitionsSettings.GetDefaultTransitionView();
            return To(sceneName, defaultView);
        }
        
        public static TransitionHandler To(string sceneName, ITransitionView transitionView)
        {
            var handler = new TransitionHandler(transitionView);
            GetRunner().StartCoroutine(handler.StartTransition(sceneName));
            return handler;
        }

        private static TransitionRunner GetRunner()
        {
            if (_runner != null) 
                return _runner;
            GameObject transitionRunner = new GameObject(name: "TransitionRunner");
            Object.DontDestroyOnLoad(transitionRunner);
            _runner = transitionRunner.AddComponent<TransitionRunner>();
            return _runner;
        }
    }
}
