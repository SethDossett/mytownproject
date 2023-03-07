using UnityEngine;
using MyTownProject.Core;
using MyTownProject.Interaction;
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

        [Tooltip("For ReadOnly Purposes, Dont Change for inspector")]
        public NPC_StateNames PreviousState;

        private NPC_BaseState _currentState;
        public NPC_BaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
        private NPC_StateFactory _states;

        #region References
        [field: SerializeField] public NPC_ScriptableObject NPC { get; private set; }
        private NPC_Interact _interactScript;
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
        [field: SerializeField] public int IdleRotY { get; private set; }
        #endregion

        #region Animator
        private int _isWalking = Animator.StringToHash("isWalking");
        public int IsWalking { get { return _isWalking; } }
        #endregion

        [Header("Movement Values")]
        public float PathRotSpeed;

        private void Awake()
        {
            AI = GetComponent<AILerp>();
            Rb = GetComponent<Rigidbody>();
            NpcAnimator = GetComponent<Animator>();
            _interactScript = GetComponent<NPC_Interact>();

            _states = new NPC_StateFactory(this);
            //This is going to be initialized with whatever state was saved last, and if no save then Idle()
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
            TimeManager.OnGlobalTick += CheckTime;
            dialogueEvents.onExit += ReturnToBaseState;
        }
        void OnDisable()
        {
            TimeManager.OnGlobalTick -= CheckTime;
            dialogueEvents.onExit -= ReturnToBaseState;
        }
        void CheckTime(int globalTick)
        {
            foreach (NPC_Action action in NPC.TimeActions)
            {
                int tick = action.ActionTick;
                if (tick < globalTick)
                {
                    // If we Remove then we need to instantiate Actions when time resets
                    //NPC.TimeActions.Remove(action);
                    continue;
                }
                else if (tick == globalTick)
                {
                    _interactScript.UpdateProperties(action.CanBeInteractedWith, action.CanBeTargeted);

                    HasStandingDir = action.hasDesiredRotation;
                    if (HasStandingDir) IdleRotY = action.DesiredIdleRotationY;

                    _currentState.SwitchStates(_states.GetBaseState(action.state));
                    break;
                }
                else
                {
                    continue;
                }
            }


        }

        void EnterWalkingState()
        {
            _currentState.SwitchStates(_states.GetBaseState(NPC_StateNames.Walk));
        }
        public void EnterTalkingState(GameObject npc, Transform target)
        {
            if (this.gameObject != npc) return;
            TargetTransform = target;

            _currentState.SwitchStates(_states.GetBaseState(NPC_StateNames.Talk));
        }
        void ReturnToBaseState()
        {
            //Need to Return to Previous state
            if (PreviousState == NPC_StateNames.Null)
            {
                Debug.LogWarning("No Previous State Found, Switching to Idle by default!");
                _currentState.SwitchStates(_states.GetBaseState(NPC_StateNames.Idle));
            }
            else _currentState.SwitchStates(_states.GetBaseState(PreviousState));
        }

        #endregion
    }
}