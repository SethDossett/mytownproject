using UnityEngine;

namespace KinematicCharacterController.Examples
{
    public class P_LadderState : P_BaseState
    {
        int _climbState = Animator.StringToHash("Climbing");
        bool _onLadder = false;
        public P_LadderState(TheCharacterController currentContext, P_StateFactory p_StateFactory)
        : base(currentContext, p_StateFactory)
        {
            IsRootState = true;
        }

        public override void OnStateEnter(P_BaseState state)
        {
            base.OnStateEnter(state);

            Ctx.CapsuleEnable(false);
            _basePlayerClimb._isClimbing = true;
            _baseAnimator.CrossFadeInFixedTime(_climbState, 0.2f, 0);
        }

        public override void OnStateExit()
        {
            base.OnStateExit();

            _onLadder = false;
            Debug.Log("EXIT LADDER");
            _baseAnimator.CrossFadeInFixedTime(_idleState, 0, 0);
            _basePlayerClimb._isClimbing = false;
            Ctx.CapsuleEnable(true);
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

            Vector3 ladderInputVector = Vector3.ClampMagnitude(new Vector3(inputs.MoveDirection.x, inputs.MoveDirection.y, 0f), 1f);

            // Move and look inputs
            _baseMoveInputVector = cameraPlanarRotation * ladderInputVector;

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

            currentRotation = Quaternion.RotateTowards(_baseTransform.rotation, Ctx.NewLadderRotation, 500 * Time.deltaTime);
        }

        public override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            base.UpdateVelocity(ref currentVelocity, deltaTime);

            if (Ctx._gettingOnOffObstacle)
            {
                currentVelocity = Vector3.zero;
                if (_baseAnimator.speed != 1f)
                    _baseAnimator.speed = 1f;
                return;
            }
            if (_baseMotor.GroundingStatus.IsStableOnGround) _baseMotor.ForceUnground();

            Vector3 newCenteredPosition = Ctx.NewCenteredPosition;

            float currentVelocityMagnitude = currentVelocity.magnitude;

            if (!_onLadder)
            {
                currentVelocity = Vector3.zero;
                if (_baseAnimator.speed != 1f)
                    _baseAnimator.speed = 1f;
                //Vector3 ladderStartPos = new Vector3(newCenteredPosition.x,transform.position.y, newCenteredPosition.z);
                //Vector3 newPlayerPos = Vector3.Lerp(_baseMotor.TransientPosition, newCenteredPosition, 1f - Mathf.Exp(-StableMovementSharpness * deltaTime));
                //Vector3 newPlayerPos = Vector3.MoveTowards(_baseMotor.TransientPosition, newCenteredPosition, _jumpOnLadderSpeed * deltaTime);
                _baseMotor.SetTransientPosition(newCenteredPosition, true, 5);
                if (Vector3.Distance(_baseMotor.TransientPosition, newCenteredPosition) <= float.Epsilon)
                {
                    _onLadder = true;
                }
                else _onLadder = false;
            }
            else
            {
                currentVelocity.y = _baseMoveInputVector.y * Ctx.ClimbSpeedY;
                //currentVelocity.z = -_moveInputVector.x * _climbSpeedX;
                currentVelocity.x = 0;
                currentVelocity.z = 0;

                if (currentVelocity.y == 0f)
                {
                    if (currentVelocity != Vector3.zero) currentVelocity = Vector3.zero;
                    if (_baseAnimator.speed != 0f)
                        _baseAnimator.speed = 0f;
                }
                else
                {
                    if (_baseAnimator.speed != 1f)
                        _baseAnimator.speed = 1f;
                }
            }



            // Reorient velocity on slope
            //currentVelocity = _baseMotor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) * currentVelocityMagnitude;
            //// Calculate target velocity
            //Vector3 inputRight = Vector3.Cross(_moveInputVector, _baseMotor.CharacterUp);
            //Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized * _moveInputVector.magnitude;
            //Vector3 targetMovementVelocity = reorientedInput * MaxStableMoveSpeed;
            //// Smooth movement Velocity
            //currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1f - Mathf.Exp(-StableMovementSharpness * deltaTime));;
        }
    }
}