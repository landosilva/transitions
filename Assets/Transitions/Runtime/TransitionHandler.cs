using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lando.Transitions
{
    public class TransitionHandler
    {
        private readonly ITransitionView _transitionView;
        private float _inDuration = 1f;
        private float _outDuration = 1f;
        private Action _onComplete;

        public TransitionHandler(ITransitionView transitionView)
        {
            _transitionView = transitionView;
        }

        public IEnumerator StartTransition(string sceneName)
        {
            yield return _transitionView.In(_inDuration);
            
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName);
            while (loadOperation is { isDone: false })
                yield return null;
            
            yield return new WaitForSeconds(0.5f);

            yield return _transitionView.Out(_outDuration);
            _onComplete?.Invoke();
        }

        public TransitionHandler SetIn(float duration)
        {
            _inDuration = duration;
            return this;
        }

        public TransitionHandler SetOut(float duration)
        {
            _outDuration = duration;
            return this;
        }

        public TransitionHandler OnComplete(Action action)
        {
            _onComplete = action;
            return this;
        }
    }
}
