using UnityEngine;

public class NPC_Manager : MonoBehaviour
{
    [SerializeField] NPC_ScriptableObject NPC;


    private void OnEnable()
    {
        NPC_DestinationHandler.OnDestinationReached += NextAction;
    }
    private void OnDisable()
    {
        NPC_DestinationHandler.OnDestinationReached -= NextAction;
    }

    void NextAction(DestinationPathsSO path)
    {
        //open door animation
        // change state if needed




        Debug.Log(path);



        if (NPC.currentDestinationIndex < NPC.destinationPaths.Length - 1)
        {
            NPC.currentDestinationIndex++;
            NPC.RaiseEventMove();
        }

        

    }

    void MoveNPC()
    {
    }


}