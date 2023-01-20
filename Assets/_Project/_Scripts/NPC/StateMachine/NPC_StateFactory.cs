using System.Collections.Generic;

namespace MyTownProject.NPC
{
    public enum NPC_StateNames
    {
        Null, Visible, Invisible, Idle, Walk, Talk
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

        public NPC_BaseState GetBaseState(NPC_StateNames newIndex)
        {
            _context.PreviousState = _context.CurrentRootName;
            _context.NPC.PreviousStateName = _context.CurrentRootName;

            if (_states[newIndex].IsRootState)
            {
                _context.CurrentRootName = newIndex;
                _context.NPC.CurrentRootName = newIndex;
            }
            else
            {
                _context.CurrentSubName = newIndex;
                _context.NPC.CurrentSubName = newIndex;
            }
            return _states[newIndex];
        }

    }

}
