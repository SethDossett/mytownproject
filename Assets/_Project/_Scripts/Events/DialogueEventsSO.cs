using UnityEngine;
using UnityEngine.Events;

namespace MyTownProject.Events
{
    [CreateAssetMenu(menuName = "ScriptableObject/Event/DialogueEventSO")]
    public class DialogueEventsSO : ScriptableObject
    {
        public UnityAction<GameObject, TextAsset> onEnter;
        public UnityAction onSubmit;
        public UnityAction onCancel;
        public UnityAction onExit;

        public void Enter(GameObject npc, TextAsset inkJSON) => onEnter?.Invoke(npc, inkJSON);

        public void Submit() => onSubmit?.Invoke();

        public void Cancel() => onCancel?.Invoke();

        public void Exit() => onExit?.Invoke();

       
    

    }

}
