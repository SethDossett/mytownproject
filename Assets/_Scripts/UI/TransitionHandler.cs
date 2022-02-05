using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionHandler : MonoBehaviour
{
    public static TransitionHandler instance;
    [SerializeField] private CanvasGroup _canvasGroup;
    private float _lerpTime;
    private float _duration;
    private float _currentAlpha;
    private float _lerpPercentage;


    private void Awake()
    {
        instance = this;
    }
    public void FadeOut()
    {
        StartCoroutine(FadeOutCoroutine());
    }
    IEnumerator FadeOutCoroutine()
    {
        _lerpTime = 0f;
        _canvasGroup.alpha = 0f;
        _duration = 1f;
        while (_lerpTime < _duration)
        {
            _lerpTime += Time.unscaledDeltaTime;
            _lerpPercentage = _lerpTime / _duration;
            _canvasGroup.alpha = Mathf.Lerp(0f, 1f, _lerpPercentage);
            yield return null;
        }
        

        yield break;
    }
    public void FadeIn()
    {
        StartCoroutine(FadeInCoroutine());
    }
    IEnumerator FadeInCoroutine()
    {
        _lerpTime = 0f;
        _canvasGroup.alpha = 1f;
        _duration = 1f;
        while (_lerpTime < _duration)
        {
            _lerpTime += Time.unscaledDeltaTime;
            _lerpPercentage = _lerpTime / _duration;
            _canvasGroup.alpha = Mathf.Lerp(1f, 0f, _lerpPercentage);
            yield return null;
        }


        yield break;
    }
}
