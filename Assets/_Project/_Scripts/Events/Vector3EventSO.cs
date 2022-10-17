using UnityEngine;
using UnityEngine.Events;

namespace MyTownProject.Events
{
    [CreateAssetMenu(menuName = "ScriptableObject/Event/Vector3Event")]
    public class Vector3EventSO : ScriptableObject
    {
        public UnityAction<Vector3> OnRaiseEvent;
        public UnityAction<Vector3, Vector3> OnRaiseEvent2;
        public UnityAction<Vector3, Vector3, Vector3> OnRaiseEvent3;

        public void OneVector3(Vector3 v) => OnRaiseEvent?.Invoke(v);
        public void TwoVector3s(Vector3 v1, Vector3 v2) => OnRaiseEvent2?.Invoke(v1, v2);
        public void ThreeVector3s(Vector3 v1, Vector3 v2, Vector3 v3) => OnRaiseEvent3?.Invoke(v1, v2, v3);
        
    }
}
