using UnityEngine;
using MyTownProject.UI;
namespace KinematicCharacterController.Examples
{
    public class P_DefaultState : P_BaseState
    {
        int anim_jumpTrigger = Animator.StringToHash("hasJumped");
        int anim_changeDirection = Animator.StringToHash("ChangeDirection");

        bool _changingDirection = false;
        float _ratePerSecond;
        float _turnAroundBuffer;
        float _maxSpeed = 6f;

        bool _canCrouch = false;

        private bool _jumpConsumed = false;
        private bool _jumpedThisFrame = false;
        private float _timeSinceJumpRequested = Mathf.Infinity;
        private float _timeSinceLastAbleToJump = 0f;

        public P_DefaultState(TheCharacterController currentContext, P_StateFactory p_StateFactory)
        : base(currentContext, p_StateFactory)
        {
            IsRootState = true;
        }

        public override void OnStateEnter(P_BaseState state)
        {
            //Get references from base state
            base.OnStateEnter(state);

            Ctx.Gravity = new Vector3(0, -30f, 0);
            _baseAnimator.SetFloat(anim_SpeedMultiplier, 2.5f, 0.1f, Time.unscaledDeltaTime);
            _lookInputVector = Vector3.zero;
            _ratePerSecond = _maxSpeed / Ctx.AccelTime;
            //_animator.CrossFadeInFixedTime(_idleState, 0.2f, 0);
            //_baseMaxStableMoveSpeed = 0; // think it might look better to be able to run out of climbing.
            _canCrouch = true;
        }
        public override void OnStateExit()
        {
            base.OnStateExit();

            Ctx.UIText.ChangePrompt(PromptName.Crouch, 0);
            Ctx.UIText.HideButtonText(HudElement.RightTrigger);
            _canCrouch = false;
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

            switch (Ctx.OrientationMethod)
            {
                case OrientationMethod.TowardsCamera:
                    _lookInputVector = cameraPlanarDirection;
                    break;
                case OrientationMethod.TowardsMovement:
                    _lookInputVector = _baseMoveInputVector.normalized;
                    break;
            }

            // Jumping input
            if (inputs.JumpDown)
            {
                _timeSinceJumpRequested = 0f;
                JumpRequested = true;
            }

            //Show or hide UI Text for Crouch
            //if (_canCrouch) UIText.ChangePrompt(PromptName.Crouch, 5);
            //else UIText.ChangePrompt(PromptName.Crouch, 0);
            if (_canCrouch) Ctx.UIText.ShowButtonText(HudElement.RightTrigger, "Crouch");
            else Ctx.UIText.HideButtonText(HudElement.RightTrigger);

            // Crouching input
            if (inputs.CrouchDown && _canCrouch)
            {
                Debug.Log("Crouch");
                SwitchStates(Factory.GetBaseState(P_StateNames.Crawling));
            }
        }

        public override void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            base.UpdateRotation(ref currentRotation, deltaTime);

            //if(changingDirection) return;

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

            //Dont move player if he is getting on and off Something.
            if (Ctx._gettingOnOffObstacle) return;
            // Ground movement
            if (_baseMotor.GroundingStatus.IsStableOnGround)
            {
                Vector3 prevInput = _baseMoveInputVector.normalized;

                float dot = Vector3.Dot(prevInput, _baseTransform.forward);

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

                // Accelerate MaxSpeed

                //Determine if change in other direction
                if (currentVelocity.magnitude < 0.1f && _changingDirection == false)
                {
                    //if player is not facing the direction of input, dont move yet
                    if (dot < 0.95f)
                    {
                        _baseMaxStableMoveSpeed = 0;
                    }
                    else
                    {
                        if (_baseMaxStableMoveSpeed < 4) _baseMaxStableMoveSpeed = 4;
                        if (_baseMaxStableMoveSpeed != _maxSpeed)
                        {
                            _baseMaxStableMoveSpeed += _ratePerSecond * deltaTime;
                            _baseMaxStableMoveSpeed = Mathf.Min(_baseMaxStableMoveSpeed, _maxSpeed);
                        }

                    }
                }
                else
                {
                    if (_baseMaxStableMoveSpeed >= _maxSpeed)
                    {
                        if (dot < -0.96f)
                        {
                            Ctx.RestrictJumping = true;
                            _changingDirection = true;
                            //_baseMaxStableMoveSpeed = 0;
                            //Decelerate changing directions to slide
                            _baseMaxStableMoveSpeed -= 50f * deltaTime;
                            _baseMaxStableMoveSpeed = Mathf.Max(_baseMaxStableMoveSpeed, 0);
                            _baseAnimator.CrossFade(anim_changeDirection, 0, 0);
                        }
                    }
                    else
                    {
                        if (_changingDirection)
                        {
                            _turnAroundBuffer += deltaTime;

                            if (_turnAroundBuffer > Ctx.SlideDuration)
                            {
                                //_baseMaxStableMoveSpeed = 2f;
                                _changingDirection = false;
                                Ctx.RestrictJumping = false;
                                _turnAroundBuffer = 0;
                            }
                        }
                        else
                        {
                            if (_baseMoveInputVector.magnitude < 0.1f)
                            {
                                //_baseMaxStableMoveSpeed = 0; //might need to change so movement is not odd with thumbstick.
                                //Decelerate
                                _baseMaxStableMoveSpeed -= _ratePerSecond * deltaTime;
                                _baseMaxStableMoveSpeed = Mathf.Max(_baseMaxStableMoveSpeed, 0);
                            }
                            else
                            {
                                //Walk
                                if (_baseMoveInputVector.magnitude < 0.3f)
                                {
                                    //_baseMaxStableMoveSpeed = 1f;
                                    _baseMaxStableMoveSpeed += _ratePerSecond * deltaTime;
                                    _baseMaxStableMoveSpeed = Mathf.Min(_baseMaxStableMoveSpeed, 1);
                                }
                                else
                                {
                                    //Run
                                    if (_baseMaxStableMoveSpeed < 4) _baseMaxStableMoveSpeed = 4;
                                    if (_baseMaxStableMoveSpeed != _maxSpeed)
                                    {
                                        _baseMaxStableMoveSpeed += _ratePerSecond * deltaTime;
                                        _baseMaxStableMoveSpeed = Mathf.Min(_baseMaxStableMoveSpeed, _maxSpeed);
                                    }
                                }
                            }

                        }
                    }
                }

                // Calculate target velocity
                Vector3 inputRight = Vector3.Cross(_baseMoveInputVector, _baseMotor.CharacterUp);
                Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized * _baseMoveInputVector.magnitude;
                if (_changingDirection) reorientedInput = -reorientedInput;
                Vector3 targetMovementVelocity = reorientedInput * _baseMaxStableMoveSpeed;

                // Smooth movement Velocity
                currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1f - Mathf.Exp(-Ctx.StableMovementSharpness * deltaTime));
                _baseAnimator.SetFloat(anim_moving, currentVelocityMagnitude, 0f, Time.deltaTime);
                //Dont Think I Need below
                //_baseAnimator.SetFloat(anim_horizontal, currentVelocity.x, 0.1f, Time.deltaTime);
                //_baseAnimator.SetFloat(anim_vertical, currentVelocity.y, 0.1f, Time.deltaTime);


                if (currentVelocityMagnitude <= 0f) _canCrouch = true;
                else _canCrouch = false;
            }
            // Air movement
            else //Could Make Falling State
            {
                Debug.Log("DOES THIS EVER RUN??");
                _canCrouch = false;
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
            //Needs To be put in its own state.
            // Handle jumping
            if (!Ctx.RestrictJumping)
            {

                _jumpedThisFrame = false;
                _timeSinceJumpRequested += deltaTime;
                if (JumpRequested && currentVelocity.magnitude >= Ctx.SpeedNeededToJump)
                {
                    // See if we actually are allowed to jump
                    if (!_jumpConsumed && ((Ctx.AllowJumpingWhenSliding ? _baseMotor.GroundingStatus.FoundAnyGround : _baseMotor.GroundingStatus.IsStableOnGround) || _timeSinceLastAbleToJump <= Ctx.JumpPostGroundingGraceTime))
                    {

                        // Calculate jump direction before ungrounding
                        Vector3 jumpDirection = _baseMotor.CharacterUp;
                        if (_baseMotor.GroundingStatus.FoundAnyGround && !_baseMotor.GroundingStatus.IsStableOnGround)
                        {
                            jumpDirection = _baseMotor.GroundingStatus.GroundNormal;
                        }

                        // Makes the character skip ground probing/snapping on its next update. 
                        // If this line weren't here, the character would remain snapped to the ground when trying to jump. Try commenting this line out and see.
                        _baseMotor.ForceUnground();

                        // Add to the return velocity and reset jump state
                        //currentVelocity += (jumpDirection * JumpUpSpeed) - Vector3.Project(currentVelocity, Motor.CharacterUp);
                        currentVelocity = Vector3.zero;
                        currentVelocity = _baseTransform.forward * Ctx.JumpScalableForwardSpeed; //current
                        currentVelocity.y = Ctx.JumpUpSpeed; //current
                                                             //currentVelocity = (jumpDirection * JumpUpSpeed) - Vector3.Project(currentVelocity, Motor.CharacterUp);
                                                             //currentVelocity += (_baseMoveInputVector * JumpScalableForwardSpeed);
                        Debug.Log(currentVelocity);
                        Debug.Log(currentVelocity.magnitude);
                        JumpRequested = false;
                        _jumpConsumed = true;
                        _jumpedThisFrame = true;
                        _baseAnimator.SetBool(anim_jumpTrigger, true);
                        Ctx.CurrentState.SwitchStates(Factory.GetBaseState(P_StateNames.Jumping));
                    }
                }
                //Drop To Hang
                else if (JumpRequested && currentVelocity.magnitude < Ctx.SpeedNeededToJump)
                {
                    if (Ctx.CanHang)
                    {
                        currentVelocity = Vector3.zero;
                        Ctx.DropToHang();
                        JumpRequested = false;
                        _jumpConsumed = true;
                        _jumpedThisFrame = true;
                    }
                    //Just Fall off
                    else
                        JumpRequested = false;

                }
                else
                    JumpRequested = false;
            }
            // Take into account additive velocity
            //Does not currently get called Anywhere
            // if (Ctx.InternalVelocityAdd.sqrMagnitude > 0f)
            // {
            //     currentVelocity += Ctx.InternalVelocityAdd;
            //     Ctx.InternalVelocityAdd = Vector3.zero;
            // }
        }
        public override void AfterCharacterUpdate(float deltaTime)
        {
            base.AfterCharacterUpdate(deltaTime);

            // Handle jump-related values
            {
                // Handle jumping pre-ground grace period
                if (JumpRequested && _timeSinceJumpRequested > Ctx.JumpPreGroundingGraceTime)
                {
                    JumpRequested = false;
                }

                if (Ctx.AllowJumpingWhenSliding ? _baseMotor.GroundingStatus.FoundAnyGround : _baseMotor.GroundingStatus.IsStableOnGround)
                {
                    // If we're on a ground surface, reset jumping values
                    if (!_jumpedThisFrame)
                    {
                        _jumpConsumed = false;
                    }
                    _timeSinceLastAbleToJump = 0f;
                }
                else
                {
                    // Keep track of time since we were last able to jump (for grace period)
                    _timeSinceLastAbleToJump += deltaTime;
                }
            }


        }

        public override void UpdateState()
        {
            base.UpdateState();
            //Do I want this updated every frame or fixed
            // Grounded Check if Can Climb
            if (_baseMotor.GroundingStatus.IsStableOnGround)
            {
                _basePlayerClimb.GroundCheck();
            }
            else
            {

            }

        }
    }
}

