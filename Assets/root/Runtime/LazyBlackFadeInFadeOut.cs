using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Lando.Transitions
{
    [Serializable]
    public class LazyBlackFadeInFadeOut : ITransitionView
    {
        private static CanvasGroup _canvasGroup;

        public CanvasGroup GetCanvasGroup()
        {
            if(_canvasGroup != null)
                return _canvasGroup;
            
            GameObject canvasObject = new GameObject(name: "Canvas");
            Object.DontDestroyOnLoad(canvasObject);
                
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                
            GameObject imageObject = new GameObject(name: "Image");
            Image image = imageObject.AddComponent<Image>();
            image.color = Color.black;
            image.transform.SetParent(canvasObject.transform);
            image.rectTransform.anchorMin = Vector2.zero;
            image.rectTransform.anchorMax = Vector2.one;
            image.rectTransform.anchoredPosition = Vector2.zero;
            image.rectTransform.sizeDelta = Vector2.zero;
                
            CanvasGroup canvasGroup = canvasObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            
            _canvasGroup = canvasGroup;
            return _canvasGroup;
        }

        public IEnumerator In(float duration)
        {
            CanvasGroup canvasGroup = GetCanvasGroup();
            
            canvasGroup.alpha = 0;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            
            float elapsedTime = 0;
            
            while (elapsedTime < duration)
            {
                canvasGroup.alpha = elapsedTime / duration;
                elapsedTime += Time.deltaTime;
                
                yield return null;
            }
            
            canvasGroup.alpha = 1;
        }

        public IEnumerator Out(float duration)
        {
            CanvasGroup canvasGroup = GetCanvasGroup();
            canvasGroup.alpha = 1;
            
            float elapsedTime = 0;
            while (elapsedTime < duration)
            {
                canvasGroup.alpha = 1 - elapsedTime / duration;
                elapsedTime += Time.deltaTime;
                
                yield return null;
            }
            
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
}