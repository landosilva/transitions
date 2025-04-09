using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Lando.Patterns.Transitions
{
    [Serializable]
    public class CanvasGroupFadeTransitionView : TransitionViewMonoBehaviour
    {
        private CanvasGroup _canvasGroup;

        public override void Initialize()
        {
            _canvasGroup = GetComponentInChildren<CanvasGroup>();
        }

        public override async Task In(float duration, CancellationToken token)
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            
            float elapsedTime = 0;
            
            while (elapsedTime < duration)
            {
                _canvasGroup.alpha = elapsedTime / duration;
                elapsedTime += Time.deltaTime;
                
                await Task.Yield();
                if (token.IsCancellationRequested) 
                    return;
            }
        }

        public override async Task Out(float duration, CancellationToken token)
        {
            _canvasGroup.alpha = 1;
            
            float elapsedTime = 0;
            
            while (elapsedTime < duration)
            {
                _canvasGroup.alpha = 1 - elapsedTime / duration;
                elapsedTime += Time.deltaTime;
                
                await Task.Yield();
                if (token.IsCancellationRequested) 
                    return;
            }
            
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }
    }
}