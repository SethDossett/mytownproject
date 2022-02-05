using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/NPCSciptable")]
public class NPC_ScriptableObject : ScriptableObject
{
    

    public bool runDestination = false;
    public bool atDestination = true;
    public int currentDestinationIndex;


    public Transform[] destinations;

    
}
