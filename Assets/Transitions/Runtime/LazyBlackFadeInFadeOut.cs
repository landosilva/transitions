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
        const int MAX_SORTING_ORDER = 32767;
        private CanvasGroupFadeTransitionView _view;

        public CanvasGroupFadeTransitionView GetOrCreateView()
        {
            if (_view != null) 
                return _view;
            
            GameObject viewObject = new(name: "BlackFade");
            Object.DontDestroyOnLoad(viewObject);
            
            GameObject canvasObject = new(name: "Canvas");
            canvasObject.transform.SetParent(viewObject.transform);
                
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = MAX_SORTING_ORDER;
                
            GameObject imageObject = new(name: "Image");
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
            
            _view = viewObject.AddComponent<CanvasGroupFadeTransitionView>();
            return _view;
        }

        public IEnumerator In(float duration)
        {
            CanvasGroupFadeTransitionView view = GetOrCreateView();
            yield return view.In(duration);
        }

        public IEnumerator Out(float duration)
        {
            CanvasGroupFadeTransitionView view = GetOrCreateView();
            yield return view.Out(duration);
        }
    }
}