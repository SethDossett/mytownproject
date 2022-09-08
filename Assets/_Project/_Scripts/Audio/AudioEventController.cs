using MyTownProject.Events;
using UnityEngine;
using FMODUnity;

namespace MyTownProject.Audio
{
    public class AudioEventController : MonoBehaviour
    {
        [SerializeField] AudioEventSO audioEventSO;
        [SerializeField] DialogueEventsSO _dialogueEvents;
        [SerializeField] MainEventChannelSO MainEventChannelSO;

        [SerializeField] EventReference enterDialogue;
        [SerializeField] EventReference exitDialogue;
        [SerializeField] EventReference pauseGame;
        [SerializeField] EventReference unpauseGame;

        private void OnEnable()
        {   //Calling for one shot to be played.
            audioEventSO.OnRaiseEvent += PlayClip;
            audioEventSO.OnRaiseEvent2 += PlayClipAndPosition;

            //Events Called where we want oneshotplayed
            MainEventChannelSO.OnGamePaused += () => PlayClip(pauseGame);
            MainEventChannelSO.OnGameUnPaused += () => PlayClip(unpauseGame);
            _dialogueEvents.onEnter += (GameObject go, TextAsset inkFile) => PlayClip(enterDialogue);
            _dialogueEvents.onExit += () => PlayClip(exitDialogue);
        }
        private void OnDisable()
        {
            audioEventSO.OnRaiseEvent -= PlayClip;
            audioEventSO.OnRaiseEvent2 -= PlayClipAndPosition;

            MainEventChannelSO.OnGamePaused -= () => PlayClip(pauseGame);
            MainEventChannelSO.OnGameUnPaused -= () => PlayClip(unpauseGame);
            _dialogueEvents.onEnter -= (GameObject go, TextAsset inkFile) => PlayClip(enterDialogue);
            _dialogueEvents.onExit -= () => PlayClip(exitDialogue);
        }

        void PlayClip(EventReference clip)
        {
            RuntimeManager.PlayOneShot(clip.Guid);
            print("C IPPPP");
        }

        //For 3D one shots where position matters.
        void PlayClipAndPosition(EventReference clip, Vector3 pos)
        {
            RuntimeManager.PlayOneShot(clip.Guid, pos);
        }


    }   
}