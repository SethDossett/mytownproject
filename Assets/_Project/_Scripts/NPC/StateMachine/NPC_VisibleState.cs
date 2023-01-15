using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace MyTownProject.NPC{
public class NPC_VisibleState : NPC_BaseState
{
    public NPC_VisibleState(NPC_StateMachine currentContext, NPC_StateFactory npcStateFactory)
    : base(currentContext,npcStateFactory){
        IsRootState = true;
    }

    public override void EnterState(){
        Debug.Log("Enter Visible State");
        InitSubState();
    }
    public override void UpdateState(){
        CheckSwitchStates();
    }
    public override void FixedUpdateState(){}
    public override void ExitState(){}
    public override void CheckSwitchStates(){
        if(Ctx.NPC.currentScene != SceneManager.GetActiveScene().buildIndex){
            SwitchStates(Factory.Invisible());
        }
    }
    public override void InitSubState(){
        SetSubState(Factory.Idle());
    }
}
}
