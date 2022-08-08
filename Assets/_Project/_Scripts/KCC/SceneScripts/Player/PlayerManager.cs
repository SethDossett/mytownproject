using System.Collections;
using UnityEngine.Events;
using UnityEngine;
using MyTownProject.SO;
using MyTownProject.Events;

namespace KinematicCharacterController.Examples
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] TransformEventSO PlayerRef;
        [SerializeField] ActionSO teleportPlayer;
        [SerializeField] GeneralEventSO UntargetEvent;
        TheCharacterController cc;
        public Transform _LookAtPoint;
        public UnityAction<TheCharacterController> OnCharacterTeleport;
        public bool isBeingTeleportedTo { get; set; }

        Animator _animator;

        private void OnEnable()
        {
            teleportPlayer.OnTeleport += TeleportPlayer;
            UntargetEvent.OnRaiseEvent += Untargeting;
            TheCharacterController.OnPlayerStateChanged += StateChange;
        }
        private void OnDisable()
        {
            teleportPlayer.OnTeleport -= TeleportPlayer;
            UntargetEvent.OnRaiseEvent -= Untargeting;
            TheCharacterController.OnPlayerStateChanged -= StateChange;
        }
        private void Awake()
        {
            cc = GetComponent<TheCharacterController>();
            _animator = GetComponent<Animator>();
            
            PlayerRef.RaiseEvent(transform);
        }
        private void TeleportPlayer(Vector3 location, Quaternion rotation)
        {
            if (!isBeingTeleportedTo)
            {
                if (cc)
                {
                    cc.Motor.SetPositionAndRotation(location, rotation);

                    if (OnCharacterTeleport != null)
                    {
                        OnCharacterTeleport(cc);
                    }
                    this.isBeingTeleportedTo = true;
                }
            }

            isBeingTeleportedTo = false;
        }
        private void Untargeting(){
            cc.TransitionToState(CharacterState.Default);
        }
        void StateChange(CharacterState state){
            if(state == CharacterState.Default){
                DefaultState();
            }
            if(state == CharacterState.Climbing){
                ClimbingState();
            }
            if(state == CharacterState.Targeting){
                TargetingState();
            }
        }
        void DefaultState(){
            _animator.SetLayerWeight(0, 1);
            _animator.SetLayerWeight(1, 0);
            _animator.SetLayerWeight(2, 0);
        }
        void ClimbingState(){
            _animator.SetLayerWeight(2, 1);
            _animator.SetLayerWeight(0, 0);
            _animator.SetLayerWeight(1, 0);
        }
        void TargetingState(){
            _animator.SetLayerWeight(1, 1);
            _animator.SetLayerWeight(0, 0);
            _animator.SetLayerWeight(2, 0);
        }
       
    }
}