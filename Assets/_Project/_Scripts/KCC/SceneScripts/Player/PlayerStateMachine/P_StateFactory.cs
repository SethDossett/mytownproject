using System.Collections.Generic;

namespace KinematicCharacterController.Examples
{
    public enum P_StateNames
    {
        Default, Climbing, Talking, ClimbLadder, Targeting, Crawling, Jumping, CutsceneControl, Hanging, Falling
    }
    public class P_StateFactory
    {
        private Dictionary<P_StateNames, P_BaseState> _states = new Dictionary<P_StateNames, P_BaseState>();
        private TheCharacterController _context;
        public P_StateFactory(TheCharacterController currentContext)
        {
            this._context = currentContext;
            _states[P_StateNames.Default] = new P_DefaultState(_context, this);
            _states[P_StateNames.Climbing] = new P_ClimbState(_context, this);
            _states[P_StateNames.Talking] = new P_TalkState(_context, this);
            _states[P_StateNames.ClimbLadder] = new P_LadderState(_context, this);
            _states[P_StateNames.Targeting] = new P_TargetingState(_context, this);
            _states[P_StateNames.Crawling] = new P_CrawlState(_context, this);
            _states[P_StateNames.Jumping] = new P_JumpState(_context, this);
            _states[P_StateNames.CutsceneControl] = new P_CutsceneState(_context, this);
            _states[P_StateNames.Hanging] = new P_HangState(_context, this);
            _states[P_StateNames.Falling] = new P_FallState(_context, this);
        }

        public P_BaseState GetBaseState(P_StateNames newIndex)
        {
            _context.PreviousState = _context.CurrentRootName;

            if (_states[newIndex].IsRootState)
            {
                _context.CurrentRootName = newIndex;
            }
            else
            {
                _context.CurrentSubName = newIndex;
            }
            return _states[newIndex];
        }
    }
}
