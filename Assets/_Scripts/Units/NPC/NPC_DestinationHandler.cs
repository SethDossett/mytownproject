using UnityEngine;

public class NPC_DestinationHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] NPC_ScriptableObject _npc;
    [SerializeField] NPC_StateHandler _stateHandler;
    private Pathfinding.AILerp AILerp;
    private Pathfinding.AIDestinationSetter AIDestinationSetter;
    Transform _transform;


    Vector3 _currentPosition;
    Vector3 _destination;
    Transform _nextDestination;
    int currentDestination;
    [SerializeField] bool _atDestination;
    private void Awake()
    {
        _transform = transform;
        AILerp = GetComponent<Pathfinding.AILerp>();
        AIDestinationSetter = GetComponent<Pathfinding.AIDestinationSetter>();
        //_stateHandler = NPC_StateHandler.instance;
    }
    private void Start()
    {
        _npc.runDestination = false;
        _npc.atDestination = true;
    }
    private void Update()
    {
        //put in a timed check so Update only runs once per second



        if (!_npc.runDestination)
            return;

        if (AILerp == null)
        {
            Debug.Log("AILerp Script is null");
            return;
        }

        if (AILerp.enabled)
        {
            if (AILerp.reachedEndOfPath)
            {
                _npc.atDestination = true;
                Debug.Log("reachedDestination");
            }
        }

        DestinationCheck();
    }

    private void DestinationCheck()
    {
        currentDestination = _npc.currentDestinationIndex;

        if (_npc.atDestination)
        {
            AtDestination();
            return;
        }

        if (AILerp.enabled == false)
        {
            AIDestinationSetter.target = _npc.destinations[currentDestination];
            AILerp.enabled = true;
            _stateHandler.UpdateNPCState(NPC_StateHandler.NPCSTATE.WALKING);
            return;
        }

        return;
    }
    

    private void AtDestination()
    {
        if (AILerp.enabled)
            AILerp.enabled = false;

        if (_npc.destinations.Length != 0 && _npc.currentDestinationIndex < _npc.destinations.Length - 1)
        {
            _nextDestination = _npc.destinations[currentDestination + 1];
            AIDestinationSetter.target = _nextDestination;
            _npc.currentDestinationIndex++;
        }
        
        _stateHandler.UpdateNPCState(NPC_StateHandler.NPCSTATE.STANDING);
        _npc.runDestination = false;
        return;
    }
}


