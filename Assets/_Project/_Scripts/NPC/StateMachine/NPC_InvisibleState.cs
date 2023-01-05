using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyTownProject.NPC{
public class NPC_InvisibleState : NPC_BaseState
{
    public NPC_InvisibleState(NPC_StateMachine currentContext, NPC_StateFactory npcStateFactory)
    : base(currentContext,npcStateFactory){
        IsRootState = true;
    }

    public override void EnterState(){
        InitSubState();
    }
    public override void UpdateState(){
        CheckSwitchStates();
    }
    public override void FixedUpdateState(){}
    public override void ExitState(){}
    public override void CheckSwitchStates(){
        if(Ctx.NPC.currentScene == SceneManager.GetActiveScene().buildIndex){
            SwitchStates(Factory.Invisible());
        }
    }
    public override void InitSubState(){
        //If not walking return, 
        //else SetSubState(Factory.Walk());
    }
}
}
