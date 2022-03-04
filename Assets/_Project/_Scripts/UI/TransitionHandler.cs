using System.Collections;
using UnityEngine;
using MyTownProject.Events;
using UnityEngine.UI;

namespace MyTownProject.UI
{
    public class TransitionHandler : MonoBehaviour
    {
        [SerializeField] UIEventChannelSO regularFade;
        public static TransitionHandler instance;
        CanvasGroup _canvasGroup;
        Image _image;
        private float _lerpTime;
        private float _currentAlpha;
        private float _lerpPercentage;

        private void OnEnable()
        {
            regularFade.OnFadeOut += FadeOut;
            regularFade.OnFadeIn += FadeIn;
        }
        private void OnDisable()
        {
            regularFade.OnFadeOut -= FadeOut;
            regularFade.OnFadeIn -= FadeIn;
        }
        private void Awake()
        {
            _image = GetComponent<Image>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }
        public void FadeOut(Color color, float duration)
        {
            _image.color = color;
            StartCoroutine(FadeOutCoroutine(duration));
        }
        IEnumerator FadeOutCoroutine(float duration)
        {
            _lerpTime = 0f;
            _canvasGroup.alpha = 0f;
            while (_lerpTime < duration)
            {
                _lerpTime += Time.unscaledDeltaTime;
                _lerpPercentage = _lerpTime / duration;
                _canvasGroup.alpha = Mathf.Lerp(0f, 1f, _lerpPercentage);
                yield return null;
            }

            yield break;
        }
        public void FadeIn(Color color, float duration)
        {
            _image.color = color;
            StartCoroutine(FadeInCoroutine(duration));
        }
        IEnumerator FadeInCoroutine(float duration)
        {
            _lerpTime = 0f;
            _canvasGroup.alpha = 1f;
            while (_lerpTime < duration)
            {
                _lerpTime += Time.unscaledDeltaTime;
                _lerpPercentage = _lerpTime / duration;
                _canvasGroup.alpha = Mathf.Lerp(1f, 0f, _lerpPercentage);
                yield return null;
            }


            yield break;
        }
    }
}