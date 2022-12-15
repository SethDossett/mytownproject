using MyTownProject.Events;
using UnityEngine;
using FMODUnity;
using System;

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

        #region Events
        private void OnEnable()
        {   //Calling for one shot to be played.
            audioEventSO.OnRaiseEvent += PlayClip;
            audioEventSO.OnRaiseEvent2 += PlayClipAndPosition;

            //Events Called where we want oneshotplayed
            MainEventChannelSO.OnGamePaused += GamePaused;
            MainEventChannelSO.OnGameUnPaused += GameUnPaused;
            _dialogueEvents.onEnter += EnteredDialogue;
            _dialogueEvents.onExit += ExitedDialogue;
        }


        private void OnDisable()
        {
            audioEventSO.OnRaiseEvent -= PlayClip;
            audioEventSO.OnRaiseEvent2 -= PlayClipAndPosition;

            MainEventChannelSO.OnGamePaused -= GamePaused;
            MainEventChannelSO.OnGameUnPaused -= GameUnPaused;
            _dialogueEvents.onEnter -= EnteredDialogue;
            _dialogueEvents.onExit -= ExitedDialogue;
        }
        void GamePaused() => PlayClip(pauseGame);
        private void GameUnPaused() => PlayClip(unpauseGame);
        private void EnteredDialogue(GameObject arg0, TextAsset arg1) => PlayClip(enterDialogue);
        private void ExitedDialogue() => PlayClip(exitDialogue);

        #endregion

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