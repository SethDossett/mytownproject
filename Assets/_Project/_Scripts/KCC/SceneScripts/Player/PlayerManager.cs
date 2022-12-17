using UnityEngine.Events;
using UnityEngine;
using MyTownProject.SO;
using MyTownProject.Events;
using MyTownProject.Core;
using MyTownProject.Enviroment;
using System.Collections;

namespace KinematicCharacterController.Examples
{
    public class PlayerManager : MonoBehaviour
    {
        [Header("References")]
        public Transform _LookAtPoint;
        public CharacterState CurrentCharacterState;
        Animator _animator;
        TheCharacterController cc;

        [Header("Events")]
        [SerializeField] TransformEventSO PlayerRef;
        [SerializeField] StateChangerEventSO changeState;
        [SerializeField] ActionSO teleportPlayer;
        [SerializeField] ActionSO OpenDoorEvent;
        [SerializeField] DialogueEventsSO DialogueEvent;
        [SerializeField] GeneralEventSO FellOffLedge;
        [SerializeField] ActionSO TurnOnTimeScaleZeroTick;
        [SerializeField] UIEventChannelSO UIEvents;
        [SerializeField] FloatEventSO RecenterCamX;
        [SerializeField] FloatEventSO RecenterCamY;

        [Header("Unity Events")]
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
        private Collider[] _probedColliders = new Collider[8];

        int _anim_Idle = Animator.StringToHash("Idle");
        int _anim_GetUp = Animator.StringToHash("GetUp");
        int _anim_PullOpenR = Animator.StringToHash("PullDoorOpenR");
        int _anim_PullOpenL = Animator.StringToHash("PullDoorOpenL");
        int _anim_PushOpenR = Animator.StringToHash("PushDoorOpenR");
        int _anim_PushOpenL = Animator.StringToHash("PushDoorOpenL");


        private void OnEnable()
        {
            DialogueEvent.onExit += DefaultState;
            teleportPlayer.OnTeleport += TeleportPlayer;
            OpenDoorEvent.OnOpenDoor += OpenDoorAnimation;
            FellOffLedge.OnRaiseEvent += Fell;
            TheCharacterController.OnPlayerStateChanged += StateChange;
            GameStateManager.OnGameStateChanged += GameStateChanged;
        }
        private void OnDisable()
        {
            DialogueEvent.onExit -= DefaultState;
            teleportPlayer.OnTeleport -= TeleportPlayer;
            OpenDoorEvent.OnOpenDoor -= OpenDoorAnimation;
            FellOffLedge.OnRaiseEvent -= Fell;
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
            print("Pass Player Reference Out");
            PlayerRef.RaiseEvent(transform);

        }
        [SerializeField] GameObject obj;
        private void TeleportPlayer(Vector3 location, Quaternion rotation)
        {
            if (!isBeingTeleportedTo)
            {
                if (cc)
                {
                    Instantiate(obj, location,rotation);
                    //If Teleport position is Obstucted Find new Position
                    if (cc.Motor.CharacterOverlap(
                        location,
                        rotation,
                        _probedColliders,
                        cc.Motor.CollidableLayers,
                        QueryTriggerInteraction.Ignore) > 0)
                    {
                        Debug.LogError("Teleport Aborted, Obstruction in Path, Need To Find New Safe Position");
                    }
                    else
                    {

                        //cc.Motor.SetPosition(location);
                        //cc.Motor.SetRotation(rotation);
                        cc.Motor.SetPositionAndRotation(location, rotation);

                        print("PlayerTeleported");

                        if (OnCharacterTeleport != null)
                        {
                            OnCharacterTeleport(cc);
                        }
                        this.isBeingTeleportedTo = true;
                    }
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
        void GameStateChanged(GameState state)
        {
            if (state == GameState.GAME_PLAYING)
            {
                GamePlayingAction?.Invoke();
                _animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            }
            else if (state == GameState.GAME_PAUSED)
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

        #region Player State Logic
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
        #endregion

        void Fell() => StartCoroutine(ResetPosition());
        IEnumerator ResetPosition()
        {
            //Fade out as player falls
            UIEvents.OnFadeTo(Color.white, 1.5f);
            yield return new WaitForSecondsRealtime(1.5f);

            //Enable Cutscene Tick & Change to Cutscene State
            TurnOnTimeScaleZeroTick.TimeScaleZeroTick(0, true);
            changeState.RaiseEventGame(GameState.CUTSCENE);

            //Teleport Player after fade complete, Prevent Player from being on Edge
            TeleportPlayer(cc.LastGroundedPosition, cc.LastGroundedRotation);
            GetComponent<FallOffPrevention>().enabled = true;
            cc.MaxStableMoveSpeed = 0;

            //Recenter Camera
            RecenterCamX.ThreeFloats(0, 0.1f, 1);
            RecenterCamY.ThreeFloats(0, 0.1f, 1);

            //Wait for Player to be set, then Disable Cutscene Tick & Fade from white
            yield return new WaitForSecondsRealtime(1.5f);
            TurnOnTimeScaleZeroTick.TimeScaleZeroTick(0, false);
            UIEvents.OnFadeFrom(Color.white, 1.5f);

            //Play animation to stand up, disable fall of prevention
            _animator.CrossFadeInFixedTime(_anim_GetUp, 0, 0);
            GetComponent<FallOffPrevention>().enabled = false;

            //Everything Done Game Playing State
            yield return new WaitForSecondsRealtime(2f);
            changeState.RaiseEventGame(GameState.GAME_PLAYING);
            yield break;
        }

    }
}