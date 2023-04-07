using UnityEngine;

namespace KinematicCharacterController.Examples
{
    public class P_JumpState : P_BaseState
    {
        public P_JumpState(TheCharacterController currentContext, P_StateFactory p_StateFactory)
        : base(currentContext, p_StateFactory)
        {
            IsRootState = true;
        }


        public override void OnStateEnter(P_BaseState state)
        {
            base.OnStateEnter(state);


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
        }

        public override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            base.UpdateVelocity(ref currentVelocity, deltaTime);

            // Might want to tighten up how much you can move in air.

            // Air movement
            if (!_baseMotor.GroundingStatus.IsStableOnGround)
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

                if (currentVelocity.y >= 0) Ctx.IsFalling = false; //Could Make this into falling state
                else
                {
                    SwitchStates(Factory.GetBaseState(P_StateNames.Falling));
                }
            }
        }
    }
}
