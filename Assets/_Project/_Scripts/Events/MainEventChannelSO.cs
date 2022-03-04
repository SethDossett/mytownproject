using UnityEngine;
using UnityEngine.Events;
using MyTownProject.SO;

namespace MyTownProject.Events
{
    [CreateAssetMenu(menuName = "ScriptableObject/Event/maineventSO")]
    public class MainEventChannelSO : ScriptableObject
    {
        public UnityAction OnGamePaused;
        public UnityAction OnGameUnPaused;
        public UnityAction OnSubmit;
        public UnityAction<int, SceneSO> OnChangeScene;

        public void RaiseEventPaused() => OnGamePaused?.Invoke();
        
        public void RaiseEventUnPaused() => OnGameUnPaused?.Invoke();
        
        public void RaiseEventSubmit() => OnSubmit?.Invoke();

        public void RaiseEventChangeScene(int index, SceneSO sceneSO) => OnChangeScene?.Invoke(index, sceneSO);
        
    }

}
