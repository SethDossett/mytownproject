using UnityEngine;
using UnityEngine.Events;
using MyTownProject.Events;

namespace MyTownProject.NPC
{
    [CreateAssetMenu(menuName = "ScriptableObject/NPCSciptable")]
    public class NPC_ScriptableObject : ScriptableObject
    {
        public float MoveSpeed = 1f;
        public Vector3 currentPosition;
        public Vector3[] significantLocation;

        public bool runDestination = false;
        public bool atDestination = true;
        public Transform[] destinations;

        public Vector3 lastDestinationPosition;
        public int currentIndex;
        public Vector3[] direction;
        public int[] distance;

        // paths 
        public int currentDestinationIndex;
        public DestinationPathsSO[] destinationPaths;


        #region Events
        public UnityAction move;


        public void RaiseEventMove()
        {
            move?.Invoke();
        }
        #endregion
    }
}