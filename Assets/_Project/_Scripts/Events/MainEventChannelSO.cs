using UnityEngine;
using UnityEngine.Events;

namespace MyTownProject.Events
{
    [CreateAssetMenu(menuName = "ScriptableObject/Event/maineventSO")]
    public class MainEventChannelSO : ScriptableObject
    {
        public UnityAction OnGamePaused;
        public UnityAction OnGameUnPaused;
        public UnityAction<GameObject, TextAsset> OnTalk;
        public UnityAction OnSubmit;

        public void RaiseEventPaused()
        {
            OnGamePaused?.Invoke();
        }
        public void RaiseEventUnPaused()
        {
            OnGameUnPaused?.Invoke();
        }
        public void RaiseEventTalk(GameObject npc, TextAsset inkJSON)
        {
            OnTalk?.Invoke(npc,inkJSON);
        }
        public void RaiseEventSubmit()
        {
            OnSubmit?.Invoke(); 
        }
    }

}
