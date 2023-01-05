using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyTownProject.NPC{
public class NPC_StateMachine : MonoBehaviour
{
    [field: SerializeField] public NPC_BaseState CurrentState {get; set;}
    private NPC_StateFactory _states;

    [SerializeField] public NPC_ScriptableObject NPC {get; set;}


    private void Awake() {
        _states = new NPC_StateFactory(this);
        CurrentState = _states.Idle();
        CurrentState.EnterState();
    }
    private void Update() {
        CurrentState.UpdateState();
    }
    private void FixedUpdate() {
        CurrentState.FixedUpdateState();
    }
}
}