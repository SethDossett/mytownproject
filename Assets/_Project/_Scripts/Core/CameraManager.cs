using Cinemachine;
using MyTownProject.NPC;
using UnityEngine;
using MyTownProject.Events;

namespace MyTownProject.Core
{
    public class CameraManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] DialogueEventsSO dialogueEvents;
        [SerializeField] TransformEventSO _targetingEvent;
        [SerializeField] GeneralEventSO _unTargetingEvent;
        private NPC_StateHandler npcStateHandler;
        private CinemachineFreeLook _freeLookCamera_01;
        private CinemachineVirtualCamera _virtualCamera_01;
        [SerializeField] private CinemachineTargetGroup _targetGroup_01;
        private Animator _animator;

        #region Animations
        int playerFreeLook01 = Animator.StringToHash("PlayerFreeLook01");
        int targetCamera = Animator.StringToHash("TargetingCamera01");
        int dialogueVCam01 = Animator.StringToHash("DialogueVirtalCamera01");
        #endregion

        #region Cameras
        private void SwitchToFreeLook() => _animator.Play(playerFreeLook01);
        private void SwitchToTargeting() => _animator.Play(targetCamera);
        private void SwitchToDialogue() => _animator.Play(dialogueVCam01);

        #endregion


        private void OnEnable()
        {
            dialogueEvents.onEnter += EnterDialogue;
            GameStateManager.OnGameStateChanged += CheckGameState;
            _targetingEvent.OnRaiseEvent += EnterTargeting;
            _unTargetingEvent.OnRaiseEvent += SwitchToFreeLook;
        }
        private void OnDisable()
        {
            dialogueEvents.onEnter -= EnterDialogue;
            GameStateManager.OnGameStateChanged -= CheckGameState;
            _targetingEvent.OnRaiseEvent -= EnterTargeting;
            _unTargetingEvent.OnRaiseEvent -= SwitchToFreeLook;
        }
        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        private void EnterDialogue(GameObject npc, TextAsset inkJSON) {
            print("dialogue camera");
            //SwitchToDialogue();
        }  


        private void CheckGameState(GameStateManager.GameState state)
        {
            if (state == GameStateManager.GameState.GAME_PLAYING)
            {
                print("freelook");
                SwitchToFreeLook();
            }
        }

        void EnterTargeting(Transform t){
            //SwitchToTargeting();
        }
    }
}