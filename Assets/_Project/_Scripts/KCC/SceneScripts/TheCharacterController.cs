using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyTownProject.Core;
using MyTownProject.Events;
using MyTownProject.SO;
using MyTownProject.UI;
using System;


namespace KinematicCharacterController.Examples
{
    #region Stucts and Enums
    public enum GroundType
    {
        Grass, Sand, Stone, Wood, ShallowWater
    }

    public enum OrientationMethod
    {
        TowardsCamera,
        TowardsMovement,
    }

    public struct PlayerCharacterInputs
    {
        public Vector2 MoveDirection;
        public float MoveAxisForward;
        public float MoveAxisRight;
        public Quaternion CameraRotation;
        public bool JumpDown;
        public bool CrouchDown;
        public bool CrouchUp;
        public bool Interact;
        public bool LeftTriggerDown;
        public bool LeftTriggerUp;
    }

    public struct AICharacterInputs
    {
        public Vector3 MoveVector;
        public Vector3 LookVector;
    }

    public enum BonusOrientationMethod
    {
        None,
        TowardsGravity,
        TowardsGroundSlopeAndGravity,
    }

    #endregion
    public class TheCharacterController : MonoBehaviour, ICharacterController
    {
        public KinematicCharacterMotor Motor;

        [Header("Stable Movement")]
        public float MaxStableMoveSpeed = 0f;
        public float StableMovementSharpness = 15f;
        public float OrientationSharpness = 10f;
        public OrientationMethod OrientationMethod = OrientationMethod.TowardsCamera;
        public float _defaultOrientationSharpness = 20f;

        [Header("Air Movement")]
        public float MaxAirMoveSpeed = 15f;
        public float AirAccelerationSpeed = 15f;
        public float Drag = 0.1f;
        float _offGroundTimeBuffer;

        [Header("Jumping")]
        public bool RestrictJumping = false;
        public bool AllowJumpingWhenSliding = false;
        public float JumpUpSpeed = 10f;
        public float JumpScalableForwardSpeed = 10f;
        public float JumpPreGroundingGraceTime = 0f;
        public float JumpPostGroundingGraceTime = 0f;
        [Range(0f, 10f)] public float SpeedNeededToJump = 4.5f;

        [Header("Climbing")]
        private PlayerClimb _playerClimb;
        public PlayerClimb PlayerClimb { get { return _playerClimb; } }
        public bool CanHang;
        public Vector3 LedgeDirection;
        public bool _isHanging;
        public bool IsDropToHang;

        [Header("ClimbingLadder")]
        public Vector3 NewCenteredPosition;
        public Quaternion NewLadderRotation;
        public float ClimbSpeedY = 2f;
        public float ClimbSpeedX = 2f;
        public bool _gettingOnOffObstacle;
        [SerializeField] float _jumpOnLadderSpeed = 5f;

        [Header("Targeting")]
        public Transform Target;
        public float TargetingRotSpeed = 500f;
        public bool HasTargetToLockOn;

        [Header("Crawling")]
        FallOffPrevention _fallOffPrevention;
        public FallOffPrevention FallOffPrevention { get { return _fallOffPrevention; } }
        public float CrawlSpeed = 2f;
        public float CrawlRotationSpeed = 1f;
        

        [Header("Falling")]
        [SerializeField] float _timeFallingInAir = 0f;
        public bool _startFallingTimer = false;
        bool _hardLanding = false;
        [SerializeField] float _timeToTriggerHardLanding = 0.5f;
        public bool IsFalling;

        [Header("Misc")]
        public List<Collider> IgnoredColliders = new List<Collider>();
        public BonusOrientationMethod BonusOrientationMethod = BonusOrientationMethod.None;
        public float BonusOrientationSharpness = 10f;
        public Vector3 Gravity = new Vector3(0, -30f, 0);
        public Transform MeshRoot;
        public Transform CameraFollowPoint;
        public float CrouchedCapsuleHeight = 0.95f;
        public float TalkingRotSpeed = 500f;

        [Header("Animation")]
        Animator _animator;
        public Animator PlayerAnimator { get { return _animator; } }
        //States


        

        int _jumpState = Animator.StringToHash("Jump");
        int _hardLandState = Animator.StringToHash("HardLanding");
        //Parameter Values
        int anim_isClimbing = Animator.StringToHash("isClimbing");
        int anim_landTrigger = Animator.StringToHash("hasLanded");
        int anim_talking = Animator.StringToHash("isTalking");

        //Climb States
        int anim_Hang = Animator.StringToHash("Hang");
        int anim_DropToHang = Animator.StringToHash("DropToHang");



        public GameState CurrentGameState { get; private set; }
        public GroundType CurrentGroundType { get; private set; }

        public HitStabilityReport CurrentHitStabilityReport { get; private set; }
        public static event Action<P_StateNames, P_StateNames> OnPlayerStateChanged;

        private RaycastHit[] _probedHits = new RaycastHit[8];
        public Vector3 _moveInputVector;



        public Vector3 InternalVelocityAdd = Vector3.zero;
        public bool _shouldBeCrouching = false;
        public bool _cannotTarget = false;

        private Vector3 lastInnerNormal = Vector3.zero;
        private Vector3 lastOuterNormal = Vector3.zero;

        Camera _cam;

        [Header("Acceleration")]

        public AnimationCurve AccelerateCurve;
        public AnimationCurve DecelerateCurve;
        public AnimationCurve SlideDecelerateCurve;
        [Range(0, 10f)] public float AccelTime = 2f;
        [Range(0, 10f)] public float DecelTime = 2f;
        [Range(0, 10f)] public float TurnSlideTime = 4f;
        [Range(0f, 0.7f)] public float SlideDuration = 0.15f;


        [Header("Event References")]
        [SerializeField] DialogueEventsSO dialogueEvent;
        [SerializeField] public FloatEventSO RecenterCamX;
        [SerializeField] public FloatEventSO RecenterCamY;
        [SerializeField] public GeneralEventSO DisableRecentering;
        [SerializeField] public UIEventChannelSO UIText;
        [SerializeField] GeneralEventSO EnableControls;
        [SerializeField] GeneralEventSO DisableControls;
        [SerializeField] ActionSO SetPlayerPosRot;
        [SerializeField] public ActionSO TurnOnTimeScaleZeroTick;


        [Tooltip("For ReadOnly Purposes, Dont Change for inspector")]
        public P_StateNames CurrentRootName;

        [Tooltip("For ReadOnly Purposes, Dont Change for inspector")]
        public P_StateNames CurrentSubName;

        [Tooltip("For ReadOnly Purposes, Dont Change for inspector")]
        public P_StateNames PreviousState;

        private P_BaseState _currentState;
        public P_BaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
        private P_StateFactory _states;
        public P_StateFactory CurrentFactory { get { return _states; } }

        private void OnEnable()
        {
            GameStateManager.OnGameStateChanged += CheckGameState;
            SetPlayerPosRot.OnSetPosRot += SetTransientPosRot;
            dialogueEvent.onEnter += SetTarget;
            dialogueEvent.onExit += EnterDefault;
        }
        private void OnDisable()
        {
            GameStateManager.OnGameStateChanged -= CheckGameState;
            SetPlayerPosRot.OnSetPosRot -= SetTransientPosRot;
            dialogueEvent.onEnter -= SetTarget;
            dialogueEvent.onExit -= EnterDefault;
        }
        private void Awake()
        {
            SetInitialReferences();
            _currentState.Init();
            // Handle initial state
            //_currentState.SwitchStates(_states.GetBaseState(P_StateNames.Default));

            // Assign the characterController to the motor
            Motor.CharacterController = this;
        }
        private void SetInitialReferences()
        {
            _animator = GetComponent<Animator>();
            _playerClimb = GetComponent<PlayerClimb>();
            _fallOffPrevention = GetComponent<FallOffPrevention>();

            MaxStableMoveSpeed = 2;

        }
        void CheckGameState(GameState state)
        {
            CurrentGameState = state;

            if (state == GameState.GAME_PLAYING)
            {
                //if(!_shouldBeCrouching && !_hasTargetToLockOn)
                //_currentState.SwitchStates(_states.GetBaseState(P_StateNames.Default));
                // I dont want to switch after pausing
                if (CurrentRootName == P_StateNames.CutsceneControl)
                    _currentState.SwitchStates(_states.GetBaseState(P_StateNames.Default));
            }
            else if (state == GameState.CUTSCENE)
            {
                if (CurrentRootName == P_StateNames.Talking)
                    _currentState.SwitchStates(_states.GetBaseState(P_StateNames.CutsceneControl));
            }
        }
        void SetTarget(GameObject npc, TextAsset inkFile)
        {
            Target = npc.transform;
        }
        private void Update()
        {
            _currentState.UpdateStates();
        }
        /// <summary>
        /// Handles movement state transitions and enter/exit callbacks
        /// </summary>
        public void CallStateChange() => OnPlayerStateChanged?.Invoke(CurrentRootName, CurrentSubName);

        private void EnterDefault()
        {
            if (CurrentRootName != P_StateNames.Default)
            {
                _currentState.SwitchStates(_states.GetBaseState(P_StateNames.Default));
            }
        }

        /// <summary>
        /// Event when entering a state
        /// </summary>
        public void OnStateEnter(P_BaseState newState)
        {
            _currentState.OnStateEnter(newState);
        }

        /// <summary>
        /// Event when exiting a state
        /// </summary>
        public void OnStateExit()
        {
            _currentState.OnStateExit();
        }

        /// <summary>
        /// This is called every frame by ExamplePlayer in order to tell the character what its inputs are
        /// </summary>
        /// 

        public void SetInputs(ref PlayerCharacterInputs inputs)
        {
            _currentState.SetInputs(ref inputs);
        }

        /// <summary>
        /// This is called every frame by the AI script in order to tell the character what its inputs are
        /// </summary>
        public void SetInputs(ref AICharacterInputs inputs)
        {
            _moveInputVector = inputs.MoveVector;
            //_lookInputVector = inputs.LookVector;
        }

        private Quaternion _tmpTransientRot;


        /// <summary>
        /// (Called by KinematicCharacterMotor during its update cycle)
        /// This is called before the character begins its movement update
        /// </summary>
        public void BeforeCharacterUpdate(float deltaTime)
        {
        }
        /// <summary>
        /// (Called by KinematicCharacterMotor during its update cycle)
        /// This is where you tell your character what its rotation should be right now. 
        /// This is the ONLY place where you should set the character's rotation
        /// </summary>
        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            _currentState.UpdateRotation(ref currentRotation, deltaTime);
        }

        /// <summary>
        /// (Called by KinematicCharacterMotor during its update cycle)
        /// This is where you tell your character what its velocity should be right now. 
        /// This is the ONLY place where you can set the character's velocity
        /// </summary>
        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            _currentState.UpdateVelocity(ref currentVelocity, deltaTime);
        }

        /// <summary>
        /// (Called by KinematicCharacterMotor during its update cycle)
        /// This is called after the character has finished its movement update
        /// </summary>
        public void AfterCharacterUpdate(float deltaTime)
        {
            _currentState.AfterCharacterUpdate(deltaTime);
        }

        public void PostGroundingUpdate(float deltaTime)
        {
            // Handle landing and leaving ground
            if (Motor.GroundingStatus.IsStableOnGround && !Motor.LastGroundingStatus.IsStableOnGround)
            {
                OnLanded();
            }
            else if (!Motor.GroundingStatus.IsStableOnGround && Motor.LastGroundingStatus.IsStableOnGround)
            {
                OnLeaveStableGround();
            }
        }

        public bool IsColliderValidForCollisions(Collider coll)
        {
            if (IgnoredColliders.Count == 0)
            {
                return true;
            }

            if (IgnoredColliders.Contains(coll))
            {
                return false;
            }

            return true;
        }

        //Need To be In same Order As Enums & Fmod "Surface" Parameter. Also same spelling as Tags in TagManager.
        List<string> _groundTypeTags = new List<string>() { "Grass", "Sand", "Stone", "Wood", "ShallowWater" };
        int _currentTagIndex;
        float _groundHitTick;

        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
            //Slows the Amount of Calls
            _groundHitTick += Time.deltaTime;
            if (_groundHitTick < 0.1f) return;
            _groundHitTick = 0;

            LastGroundedPosition = Motor.TransientPosition - (hitStabilityReport.LedgeFacingDirection.normalized);
            LastGroundedRotation = Motor.TransientRotation;

            int tagIndex = _groundTypeTags.IndexOf(hitCollider.tag);
            if (tagIndex == _currentTagIndex) return;
            CurrentGroundType = (GroundType)tagIndex;
            _currentTagIndex = tagIndex;

            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Surface", (float)_currentTagIndex);
        }

        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
        }

        public void AddVelocity(Vector3 velocity)
        {
            InternalVelocityAdd += velocity;
        }

        public Vector3 LastGroundedPosition { get; private set; }
        public Quaternion LastGroundedRotation { get; private set; }
        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        {
            CurrentHitStabilityReport = hitStabilityReport;
        }

        protected void OnLanded()
        {   //Prevent player to ground self if decided to Hang
            if (_isHanging) return;

            if (_hardLanding) // Need a delay to input and climbability after hard landing.
            {
                _animator.CrossFade(_hardLandState, 0f, 0);
                _hardLanding = false;
            }
            else
            {
                _animator.SetBool(anim_landTrigger, true);
            }
            IsFalling = false;
            _timeFallingInAir = 0f;
            if (CurrentGameState == GameState.GAME_PLAYING)
            {
                print("Here2");
                _currentState.SwitchStates(_states.GetBaseState(P_StateNames.Default));
            }// could make bool for landing
        }

        protected void OnLeaveStableGround()
        { //This is called if player is off ground,
          // I need to know if player has jumped.
            _animator.SetBool(anim_landTrigger, false);

            switch (CurrentRootName)
            {
                case P_StateNames.Default: // one for climbing
                    {
                        Debug.Log("jump");

                        _animator.CrossFade(_jumpState, 0, 0);
                        IsFalling = true;
                        FallingCheck();
                        CurrentState.JumpRequested = true;
                        break;
                    }

            }

        }
        public void CapsuleEnable(bool enable = true)
        {
            Motor.Capsule.enabled = enable;
        }
        void SetTransientPosRot(Vector3 loc, float lerpSpeed, Quaternion rot, bool LerpPosition)
        {
            if (LerpPosition)// Does Not work while Time.TimeScale = 0.
                Motor.SetTransientPosition(loc, true, lerpSpeed);
            //Motor.LerpPosition(loc, 5f);
            else
                Motor.SetPosition(loc, false);

            Motor.SetRotation(rot);
        }


        public void DropToHang() => StartCoroutine(EnumerateDropToHang());
        IEnumerator EnumerateDropToHang()
        {
            print("droptohang");
            _gettingOnOffObstacle = true;
            LedgeDirection = CurrentHitStabilityReport.LedgeFacingDirection;
            _timeFallingInAir = 0;
            IsFalling = false;
            _playerClimb._isClimbing = true;
            _isHanging = true;
            IsDropToHang = true;
            Gravity = Vector3.zero;
            CapsuleEnable(false);
            _animator.CrossFadeInFixedTime(anim_DropToHang, 0.1f, 0);
            Vector3 goalPos = transform.TransformPoint(0, -1.3f, 0.1f);
            _currentState.SwitchStates(_states.GetBaseState(P_StateNames.Hanging));

            float timer = 0;
            while (timer < 0.6f)
            {
                timer += Time.deltaTime;
                Motor.SetTransientPosition(goalPos, true, 5);
                yield return null;
            }
            _gettingOnOffObstacle = false;
            IsDropToHang = false;
            print("done");
            yield break;

        }
        
        public void FallingCheck() // make into coroutine
        {
            StartCoroutine(FallingTimer());
            // if (!_startFallingTimer) return;

            // _timeFallingInAir += Time.deltaTime;

            // if (_timeFallingInAir >= _timeToTriggerHardLanding)
            // {
            //     _hardLanding = true;
            // }
        }

        IEnumerator FallingTimer()
        {
            _hardLanding = false;
            _timeFallingInAir = 0;

            while (IsFalling)
            {
                _timeFallingInAir += Time.deltaTime;
                if (_timeFallingInAir >= _timeToTriggerHardLanding)
                {
                    _hardLanding = true;
                }
                yield return null;
            }

            _timeFallingInAir = 0;

            yield break;
        }

        public void OnDiscreteCollisionDetected(Collider hitCollider)
        {
        }
    }
}
