using UnityEngine;
using UnityEngine.Events;

namespace MyTownProject.Events
{
    [CreateAssetMenu(menuName = "ScriptableObject/ Event/ DialogueEventSO")]
    public class DialogueEventsSO : ScriptableObject
    {
        public UnityAction onEnter;
        public UnityAction onSubmit;
        public UnityAction onCancel;
        public UnityAction onExit;

        public void Enter() => onEnter?.Invoke();

        public void Submit() => onSubmit?.Invoke();

        public void Cancel() => onCancel?.Invoke();

        public void Exit() => onExit?.Invoke();

       
    

    }

}
