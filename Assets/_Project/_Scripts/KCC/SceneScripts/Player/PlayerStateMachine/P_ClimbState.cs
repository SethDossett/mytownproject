using UnityEngine;
using MyTownProject.UI;
namespace KinematicCharacterController.Examples
{
    public class P_ClimbState : P_BaseState
    {
        int anim_FreeHangDrop = Animator.StringToHash("FreeHangDrop");
        int anim_ClimbUp = Animator.StringToHash("ClimbUp");
        public P_ClimbState(TheCharacterController currentContext, P_StateFactory p_StateFactory)
        : base(currentContext, p_StateFactory)
        {
            IsRootState = true;
        }

        public override void OnStateEnter(P_BaseState state)
        {
            base.OnStateEnter(state);

            _baseMotor = Ctx.Motor;
            _baseAnimator = Ctx.PlayerAnimator;
            _baseMainCam = Ctx.CamMain;

            _baseMotor.ForceUnground();
        }

        public override void OnStateExit()
        {
            base.OnStateExit();
            
            // If we switch to hanging state then dont switch animation
            if(Ctx._isHanging) return; 

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

            // Move and look inputs
            _baseMoveInputVector = cameraPlanarRotation * moveInputVector;
            
        }

        public override void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            base.UpdateRotation(ref currentRotation, deltaTime);

        }

        public override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            base.UpdateVelocity(ref currentVelocity, deltaTime);

            //During Animation, we dont want to be able to move
            if (Ctx._gettingOnOffObstacle)
            {
                currentVelocity = Vector3.zero;
                return;
            }
            if (Ctx._isHanging)
            {
                SwitchStates(Factory.GetBaseState(P_StateNames.Hanging));
            }
        }

      
    }
}
