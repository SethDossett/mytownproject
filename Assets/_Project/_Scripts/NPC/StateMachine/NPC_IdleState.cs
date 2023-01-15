using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyTownProject.NPC
{
    public class NPC_IdleState : NPC_BaseState
    {
        int _isStanding = Animator.StringToHash("isStanding");
        public NPC_IdleState(NPC_StateMachine currentContext, NPC_StateFactory npcStateFactory)
        : base(currentContext, npcStateFactory) { }

        public override void EnterState()
        {
            Debug.Log("Idle State Entered");
            Ctx.NpcAnimator.SetTrigger(_isStanding);

            //If has a prefered standing position, rotate back to it ?
            if (Ctx.HasStandingDir)
            {
                Ctx.transform.rotation = Ctx.StandingDir;
            }
        }
        public override void UpdateState()
        {
            Debug.Log("update idle");
            CheckSwitchStates();
        }
        public override void FixedUpdateState() { }
        public override void ExitState() { }
        public override void CheckSwitchStates()
        {
            //Might not want a consistant checking for idle state which will be used all the time,
            //I want to stansfer to Walk based on event called, or check on time.
            if (!Ctx.AI.reachedEndOfPath && Ctx.AI.canMove)
            {
                SwitchStates(Factory.Walk());
            }
        }
        public override void InitSubState() { }
    }
}