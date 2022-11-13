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
        public UnityAction<Color, float> OnFadeOut;
        public UnityAction<Color, float> OnFadeIn;

        public void RaiseFadeOut(Color color, float duration) => OnFadeOut?.Invoke(color, duration);
        // Fade out to Black
        public void RaiseFadeIn(Color color, float duration) => OnFadeIn?.Invoke(color, duration);
        // Fade in from Black
        #endregion

        #region Cinematic Bars
        public UnityAction<float> OnBarsOn;
        public UnityAction<float> OnBarsOff;

        public void RaiseBarsOn(float speed) => OnBarsOn?.Invoke(speed);
        public void RaiseBarsOff(float speed) => OnBarsOff?.Invoke(speed);
        #endregion

        #region Interact Text
        public UnityAction<string> OnShowTextInteract;
        public UnityAction OnHideTextInteract;

        public void ShowTextInteract(string text) => OnShowTextInteract?.Invoke(text);
        public void HideTextInteract() => OnHideTextInteract?.Invoke();
        #endregion

        #region Interact Prompt
        public UnityAction<Transform> OnShowPrompt;
        public UnityAction OnHidePrompt;
        public UnityAction<PromptName, int> OnChangePrompt;
        const string _empty = "";

        public void ShowPromptInteract(Transform pos) => OnShowPrompt?.Invoke(pos);
        public void HidePromptInteract() => OnHidePrompt?.Invoke();
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
