using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;


[CreateAssetMenu(menuName = "ScriptableObject/NPCSciptable")]
public class NPC_ScriptableObject : ScriptableObject
{
    public float MoveSpeed = 1f;
    public Vector3[] significantLocation;

    public bool runDestination = false;
    public bool atDestination = true;
    public int currentDestinationIndex;
    public Transform[] destinations;

    public Vector3 lastDestinationPosition;
    public int currentIndex;
    public Vector3[] direction;
    public int[] distance;

    public DestinationPathsSO[] destinationPaths;


    [Header("Events")]
    public UnityAction move;


    public void RaiseEventMove() {
        move?.Invoke();
    }
    
}
