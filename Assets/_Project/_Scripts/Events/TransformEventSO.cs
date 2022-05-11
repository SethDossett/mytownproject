using UnityEngine.Events;
using UnityEngine;

namespace MyTownProject.Events
{
    [CreateAssetMenu(menuName = "ScriptableObject/Event/Transform")]
    public class TransformEventSO : ScriptableObject
    {
        public UnityAction<Transform> OnRaiseEvent;

        public void RaiseEvent(Transform transform) => OnRaiseEvent?.Invoke(transform);
    }
}
