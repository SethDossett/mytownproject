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
        private NPC_StateHandler npcStateHandler;
        private CinemachineFreeLook _freeLookCamera_01;
        private CinemachineVirtualCamera _virtualCamera_01;
        [SerializeField] private CinemachineTargetGroup _targetGroup_01;
        private Animator _animator;

        [Header("Animation")]
        int playerFreeLook01 = Animator.StringToHash("PlayerFreeLook01");
        int dialogueVCam01 = Animator.StringToHash("DialogueVirtalCamera01");

        private void OnEnable()
        {
            dialogueEvents.onEnter += SwitchToDialogue;
            GameManager.OnGameStateChanged += CheckGameState;
        }
        private void OnDisable()
        {
            dialogueEvents.onEnter -= SwitchToDialogue;
            GameManager.OnGameStateChanged -= CheckGameState;
        }
        private void Start()
        {
            _animator = GetComponent<Animator>();
        }
        private void SwitchToDialogue(GameObject npc, TextAsset inkJSON)
        {
            _animator.Play(dialogueVCam01);
        }
        private void SwitchToFreeLook()
        {
            _animator.Play(playerFreeLook01);
        }

        void CheckGameState(GameManager.GameState state)
        {
            if (state == GameManager.GameState.GAME_PLAYING)
            {
                SwitchToFreeLook();
            }
        }
    }
}