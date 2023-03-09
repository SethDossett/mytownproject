using Cinemachine;
using MyTownProject.NPC;
using UnityEngine;
using MyTownProject.Events;
using MyTownProject.Core;
using MyTownProject.SO;
using MyTownProject.Enviroment;

namespace MyTownProject.Cameras
{
    public class CameraManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CinemachineTargetGroup _targetGroup_01;
        private CinemachineFreeLook _freeLookCamera_01;
        private CinemachineVirtualCamera _virtualCamera_01;
        private Animator _animator;

        [Header("Events")]
        [SerializeField] DialogueEventsSO dialogueEvents;
        [SerializeField] TransformEventSO _targetingEvent;
        [SerializeField] GeneralEventSO _unTargetingEvent;
        [SerializeField] GeneralEventSO _startOfGame;
        [SerializeField] ActionSO OpenDoor;
        [SerializeField] TransformEventSO EnteredNewScene;


        #region Animations
        int playerFreeLook01 = Animator.StringToHash("PlayerFreeLook01");
        int targetCamera = Animator.StringToHash("TargetingCamera01");
        int dialogueVCam01 = Animator.StringToHash("DialogueVirtalCamera01");
        int doorVCam01 = Animator.StringToHash("DoorVirtalCamera01");
        #endregion

        #region Cameras
        private void SwitchToFreeLook() => _animator.Play(playerFreeLook01);
        private void SwitchToTargeting() => _animator.Play(targetCamera);
        private void SwitchToDialogue() => _animator.Play(dialogueVCam01);
        private void SwitchToDoor() => _animator.Play(doorVCam01);

        #endregion


        private void OnEnable()
        {
            OpenDoor.OnOpenDoor += DoorView;
            EnteredNewScene.OnRaiseEvent += DoorViewExit;
            _startOfGame.OnRaiseEvent += InitialGameStartingCamera;
            dialogueEvents.onEnter += EnterDialogue;
            dialogueEvents.onExit += ExitDialogue;
            GameStateManager.OnGameStateChanged += CheckGameState;
            _targetingEvent.OnRaiseEvent += EnterTargeting;
            _unTargetingEvent.OnRaiseEvent += SwitchToFreeLook;
        }
        private void OnDisable()
        {
            OpenDoor.OnOpenDoor -= DoorView;
            EnteredNewScene.OnRaiseEvent -= DoorViewExit;
            _startOfGame.OnRaiseEvent -= InitialGameStartingCamera;
            dialogueEvents.onEnter -= EnterDialogue;
            dialogueEvents.onExit -= ExitDialogue;
            GameStateManager.OnGameStateChanged -= CheckGameState;
            _targetingEvent.OnRaiseEvent -= EnterTargeting;
            _unTargetingEvent.OnRaiseEvent -= SwitchToFreeLook;
        }
        void Start()
        {
            _animator = GetComponent<Animator>();
            SwitchToFreeLook();
        }
        void InitialGameStartingCamera()
        {
            SwitchToFreeLook();
        }

        void EnterDialogue(GameObject npc, TextAsset inkJSON)
        {
            print("dialogue camera");
            SwitchToDialogue();
        }
        void ExitDialogue()
        {
            SwitchToFreeLook();
        }
        void EnterTargeting(Transform t)
        {
            SwitchToTargeting();
        }
        void DoorView(DoorType doorType, GameObject door)
        {
            SwitchToDoor();
        }
        void DoorViewExit(Transform t)
        {
            SwitchToDoor();
        }
        void CheckGameState(GameState state)
        {
            if (state == GameState.GAME_PLAYING)
            {
                SwitchToFreeLook();
            }
        }
    }
}