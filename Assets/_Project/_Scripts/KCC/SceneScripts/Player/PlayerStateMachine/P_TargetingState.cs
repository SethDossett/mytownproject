using UnityEngine;

namespace KinematicCharacterController.Examples
{
    public class P_TargetingState : P_BaseState
    {
        private Vector3 _rawMoveInputVector;
        private Quaternion _cameraPlanarRotation;
        int _strafeState = Animator.StringToHash("Strafing");
        public P_TargetingState(TheCharacterController currentContext, P_StateFactory p_StateFactory)
        : base(currentContext, p_StateFactory)
        {
            IsRootState = true;
        }

        public override void OnStateEnter(P_BaseState state)
        {
            base.OnStateEnter(state);
            _baseAnimator.CrossFadeInFixedTime(_strafeState, 0.25f, 0);
        }

        public override void OnStateExit()
        {
            base.OnStateExit();

            _baseAnimator.CrossFadeInFixedTime(_idleState, 0.25f, 0);
        }

        public override void SetInputs(ref PlayerCharacterInputs inputs)
        {
            base.SetInputs(ref inputs);

            // Clamp input
            //Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(inputs.MoveAxisRight, 0f, inputs.MoveAxisForward), 1f);
            Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(inputs.MoveDirection.x, 0f, inputs.MoveDirection.y), 1f);

            // Calculate camera direction and rotation on the character plane
            Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(_baseMainCam.transform.rotation * Vector3.forward, _baseMotor.CharacterUp).normalized;
            if (cameraPlanarDirection.sqrMagnitude == 0f)
            {
                cameraPlanarDirection = Vector3.ProjectOnPlane(_baseMainCam.transform.rotation * Vector3.up, _baseMotor.CharacterUp).normalized;
            }
            Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, _baseMotor.CharacterUp);

            // Move and look inputs
            _baseMoveInputVector = cameraPlanarRotation * moveInputVector;
            _rawMoveInputVector = moveInputVector.normalized;
            _cameraPlanarRotation = cameraPlanarRotation;

            switch (Ctx.OrientationMethod)
            {
                case OrientationMethod.TowardsCamera:
                    _lookInputVector = cameraPlanarDirection;
                    break;
                case OrientationMethod.TowardsMovement:
                    _lookInputVector = _baseMoveInputVector.normalized;
                    break;
            }
        }

        public override void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            base.UpdateRotation(ref currentRotation, deltaTime);

            //if not Target then dont rotate
            if (!Ctx.HasTargetToLockOn) return;
            Quaternion lookRot = Quaternion.LookRotation(Ctx.Target.position - _baseTransform.position, Vector3.up);
            lookRot.z = 0;
            lookRot.x = 0;
            currentRotation = Quaternion.RotateTowards(_baseTransform.rotation, lookRot, Ctx.TargetingRotSpeed * Time.deltaTime);
        }

        public override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            base.UpdateVelocity(ref currentVelocity, deltaTime);

            // Ground movement
            if (_baseMotor.GroundingStatus.IsStableOnGround)
            {
                float movementDot;
                _baseMaxStableMoveSpeed = 4;
                float currentVelocityMagnitude = currentVelocity.magnitude;

                Vector3 effectiveGroundNormal = _baseMotor.GroundingStatus.GroundNormal;
                if (currentVelocityMagnitude > 0f && _baseMotor.GroundingStatus.SnappingPrevented)
                {
                    // Take the normal from where we're coming from
                    Vector3 groundPointToCharacter = _baseMotor.TransientPosition - _baseMotor.GroundingStatus.GroundPoint;
                    if (Vector3.Dot(currentVelocity, groundPointToCharacter) >= 0f)
                    {
                        effectiveGroundNormal = _baseMotor.GroundingStatus.OuterGroundNormal;
                    }
                    else
                    {
                        effectiveGroundNormal = _baseMotor.GroundingStatus.InnerGroundNormal;
                    }
                }

                // Reorient velocity on slope
                currentVelocity = _baseMotor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) * currentVelocityMagnitude;

                // Calculate target velocity
                Vector3 inputRight = Vector3.Cross(_baseMoveInputVector, _baseMotor.CharacterUp);
                Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized * _baseMoveInputVector.magnitude;
                Vector3 targetMovementVelocity = reorientedInput * _baseMaxStableMoveSpeed;

                // Smooth movement Velocity
                currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1f - Mathf.Exp(-Ctx.StableMovementSharpness * deltaTime));
                //Find Dot product of target pos and player direction.
                if (Ctx.HasTargetToLockOn)
                {
                    // not working
                    movementDot = Vector3.Dot(_baseMoveInputVector, _baseTransform.forward);
                    //print(dot);

                    Vector3 dir = Vector3.Cross(_cameraPlanarRotation * _baseTransform.forward, _baseMoveInputVector);
                    //print(dir);
                }
                else
                {
                    movementDot = 0;
                }
                //Set Animator values
                _baseAnimator.SetFloat(anim_moving, currentVelocityMagnitude, 0f, Time.deltaTime);
                _baseAnimator.SetFloat(anim_horizontal, _rawMoveInputVector.x, 0f, Time.deltaTime);
                _baseAnimator.SetFloat(anim_vertical, _rawMoveInputVector.z, 0f, Time.deltaTime);
            }
            // Air movement
            else
            {
                // Add move input
                if (_baseMoveInputVector.sqrMagnitude > 0f)
                {
                    Vector3 addedVelocity = _baseMoveInputVector * Ctx.AirAccelerationSpeed * deltaTime;

                    Vector3 currentVelocityOnInputsPlane = Vector3.ProjectOnPlane(currentVelocity, _baseMotor.CharacterUp);

                    // Limit air velocity from inputs
                    if (currentVelocityOnInputsPlane.magnitude < Ctx.MaxAirMoveSpeed)
                    {
                        // clamp addedVel to make total vel not exceed max vel on inputs plane
                        Vector3 newTotal = Vector3.ClampMagnitude(currentVelocityOnInputsPlane + addedVelocity, Ctx.MaxAirMoveSpeed);
                        addedVelocity = newTotal - currentVelocityOnInputsPlane;
                    }
                    else
                    {
                        // Make sure added vel doesn't go in the direction of the already-exceeding velocity
                        if (Vector3.Dot(currentVelocityOnInputsPlane, addedVelocity) > 0f)
                        {
                            addedVelocity = Vector3.ProjectOnPlane(addedVelocity, currentVelocityOnInputsPlane.normalized);
                        }
                    }

                    // Prevent air-climbing sloped walls
                    if (_baseMotor.GroundingStatus.FoundAnyGround)
                    {
                        if (Vector3.Dot(currentVelocity + addedVelocity, addedVelocity) > 0f)
                        {
                            Vector3 perpenticularObstructionNormal = Vector3.Cross(Vector3.Cross(_baseMotor.CharacterUp, _baseMotor.GroundingStatus.GroundNormal), _baseMotor.CharacterUp).normalized;
                            addedVelocity = Vector3.ProjectOnPlane(addedVelocity, perpenticularObstructionNormal);
                        }
                    }

                    // Apply added velocity
                    currentVelocity += addedVelocity;

                }

                // Gravity
                currentVelocity += Ctx.Gravity * deltaTime;

                // Drag
                currentVelocity *= (1f / (1f + (Ctx.Drag * deltaTime)));
            }
            // Take into account additive velocity
            //Does not currently get called Anywhere
            // if (Ctx.InternalVelocityAdd.sqrMagnitude > 0f)
            // {
            //     currentVelocity += Ctx.InternalVelocityAdd;
            //     Ctx.InternalVelocityAdd = Vector3.zero;
            // }
        }
    }
}