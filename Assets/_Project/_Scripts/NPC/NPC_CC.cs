using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;


namespace MyTownProject.NPC
{
    public struct NPCCharacterInputs
    {
        public Vector3 MoveVector;
        public Vector3 LookVector;
    }

    public class NPC_CC : MonoBehaviour, ICharacterController
    {
        public enum NPCCharacterState
        {
            Default, Talking
        }

    

        public NPCCharacterState CurrentCharacterState { get; private set; }

        [Header("References")]
        public KinematicCharacterMotor Motor;
        private CharacterController characterController;
        Animator animator;

        [Header("Stable Movement")]
        [SerializeField] float MaxStableMoveSpeed = 10f;
        [SerializeField] float StableMovementSharpness = 15f;
        [SerializeField] float OrientationSharpness = 10f;

        [Header("Misc")]
        public List<Collider> IgnoredColliders = new List<Collider>();
        public float BonusOrientationSharpness = 10f;
        public Vector3 Gravity = new Vector3(0, -30f, 0);
        public Transform MeshRoot;
        public Transform CameraFollowPoint;
        public float CrouchedCapsuleHeight = 1f;

        private Collider[] _probedColliders = new Collider[8];
        private RaycastHit[] _probedHits = new RaycastHit[8];
        private Vector3 _internalVelocityAdd = Vector3.zero;

        private Vector3 _lookInputVector;
        private Vector3 _moveInputVector;

    


        private void Awake()
        {
            // Handle initial state
            TransitionToState(NPCCharacterState.Default);

            // Assign the characterController to the motor
            Motor.CharacterController = this;

            SetInitialReferences();
        }
        private void Update()
        {

        }
        /// <summary>
        /// Handles movement state transitions and enter/exit callbacks
        /// </summary>
        public void TransitionToState(NPCCharacterState newState)
        {
            NPCCharacterState tmpInitialState = CurrentCharacterState;
            OnStateExit(tmpInitialState, newState);
            CurrentCharacterState = newState;
            OnStateEnter(newState, tmpInitialState);
        }

        /// <summary>
        /// Event when entering a state
        /// </summary>
        public void OnStateEnter(NPCCharacterState state, NPCCharacterState fromState)
        {
            switch (state)
            {
                case NPCCharacterState.Default:
                    {
                        break;
                    }
            }
        }

        /// <summary>
        /// Event when exiting a state
        /// </summary>
        public void OnStateExit(NPCCharacterState state, NPCCharacterState toState)
        {
            switch (state)
            {
                case NPCCharacterState.Default:
                    {

                        break;
                    }
            }
        }
        private void SetInitialReferences()
        {
            animator = GetComponent<Animator>();
            characterController = GetComponent<CharacterController>();

        }

        public void SetInputs(ref NPCCharacterInputs inputs)
        {
            _moveInputVector = inputs.MoveVector;
            _lookInputVector = inputs.LookVector;
        }

        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            //if state is Talking to player, then rotate to face player;
            // can have npc rotate to face player within overlapSphere
            // but if Working than dont rotate until Talking

            _lookInputVector = _moveInputVector.normalized;

            if (_lookInputVector.sqrMagnitude > 0f && OrientationSharpness > 0f)
            {
                // Smoothly interpolate from current to target look direction
                Vector3 smoothedLookInputDirection = Vector3.Slerp(Motor.CharacterForward, _lookInputVector, 1 - Mathf.Exp(-OrientationSharpness * deltaTime)).normalized;

                // Set the current rotation (which will be used by the KinematicCharacterMotor)
                currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, Motor.CharacterUp);
            }

            Vector3 currentUp = (currentRotation * Vector3.up);

            Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, Vector3.up, 1 - Mathf.Exp(-BonusOrientationSharpness * deltaTime));
            currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
        }

        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            if (!Motor.GroundingStatus.IsStableOnGround)
            {
                currentVelocity += Gravity * deltaTime;
            }

            float currentVelocityMagnitude = currentVelocity.magnitude;

            Vector3 effectiveGroundNormal = Motor.GroundingStatus.GroundNormal;
            if (currentVelocityMagnitude > 0f && Motor.GroundingStatus.SnappingPrevented)
            {
                // Take the normal from where we're coming from
                Vector3 groundPointToCharacter = Motor.TransientPosition - Motor.GroundingStatus.GroundPoint;
                if (Vector3.Dot(currentVelocity, groundPointToCharacter) >= 0f)
                {
                    effectiveGroundNormal = Motor.GroundingStatus.OuterGroundNormal;
                }
                else
                {
                    effectiveGroundNormal = Motor.GroundingStatus.InnerGroundNormal;
                }
            }

            // Reorient velocity on slope
            currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) * currentVelocityMagnitude;

            // Calculate target velocity
            Vector3 inputRight = Vector3.Cross(_moveInputVector, Motor.CharacterUp);
            Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized * _moveInputVector.magnitude;
            Vector3 targetMovementVelocity = reorientedInput * MaxStableMoveSpeed;

            // Smooth movement Velocity
            currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1f - Mathf.Exp(-StableMovementSharpness * deltaTime));
            //sanimator.SetBool(moving, true);




            if (_internalVelocityAdd.sqrMagnitude > 0f)
            {
                currentVelocity += _internalVelocityAdd;
                _internalVelocityAdd = Vector3.zero;
            }
        }

        public void BeforeCharacterUpdate(float deltaTime)
        {
        }

        public void PostGroundingUpdate(float deltaTime)
        {
        }

        public void AfterCharacterUpdate(float deltaTime)
        {
        }

        public bool IsColliderValidForCollisions(Collider coll)
        {
            if (IgnoredColliders.Count == 0)
            {
                return true;
            }

            if (IgnoredColliders.Contains(coll))
            {
                return false;
            }

            return true;
        }

        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
        }

        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
        }

        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        {
        }

        public void OnDiscreteCollisionDetected(Collider hitCollider)
        {
        }
    }

}
