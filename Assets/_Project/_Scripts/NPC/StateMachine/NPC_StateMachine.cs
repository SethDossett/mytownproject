using UnityEngine;
using MyTownProject.Events;

namespace MyTownProject.NPC
{
    public class NPC_StateMachine : MonoBehaviour
    {
        [field: SerializeField] public NPC_BaseState CurrentState { get; set; }
        private NPC_StateFactory _states;

        [SerializeField] public NPC_ScriptableObject NPC { get; set; }


        private void Awake()
        {
            _states = new NPC_StateFactory(this);
            CurrentState = _states.Visible();
            CurrentState.EnterState();
        }
        private void Update()
        {
            CurrentState.UpdateState();
        }
        private void FixedUpdate()
        {
            CurrentState.FixedUpdateState();
        }

        #region Changing State
        [SerializeField] DialogueEventsSO dialogueEvents;
        [SerializeField] StateChangerEventSO stateChanger;

        void OnEnable()
        {
            stateChanger.OnNPCStateVoid += ChangeState;
            dialogueEvents.onEnter += EnterTalkingState;
            dialogueEvents.onExit += ChangeState;
        }
        void OnDisable()
        {
            stateChanger.OnNPCStateVoid -= ChangeState;
            dialogueEvents.onEnter -= EnterTalkingState;
            dialogueEvents.onExit -= ChangeState;
        }
        void ChangeState()
        {
            if (NPC.moveTowardsDestination)
                EnterWalkingState();
            else
                ReturnToBaseState();
        }

        void EnterWalkingState()
        {
            // if (npcState != NPCSTATE.WALKING)
            //     UpdateNPCState(NPCSTATE.WALKING);
        }
        public void EnterTalkingState(GameObject npc, TextAsset inkJSON)
        {
            if (this.gameObject != npc) return;

            

            //if(NPC.currentState != NPCSTATE.TALKING) return;

            // if (npcState != NPCSTATE.TALKING)
            //     UpdateNPCState(NPCSTATE.TALKING);
        }
        void ReturnToBaseState()
        {
            // if (npcState != NPCSTATE.STANDING)
            //     UpdateNPCState(NPCSTATE.STANDING);
        }


        #endregion
    }
}