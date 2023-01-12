using UnityEngine;
using MyTownProject.Core;

namespace MyTownProject.NPC
{
    public class NPC_WalkState : NPC_BaseState
    {
        private bool _isReplay;

        public NPC_WalkState(NPC_StateMachine currentContext, NPC_StateFactory npcStateFactory)
        : base(currentContext, npcStateFactory) { }

        public override void EnterState()
        {
            _isReplay = false;

            if (Ctx.IsReplay)
            {;
                _isReplay = true;
            }

        }
        public override void UpdateState()
        {
            CheckSwitchStates();
        }
        public override void FixedUpdateState()
        {
            if (_isReplay)
            {
                UpdateTransform();
            }
        }
        public override void ExitState()
        {
            _isReplay = false;
        }
        public override void CheckSwitchStates()
        {
            if (Ctx.AI.reachedEndOfPath)
            {
                SwitchStates(Factory.Idle());
            }
        }
        public override void InitSubState() { }


        void UpdateTransform()
        {
            if (Ctx.CurrentPath == null) return;

            float globalTime = TimeManager.GlobalTime;

            DateTime currentTime = TimeManager.DateTime;
            int day = (int)currentTime.Day;
            int hour = currentTime.Hour;
            int minute = currentTime.Minutes;

            int index1 = Mathf.FloorToInt(globalTime);
            int index2 = Mathf.CeilToInt(globalTime);

            if (index2 >= Ctx.CurrentPath.Records.Count)
            {
                Ctx.IsReplay = false;
                return;
            }

            if (index1 == index2)
            {
                Ctx.transform.position = Ctx.CurrentPath.Records[index1].Positions;
                Ctx.transform.eulerAngles = Ctx.CurrentPath.Records[index2].Rotations;
            }
            else
            {
                float interpolationFactor = (globalTime - index1);

                Ctx.transform.position = Vector3.Lerp(Ctx.CurrentPath.Records[index1].Positions, Ctx.CurrentPath.Records[index2].Positions, interpolationFactor);
                Ctx.transform.eulerAngles = Vector3.Lerp(Ctx.CurrentPath.Records[index1].Rotations, Ctx.CurrentPath.Records[index2].Rotations, interpolationFactor);

            }
        }
        
    }
}