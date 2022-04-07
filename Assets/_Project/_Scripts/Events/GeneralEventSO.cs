using UnityEngine.Events;
using UnityEngine;

namespace MyTownProject.Events
{
    [CreateAssetMenu(menuName = "ScriptableObject/Event/General")]
    public class GeneralEventSO : ScriptableObject
    {
        public UnityAction OnRaiseEvent;

        public void RaiseEvent() => OnRaiseEvent?.Invoke();
    }
}