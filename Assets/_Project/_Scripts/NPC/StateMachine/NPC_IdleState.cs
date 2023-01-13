using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyTownProject.NPC{
public class NPC_IdleState : NPC_BaseState
{
    int _isStanding = Animator.StringToHash("isStanding");
    public NPC_IdleState(NPC_StateMachine currentContext, NPC_StateFactory npcStateFactory)
    : base(currentContext,npcStateFactory){}

    public override void EnterState(){
        Debug.Log("Idle State Entered");
        Ctx.NpcAnimator.SetTrigger(_isStanding);
    }
    public override void UpdateState(){
        CheckSwitchStates();
    }
    public override void FixedUpdateState(){}
    public override void ExitState(){}
    public override void CheckSwitchStates(){
        if(!Ctx.AI.reachedEndOfPath && Ctx.AI.canMove){
            SwitchStates(Factory.Walk());
        }
    }
    public override void InitSubState(){}
}
}