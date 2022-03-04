using UnityEngine;
using UnityEngine.Events;

namespace MyTownProject.Events
{
    [CreateAssetMenu(menuName = "ScriptableObject/Event/InteractionEventSO")]
    public class InteractionEventSO : ScriptableObject
    {
        public UnityAction OnRaiseEvent;

        public void RaiseEvent() => OnRaiseEvent?.Invoke();
        
    }
}