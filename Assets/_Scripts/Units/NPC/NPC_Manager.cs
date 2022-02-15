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

    private void Start()
    {
        transform.position = NPC.currentPosition;
    }

    void NextAction(DestinationPathsSO path)
    {
        //open door animation
        // change state if needed
        if (path.hasDoor)
        {
            //door animation, could continue only after animation is over
            //if animator is null, then waitforseconds(lengthofanimation).

            if (path.continueMoving)
            {
                NPC.RaiseEventMove();
            }
            else
            {
                // switch to next state
            }
        }



        Debug.Log(path);



        if (NPC.currentDestinationIndex < NPC.destinationPaths.Length - 1)
        {
            NPC.currentDestinationIndex++;
            
        }

        

    }

    void MoveNPC()
    {
    }


}