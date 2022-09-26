using UnityEngine;
using UnityEngine.Events;

namespace MyTownProject.Events
{
    [CreateAssetMenu(menuName = "ScriptableObject/Event/FloatEvent")]
    public class FloatEventSO : ScriptableObject
    {
        public UnityAction<float> OnRaiseEvent;
        public UnityAction<float, float> OnRaiseEvent2;
        public UnityAction<float, float, float> OnRaiseEvent3;

        public void OneFloat(float f) => OnRaiseEvent?.Invoke(f);
        public void TwoFloats(float f1, float f2) => OnRaiseEvent2?.Invoke(f1, f2);
        public void ThreeFloats(float f1, float f2, float f3) => OnRaiseEvent3?.Invoke(f1, f2, f3);
        
    }
}
