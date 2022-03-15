using UnityEngine;
using System;
using MyTownProject.Events;

namespace MyTownProject.NPC
{
    public class NPC_StateHandler : MonoBehaviour
    {
        #region State of NPC
        
        public NPCSTATE npcState;
        public static event Action<NPCSTATE> OnNPCStateChange;

        public void UpdateNPCState(NPCSTATE newState)
        {
            npcState = newState;

            switch (newState)
            {
                case NPCSTATE.STANDING:
                    HandleStanding();
                    break;
                case NPCSTATE.WALKING:
                    HandleWalking();
                    break;
                case NPCSTATE.TALKING:
                    HandleTalking();
                    break;
                case NPCSTATE.WORKING:
                    HandleWorking();
                    break;
                default:
                    break;
            }

            OnNPCStateChange?.Invoke(newState);
            NPC.currentState = newState;
        }

        public enum NPCSTATE
        {
            STANDING, WALKING, TALKING, WORKING
        }
        #endregion

        #region State Logic
        Animator _animator;
        int _isWalking = Animator.StringToHash("isWalking");
        int _isStanding = Animator.StringToHash("isStanding");
        public static NPC_StateHandler instance;

        private void Awake()
        {
            instance = this;
            _animator = GetComponent<Animator>();
        }

        private void HandleWorking()
        {
            _animator.SetBool(_isWalking, false);
        }

        private void HandleTalking()
        {
            _animator.SetBool(_isWalking, false);
            NPC.RaiseChangedState(npcState);
            Debug.Log("Talking to Player");
        }

        private void HandleWalking()
        {
            _animator.SetTrigger(_isStanding);
            _animator.SetBool(_isWalking, true);
            Debug.Log("Walking");
        }

        private void HandleStanding()
        {
            _animator.SetBool(_isWalking, false);
            _animator.SetTrigger(_isStanding);
        }


        #endregion

        #region Changing State
        [SerializeField] NPC_ScriptableObject NPC;
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
            if (npcState != NPCSTATE.WALKING)
                UpdateNPCState(NPCSTATE.WALKING);
        }
        void EnterTalkingState(GameObject npc, TextAsset inkJSON)
        {
            if (npcState != NPCSTATE.TALKING)
                UpdateNPCState(NPCSTATE.TALKING);
        }
        void ReturnToBaseState()
        {
            if(npcState != NPCSTATE.STANDING)
                UpdateNPCState(NPCSTATE.STANDING);
        }


        #endregion
    }
}