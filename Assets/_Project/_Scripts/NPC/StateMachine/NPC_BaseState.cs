namespace MyTownProject.NPC
{
    public abstract class NPC_BaseState
    {
        private bool _isRootState = false;
        private NPC_StateMachine _ctx;
        private NPC_StateFactory _factory;
        private NPC_BaseState _currentSuperState;
        private NPC_BaseState _currentSubState;

        public bool IsRootState { get { return _isRootState; } set { _isRootState = value; } }
        protected NPC_StateMachine Ctx { get { return _ctx; } }
        protected NPC_StateFactory Factory { get { return _factory; } }

        public NPC_BaseState(NPC_StateMachine currentContext, NPC_StateFactory npcStateFactory)
        {
            _ctx = currentContext;
            _factory = npcStateFactory;
        }

        public abstract void EnterState();
        public abstract void UpdateState();
        public abstract void FixedUpdateState();
        public abstract void ExitState();
        public abstract void CheckSwitchStates();
        public abstract void InitSubState();

        public void UpdateStates()
        {
            UpdateState();
            
            if (_currentSubState != null) _currentSubState.UpdateStates();
            
        }
        public void SwitchStates(NPC_BaseState newState)
        {
            ExitState();

            newState.EnterState();

            if (newState._isRootState)
            {
                _ctx.CurrentState = newState;
            }
            else if (_currentSuperState != null)
            {
                _currentSuperState.SetSubState(newState);
            }
        }
        protected void SetSuperState(NPC_BaseState newSuperState)
        {
            _currentSuperState = newSuperState;
        }
        protected void SetSubState(NPC_BaseState newSubState)
        {
            _currentSubState = newSubState;
            newSubState.SetSuperState(this);
        }

    }
}
