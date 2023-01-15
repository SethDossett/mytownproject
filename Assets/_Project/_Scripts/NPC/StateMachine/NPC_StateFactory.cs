using System.Collections.Generic;

namespace MyTownProject.NPC
{
    public enum NPC_StateNames
    {
        Visible, Invisible, Idle, Walk, Talk
    }
    public class NPC_StateFactory
    {
        Dictionary<NPC_StateNames, NPC_BaseState> _states = new Dictionary<NPC_StateNames, NPC_BaseState>();

        NPC_StateMachine _context;

        public NPC_StateFactory(NPC_StateMachine currentContext)
        {
            this._context = currentContext;
            _states[NPC_StateNames.Visible] = new NPC_VisibleState(_context, this);
            _states[NPC_StateNames.Invisible] = new NPC_InvisibleState(_context, this);
            _states[NPC_StateNames.Idle] = new NPC_IdleState(_context, this);
            _states[NPC_StateNames.Walk] = new NPC_WalkState(_context, this);
            _states[NPC_StateNames.Talk] = new NPC_TalkState(_context, this);

        }

        NPC_BaseState GetBaseState(NPC_StateNames newIndex)
        {
            if (_states[newIndex].IsRootState)
            {
                _context.CurrentRootName = newIndex;
            }
            else
            {
                _context.CurrenSubName = newIndex;
            }
            return _states[newIndex];
        }

        public NPC_BaseState Visible()
        {
            return GetBaseState(NPC_StateNames.Visible);
        }
        public NPC_BaseState Invisible()
        {
            return GetBaseState(NPC_StateNames.Invisible);
        }
        public NPC_BaseState Idle()
        {
            return GetBaseState(NPC_StateNames.Idle);
        }
        public NPC_BaseState Walk()
        {
            return GetBaseState(NPC_StateNames.Walk);
        }
        public NPC_BaseState Talk()
        {
            return GetBaseState(NPC_StateNames.Talk);
        }

    }

}
