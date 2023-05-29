using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyTownProject.NPC
{
    public class NPC_IdleState : NPC_BaseState
    {
        public NPC_IdleState(NPC_StateMachine currentContext, NPC_StateFactory npcStateFactory)
        : base(currentContext, npcStateFactory)
        {
            IsRootState = true;
        }

        Quaternion _lookRot;
        bool _rotate;
        float _str;
        AnimationCurve _curve;
        Vector3 _desiredRot;
        float _timer;

        public override void EnterState()
        {
            Debug.Log("Idle State Entered");
            Ctx.NpcAnimator.SetBool(Ctx.IsWalking, false);

            //If has a prefered standing position, rotate back to it ?
            if (Ctx.HasStandingDir)
            {
                _str = 0;
                _curve = Ctx.FaceTargetCurve;
                _desiredRot = new Vector3(0, (float)Ctx.IdleRotY, 0);
                // Rotate To Face Player
                _lookRot = Quaternion.LookRotation(_desiredRot, Vector3.up);
                _lookRot.x = 0;
                _lookRot.z = 0;
                _timer = 0;
                _rotate = true;
            }
        }
        public override void UpdateState()
        {
            //Debug.Log(_rotate);
            if (_rotate) RotateNPC();
            CheckSwitchStates();
        }
        public override void FixedUpdateState() { }
        public override void ExitState() { }
        public override void CheckSwitchStates() { }
        public override void InitSubState() { }

        void RotateNPC()
        {
            _timer += Time.unscaledDeltaTime;
            //if (Vector3.Angle(Ctx.transform.forward, _desiredRot) > 0.1f)
            if (_timer < 2f)
            {
                _str = Mathf.MoveTowards(_str, 1, 0.2f * Time.unscaledDeltaTime);
                Ctx.NPC.currentRotation.y = Mathf.Lerp(Ctx.NPC.currentRotation.y, Ctx.IdleRotY, _str);
                //Ctx.NPC.currentRotation = Quaternion.Slerp(Ctx.NPC.currentRotation, _lookRot, _curve.Evaluate(_str));
                Ctx.transform.rotation = Ctx.NPC.currentRotation;

            }
            else
            {
                _timer = 0;
                _rotate = false;
            }

        }
    }
}