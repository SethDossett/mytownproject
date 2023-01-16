using UnityEngine;
using MyTownProject.Core;

namespace MyTownProject.NPC
{
    public class NPC_WalkState : NPC_BaseState
    {
        private bool _isReplay;
        private bool _moveByPathfinding;
        private Pathfinding.AILerp _aI;
        int _isWalking = Animator.StringToHash("isWalking");

        public NPC_WalkState(NPC_StateMachine currentContext, NPC_StateFactory npcStateFactory)
        : base(currentContext, npcStateFactory)
        {
            IsRootState = true;
        }

        public override void EnterState()
        {
            _isReplay = false;
            _moveByPathfinding = false;

            if (Ctx.MoveByRecorded)
            {
                _moveByPathfinding = false;
                _isReplay = true;
            }
            else if (Ctx.MoveByPathfinding)
            {
                _isReplay = false;
                _moveByPathfinding = true;
                _aI = Ctx.AI;
            }
            else
            {
                SwitchStates(Factory.Idle());
                return;
            }

            //Ctx.NpcAnimator.SetTrigger(_isStanding);
            Ctx.NpcAnimator.SetBool(_isWalking, true);
            //Use CrossFade Because each state can hold a different animation
        }
        public override void UpdateState()
        {
            Debug.Log("update walk");
            if (_moveByPathfinding)
            {
                PathfindingMovement();
            }
            CheckSwitchStates();
        }
        public override void FixedUpdateState()
        {
            if (_isReplay)
            {
                RecordedMovement();
            }
        }
        public override void ExitState()
        {
            _isReplay = false;
            _moveByPathfinding = false;
        }
        public override void CheckSwitchStates() { }
        public override void InitSubState() { }

        void PathfindingMovement()
        {

            _aI.canMove = true;
            _aI.speed = 1.5f;
            _aI.rotationSpeed = 10f;

            if (_aI.reachedEndOfPath)
            {
                //Check Time, and see what npc should do
                _aI.canMove = false;
            }
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
                SwitchStates(Factory.Idle());
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