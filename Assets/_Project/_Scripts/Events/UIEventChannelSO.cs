using UnityEngine;
using UnityEngine.Events;
using MyTownProject.Core;
using MyTownProject.UI;

namespace MyTownProject.Events
{
    [CreateAssetMenu(menuName = "ScriptableObject/Event/UIEventSO")]
    public class UIEventChannelSO : ScriptableObject
    {
        #region Fader
        public UnityAction<Color, float> OnFadeTo;
        public UnityAction<Color, float> OnFadeFrom;

        public void FadeTo(Color color, float duration) => OnFadeTo?.Invoke(color, duration);
        // Fade out to Color
        public void FadeFrom(Color color, float duration) => OnFadeFrom?.Invoke(color, duration);
        // Fade in from Color
        #endregion

        #region Cinematic Bars
        public UnityAction<float> OnBarsOn;
        public UnityAction<float> OnBarsOff;

        public void RaiseBarsOn(float speed) => OnBarsOn?.Invoke(speed);
        public void RaiseBarsOff(float speed) => OnBarsOff?.Invoke(speed);
        #endregion

        #region Interact Text
        public UnityAction<HudElement, string> OnShowButtonText;
        public UnityAction<HudElement> OnHideButtonText;

        public void ShowButtonText(HudElement hudElement, string text) => OnShowButtonText?.Invoke(hudElement ,text);
        public void HideButtonText(HudElement hudElement) => OnHideButtonText?.Invoke(hudElement);
        #endregion

        #region Interact Prompt
        public UnityAction<PromptName, int> OnChangePrompt;

        public void ChangePrompt(PromptName name, int priority) => OnChangePrompt?.Invoke(name, priority);
        
        #endregion

        #region Misc
        public UnityAction<Vector2, float, string> OnShowExplaination;
        public UnityAction<ControllerType> OnChangeControllerType;
        public void ShowExplaination(Vector2 screenPos, float duration, string message) => OnShowExplaination?.Invoke(screenPos, duration, message);
        public void ChangeController(ControllerType type) => OnChangeControllerType?.Invoke(type);
        
        #endregion
    }
}
