using UnityEngine;

namespace KinematicCharacterController.Examples
{
    public class P_CutsceneState : P_BaseState
    {
        public P_CutsceneState(TheCharacterController currentContext, P_StateFactory p_StateFactory)
        : base(currentContext, p_StateFactory)
        {
            IsRootState = true;
        }

        public override void OnStateEnter(P_BaseState state)
        {
            base.OnStateEnter(state);

            Ctx.MaxStableMoveSpeed = 1.2f;
            Ctx.IsFalling = false; 
        }

        public override void OnStateExit()
        {
            base.OnStateExit();
        }

        public override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            base.UpdateVelocity(ref currentVelocity, deltaTime);

            Debug.Log("WalkingForward");

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


            Vector3 targetMovementVelocity = _baseTransform.forward * Ctx.MaxStableMoveSpeed;

            currentVelocity = _baseMotor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) * currentVelocityMagnitude;

            currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1f - Mathf.Exp(-Ctx.StableMovementSharpness * deltaTime));

            _baseAnimator.SetFloat(anim_moving, currentVelocityMagnitude, 0f, Time.unscaledDeltaTime);
            _baseAnimator.SetFloat(anim_SpeedMultiplier, 1.5f, 0.1f, Time.unscaledDeltaTime);
        }
    }
}
