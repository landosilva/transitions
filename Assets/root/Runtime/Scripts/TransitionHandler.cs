using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Lando.Patterns.Transitions
{
    public class TransitionHandler
    {
        private readonly ITransitionView _transitionView;
        private static CancellationTokenSource _cancellationTokenSource;
        private readonly TaskCompletionSource<bool> _taskCompletionSource = new();
        
        private float _inDuration = 1f;
        private float _outDuration = 1f;

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            EditorApplication.playModeStateChanged += OnEditorApplicationOnplayModeStateChanged;
            
            return;

            void OnEditorApplicationOnplayModeStateChanged(PlayModeStateChange state)
            {
                EditorApplication.playModeStateChanged -= OnEditorApplicationOnplayModeStateChanged;
                
                if (state == PlayModeStateChange.ExitingPlayMode) 
                    _cancellationTokenSource?.Cancel();
            }
        }
#endif

        public TransitionHandler(ITransitionView transitionView)
        {
            _transitionView = transitionView;
        }

        public async Task StartTransition(string sceneName)
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = _cancellationTokenSource.Token;
            
            try
            {
                await Task.Yield();
                
                await _transitionView.In(_inDuration, token);
                
                if (token.IsCancellationRequested) 
                    return;
                
                await SceneManager.LoadSceneAsync(sceneName);
                
                if (token.IsCancellationRequested) 
                    return;
                
                await _transitionView.Out(_outDuration, token);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
            
            _taskCompletionSource.SetResult(true);
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
            _taskCompletionSource.Task.ContinueWith(Action, TaskScheduler.FromCurrentSynchronizationContext());

            return this;

            void Action(Task<bool> _) => action?.Invoke();
        }
    }
}