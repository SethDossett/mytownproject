using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace MyTownProject.UI
{
    public class PostProcessingHandler : MonoBehaviour
    {
        private Volume volume;
        private Vignette vignette;

        private float lerpPercentage;
        private float lerpTime;
        private float duration = 2f;

        private void Awake()
        {
            volume = GetComponent<Volume>();
            //vignette = GetComponent<Vignette>();
        }
        private void Start()
        {
            volume.profile.TryGet(out vignette);
            vignette.intensity.value = 0f;
        }

        public void FadeOut()
        {
            lerpTime = 0f;

            StartCoroutine(FadeOutCoroutine());
        }

        IEnumerator FadeOutCoroutine()
        {


            while (lerpTime < duration)
            {
                lerpTime += Time.unscaledDeltaTime;
                lerpPercentage = lerpTime / duration;
                vignette.intensity.value = Mathf.Lerp(0f, 1f, lerpPercentage);
                yield return null;
            }

            yield break;
        }
        public void FadeIn()
        {
            lerpTime = 0f;

            StartCoroutine(FadeInCoroutine());
        }
        IEnumerator FadeInCoroutine()
        {


            while (lerpTime < duration)
            {
                lerpTime += Time.unscaledDeltaTime;
                lerpPercentage = lerpTime / duration;
                vignette.intensity.value = Mathf.Lerp(1f, 0f, lerpPercentage);
                yield return null;
            }

            yield break;
        }
    }
}