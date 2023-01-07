using UnityEngine;
using MyTownProject.Events;

namespace MyTownProject.NPC
{
    public class NPC_StateMachine : MonoBehaviour
    {
        [field: SerializeField] public NPC_BaseState CurrentState { get; set; }
        private NPC_StateFactory _states;

        #region References
        [field: SerializeField] public NPC_ScriptableObject NPC { get; private set; }
        [SerializeField] public Transform TargetTransform { get; private set; }
        #endregion

        #region Values
        [field: SerializeField] public float RotSpeed { get; private set; }
        #endregion
        
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
            dialogueEvents.onExit += ChangeState;
        }
        void OnDisable()
        {
            stateChanger.OnNPCStateVoid -= ChangeState;
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
        public void EnterTalkingState(GameObject npc, Transform target)
        {
            if (this.gameObject != npc) return;
            TargetTransform = target;

            CurrentState.SwitchStates(_states.Talk());

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