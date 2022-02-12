using UnityEngine;

public class TimedEventManager : MonoBehaviour
{
    private TimedEventMaster _master;
    [SerializeField] NPC_ScriptableObject[] NPC;
    int jerry = 0;
    int boyty = 2;
    void OnEnable()
    {
        _master = GameObject.Find("EventMaster").GetComponent<TimedEventMaster>();
        _master.sixThirtyEvent += SixThirtyEvent;
        _master.sevenEvent += SevenEvent;
        _master.sevenThirtyEvent += SevenThirtyEvent;
        _master.eightEvent += EightEvent;
    }
    void OnDisable()
    {
        _master.sixThirtyEvent -= SixThirtyEvent;
    }
    
    private void SixThirtyEvent()
    {
        NPC[boyty].currentDestinationIndex = 0;
        NPC[boyty].RaiseEventMove();


        NPC[jerry].currentDestinationIndex = 3;
        NPC[0].atDestination = false;
        NPC[0].runDestination = true;

    }
    private void SevenEvent()
    {
        NPC[1].currentDestinationIndex = 0;
        NPC[1].atDestination = false;
        NPC[1].runDestination = true;

    }
    private void SevenThirtyEvent()
    {
        

    }
    private void EightEvent()
    {
        

    }
}
