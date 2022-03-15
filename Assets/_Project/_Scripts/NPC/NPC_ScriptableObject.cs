using UnityEngine;
using UnityEngine.Events;
using MyTownProject.Events;

namespace MyTownProject.NPC
{
    [CreateAssetMenu(menuName = "ScriptableObject/NPCSciptable")]
    public class NPC_ScriptableObject : ScriptableObject
    {
        public float MoveSpeed = 1f;
        public NPC_StateHandler.NPCSTATE currentState;
        public Vector3 currentPosition;
        public int currentScene;
        public Vector3[] significantLocation;

        public bool moveTowardsDestination = false; // need to set based on save data when game starts.
        public bool atDestination = true;
        public Transform[] destinations;

        public Vector3 lastDestinationPosition;
        public int currentIndex;
        public Vector3[] direction;
        public int[] distance;

        // paths 
        public int currentDestinationIndex; // need to set to 0 when time resets.
        public DestinationPathsSO[] destinationPaths;


        #region Events
        public UnityAction OnMove;
        public UnityAction<NPC_StateHandler.NPCSTATE> OnChangedState;

        public void RaiseEventMove() => OnMove?.Invoke();
        public void RaiseChangedState(NPC_StateHandler.NPCSTATE state) => OnChangedState?.Invoke(state);
        #endregion
    }
}