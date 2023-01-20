using UnityEngine;
using MyTownProject.Core;

namespace MyTownProject.NPC
{
    public class NPC_WalkState : NPC_BaseState
    {
        private bool _moveByRecorded;
        private bool _moveByPathfinding;
        private Pathfinding.AILerp _aI;

        public NPC_WalkState(NPC_StateMachine currentContext, NPC_StateFactory npcStateFactory)
        : base(currentContext, npcStateFactory)
        {
            IsRootState = true;
        }

        public override void EnterState()
        {
            _moveByRecorded = false;
            _moveByPathfinding = false;

            if (Ctx.NPC.MoveByRecorded)
            {
                _moveByPathfinding = false;
                _moveByRecorded = true;
            }
            else if (Ctx.NPC.MoveByPathfinding)
            {
                _aI = Ctx.AI;
                _moveByRecorded = false;
                _moveByPathfinding = true;
            }
            else
            {
                SwitchStates(Factory.GetBaseState(NPC_StateNames.Idle));
                return;
            }

            //Ctx.NpcAnimator.SetTrigger(_isStanding);
            Ctx.NpcAnimator.SetBool(Ctx.IsWalking, true);
            //Use CrossFade Because each state can hold a different animation
        }
        public override void UpdateState()
        {
            if (_moveByPathfinding)
            {
                PathfindingMovement();
            }
            CheckSwitchStates();
        }
        public override void FixedUpdateState()
        {
            if (_moveByRecorded)
            {
                RecordedMovement();
            }
        }
        public override void ExitState()
        {
            _moveByRecorded = false;
            _moveByPathfinding = false;
        }
        public override void CheckSwitchStates() { }
        public override void InitSubState() { }

        void PathfindingMovement()
        {
            Debug.Log("update path");
            _aI.canMove = true;
            _aI.speed = Ctx.NPC.MoveSpeed;
            _aI.rotationSpeed = 10f;

            if (_aI.reachedEndOfPath)
            {
                //Check Time, and see what npc should do
                _aI.canMove = false;
                SwitchStates(Factory.GetBaseState(NPC_StateNames.Idle));
            }

            Ctx.NPC.currentPosition = Ctx.transform.position;
            Ctx.NPC.currentRotation = Ctx.transform.rotation;
        }
        void RecordedMovement()
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
                Ctx.NPC.currentPosition = Ctx.transform.position;
                Ctx.NPC.currentRotation = Ctx.transform.rotation;
                SwitchStates(Factory.GetBaseState(NPC_StateNames.Idle));
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

            Ctx.NPC.currentPosition = Ctx.transform.position;
            Ctx.NPC.currentRotation = Ctx.transform.rotation;
        }

    }
}