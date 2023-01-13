using System.Collections.Generic;

namespace MyTownProject.NPC
{
    public class NPC_StateFactory
    {
        public enum NPC_StateNames
        {
            Visible, Invisible, Idle, Walk, Talk
        }

        Dictionary<NPC_StateNames, NPC_BaseState> _states = new Dictionary<NPC_StateNames, NPC_BaseState>();

        NPC_StateMachine _context;

        public NPC_StateFactory(NPC_StateMachine currentContext){
            this._context = currentContext;
            _states[NPC_StateNames.Visible] = new NPC_VisibleState(_context, this);
            _states[NPC_StateNames.Invisible] = new NPC_InvisibleState(_context, this);
            _states[NPC_StateNames.Idle] = new NPC_IdleState(_context, this);
            _states[NPC_StateNames.Walk] = new NPC_WalkState(_context, this);
            _states[NPC_StateNames.Talk] = new NPC_TalkState(_context, this);

        }

        public NPC_BaseState Visible() {return _states[NPC_StateNames.Visible];}
        public NPC_BaseState Invisible() {return _states[NPC_StateNames.Invisible];}
        public NPC_BaseState Idle() {return _states[NPC_StateNames.Idle];}
        public NPC_BaseState Walk() {return _states[NPC_StateNames.Walk];}
        public NPC_BaseState Talk() {return _states[NPC_StateNames.Talk];}

    }
    
}
