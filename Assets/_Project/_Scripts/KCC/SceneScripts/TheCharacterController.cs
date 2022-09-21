using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyTownProject.Events;
using KinematicCharacterController;
using System;
using UnityEngine.InputSystem;

namespace KinematicCharacterController.Examples
{
    public enum CharacterState
    {
        Default, Jumping, Talking, Climbing, Targeting, Crawling
    }
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

    public class TheCharacterController : MonoBehaviour, ICharacterController
    {
        public KinematicCharacterMotor Motor;

        [Header("Stable Movement")]
        public float MaxStableMoveSpeed = 0f;
        public float StableMovementSharpness = 15f;
        public float OrientationSharpness = 10f;
        public OrientationMethod OrientationMethod = OrientationMethod.TowardsCamera;

        [Header("Air Movement")]
        public float MaxAirMoveSpeed = 15f;
        public float AirAccelerationSpeed = 15f;
        public float Drag = 0.1f;

        [Header("Jumping")]
        public bool AllowJumpingWhenSliding = false;
        public float JumpUpSpeed = 10f;
        public float JumpScalableForwardSpeed = 10f;
        public float JumpPreGroundingGraceTime = 0f;
        public float JumpPostGroundingGraceTime = 0f;

        [Header("Climbing")]
        public Vector3 _newCenteredPosition;
        public Quaternion _newLadderRotation;
        public float _climbSpeedY = 2f;
        public float _climbSpeedX = 2f;
        bool _onLadder = false;
        public bool _gettingOnOffLadder;
        [SerializeField] float _jumpOnLadderSpeed = 5f;

        [Header("Targeting")]
        public Transform _target;
        [SerializeField] float _targetingRotSpeed = 500f;
        public bool _hasTargetToLockOn;

        [Header("Crawling")]
        [SerializeField] float _crawlSpeed = 2f;
        bool _hasFinishedCrouch;

        [Header("Falling")]
        [SerializeField] float _timeFallingInAir = 0f;
        bool _startFallingTimer = false;
        bool _hardLanding = false;
        [SerializeField] float _timetotriggerHardLanding = 0.5f;

        [Header("Misc")]
        public List<Collider> IgnoredColliders = new List<Collider>();
        public BonusOrientationMethod BonusOrientationMethod = BonusOrientationMethod.None;
        public float BonusOrientationSharpness = 10f;
        public Vector3 Gravity = new Vector3(0, -30f, 0);
        public Transform MeshRoot;
        public Transform CameraFollowPoint;
        public float CrouchedCapsuleHeight = 1f;
        [SerializeField] float _talkingRotSpeed = 500f;

        [Header("Animation")]
        Animator _animator;
        int _talkState = Animator.StringToHash("Talking");
        int _idleState = Animator.StringToHash("Idle");
        int _climbState = Animator.StringToHash("Climbing");
        int _strafeState = Animator.StringToHash("Strafing");
        int _jumpState = Animator.StringToHash("Jump");
        int _hardLandState = Animator.StringToHash("HardLanding");
        int anim_isCrouched = Animator.StringToHash("isCrouched");
        int anim_isClimbing = Animator.StringToHash("isClimbing");
        int anim_jumpTrigger = Animator.StringToHash("hasJumped");
        int anim_landTrigger = Animator.StringToHash("hasLanded");
        int anim_moving = Animator.StringToHash("Moving");
        int anim_talking = Animator.StringToHash("isTalking");
        int anim_horizontal = Animator.StringToHash("Horizontal");
        int anim_vertical = Animator.StringToHash("Vertical");

        


        public CharacterState CurrentCharacterState { get; private set; }
        public GroundType CurrentGroundType { get; private set; }
        public static event Action<CharacterState> OnPlayerStateChanged;

        private Collider[] _probedColliders = new Collider[8];
        private RaycastHit[] _probedHits = new RaycastHit[8];
        public Vector3 _moveInputVector;
        private Vector3 _lookInputVector;
        private Vector3 _rawMoveInputVector;
        private bool _jumpRequested = false;
        private bool _jumpConsumed = false;
        private bool _jumpedThisFrame = false;
        private float _timeSinceJumpRequested = Mathf.Infinity;
        private float _timeSinceLastAbleToJump = 0f;
        private Vector3 _internalVelocityAdd = Vector3.zero;
        private bool _canCrouch = false;
        private bool _shouldBeCrouching = false;
        public bool _isCrouching = false;
        public bool _cannotTarget = false;

        private Vector3 lastInnerNormal = Vector3.zero;
        private Vector3 lastOuterNormal = Vector3.zero;

        Camera cam;

        [Header("Acceleration")]
        float MaxSpeed = 6f;
        [SerializeField] float accelTime = 3f;
        float RatePerSecond;
        float turnAroundBuffer;
        [Range(0f,0.7f)][SerializeField] float _slideDuration = 0.15f;
        bool changingDirection = false;
        
        [SerializeField] DialogueEventsSO dialogueEvent;
        [SerializeField] FloatEventSO RecenterCamX;
        [SerializeField] FloatEventSO RecenterCamY;
        [SerializeField] UIEventChannelSO UIText;


        private void OnEnable() {
            dialogueEvent.onEnter += (GameObject npc, TextAsset inkFile) => _target = npc.transform;
        }
        private void OnDisable() {
            dialogueEvent.onEnter -= (GameObject npc, TextAsset inkFile) => _target = npc.transform;
        }
        private void Awake()
        {
            SetInitialReferences();
            // Handle initial state
            TransitionToState(CharacterState.Default);

            // Assign the characterController to the motor
            Motor.CharacterController = this;

        }
        private void Update()
        {
            FallingCheck();
        }
        /// <summary>
        /// Handles movement state transitions and enter/exit callbacks
        /// </summary>
        public void TransitionToState(CharacterState newState)
        {
            CharacterState tmpInitialState = CurrentCharacterState;
            OnStateExit(tmpInitialState, newState);
            CurrentCharacterState = newState;
            OnStateEnter(newState, tmpInitialState);
        }

        /// <summary>
        /// Event when entering a state
        /// </summary>
        public void OnStateEnter(CharacterState state, CharacterState fromState)
        {
            switch (state)
            {
                case CharacterState.Default:
                    {
                        //_animator.CrossFadeInFixedTime(_idleState, 0.2f, 0);
                        //MaxStableMoveSpeed = 0; // think it might look better to be able to run out of climbing.
                        break;
                    }
                case CharacterState.Jumping:
                    {
                        
                        break;
                    }    
                case CharacterState.Climbing:
                    {
                        _animator.CrossFadeInFixedTime(_climbState, 0.2f, 0);
                        break;
                    }
                case CharacterState.Targeting:
                    {
                        _animator.CrossFadeInFixedTime(_strafeState, 0.25f, 0);
                        break;
                    }
                case CharacterState.Talking:
                    {
                        _animator.CrossFadeInFixedTime(_talkState, 0.25f, 0);
                        break;
                    } 
                case CharacterState.Crawling:
                    {
                        _canCrouch = false;
                        break;
                    }        
                
            }
            OnPlayerStateChanged?.Invoke(state);

        }

        /// <summary>
        /// Event when exiting a state
        /// </summary>
        public void OnStateExit(CharacterState state, CharacterState toState)
        {
            switch (state)
            {
                case CharacterState.Default:
                    {
                        break;
                    }
                case CharacterState.Jumping:
                    {
                        
                        break;
                    }       
                case CharacterState.Climbing:
                    {
                        _onLadder = false;
                        print("EXIT CLIMB");
                        _animator.CrossFadeInFixedTime(_idleState, 0, 0);
                        break;
                    }
                case CharacterState.Targeting:
                    {
                        _animator.CrossFadeInFixedTime(_idleState, 0.25f, 0);
                        break;
                    }
                case CharacterState.Talking:
                    {
                        _animator.CrossFadeInFixedTime(_idleState, 0.2f, 0);
                        break;
                    } 
                case CharacterState.Crawling:
                    {
                        _hasFinishedCrouch = false;
                        if (_animator.speed != 1f)
                            _animator.speed = 1f;

                        _canCrouch = true;     
                        break;
                    }          
            }


        }

        /// <summary>
        /// This is called every frame by ExamplePlayer in order to tell the character what its inputs are
        /// </summary>
        /// 
        private void SetInitialReferences()
        {
            _animator = GetComponent<Animator>();
            cam = Camera.main;
            RatePerSecond = MaxSpeed / accelTime;
            MaxStableMoveSpeed = 2;

        }
        public void SetInputs(ref PlayerCharacterInputs inputs)
        {
            // Clamp input
            //Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(inputs.MoveAxisRight, 0f, inputs.MoveAxisForward), 1f);
            Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(inputs.MoveDirection.x, 0f, inputs.MoveDirection.y), 1f);

            // Calculate camera direction and rotation on the character plane
            Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.forward, Motor.CharacterUp).normalized;
            if (cameraPlanarDirection.sqrMagnitude == 0f)
            {
                cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.up, Motor.CharacterUp).normalized;
            }
            Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, Motor.CharacterUp);

            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                    {
                        // Move and look inputs
                        _moveInputVector = cameraPlanarRotation * moveInputVector;

                        switch (OrientationMethod)
                        {
                            case OrientationMethod.TowardsCamera:
                                _lookInputVector = cameraPlanarDirection;
                                break;
                            case OrientationMethod.TowardsMovement:
                                _lookInputVector = _moveInputVector.normalized;
                                break;
                        }

                        // Jumping input
                        if (inputs.JumpDown)
                        {
                            _timeSinceJumpRequested = 0f;
                            _jumpRequested = true;
                        }

                        //Show or hide UI Text for Crouch
                        if(_canCrouch) UIText.ShowTextInteract("Crouch");
                        else UIText.HideTextInteract();

                        // Crouching input
                        if (inputs.CrouchDown && _canCrouch)
                        {
                            _shouldBeCrouching = true;

                            if (!_isCrouching)
                            {
                                _isCrouching = true;
                                Motor.SetCapsuleDimensions(0.5f, CrouchedCapsuleHeight, CrouchedCapsuleHeight * 0.5f);
                                //MeshRoot.localScale = new Vector3(1f, 0.5f, 1f);
                                if (_animator.GetBool(anim_isCrouched) != true)
                                    _animator.SetBool(anim_isCrouched, true);


                                //RecenterCamX.RaiseEvent2(0, 0.1f);
                                //RecenterCamY.RaiseEvent2(0, 0.1f);

                                TransitionToState(CharacterState.Crawling);  
                                Debug.Log("Crouch");
                            }
                        }
                        
                        break;
                    }
                case CharacterState.Jumping:
                    {
                        
                        break;
                    }       
                case CharacterState.Climbing:
                    {
                        Vector3 ladderInputVector = Vector3.ClampMagnitude(new Vector3(inputs.MoveDirection.x, inputs.MoveDirection.y, 0f), 1f);

                        // Move and look inputs
                        _moveInputVector = cameraPlanarRotation * ladderInputVector;

                        switch (OrientationMethod)
                        {
                            case OrientationMethod.TowardsCamera:
                                _lookInputVector = cameraPlanarDirection;
                                break;
                            case OrientationMethod.TowardsMovement:
                                _lookInputVector = _moveInputVector.normalized;
                                break;
                        }
                        break;
                    }
                case CharacterState.Targeting:
                    {
                        // Move and look inputs
                        _moveInputVector = cameraPlanarRotation * moveInputVector;
                        _rawMoveInputVector = moveInputVector.normalized;

                        switch (OrientationMethod)
                        {
                            case OrientationMethod.TowardsCamera:
                                _lookInputVector = cameraPlanarDirection;
                                break;
                            case OrientationMethod.TowardsMovement:
                                _lookInputVector = _moveInputVector.normalized;
                                break;
                        }

                        break;
                    }
                case CharacterState.Talking:
                    {
                        break;
                    }
                case CharacterState.Crawling:
                    {
                        // Move and look inputs
                        _moveInputVector = cameraPlanarRotation * moveInputVector;

                        switch (OrientationMethod)
                        {
                            case OrientationMethod.TowardsCamera:
                                _lookInputVector = cameraPlanarDirection;
                                break;
                            case OrientationMethod.TowardsMovement:
                                _lookInputVector = _moveInputVector.normalized;
                                break;
                        }

                        if (inputs.CrouchUp)
                        {
                            _shouldBeCrouching = false;
                        }
                        break;
                    }               
            }
        }

        /// <summary>
        /// This is called every frame by the AI script in order to tell the character what its inputs are
        /// </summary>
        public void SetInputs(ref AICharacterInputs inputs)
        {
            _moveInputVector = inputs.MoveVector;
            _lookInputVector = inputs.LookVector;
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
            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                    {
                        if (_lookInputVector.sqrMagnitude > 0f && OrientationSharpness > 0f)
                        {
                            // Smoothly interpolate from current to target look direction
                            Vector3 smoothedLookInputDirection = Vector3.Slerp(Motor.CharacterForward, _lookInputVector, 1 - Mathf.Exp(-OrientationSharpness * deltaTime)).normalized;

                            // Set the current rotation (which will be used by the KinematicCharacterMotor)
                            currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, Motor.CharacterUp);
                        }

                        Vector3 currentUp = (currentRotation * Vector3.up);
                        if (BonusOrientationMethod == BonusOrientationMethod.TowardsGravity)
                        {
                            // Rotate from current up to invert gravity
                            Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, -Gravity.normalized, 1 - Mathf.Exp(-BonusOrientationSharpness * deltaTime));
                            currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
                        }
                        else if (BonusOrientationMethod == BonusOrientationMethod.TowardsGroundSlopeAndGravity)
                        {
                            if (Motor.GroundingStatus.IsStableOnGround)
                            {
                                Vector3 initialCharacterBottomHemiCenter = Motor.TransientPosition + (currentUp * Motor.Capsule.radius);

                                Vector3 smoothedGroundNormal = Vector3.Slerp(Motor.CharacterUp, Motor.GroundingStatus.GroundNormal, 1 - Mathf.Exp(-BonusOrientationSharpness * deltaTime));
                                currentRotation = Quaternion.FromToRotation(currentUp, smoothedGroundNormal) * currentRotation;

                                // Move the position to create a rotation around the bottom hemi center instead of around the pivot
                                Motor.SetTransientPosition(initialCharacterBottomHemiCenter + (currentRotation * Vector3.down * Motor.Capsule.radius), false);
                            }
                            else
                            {
                                Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, -Gravity.normalized, 1 - Mathf.Exp(-BonusOrientationSharpness * deltaTime));
                                currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
                            }
                        }
                        else
                        {
                            Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, Vector3.up, 1 - Mathf.Exp(-BonusOrientationSharpness * deltaTime));
                            currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
                        }
                        break;
                    }
                case CharacterState.Jumping:
                    {
                        
                        break;
                    }       
                case CharacterState.Climbing:
                    {
                        //Quaternion lookRot = Quaternion.LookRotation(_newCenteredPosition, Vector3.up);
                        currentRotation = Quaternion.RotateTowards(transform.rotation, _newLadderRotation, 500 * Time.deltaTime);   

                        break;
                    }
                case CharacterState.Targeting:
                    {
                        //if not Target then dont rotate
                        if (!_hasTargetToLockOn) return;
                        Quaternion lookRot = Quaternion.LookRotation(_target.position - transform.position, Vector3.up);
                        lookRot.z = 0;
                        lookRot.x = 0;
                        currentRotation = Quaternion.RotateTowards(transform.rotation, lookRot, _targetingRotSpeed * Time.deltaTime);
                        break;
                    }
                case CharacterState.Talking:
                    {
                        Quaternion lookRot = Quaternion.LookRotation(_target.position - transform.position, Vector3.up);
                        lookRot.z = 0;
                        lookRot.x = 0;
                        currentRotation = Quaternion.RotateTowards(transform.rotation, lookRot, _talkingRotSpeed * Time.unscaledDeltaTime);
                        print("WOrk?");
                        break;
                    } 
                case CharacterState.Crawling:
                    {
                        if (_lookInputVector.sqrMagnitude > 0f && OrientationSharpness > 0f)
                        {
                            // Smoothly interpolate from current to target look direction
                            Vector3 smoothedLookInputDirection = Vector3.Slerp(Motor.CharacterForward, _lookInputVector, 1 - Mathf.Exp(-OrientationSharpness * deltaTime)).normalized;

                            // Set the current rotation (which will be used by the KinematicCharacterMotor)
                            currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, Motor.CharacterUp);
                        }

                        Vector3 currentUp = (currentRotation * Vector3.up);
                        if (BonusOrientationMethod == BonusOrientationMethod.TowardsGravity)
                        {
                            // Rotate from current up to invert gravity
                            Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, -Gravity.normalized, 1 - Mathf.Exp(-BonusOrientationSharpness * deltaTime));
                            currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
                        }
                        else if (BonusOrientationMethod == BonusOrientationMethod.TowardsGroundSlopeAndGravity)
                        {
                            if (Motor.GroundingStatus.IsStableOnGround)
                            {
                                Vector3 initialCharacterBottomHemiCenter = Motor.TransientPosition + (currentUp * Motor.Capsule.radius);

                                Vector3 smoothedGroundNormal = Vector3.Slerp(Motor.CharacterUp, Motor.GroundingStatus.GroundNormal, 1 - Mathf.Exp(-BonusOrientationSharpness * deltaTime));
                                currentRotation = Quaternion.FromToRotation(currentUp, smoothedGroundNormal) * currentRotation;

                                // Move the position to create a rotation around the bottom hemi center instead of around the pivot
                                Motor.SetTransientPosition(initialCharacterBottomHemiCenter + (currentRotation * Vector3.down * Motor.Capsule.radius), false);
                            }
                            else
                            {
                                Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, -Gravity.normalized, 1 - Mathf.Exp(-BonusOrientationSharpness * deltaTime));
                                currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
                            }
                        }
                        else
                        {
                            Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, Vector3.up, 1 - Mathf.Exp(-BonusOrientationSharpness * deltaTime));
                            currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
                        }
                        break;
                    }              
            }
        }

        /// <summary>
        /// (Called by KinematicCharacterMotor during its update cycle)
        /// This is where you tell your character what its velocity should be right now. 
        /// This is the ONLY place where you can set the character's velocity
        /// </summary>
        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                    {
                        // Ground movement
                        if (Motor.GroundingStatus.IsStableOnGround)
                        {
                            Vector3 prevInput = _moveInputVector.normalized;

                            float dot = Vector3.Dot(prevInput, transform.forward);

                            float currentVelocityMagnitude = currentVelocity.magnitude;

                            Vector3 effectiveGroundNormal = Motor.GroundingStatus.GroundNormal;
                            if (currentVelocityMagnitude > 0f && Motor.GroundingStatus.SnappingPrevented)
                            {
                                // Take the normal from where we're coming from
                                Vector3 groundPointToCharacter = Motor.TransientPosition - Motor.GroundingStatus.GroundPoint;
                                if (Vector3.Dot(currentVelocity, groundPointToCharacter) >= 0f)
                                {
                                    effectiveGroundNormal = Motor.GroundingStatus.OuterGroundNormal;
                                }
                                else
                                {
                                    effectiveGroundNormal = Motor.GroundingStatus.InnerGroundNormal;
                                }
                            }

                            // Reorient velocity on slope
                            currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) * currentVelocityMagnitude;
                            
                            // Accelerate MaxSpeed
                            
                            //Determine if change in other direction
                            if(currentVelocity.magnitude < 0.1f && changingDirection == false){
                                if(dot < 0.95f){
                                    MaxStableMoveSpeed = 0;
                                }
                                else{
                                    if(MaxStableMoveSpeed < 4) MaxStableMoveSpeed = 4;
                                    if(MaxStableMoveSpeed != MaxSpeed){
                                        MaxStableMoveSpeed += RatePerSecond * deltaTime;
                                        MaxStableMoveSpeed = Mathf.Min(MaxStableMoveSpeed, MaxSpeed);
                                    }
                                
                                }
                            }
                            else{
                                if(MaxStableMoveSpeed >= MaxSpeed){
                                    if(dot < -0.92f){
                                        changingDirection = true;
                                        MaxStableMoveSpeed = 0;
                                        _animator.CrossFade("ChangeDirection", 0, 0);
                                    }
                                }
                                else{
                                    if(changingDirection){
                                        turnAroundBuffer += deltaTime;

                                        if(turnAroundBuffer > _slideDuration){
                                            //MaxStableMoveSpeed = 2f;
                                            changingDirection = false;
                                            turnAroundBuffer = 0;
                                        }
                                    }
                                    else{
                                        if(_moveInputVector.magnitude < 0.1f){
                                            MaxStableMoveSpeed = 0; //might need to change so movement is not odd with thumbstick.
                                        }
                                        else{
                                            //Walk
                                            if (_moveInputVector.magnitude < 0.3f) MaxStableMoveSpeed = 1f;
                                            else
                                            {
                                                //Run
                                                if (MaxStableMoveSpeed < 4) MaxStableMoveSpeed = 4;
                                                if (MaxStableMoveSpeed != MaxSpeed)
                                                {
                                                    MaxStableMoveSpeed += RatePerSecond * deltaTime;
                                                    MaxStableMoveSpeed = Mathf.Min(MaxStableMoveSpeed, MaxSpeed);
                                                }
                                            }
                                        }
                                        
                                    }
                                }
                            }
                            
                            // Calculate target velocity
                            Vector3 inputRight = Vector3.Cross(_moveInputVector, Motor.CharacterUp);
                            Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized * _moveInputVector.magnitude;
                            Vector3 targetMovementVelocity = reorientedInput * MaxStableMoveSpeed;
                            
                            // Smooth movement Velocity
                            currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1f - Mathf.Exp(-StableMovementSharpness * deltaTime));
                            _animator.SetFloat(anim_moving, currentVelocityMagnitude, 0f, Time.deltaTime);
                            //Dont Think I Need below
                            //_animator.SetFloat(anim_horizontal, currentVelocity.x, 0.1f, Time.deltaTime);
                            //_animator.SetFloat(anim_vertical, currentVelocity.y, 0.1f, Time.deltaTime);
                            

                            if (currentVelocityMagnitude <= 0f) _canCrouch = true;   
                            else _canCrouch = false;
                        }
                        // Air movement
                        else
                        {
                            _canCrouch = false;
                            // Add move input
                            if (_moveInputVector.sqrMagnitude > 0f)
                            {
                                Vector3 addedVelocity = _moveInputVector * AirAccelerationSpeed * deltaTime;

                                Vector3 currentVelocityOnInputsPlane = Vector3.ProjectOnPlane(currentVelocity, Motor.CharacterUp);

                                // Limit air velocity from inputs
                                if (currentVelocityOnInputsPlane.magnitude < MaxAirMoveSpeed)
                                {
                                    // clamp addedVel to make total vel not exceed max vel on inputs plane
                                    Vector3 newTotal = Vector3.ClampMagnitude(currentVelocityOnInputsPlane + addedVelocity, MaxAirMoveSpeed);
                                    addedVelocity = newTotal - currentVelocityOnInputsPlane;
                                }
                                else
                                {
                                    // Make sure added vel doesn't go in the direction of the already-exceeding velocity
                                    if (Vector3.Dot(currentVelocityOnInputsPlane, addedVelocity) > 0f)
                                    {
                                        addedVelocity = Vector3.ProjectOnPlane(addedVelocity, currentVelocityOnInputsPlane.normalized);
                                    }
                                }

                                // Prevent air-climbing sloped walls
                                if (Motor.GroundingStatus.FoundAnyGround)
                                {
                                    if (Vector3.Dot(currentVelocity + addedVelocity, addedVelocity) > 0f)
                                    {
                                        Vector3 perpenticularObstructionNormal = Vector3.Cross(Vector3.Cross(Motor.CharacterUp, Motor.GroundingStatus.GroundNormal), Motor.CharacterUp).normalized;
                                        addedVelocity = Vector3.ProjectOnPlane(addedVelocity, perpenticularObstructionNormal);
                                    }
                                }

                                // Apply added velocity
                                currentVelocity += addedVelocity;
                                
                            }

                            // Gravity
                            currentVelocity += Gravity * deltaTime;

                            // Drag
                            currentVelocity *= (1f / (1f + (Drag * deltaTime)));
                        }
                        //Needs To be put in its own state.
                        // Handle jumping 
                        _jumpedThisFrame = false;
                        _timeSinceJumpRequested += deltaTime;
                        if (_jumpRequested)
                        {
                            // See if we actually are allowed to jump
                            if (!_jumpConsumed && ((AllowJumpingWhenSliding ? Motor.GroundingStatus.FoundAnyGround : Motor.GroundingStatus.IsStableOnGround) || _timeSinceLastAbleToJump <= JumpPostGroundingGraceTime))
                            {
                                // Calculate jump direction before ungrounding
                                Vector3 jumpDirection = Motor.CharacterUp;
                                if (Motor.GroundingStatus.FoundAnyGround && !Motor.GroundingStatus.IsStableOnGround)
                                {
                                    jumpDirection = Motor.GroundingStatus.GroundNormal;
                                }

                                // Makes the character skip ground probing/snapping on its next update. 
                                // If this line weren't here, the character would remain snapped to the ground when trying to jump. Try commenting this line out and see.
                                Motor.ForceUnground();

                                // Add to the return velocity and reset jump state
                                //currentVelocity += (jumpDirection * JumpUpSpeed) - Vector3.Project(currentVelocity, Motor.CharacterUp);
                                currentVelocity = (jumpDirection * JumpUpSpeed) - Vector3.Project(currentVelocity, Motor.CharacterUp);
                                currentVelocity += (_moveInputVector * JumpScalableForwardSpeed);
                                _jumpRequested = false;
                                _jumpConsumed = true;
                                _jumpedThisFrame = true;
                                _animator.SetBool(anim_jumpTrigger, true);
                            }
                        }

                        // Take into account additive velocity
                        if (_internalVelocityAdd.sqrMagnitude > 0f)
                        {
                            currentVelocity += _internalVelocityAdd;
                            _internalVelocityAdd = Vector3.zero;
                        }
                        break;
                    }
                case CharacterState.Jumping:
                    {
                        
                        break;
                    }       
                case CharacterState.Climbing:
                    {
                        if(_gettingOnOffLadder){
                            currentVelocity.y = 0f;
                            currentVelocity.x = 0f;
                            currentVelocity.z = 0f;
                            if (_animator.speed != 1f)
                                _animator.speed = 1f;
                            return;
                        } 
                        if(Motor.GroundingStatus.IsStableOnGround) Motor.ForceUnground();

                        Vector3 newCenteredPosition = _newCenteredPosition;    

                        float currentVelocityMagnitude = currentVelocity.magnitude;
                        
                        if(!_onLadder){
                            currentVelocity.y = 0f;
                            currentVelocity.x = 0f;
                            currentVelocity.z = 0f;
                            if (_animator.speed != 1f)
                                _animator.speed = 1f;
                            //Vector3 ladderStartPos = new Vector3(newCenteredPosition.x,transform.position.y, newCenteredPosition.z);
                            //Vector3 newPlayerPos = Vector3.Lerp(Motor.TransientPosition, newCenteredPosition, 1f - Mathf.Exp(-StableMovementSharpness * deltaTime));
                            //Vector3 newPlayerPos = Vector3.MoveTowards(Motor.TransientPosition, newCenteredPosition, _jumpOnLadderSpeed * deltaTime);
                            Motor.SetTransientPosition(newCenteredPosition, true);
                            if(Vector3.Distance(Motor.TransientPosition, newCenteredPosition) <= float.Epsilon){
                                _onLadder = true;
                                print("ladder is true");
                            }
                            else _onLadder = false; 
                        }
                        else{
                            currentVelocity.y = _moveInputVector.y * _climbSpeedY;
                            //currentVelocity.z = -_moveInputVector.x * _climbSpeedX;
                            currentVelocity.x = 0;
                            currentVelocity.z = 0;

                            if(currentVelocity.y == 0f)
                            {
                                if(currentVelocity != Vector3.zero) currentVelocity = Vector3.zero;
                                if (_animator.speed != 0f)
                                    _animator.speed = 0f;
                            }
                            else
                            {
                                if (_animator.speed != 1f)
                                    _animator.speed = 1f;
                            }
                        }
                        
                        

                        // Reorient velocity on slope
                        //currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) * currentVelocityMagnitude;
                        //// Calculate target velocity
                        //Vector3 inputRight = Vector3.Cross(_moveInputVector, Motor.CharacterUp);
                        //Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized * _moveInputVector.magnitude;
                        //Vector3 targetMovementVelocity = reorientedInput * MaxStableMoveSpeed;
                        //// Smooth movement Velocity
                        //currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1f - Mathf.Exp(-StableMovementSharpness * deltaTime));;
                        break;
                    }
                case CharacterState.Targeting:
                    {
                        // Ground movement
                        if (Motor.GroundingStatus.IsStableOnGround)
                        {
                            MaxStableMoveSpeed = 4;
                            float currentVelocityMagnitude = currentVelocity.magnitude;

                            Vector3 effectiveGroundNormal = Motor.GroundingStatus.GroundNormal;
                            if (currentVelocityMagnitude > 0f && Motor.GroundingStatus.SnappingPrevented)
                            {
                                // Take the normal from where we're coming from
                                Vector3 groundPointToCharacter = Motor.TransientPosition - Motor.GroundingStatus.GroundPoint;
                                if (Vector3.Dot(currentVelocity, groundPointToCharacter) >= 0f)
                                {
                                    effectiveGroundNormal = Motor.GroundingStatus.OuterGroundNormal;
                                }
                                else
                                {
                                    effectiveGroundNormal = Motor.GroundingStatus.InnerGroundNormal;
                                }
                            }

                            // Reorient velocity on slope
                            currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) * currentVelocityMagnitude;

                            // Calculate target velocity
                            Vector3 inputRight = Vector3.Cross(_moveInputVector, Motor.CharacterUp);
                            Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized * _moveInputVector.magnitude;
                            Vector3 targetMovementVelocity = reorientedInput * MaxStableMoveSpeed;

                            // Smooth movement Velocity
                            currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1f - Mathf.Exp(-StableMovementSharpness * deltaTime));
                            _animator.SetFloat(anim_moving, currentVelocityMagnitude, 0f, Time.deltaTime);
                            _animator.SetFloat(anim_horizontal, _moveInputVector.x, 0f, Time.deltaTime);
                            _animator.SetFloat(anim_vertical, _moveInputVector.z, 0, Time.deltaTime);
                        }
                        // Air movement
                        else
                        {
                            _canCrouch = false;
                            // Add move input
                            if (_moveInputVector.sqrMagnitude > 0f)
                            {
                                Vector3 addedVelocity = _moveInputVector * AirAccelerationSpeed * deltaTime;

                                Vector3 currentVelocityOnInputsPlane = Vector3.ProjectOnPlane(currentVelocity, Motor.CharacterUp);

                                // Limit air velocity from inputs
                                if (currentVelocityOnInputsPlane.magnitude < MaxAirMoveSpeed)
                                {
                                    // clamp addedVel to make total vel not exceed max vel on inputs plane
                                    Vector3 newTotal = Vector3.ClampMagnitude(currentVelocityOnInputsPlane + addedVelocity, MaxAirMoveSpeed);
                                    addedVelocity = newTotal - currentVelocityOnInputsPlane;
                                }
                                else
                                {
                                    // Make sure added vel doesn't go in the direction of the already-exceeding velocity
                                    if (Vector3.Dot(currentVelocityOnInputsPlane, addedVelocity) > 0f)
                                    {
                                        addedVelocity = Vector3.ProjectOnPlane(addedVelocity, currentVelocityOnInputsPlane.normalized);
                                    }
                                }

                                // Prevent air-climbing sloped walls
                                if (Motor.GroundingStatus.FoundAnyGround)
                                {
                                    if (Vector3.Dot(currentVelocity + addedVelocity, addedVelocity) > 0f)
                                    {
                                        Vector3 perpenticularObstructionNormal = Vector3.Cross(Vector3.Cross(Motor.CharacterUp, Motor.GroundingStatus.GroundNormal), Motor.CharacterUp).normalized;
                                        addedVelocity = Vector3.ProjectOnPlane(addedVelocity, perpenticularObstructionNormal);
                                    }
                                }

                                // Apply added velocity
                                currentVelocity += addedVelocity;

                            }

                            // Gravity
                            currentVelocity += Gravity * deltaTime;

                            // Drag
                            currentVelocity *= (1f / (1f + (Drag * deltaTime)));
                        }
                        // Take into account additive velocity
                        if (_internalVelocityAdd.sqrMagnitude > 0f)
                        {
                            currentVelocity += _internalVelocityAdd;
                            _internalVelocityAdd = Vector3.zero;
                        }

                        break;
                    }
                case CharacterState.Talking:
                    {
                        float currentVelocityMagnitude = currentVelocity.magnitude;
                        currentVelocity = Vector3.zero;
                        _animator.SetFloat(anim_moving, currentVelocityMagnitude, 0f, Time.deltaTime);
                        _animator.SetFloat(anim_horizontal, currentVelocity.x, 0.1f, Time.deltaTime);
                        _animator.SetFloat(anim_vertical, currentVelocity.y, 0.1f, Time.deltaTime);
                        break;
                    }
                case CharacterState.Crawling:
                    {
                        //if (!Motor.GroundingStatus.IsStableOnGround) return;
                        if (!_hasFinishedCrouch)
                        {
                            if (currentVelocity != Vector3.zero) currentVelocity = Vector3.zero;
                            _animator.SetFloat(anim_moving, 0f, 0f, Time.deltaTime);

                            if (_animator.speed != 1f)
                                _animator.speed = 1f;

                            if(_moveInputVector.magnitude > 0f)
                            {
                                _hasFinishedCrouch = true;
                            }
                            return;
                        }

                        float currentVelocityMagnitude = currentVelocity.magnitude;

                        Vector3 effectiveGroundNormal = Motor.GroundingStatus.GroundNormal;
                        if (currentVelocityMagnitude > 0f && Motor.GroundingStatus.SnappingPrevented)
                        {
                            // Take the normal from where we're coming from
                            Vector3 groundPointToCharacter = Motor.TransientPosition - Motor.GroundingStatus.GroundPoint;
                            if (Vector3.Dot(currentVelocity, groundPointToCharacter) >= 0f)
                            {
                                effectiveGroundNormal = Motor.GroundingStatus.OuterGroundNormal;
                            }
                            else
                            {
                                effectiveGroundNormal = Motor.GroundingStatus.InnerGroundNormal;
                            }
                        }

                        MaxStableMoveSpeed = 2f;
                        if(_moveInputVector.magnitude == 0)
                        {
                            if (_animator.speed != 0f)
                                _animator.speed = 0f;
                        }
                        else
                        {
                            if (_animator.speed != 1f)
                                _animator.speed = 1f;
                        }


                        // Reorient velocity on slope
                        currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) * currentVelocityMagnitude;

                        // Calculate target velocity
                        Vector3 inputRight = Vector3.Cross(_moveInputVector, Motor.CharacterUp);
                        Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized * _moveInputVector.magnitude;
                        Vector3 targetMovementVelocity = reorientedInput * MaxStableMoveSpeed;

                        // Smooth movement Velocity
                        currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1f - Mathf.Exp(-StableMovementSharpness * deltaTime));
                        _animator.SetFloat(anim_moving, currentVelocityMagnitude, 0f, Time.deltaTime);
                        _animator.SetFloat(anim_horizontal, currentVelocity.x, 0.1f, Time.deltaTime);
                        _animator.SetFloat(anim_vertical, currentVelocity.y, 0.1f, Time.deltaTime);
                        break;
                    }               
            }
        }

        /// <summary>
        /// (Called by KinematicCharacterMotor during its update cycle)
        /// This is called after the character has finished its movement update
        /// </summary>
        public void AfterCharacterUpdate(float deltaTime)
        {
            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                    {
                        

                        // Handle jump-related values
                        {
                            // Handle jumping pre-ground grace period
                            if (_jumpRequested && _timeSinceJumpRequested > JumpPreGroundingGraceTime)
                            {
                                _jumpRequested = false;
                            }

                            if (AllowJumpingWhenSliding ? Motor.GroundingStatus.FoundAnyGround : Motor.GroundingStatus.IsStableOnGround)
                            {
                                // If we're on a ground surface, reset jumping values
                                if (!_jumpedThisFrame)
                                {
                                    _jumpConsumed = false;
                                }
                                _timeSinceLastAbleToJump = 0f;
                            }
                            else
                            {
                                // Keep track of time since we were last able to jump (for grace period)
                                _timeSinceLastAbleToJump += deltaTime;
                            }
                        }

                        break;
                    }
                    case CharacterState.Jumping:
                    {
                        
                        break;
                    }   
                    case CharacterState.Crawling:
                    {
                        //Show or hide UI Text for Crouch
                        if(_canCrouch) UIText.ShowTextInteract("Crouch");
                        else UIText.HideTextInteract();    

                        // Handle uncrouching
                        if (_isCrouching && !_shouldBeCrouching)
                        {
                            // Do an overlap test with the character's standing height to see if there are any obstructions
                            Motor.SetCapsuleDimensions(0.5f, 2f, 1f);
                            if (Motor.CharacterOverlap(
                                Motor.TransientPosition,
                                Motor.TransientRotation,
                                _probedColliders,
                                Motor.CollidableLayers,
                                QueryTriggerInteraction.Ignore) > 0)
                            {
                                // If obstructions, just stick to crouching dimensions
                                Motor.SetCapsuleDimensions(0.5f, CrouchedCapsuleHeight, CrouchedCapsuleHeight * 0.5f);
                            }
                            else
                            {
                                // If no obstructions, uncrouch
                                //MeshRoot.localScale = new Vector3(1f, 1f, 1f);
                                _isCrouching = false;
                                if (_animator.GetBool(anim_isCrouched) != false)
                                    _animator.SetBool(anim_isCrouched, false);

                                TransitionToState(CharacterState.Default);   
                                Debug.Log("Uncrouched");
                            }
                        }

                        break;
                    } 
            }
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

        string stone = "Stone";
        string grass = "Grass";
        string sand = "Sand";
        string wood = "Wood";
        string shallowWater = "ShallowWater";
        string currentString;

        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
            string tag = hitCollider.tag;
            if (tag == currentString) {
                return;
            }
            

            if (tag == stone) {
                currentString = tag;
                CurrentGroundType = GroundType.Stone;
            }
            else if (tag == grass) {
                currentString = tag;
                CurrentGroundType = GroundType.Grass;
            }
            else if (tag == sand) { 
            
                currentString = tag;
                CurrentGroundType = GroundType.Sand;
            }
            else if (tag == wood) {
                currentString = tag;
                CurrentGroundType = GroundType.Wood;
            }
            else if (tag == shallowWater) {
                currentString = tag;
                CurrentGroundType = GroundType.ShallowWater;
            }

            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Surface", (float)CurrentGroundType);
        }

        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
        }

        public void AddVelocity(Vector3 velocity)
        {
            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                    {
                        _internalVelocityAdd += velocity;
                        break;
                    }
                case CharacterState.Targeting:
                    {
                        _internalVelocityAdd += velocity;
                        break;
                    }
            }
        }

        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        {
        }

        protected void OnLanded()
        {
            if(_hardLanding){
                _startFallingTimer = false;
                _animator.CrossFade(_hardLandState, 0f, 0);
                _hardLanding = false;
                _timeFallingInAir = 0f;
            }
            else{
                _animator.SetBool(anim_landTrigger, true); 
                _timeFallingInAir = 0f;
                _startFallingTimer = false;
            }
            
        }

        protected void OnLeaveStableGround()
        { //This is called if player is off ground,
          // I need to know if player has jumped.
            _animator.SetBool(anim_landTrigger, false); 

            switch(CurrentCharacterState){ 
                case CharacterState.Default:
                    {
                        Debug.Log("jump");
                        
                        _animator.CrossFade(_jumpState, 0, 0);
                        _startFallingTimer = true;
                        _jumpRequested = true;
                        break;
                    }
            }            
            
        }
        void FallingCheck(){
            if(!_startFallingTimer) return;

            _timeFallingInAir += Time.deltaTime;

            if(_timeFallingInAir >= _timetotriggerHardLanding){
                _hardLanding = true;
            }



        }

        public void OnDiscreteCollisionDetected(Collider hitCollider)
        {
        }
    }
}
