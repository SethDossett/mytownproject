using UnityEngine;

namespace MyTownProject.NPC
{
    public class NPC_TalkState : NPC_BaseState
    {
        public NPC_TalkState(NPC_StateMachine currentContext, NPC_StateFactory npcStateFactory)
        : base(currentContext, npcStateFactory) { }

        Transform _target;
        Transform _npc;
        Quaternion _lookRot;
        bool _rotate;

        public override void EnterState()
        {
            Debug.Log("Talking State");
            _target = Ctx.TargetTransform;
            _npc = Ctx.transform;
            // Rotate To Face Player
            _lookRot = Quaternion.LookRotation(_target.position - _npc.position, Vector3.up);
            _lookRot.Normalize();
            _lookRot.x = 0;
            _lookRot.z = 0;
            _rotate = true;
        }
        public override void UpdateState()
        {
            if(_rotate) RotateToFacePlayer();

            CheckSwitchStates();
        }
        public override void FixedUpdateState() { }
        public override void ExitState() { 
            _rotate = false;
        }
        public override void CheckSwitchStates() { }
        public override void InitSubState() { }
        private void RotateToFacePlayer()
        {
            //if angle is less than certain amount then just turn head, then rotate rest of body.
            // also set up bool check for just eyes, eyes range, head, range, rotation range.
            Ctx.NPC.currentRotation = Quaternion.RotateTowards(Ctx.NPC.currentRotation, _lookRot, Ctx.RotSpeed * Time.unscaledDeltaTime);
            Ctx.transform.rotation = Ctx.NPC.currentRotation.normalized;

            if(Vector3.Dot(_target.forward, _npc.forward) >= -0.98f) _rotate = false;
        }
    }
}