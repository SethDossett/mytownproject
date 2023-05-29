using UnityEngine;
namespace KinematicCharacterController.Examples
{
    public abstract class P_BaseState
    {
        #region Base Variables
        private bool _isRootState = false;
        private TheCharacterController _ctx;
        private P_StateFactory _factory;

        public bool IsRootState { get { return _isRootState; } set { _isRootState = value; } }
        protected TheCharacterController Ctx { get { return _ctx; } }
        protected P_StateFactory Factory { get { return _factory; } }

        private P_BaseState _currentSuperState;
        private P_BaseState _currentSubState;
        #endregion

        #region Machine References
        protected Animator _baseAnimator;
        protected KinematicCharacterMotor _baseMotor;
        protected FallOffPrevention _baseFallOffPrevention;
        protected PlayerClimb _basePlayerClimb;
        protected Camera _baseMainCam;
        protected Transform _baseTransform;
        #endregion

        #region  Machine Variables
        protected float _baseMaxStableMoveSpeed;
        protected Vector3 _baseMoveInputVector;
        protected Vector3 _lookInputVector;
        public bool JumpRequested = false;

        #endregion

        protected int anim_moving = Animator.StringToHash("Moving");
        protected int anim_horizontal = Animator.StringToHash("Horizontal");
        protected int anim_vertical = Animator.StringToHash("Vertical");
        protected int _talkState = Animator.StringToHash("Talking");
        protected int _idleState = Animator.StringToHash("Idle");
        protected int anim_SpeedMultiplier = Animator.StringToHash("SpeedMultiplier");


        public P_BaseState(TheCharacterController currentContext, P_StateFactory p_StateFactory)
        {
            this._ctx = currentContext;
            this._factory = p_StateFactory;


        }

        public void Init()
        {
            _baseMotor = _ctx.Motor;
            _baseAnimator = _ctx.PlayerAnimator;
            _baseFallOffPrevention = _ctx.FallOffPrevention;
            _basePlayerClimb = _ctx.PlayerClimb;
            _baseMainCam = _ctx.CamMain;
            _baseTransform = _ctx.transform;
            _baseMaxStableMoveSpeed = _ctx.MaxStableMoveSpeed;
        }

        public virtual void OnStateEnter(P_BaseState state)
        {
            _baseMaxStableMoveSpeed = _ctx.MaxStableMoveSpeed;
            Debug.Log("Entering state: " + state);

            if (_isRootState)
            {
            }
        }
        public virtual void OnStateExit()
        {
            _ctx.MaxStableMoveSpeed = _baseMaxStableMoveSpeed;
        }
        public virtual void SetInputs(ref PlayerCharacterInputs inputs)
        {

        }
        public virtual void BeforeCharacterUpdate(float deltaTime) { }
        public virtual void UpdateRotation(ref Quaternion currentRotation, float deltaTime) { }
        public virtual void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime) { }
        public virtual void AfterCharacterUpdate(float deltaTime) { }
        public virtual void UpdateState() { }

        public void UpdateStates()
        {
            UpdateState();

            //if (_currentSubState != null) _currentSubState.UpdateStates();

        }
        public void SwitchStates(P_BaseState newState)
        {
            OnStateExit();

            newState.OnStateEnter(newState);

            if (newState._isRootState)
            {
                _ctx.CurrentState = newState;
            }
            else if (_currentSuperState != null)
            {
                _currentSuperState.SetSubState(newState);
            }

            _ctx.CallStateChange();
        }
        protected void SetSuperState(P_BaseState newSuperState)
        {
            _currentSuperState = newSuperState;
        }
        protected void SetSubState(P_BaseState newSubState)
        {
            _currentSubState = newSubState;
            newSubState.SetSuperState(this);
        }

    }
}
