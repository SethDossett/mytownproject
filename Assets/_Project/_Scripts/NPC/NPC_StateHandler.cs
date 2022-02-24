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
            _animator.SetBool(_isStanding, false);
            _animator.SetBool(_isWalking, false);
        }

        private void HandleTalking()
        {
            _animator.SetBool(_isWalking, false);
            Debug.Log("Talking to Player");
        }

        private void HandleWalking()
        {
            _animator.SetBool(_isStanding, false);
            _animator.SetBool(_isWalking, true);
            Debug.Log("Walking");
        }

        private void HandleStanding()
        {
            _animator.SetBool(_isWalking, false);
            _animator.SetBool(_isStanding, true);
        }


        #endregion

        #region Changing State

        [SerializeField] DialogueEventsSO dialogueEvents;
        
        void OnEnable()
        {
            dialogueEvents.onEnter += EnterTalkingState;
            dialogueEvents.onExit += ReturnToBaseState;
        }
        void OnDisable()
        {
            dialogueEvents.onEnter -= EnterTalkingState;
            dialogueEvents.onExit -= ReturnToBaseState;
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