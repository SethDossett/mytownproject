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
        [SerializeField] P_StateNames _currentRootState;
        [SerializeField] P_StateNames _currentSubState;
        Animator _animator;
        TheCharacterController cc;

        [Header("Events")]
        [SerializeField] TransformEventSO PlayerRef;
        [SerializeField] StateChangerEventSO changeState;
        [SerializeField] ActionSO teleportPlayer;
        [SerializeField] ActionSO OpenDoorEvent;
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
            teleportPlayer.OnTeleport += TeleportPlayer;
            OpenDoorEvent.OnOpenDoor += OpenDoorAnimation;
            FellOffLedge.OnRaiseEvent += Fell;
            TheCharacterController.OnPlayerStateChanged += StateChange;
            GameStateManager.OnGameStateChanged += GameStateChanged;
        }
        private void OnDisable()
        {
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
        private void TeleportPlayer(Vector3 location, Quaternion rotation)
        {
            if (!isBeingTeleportedTo)
            {
                if (cc)
                {
                    //If Teleport position is Obstucted Find new Position
                    if (cc.Motor.CharacterOverlap(
                        location,
                        rotation,
                        _probedColliders,
                        cc.Motor.CollidableLayers,
                        QueryTriggerInteraction.Ignore) > 0)
                    {
                        Debug.LogError("Teleport Aborted, Obstruction in Path, Need To Find New Safe Position");
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
        void StateChange(P_StateNames rootState, P_StateNames subState)
        {
            _currentRootState = rootState;
            _currentSubState = subState;
        }

        #endregion

        void Fell() => StartCoroutine(ResetPosition());
        IEnumerator ResetPosition()
        {
            //Fade out as player falls
            UIEvents.FadeTo(Color.white, 1.5f);
            //Need to disable controls and change to new falling camera position
            yield return new WaitForSecondsRealtime(1.5f);

            //Enable Cutscene Tick & Change to Cutscene State
            //Teleport Player after fade complete, Prevent Player from being on Edge
            TurnOnTimeScaleZeroTick.TimeScaleZeroTick(0, true);
            TeleportPlayer(cc.LastGroundedPosition, cc.LastGroundedRotation);
            GetComponent<FallOffPrevention>().enabled = true;
            cc.MaxStableMoveSpeed = 0;
            changeState.RaiseEventGame(GameState.CUTSCENE);

            //Recenter Camera
            RecenterCamX.ThreeFloats(0, 0.1f, 1);
            RecenterCamY.ThreeFloats(0, 0.1f, 1);

            //Wait for Player to be set, then Disable Cutscene Tick & Fade from white
            yield return new WaitForSecondsRealtime(1.5f);
            TurnOnTimeScaleZeroTick.TimeScaleZeroTick(0, false);
            UIEvents.RaiseBarsOn(0.1f);
            UIEvents.FadeFrom(Color.white, 1.5f);

            //Play animation to stand up, disable fall of prevention
            _animator.CrossFadeInFixedTime(_anim_GetUp, 0, 0);
            GetComponent<FallOffPrevention>().enabled = false;

            //Everything Done Game Playing State
            yield return new WaitForSecondsRealtime(2f);
            UIEvents.RaiseBarsOff(0.3f);
            changeState.RaiseEventGame(GameState.GAME_PLAYING);
            yield break;
        }

    }
}