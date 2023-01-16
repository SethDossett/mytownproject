using UnityEngine;
using UnityEngine.Events;
using MyTownProject.Core;
using MyTownProject.Events;

namespace MyTownProject.NPC
{
    [CreateAssetMenu(menuName = "ScriptableObject/NPCSciptable")]
    public class NPC_ScriptableObject : ScriptableObject
    {
        [Header("Values")]
        public NPC_StateNames CurrentRootName;
        public NPC_StateNames CurrentSubName;
        public float MoveSpeed = 1f;
        public NPC_StateHandler.NPCSTATE currentState;
        public Vector3 currentPosition;
        public Quaternion currentRotation;
        public int currentScene;
        

        [Header("Bool Checks")]
        public bool moveTowardsDestination = false; // need to set based on save data when game starts.
        public bool atDestination = true;
        
        [Header("Talking")]
        public bool returnToStandingRotation = true;
        public Quaternion shouldBeStandingRotation;

        [Header("Paths")]
        public int currentDestinationIndex; // need to set to 0 when time resets.
        public DestinationPathsSO[] destinationPaths;

        [Header("Time Triggers")]
        //Might make a new Class that hold DateTime, and more info about state to be in etc.
        public DateTime[] TimeTriggers;


        #region Events
        public UnityAction OnMove;
        public UnityAction<NPC_StateHandler.NPCSTATE> OnChangedState;

        public void RaiseEventMove() => OnMove?.Invoke();
        public void RaiseChangedState(NPC_StateHandler.NPCSTATE state) => OnChangedState?.Invoke(state);
        #endregion
    }
}