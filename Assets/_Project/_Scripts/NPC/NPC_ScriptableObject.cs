using UnityEngine;
using System.Collections.Generic;
using MyTownProject.Core;


namespace MyTownProject.NPC
{
    [CreateAssetMenu(menuName = "ScriptableObject/NPCSciptable")]
    public class NPC_ScriptableObject : ScriptableObject
    {
        [Header("Status")]
        public NPC_StateNames CurrentRootName;
        public NPC_StateNames CurrentSubName;
        public NPC_StateNames PreviousStateName;
        public CurrentScene currentScene;

        [Header("Values")]
        public float MoveSpeed = 1f;
        public Vector3 currentPosition;
        public Quaternion currentRotation;


        [Header("Bool Checks")]
        public bool MoveByRecorded;
        public bool MoveByPathfinding;

        [Header("Talking")]
        public bool returnToStandingRotation = true;
        public Quaternion shouldBeStandingRotation;

        [Header("Paths")]


        [Header("Actions")]
        //Might make a new Class that hold DateTime, and more info about state to be in etc.
        public List<NPC_Action> TimeActions;
  
    }
    
    [System.Serializable]
    public class NPC_Action
    {
        public DateTime ActionTime; //could just be an int value for global tick;
        public int ActionTick;
        public NPC_StateNames state; //state that it will go into
        public bool CanBeTargeted;
        public bool CanBeInteractedWith;
        public bool hasDesiredRotation;
        public int  DesiredIdleRotationY;

    }
}