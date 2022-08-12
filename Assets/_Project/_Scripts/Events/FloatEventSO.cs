using UnityEngine;
using UnityEngine.Events;

namespace MyTownProject.Events
{
    [CreateAssetMenu(menuName = "ScriptableObject/Event/FloatEvent")]
    public class FloatEventSO : ScriptableObject
    {
        public UnityAction<float> OnRaiseEvent;
        public UnityAction<float, float> OnRaiseEvent2;

        public void RaiseEvent(float f) => OnRaiseEvent?.Invoke(f);
        public void RaiseEvent2(float f1, float f2) => OnRaiseEvent2?.Invoke(f1, f2);
        
    }
}
