using UnityEngine;

namespace KinematicCharacterController.Examples
{
    public class P_TalkState : P_BaseState
    {

        public P_TalkState(TheCharacterController currentContext, P_StateFactory p_StateFactory)
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
            _baseTransform = Ctx.transform;

            Ctx.TurnOnTimeScaleZeroTick.TimeScaleZeroTick(1f, false);
            _baseAnimator.SetFloat(anim_moving, 0, 0f, Time.deltaTime);
            _baseAnimator.SetFloat(anim_horizontal, 0, 0.1f, Time.deltaTime);
            _baseAnimator.SetFloat(anim_vertical, 0, 0.1f, Time.deltaTime);
            _baseAnimator.CrossFadeInFixedTime(_talkState, 0.25f, 0);
            Quaternion lookRot = Quaternion.LookRotation(Ctx.Target.position - _baseTransform.position, Vector3.up);
            lookRot.z = 0;
            lookRot.x = 0;
            _baseMotor.SetRotation(lookRot, false);
            Debug.Log("Work");
        }

        public override void OnStateExit()
        {
            base.OnStateExit();

            _baseAnimator.CrossFadeInFixedTime(_idleState, 0.4f, 0);
        }
        public override void SetInputs(ref PlayerCharacterInputs inputs)
        {
            base.SetInputs(ref inputs);

            _baseMoveInputVector = Vector3.zero;
        }

        public override void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            base.UpdateRotation(ref currentRotation, deltaTime);

            Quaternion lookRot = Quaternion.LookRotation(Ctx.Target.position - _baseTransform.position, Vector3.up);
            lookRot.z = 0;
            lookRot.x = 0;
            currentRotation = Quaternion.RotateTowards(_baseTransform.rotation, lookRot, Ctx.TalkingRotSpeed * Time.unscaledDeltaTime);
        }

        public override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            base.UpdateVelocity(ref currentVelocity, deltaTime);

            currentVelocity = Vector3.zero;
        }
    }
}