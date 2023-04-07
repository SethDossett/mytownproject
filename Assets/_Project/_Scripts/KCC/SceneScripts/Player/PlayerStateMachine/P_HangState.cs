using UnityEngine;
using MyTownProject.UI;
namespace KinematicCharacterController.Examples
{
    public class P_HangState : P_BaseState
    {
        int anim_FreeHangDrop = Animator.StringToHash("FreeHangDrop");
        int anim_ClimbUp = Animator.StringToHash("ClimbUp");
        float _climbTimer;
        bool _dropDownRequested;
        bool _checkInput;
        public P_HangState(TheCharacterController currentContext, P_StateFactory p_StateFactory)
        : base(currentContext, p_StateFactory)
        {
            IsRootState = true;
        }

        public override void OnStateEnter(P_BaseState state)
        {
            base.OnStateEnter(state);

            Ctx._isHanging = true;
            _baseMotor.ForceUnground();
            _climbTimer = 0;
        }

        public override void OnStateExit()
        {
            base.OnStateExit();

            if (Ctx.IsFalling)
                _baseAnimator.CrossFadeInFixedTime(anim_FreeHangDrop, 0.25f, 0);
            else
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

            if (_dropDownRequested) return;
            // Move and look inputs
            _baseMoveInputVector = cameraPlanarRotation * moveInputVector;

            if (inputs.Interact)
            {
                _dropDownRequested = true;
            }
        }

        public override void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            base.UpdateRotation(ref currentRotation, deltaTime);

            if (Ctx.IsDropToHang)
            {
                Quaternion toRot = Quaternion.LookRotation(-Ctx.LedgeDirection, Vector3.up);
                currentRotation = Quaternion.RotateTowards(_baseTransform.rotation, toRot, 500 * Time.deltaTime);
                Debug.Log("CC Rotation");
            }
        }

        public override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            base.UpdateVelocity(ref currentVelocity, deltaTime);

            //During Animation, we dont want to be able to move
            if (Ctx._gettingOnOffObstacle)
            {
                currentVelocity = Vector3.zero;
                _dropDownRequested = false;
                _checkInput = false;
                return;
            }
            else
            {
                _checkInput = true;
            }

        }

        public override void UpdateState()
        {
            if(_checkInput)
                HangingChecks();
        }

        void HangingChecks()
        { // make a coroutine possibly
            //if (_moveInputVector.sqrMagnitude <= 0) return;
            Ctx.UIText.ChangePrompt(PromptName.Drop, 20);

            float dot = Vector3.Dot(_baseMoveInputVector, _baseTransform.forward.normalized);
            Quaternion rot = Quaternion.Euler(_baseTransform.forward.normalized);
            //Press towards ledge to climb up
            if (dot >= 0.9f)
            {
                _climbTimer += Time.deltaTime;
                if (_climbTimer >= 0.3f)
                {
                    _climbTimer = 0;
                    _baseAnimator.CrossFadeInFixedTime(anim_ClimbUp, 0.1f, 0);
                    //StartCoroutine(ClimbBackUp(rot));
                    Ctx.UIText.ChangePrompt(PromptName.Drop, 0);
                    _basePlayerClimb.ClimbBackUp();
                }
            }
            //Could Slide if held left or right
            else
            {
                _climbTimer = 0;
            }
            //Pressed Interact Btn to drop
            if (_dropDownRequested)
            {
                Ctx.UIText.ChangePrompt(PromptName.Drop, 0);
                Ctx._startFallingTimer = true;
                _dropDownRequested = false;
                Ctx.CapsuleEnable(true);
                SwitchStates(Factory.GetBaseState(P_StateNames.Falling));
                _basePlayerClimb._isClimbing = false;
                Ctx._gettingOnOffObstacle = false;
                Ctx._isHanging = false;
            }

        }
    }
}
