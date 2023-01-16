using UnityEngine;
using MyTownProject.Events;
using MyTownProject.SO;
using Pathfinding;

namespace MyTownProject.NPC
{
    public class NPC_StateMachine : MonoBehaviour
    {
        [Tooltip("For ReadOnly Purposes, Dont Change for inspector")]
        public NPC_StateNames CurrentRootName;

        [Tooltip("For ReadOnly Purposes, Dont Change for inspector")]
        public NPC_StateNames CurrentSubName;

        private NPC_BaseState _currentState;
        public NPC_BaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
        private NPC_StateFactory _states;

        #region References
        [field: SerializeField] public NPC_ScriptableObject NPC { get; private set; }
        public Rigidbody Rb { get; private set; }
        public Animator NpcAnimator { get; private set; }
        public AILerp AI { get; private set; }
        [SerializeField] public Transform TargetTransform { get; private set; }
        [field: SerializeField] public PathSO CurrentPath { get; private set; }
        #endregion

        #region Values
        [field: SerializeField] public float RotSpeed { get; private set; }
        [field: SerializeField] public AnimationCurve FaceTargetCurve { get; private set; }
        [field: SerializeField] public bool HasStandingDir { get; private set; }
        [field: SerializeField] public Quaternion StandingDir { get; private set; }

        #endregion

        [Header("Movement Values")]
        public bool MoveByRecorded;
        public bool MoveByPathfinding;
        public float PathSpeed;
        public float PathRotSpeed;

        private void Awake()
        {
            AI = GetComponent<AILerp>();
            Rb = GetComponent<Rigidbody>();
            NpcAnimator = GetComponent<Animator>();

            _states = new NPC_StateFactory(this);
            CurrentRootName = NPC.CurrentRootName;
            CurrentSubName = NPC.CurrentSubName;
            _currentState = _states.GetBaseState(CurrentRootName);
            _currentState.EnterState();
        }
        private void Update()
        {
            _currentState.UpdateStates();
        }
        private void FixedUpdate()
        {
            //Need FixedUpdateStates();
            _currentState.FixedUpdateState();
        }

        #region Changing State
        [SerializeField] DialogueEventsSO dialogueEvents;
        [SerializeField] StateChangerEventSO stateChanger;

        void OnEnable()
        {
            //stateChanger.OnNPCStateVoid += ChangeState;
            dialogueEvents.onExit += ReturnToBaseState;
        }
        void OnDisable()
        {
            //stateChanger.OnNPCStateVoid -= ChangeState;
            dialogueEvents.onExit -= ReturnToBaseState;
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