using UnityEngine.Events;
using UnityEngine;
using MyTownProject.SO;
using MyTownProject.Events;
using MyTownProject.Core;
using MyTownProject.Enviroment;

namespace KinematicCharacterController.Examples
{
    public class PlayerManager : MonoBehaviour
    {
        [Header("References")]
        public Transform _LookAtPoint;
        [SerializeField] CharacterState CurrentCharacterState;
        Animator _animator;
        TheCharacterController cc;

        [Header("Events")]
        [SerializeField] TransformEventSO PlayerRef;
        [SerializeField] StateChangerEventSO stateChangedEvent;
        [SerializeField] ActionSO teleportPlayer;
        [SerializeField] ActionSO OpenDoorEvent;
        [SerializeField] DialogueEventsSO DialogueEvent;
        [SerializeField] UnityEvent GamePlayingAction;
        [SerializeField] UnityEvent GamePausedAction;
        [SerializeField] UnityEvent CutSceneAction;
        public UnityAction<TheCharacterController> OnCharacterTeleport;

        [Header("States")]
        CharacterState _default = CharacterState.Default;
        CharacterState _climbing = CharacterState.ClimbLadder;
        CharacterState _targeting = CharacterState.Targeting;
        CharacterState _talking = CharacterState.Talking;
        CharacterState _crawling = CharacterState.Crawling;

        public bool isBeingTeleportedTo { get; set; }

        int _anim_PullOpenR = Animator.StringToHash("PullDoorOpenR");
        int _anim_PullOpenL = Animator.StringToHash("PullDoorOpenL");
        int _anim_PushOpenR = Animator.StringToHash("PushDoorOpenR");
        int _anim_PushOpenL = Animator.StringToHash("PushDoorOpenL");


        private void OnEnable()
        {
            DialogueEvent.onExit += DefaultState;
            teleportPlayer.OnTeleport += TeleportPlayer;
            OpenDoorEvent.OnOpenDoor += OpenDoorAnimation;
            TheCharacterController.OnPlayerStateChanged += StateChange;
            GameStateManager.OnGameStateChanged += GameStateChanged;
        }
        private void OnDisable()
        {
            DialogueEvent.onExit -= DefaultState;
            teleportPlayer.OnTeleport -= TeleportPlayer;
            OpenDoorEvent.OnOpenDoor -= OpenDoorAnimation;
            TheCharacterController.OnPlayerStateChanged -= StateChange;
            GameStateManager.OnGameStateChanged -= GameStateChanged;
        }
        private void Awake()
        {
            cc = GetComponent<TheCharacterController>();
            _animator = GetComponent<Animator>();
        }
        void Start()
        {
            //Calls Event to pass a refence to player to other scripts
            //Other Scripts are subscribed in Awake()
            print("Start Event Called");
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
        void OpenDoorAnimation(DoorType doorType, GameObject door)
        {
            int animationHash = _anim_PullOpenR; //Default Door anim
            switch (doorType)
            {
                case DoorType.PushOpenR:
                    {
                        animationHash = _anim_PushOpenR;
                        break;
                    }
                case DoorType.PullOpenR:
                    {
                        animationHash = _anim_PullOpenR;
                        break;
                    }
                case DoorType.PushOpenL:
                    {
                        animationHash = _anim_PushOpenL;
                        break;
                    }
                case DoorType.PullOpenL:
                    {
                        animationHash = _anim_PullOpenL;
                        break;
                    }
            }

            _animator.CrossFadeInFixedTime(animationHash, 0, 0);
        }
        void GameStateChanged(GameStateManager.GameState state)
        {
            if (state == GameStateManager.GameState.GAME_PLAYING)
            {
                GamePlayingAction?.Invoke();
                _animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            }
            else if (state == GameStateManager.GameState.GAME_PAUSED)
            {
                GamePausedAction?.Invoke();
                _animator.updateMode = AnimatorUpdateMode.Normal;
            }
            else
            {
                CutSceneAction?.Invoke();
                _animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            }
        }
        void StateChange(CharacterState state)
        {
            CurrentCharacterState = state;

            if (state == _default)
            {
                DefaultState();
            }
            if (state == _climbing)
            {
                ClimbingState();
            }
            if (state == _targeting)
            {
                TargetingState();
            }
            if (state == _talking)
            {
                TalkingState();
            }
            if (state == _crawling)
            {
                CrawlingState();
            }
        }
        void DefaultState()
        {

            if (cc.CurrentCharacterState != _default)
                cc.TransitionToState(_default);

        }
        void ClimbingState()
        {

        }
        void TargetingState()
        {


        }
        void TalkingState()
        {


        }
        void CrawlingState()
        {

        }

    }
}