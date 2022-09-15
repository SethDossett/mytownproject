using System.Collections;
using UnityEngine.Events;
using UnityEngine;
using MyTownProject.SO;
using MyTownProject.Events;
using MyTownProject.Core;

namespace KinematicCharacterController.Examples
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] CharacterState CurrentCharacterState;
        [SerializeField] TransformEventSO PlayerRef;
        [SerializeField] StateChangerEventSO stateChangedEvent;
        [SerializeField] ActionSO teleportPlayer;
        [SerializeField] GeneralEventSO UntargetEvent;
        [SerializeField] DialogueEventsSO DialogueEvent;
        TheCharacterController cc;
        CharacterState _default = CharacterState.Default;
        CharacterState _climbing = CharacterState.Climbing;
        CharacterState _targeting = CharacterState.Targeting;
        CharacterState _talking = CharacterState.Talking;
        CharacterState _crawling = CharacterState.Crawling;
        public Transform _LookAtPoint;
        public UnityAction<TheCharacterController> OnCharacterTeleport;
        [SerializeField] UnityEvent DefaultStateAction;
        [SerializeField] UnityEvent ClimbingStateAction;
        [SerializeField] UnityEvent TalkingStateAction;
        [SerializeField] UnityEvent TargetingStateAction;
        [SerializeField] UnityEvent CrawlingStateAction;
        public bool isBeingTeleportedTo { get; set; }
        
        Animator _animator;

        private void OnEnable()
        {   DialogueEvent.onExit += DefaultState;
            teleportPlayer.OnTeleport += TeleportPlayer;
            UntargetEvent.OnRaiseEvent += Untargeting;
            TheCharacterController.OnPlayerStateChanged += StateChange;
            stateChangedEvent.OnGameState += GameStateChanged;
        }
        private void OnDisable()
        {
            DialogueEvent.onExit -= DefaultState;
            teleportPlayer.OnTeleport -= TeleportPlayer;
            UntargetEvent.OnRaiseEvent -= Untargeting;
            TheCharacterController.OnPlayerStateChanged -= StateChange;
            stateChangedEvent.OnGameState += GameStateChanged;
        }
        private void Awake()
        {
            cc = GetComponent<TheCharacterController>();
            _animator = GetComponent<Animator>();
            PlayerRef.RaiseEvent(transform);
        }
        private void TeleportPlayer(Vector3 location, Quaternion rotation)
        {
            if (!isBeingTeleportedTo)
            {
                if (cc)
                {
                    cc.Motor.SetPositionAndRotation(location, rotation);

                    if (OnCharacterTeleport != null)
                    {
                        OnCharacterTeleport(cc);
                    }
                    this.isBeingTeleportedTo = true;
                }
            }

            isBeingTeleportedTo = false;
        }
        private void Untargeting(){
            //cc.TransitionToState(CharacterState.Default);
        }
        void GameStateChanged(GameStateManager.GameState state){
            if(state == GameStateManager.GameState.GAME_PAUSED){
                _animator.updateMode = AnimatorUpdateMode.Normal;
            }
            else{
                _animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            } 
        }
        void StateChange(CharacterState state){
            CurrentCharacterState = state;

            if(state == _default){
                DefaultState();
            }
            if(state == _climbing){
                ClimbingState();
            }
            if(state == _targeting){
                TargetingState();
            }
            if(state == _talking){
                TalkingState();
            }
            if(state == _crawling){
                CrawlingState();
            }
        }
        void DefaultState(){
            DefaultStateAction?.Invoke();
            if(cc.CurrentCharacterState != _default)
                cc.TransitionToState(_default);
            _animator.SetLayerWeight(0, 1);
            _animator.SetLayerWeight(1, 0);
            _animator.SetLayerWeight(2, 0);
        }
        void ClimbingState(){
            ClimbingStateAction?.Invoke();
            _animator.SetLayerWeight(2, 1);
            _animator.SetLayerWeight(0, 0);
            _animator.SetLayerWeight(1, 0);
            _animator.Play("Idle");
        }
        void TargetingState(){
            TargetingStateAction?.Invoke();
            _animator.SetLayerWeight(1, 1);
            _animator.SetLayerWeight(0, 0);
            _animator.SetLayerWeight(2, 0);
        }
        void TalkingState(){
            TalkingStateAction?.Invoke();
            _animator.SetLayerWeight(0, 1);
            _animator.SetLayerWeight(1, 0);
            _animator.SetLayerWeight(2, 0);
        }
        void CrawlingState(){
            CrawlingStateAction?.Invoke();
        }
    }
}