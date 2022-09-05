using MyTownProject.Events;
using UnityEngine;
using FMODUnity;

namespace MyTownProject.Audio
{
    public class AudioEventController : MonoBehaviour
    {
        [SerializeField] AudioEventSO _oneShot;
        [SerializeField] DialogueEventsSO _dialogueEvents;
        [SerializeField] EventReference enterDialogue;
        [SerializeField] EventReference exitDialogue;

        private void OnEnable()
        {
            _dialogueEvents.onEnter += (GameObject go, TextAsset inkFile) => _oneShot.RaiseEvent(enterDialogue);
            _dialogueEvents.onExit += () => _oneShot.RaiseEvent(exitDialogue);
        }
        private void OnDisable()
        {
            _dialogueEvents.onEnter -= (GameObject go, TextAsset inkFile) => _oneShot.RaiseEvent(enterDialogue); 
            _dialogueEvents.onExit -= () => _oneShot.RaiseEvent(exitDialogue);
        }

    }   
}