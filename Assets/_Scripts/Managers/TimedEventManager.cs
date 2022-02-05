using UnityEngine;

public class TimedEventManager : MonoBehaviour
{
    private TimedEventMaster _master;
    [SerializeField] NPC_ScriptableObject[] _ScriptableObjects;
    int jerry = 0;

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
        _ScriptableObjects[jerry].currentDestinationIndex = 3;
        _ScriptableObjects[0].atDestination = false;
        _ScriptableObjects[0].runDestination = true;

    }
    private void SevenEvent()
    {
        _ScriptableObjects[1].currentDestinationIndex = 0;
        _ScriptableObjects[1].atDestination = false;
        _ScriptableObjects[1].runDestination = true;

    }
    private void SevenThirtyEvent()
    {
        

    }
    private void EightEvent()
    {
        

    }
}
