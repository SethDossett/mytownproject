using UnityEngine;
using MyTownProject.Events;
using MyTownProject.SO;
using Pathfinding;

namespace MyTownProject.NPC
{
    public class NPC_StateMachine : MonoBehaviour
    {
        [SerializeField] private NPC_BaseState _currentState;
        public NPC_BaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
        private NPC_StateFactory _states;

        #region References
        [field: SerializeField] public NPC_ScriptableObject NPC { get; private set; }
        public Rigidbody Rb { get; private set; }
        public Animator NpcAnimator { get; private set; }
        public IAstarAI AI { get; private set; }
        [SerializeField] public Transform TargetTransform { get; private set; }
        [field: SerializeField] public PathSO CurrentPath { get; private set; }
        #endregion

        #region Values
        [field: SerializeField] public float RotSpeed { get; private set; }
        #endregion

        [Header("Recorded Movement")]
        public bool IsReplay;

        private void Awake()
        {
            AI = GetComponent<IAstarAI>();
            Rb = GetComponent<Rigidbody>();
            NpcAnimator = GetComponent<Animator>();
            _states = new NPC_StateFactory(this);
            _currentState = _states.Visible();
            _currentState.EnterState();
        }
        private void Update()
        {
            _currentState.UpdateState();
        }
        private void FixedUpdate()
        {
            _currentState.FixedUpdateState();
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
            _currentState.SwitchStates(_states.Walk());
        }
        public void EnterTalkingState(GameObject npc, Transform target)
        {
            if (this.gameObject != npc) return;
            TargetTransform = target;

            _currentState.SwitchStates(_states.Talk());
        }
        void ReturnToBaseState()
        {
            _currentState.SwitchStates(_states.Idle());
        }
        public void ResetData()
        {
            CurrentPath.Records.Clear();
        }


        #endregion
    }
}