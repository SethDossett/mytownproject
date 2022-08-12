using UnityEngine;
using UnityEngine.Events;

namespace MyTownProject.Events
{
    [CreateAssetMenu(menuName = "ScriptableObject/Event/IntEvent")]
    public class IntEventSO : ScriptableObject
    {
        public UnityAction<int> OnRaiseEvent;
        public UnityAction<int, int> OnRaiseEvent2;

        public void RaiseEvent(int i) => OnRaiseEvent?.Invoke(i);
        public void RaiseEvent2(int i1, int i2) => OnRaiseEvent2?.Invoke(i1, i2);
        
    }
}
