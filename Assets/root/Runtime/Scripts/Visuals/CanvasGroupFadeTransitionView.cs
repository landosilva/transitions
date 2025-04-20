using System;
using System.Collections;
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

        public override IEnumerator In(float duration)
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            
            float elapsedTime = 0;
            
            while (elapsedTime < duration)
            {
                _canvasGroup.alpha = elapsedTime / duration;
                elapsedTime += Time.deltaTime;
                
                yield return null;
            }
            
            _canvasGroup.alpha = 1;
        }
        
        public override IEnumerator Out(float duration)
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            
            float elapsedTime = 0;
            
            while (elapsedTime < duration)
            {
                _canvasGroup.alpha = 1 - (elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                
                yield return null;
            }
            
            _canvasGroup.alpha = 0;
        }
    }
}