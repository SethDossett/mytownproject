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
        [SerializeField] DialogueEventsSO DialogueEvent;
        TheCharacterController cc;
        CharacterState _default = CharacterState.Default;
        CharacterState _climbing = CharacterState.ClimbLadder;
        CharacterState _targeting = CharacterState.Targeting;
        CharacterState _talking = CharacterState.Talking;
        CharacterState _crawling = CharacterState.Crawling;
        public Transform _LookAtPoint;
        public UnityAction<TheCharacterController> OnCharacterTeleport;
        [SerializeField] UnityEvent GamePlayingAction;
        [SerializeField] UnityEvent GamePausedAction;
        [SerializeField] UnityEvent CutSceneAction;
        public bool isBeingTeleportedTo { get; set; }
        
        Animator _animator;

        private void OnEnable()
        {   DialogueEvent.onExit += DefaultState;
            teleportPlayer.OnTeleport += TeleportPlayer;
            TheCharacterController.OnPlayerStateChanged += StateChange;
            GameStateManager.OnGameStateChanged += GameStateChanged;
        }
        private void OnDisable()
        {
            DialogueEvent.onExit -= DefaultState;
            teleportPlayer.OnTeleport -= TeleportPlayer;
            TheCharacterController.OnPlayerStateChanged -= StateChange;
            GameStateManager.OnGameStateChanged  -= GameStateChanged;
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
        
        void GameStateChanged(GameStateManager.GameState state){
            if(state == GameStateManager.GameState.GAME_PLAYING){
                GamePlayingAction?.Invoke();
                _animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            }
            else if(state == GameStateManager.GameState.GAME_PAUSED){
                GamePausedAction?.Invoke();
                _animator.updateMode = AnimatorUpdateMode.Normal;
            } 
            else{
                CutSceneAction?.Invoke();
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
            
            if(cc.CurrentCharacterState != _default)
                cc.TransitionToState(_default);
            
        }
        void ClimbingState(){
            
            
        }
        void TargetingState(){

            
        }
        void TalkingState(){
            
            
        }
        void CrawlingState(){

        }

    }
}