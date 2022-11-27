using UnityEngine;
using UnityEngine.Events;
using FMODUnity;
using FMOD.Studio;

namespace MyTownProject.Events
{
    [CreateAssetMenu(menuName = "ScriptableObject/Event/AudioEvent")]
    public class AudioEventSO : ScriptableObject
    {
        public UnityAction<EventReference> OnRaiseEvent;
        public UnityAction<EventReference, Vector3> OnRaiseEvent2;

        public void RaiseEvent(EventReference clip) => OnRaiseEvent?.Invoke(clip);
        public void RaiseEvent2(EventReference clip, Vector3 position) => OnRaiseEvent2?.Invoke(clip, position);



        public UnityAction<Bus, float> onChangeBus;
        public UnityAction<Bus, bool> onPauseBus;

        public void ChangeBus(Bus bus, float value) => onChangeBus?.Invoke(bus, value);
        public void PauseBus(Bus bus, bool value) => onPauseBus?.Invoke(bus, value);

    }
}
