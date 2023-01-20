using UnityEngine;

namespace MyTownProject.NPC
{
    public class NPC_IdleState : NPC_BaseState
    {
        public NPC_IdleState(NPC_StateMachine currentContext, NPC_StateFactory npcStateFactory)
        : base(currentContext, npcStateFactory)
        {
            IsRootState = true;
        }

        public override void EnterState()
        {
            Debug.Log("Idle State Entered");
            Ctx.NpcAnimator.SetBool(Ctx.IsWalking, false);

            //If has a prefered standing position, rotate back to it ?
            if (Ctx.HasStandingDir)
            {
                Ctx.transform.rotation = Ctx.StandingDir;
            }
        }
        public override void UpdateState()
        {
            CheckSwitchStates();
        }
        public override void FixedUpdateState() { }
        public override void ExitState() { }
        public override void CheckSwitchStates() { }
        public override void InitSubState() { }
    }
}