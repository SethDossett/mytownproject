using UnityEngine;

namespace MyTownProject.NPC
{
    public class NPC_TalkState : NPC_BaseState
    {
        public NPC_TalkState(NPC_StateMachine currentContext, NPC_StateFactory npcStateFactory)
        : base(currentContext, npcStateFactory)
        {
            IsRootState = true;
        }

        Transform _target;
        Transform _npc;
        Quaternion _lookRot;
        bool _rotate;
        float _str;
        AnimationCurve _curve;
        float _timer;

        public override void EnterState()
        {
            Debug.Log("Enter Talking State");
            _str = 0;
            Ctx.NpcAnimator.SetBool(Ctx.IsWalking, false);
            Ctx.NpcAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
            _target = Ctx.TargetTransform;
            _npc = Ctx.transform;
            _curve = Ctx.FaceTargetCurve;
            // Rotate To Face Player
            _lookRot = Quaternion.LookRotation((_target.position - _npc.position), Vector3.up);
            _lookRot.x = 0;
            _lookRot.z = 0;
            _timer = 0;
            _rotate = true;
        }
        public override void UpdateState()
        {
            Debug.Log(_rotate);
            if (_rotate) RotateToFacePlayer();

            CheckSwitchStates();
        }
        public override void FixedUpdateState() { }
        public override void ExitState()
        {
            _rotate = false;
            Ctx.NpcAnimator.updateMode = AnimatorUpdateMode.Normal;
        }
        public override void CheckSwitchStates() { }
        public override void InitSubState() { }
        private void RotateToFacePlayer()
        {
            Debug.Log("Rotate Talk");
            //if angle is less than certain amount then just turn head, then rotate rest of body.
            // also set up bool check for just eyes, eyes range, head, range, rotation range.

            //_str = Mathf.Min(Ctx.RotSpeed * Time.unscaledDeltaTime, 1);
            //Ctx.NPC.currentRotation = Quaternion.Slerp(Ctx.NPC.currentRotation, _lookRot, _curve.Evaluate(_str));
            //Ctx.transform.rotation = Quaternion.RotateTowards(Ctx.transform.rotation, _lookRot, Ctx.RotSpeed * Time.unscaledDeltaTime);
            //Ctx.transform.rotation = Ctx.NPC.currentRotation;
            _timer += Time.unscaledDeltaTime;
            //if (Vector3.Angle(Ctx.transform.forward, _desiredRot) > 0.1f)
            if (_timer < 2f)
            {
                //_str = Mathf.Min(Ctx.RotSpeed * Time.unscaledDeltaTime, 1);
                _str = Mathf.MoveTowards(_str, 1, 0.5f * Time.unscaledDeltaTime);
                //Ctx.NPC.currentRotation.y = Mathf.Lerp(Ctx.NPC.currentRotation.y, _lookRot.y, _str);
                Ctx.NPC.currentRotation = Quaternion.Slerp(Ctx.NPC.currentRotation, _lookRot, _curve.Evaluate(_str));
                Ctx.transform.rotation = Ctx.NPC.currentRotation;

            }
            else
            {
                _timer = 0;
                _rotate = false;
            }
            // while (Vector3.Angle(_npc.forward, (_target.position - _npc.position).normalized) >= 0.5f);

            //if (Vector3.Dot(_target.forward, _npc.forward) <= -0.98f) _rotate = false;
            //_rotate = false;
        }
    }
}