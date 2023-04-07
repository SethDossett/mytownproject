using UnityEngine;
using MyTownProject.UI;

namespace KinematicCharacterController.Examples
{
    public class P_CrawlState : P_BaseState
    {
        int anim_isCrouched = Animator.StringToHash("isCrouched");
        private Collider[] _probedColliders = new Collider[8];
        bool _isCrouching = false;
        bool _hasFinishedCrouch;
        bool _moveBackwards;

        public P_CrawlState(TheCharacterController currentContext, P_StateFactory p_StateFactory)
        : base(currentContext, p_StateFactory)
        {
            IsRootState = true;
        }
        public override void OnStateEnter(P_BaseState state)
        {
            base.OnStateEnter(state);

            Ctx._shouldBeCrouching = true;
            _isCrouching = true;
            _baseMotor.SetCapsuleDimensions(0.44f, Ctx.CrouchedCapsuleHeight, Ctx.CrouchedCapsuleHeight * 0.5f);
            //MeshRoot.localScale = new Vector3(1f, 0.5f, 1f);
            if (_baseAnimator.GetBool(anim_isCrouched) != true)
                _baseAnimator.SetBool(anim_isCrouched, true);


            _baseFallOffPrevention.enabled = true;
            _moveBackwards = false;
            Ctx.OrientationSharpness = Ctx.CrawlRotationSpeed;
            Ctx.UIText.ChangePrompt(PromptName.Null, 100);
        }

        public override void OnStateExit()
        {
            base.OnStateExit();

            _baseFallOffPrevention.enabled = false;
            Ctx.DisableRecentering.RaiseEvent();
            //UIText.ChangePrompt(PromptName.Crouch, 0);
            Ctx.UIText.HideButtonText(HudElement.RightTrigger);
            Ctx.OrientationSharpness = Ctx._defaultOrientationSharpness;
            _hasFinishedCrouch = false;

            if (_baseAnimator.speed != 1f)
                _baseAnimator.speed = 1f;

            Ctx.UIText.ChangePrompt(PromptName.Null, 1);
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

            // If player presses opposite direction crawl backwards
            float dot = Vector3.Dot(_baseMoveInputVector, _baseTransform.forward);
            if (dot <= -0.9f)
                _moveBackwards = true;
            else
                _moveBackwards = false;

            // Camera Recenter while Moving
            if (_baseMoveInputVector.sqrMagnitude > 0 && _moveBackwards == false)
                Ctx.RecenterCamX.ThreeFloats(0, 2f, 0);
            else// need to turn off once but not over and over
                Ctx.DisableRecentering.RaiseEvent();


            switch (Ctx.OrientationMethod)
            {
                case OrientationMethod.TowardsCamera:
                    _lookInputVector = cameraPlanarDirection;
                    break;
                case OrientationMethod.TowardsMovement:
                    _lookInputVector = _baseMoveInputVector.normalized;
                    break;
            }

            if (inputs.CrouchUp)
                Ctx._shouldBeCrouching = false;

            if (inputs.CrouchDown)
                Ctx._shouldBeCrouching = true;
        }

        public override void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            base.UpdateRotation(ref currentRotation, deltaTime);

            if (_moveBackwards) return;

            if (_lookInputVector.sqrMagnitude > 0f && Ctx.OrientationSharpness > 0f)
            {
                // Smoothly interpolate from current to target look direction
                Vector3 smoothedLookInputDirection = Vector3.Slerp(_baseMotor.CharacterForward, _lookInputVector, 1 - Mathf.Exp(-Ctx.OrientationSharpness * deltaTime)).normalized;

                // Set the current rotation (which will be used by the KinematicCharacterMotor)
                currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, _baseMotor.CharacterUp);
            }

            Vector3 currentUp = (currentRotation * Vector3.up);
            if (Ctx.BonusOrientationMethod == BonusOrientationMethod.TowardsGravity)
            {
                // Rotate from current up to invert gravity
                Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, -Ctx.Gravity.normalized, 1 - Mathf.Exp(-Ctx.BonusOrientationSharpness * deltaTime));
                currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
            }
            else if (Ctx.BonusOrientationMethod == BonusOrientationMethod.TowardsGroundSlopeAndGravity)
            {
                if (_baseMotor.GroundingStatus.IsStableOnGround)
                {
                    Vector3 initialCharacterBottomHemiCenter = _baseMotor.TransientPosition + (currentUp * _baseMotor.Capsule.radius);

                    Vector3 smoothedGroundNormal = Vector3.Slerp(_baseMotor.CharacterUp, _baseMotor.GroundingStatus.GroundNormal, 1 - Mathf.Exp(-Ctx.BonusOrientationSharpness * deltaTime));
                    currentRotation = Quaternion.FromToRotation(currentUp, smoothedGroundNormal) * currentRotation;

                    // Move the position to create a rotation around the bottom hemi center instead of around the pivot
                    _baseMotor.SetTransientPosition(initialCharacterBottomHemiCenter + (currentRotation * Vector3.down * _baseMotor.Capsule.radius), false, 0);
                }
                else
                {
                    Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, -Ctx.Gravity.normalized, 1 - Mathf.Exp(-Ctx.BonusOrientationSharpness * deltaTime));
                    currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
                }
            }
            else
            {
                Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, Vector3.up, 1 - Mathf.Exp(-Ctx.BonusOrientationSharpness * deltaTime));
                currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
            }
        }

        public override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            base.UpdateVelocity(ref currentVelocity, deltaTime);

            //if (!_baseMotor.GroundingStatus.IsStableOnGround) return;
            if (!_hasFinishedCrouch)
            {
                if (currentVelocity != Vector3.zero) currentVelocity = Vector3.zero;
                _baseAnimator.SetFloat(anim_moving, 0f, 0f, Time.deltaTime);

                if (_baseAnimator.speed != 1f)
                    _baseAnimator.speed = 1f;

                if (_baseMoveInputVector.magnitude > 0f)
                {
                    _hasFinishedCrouch = true;
                }
                return;
            }

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

            Ctx.MaxStableMoveSpeed = Ctx.CrawlSpeed;
            if (_baseMoveInputVector.magnitude == 0)
            {
                if (_baseAnimator.speed != 0f)
                    _baseAnimator.speed = 0f;
            }
            else
            {
                if (_baseAnimator.speed != 1f)
                    _baseAnimator.speed = 1f;
            }


            // Reorient velocity on slope
            currentVelocity = _baseMotor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) * currentVelocityMagnitude;

            // Calculate target velocity
            Vector3 inputRight = Vector3.Cross(_baseTransform.forward, _baseMotor.CharacterUp);
            Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized * _baseMoveInputVector.magnitude;
            Vector3 targetMovementVelocity = reorientedInput * Ctx.MaxStableMoveSpeed;

            // Smooth movement Velocity
            if (!_moveBackwards)
                currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1f - Mathf.Exp(-Ctx.StableMovementSharpness * deltaTime));
            else
                currentVelocity = Vector3.Lerp(currentVelocity, -targetMovementVelocity, 1f - Mathf.Exp(-Ctx.StableMovementSharpness * deltaTime));

            _baseAnimator.SetFloat(anim_moving, currentVelocityMagnitude, 0f, Time.deltaTime);
        }

        public override void AfterCharacterUpdate(float deltaTime)
        {
            base.AfterCharacterUpdate(deltaTime);

            // Handle uncrouching
            if (_isCrouching && !Ctx._shouldBeCrouching)
            {
                // Do an overlap test with the character's standing height to see if there are any obstructions
                // Original Values SetCapsuleDimensions(0.5f, 2f, 1f);
                _baseMotor.SetCapsuleDimensions(0.44f, 1.6f, 0.8f);
                if (_baseMotor.CharacterOverlap(
                    _baseMotor.TransientPosition,
                    _baseMotor.TransientRotation,
                    _probedColliders,
                    _baseMotor.CollidableLayers,
                    QueryTriggerInteraction.Ignore) > 0)
                {
                    // If obstructions, just stick to crouching dimensions
                    _baseMotor.SetCapsuleDimensions(0.44f, Ctx.CrouchedCapsuleHeight, Ctx.CrouchedCapsuleHeight * 0.5f);
                }
                else
                {
                    // If no obstructions, uncrouch
                    //MeshRoot.localScale = new Vector3(1f, 1f, 1f);
                    _isCrouching = false;
                    if (_baseAnimator.GetBool(anim_isCrouched) != false)
                        _baseAnimator.SetBool(anim_isCrouched, false);

                    SwitchStates(Factory.GetBaseState(P_StateNames.Default));
                    Debug.Log("Uncrouched");
                }
            }
        }
    }


}
