using UnityEngine;

public class NPC_DestinationHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] NPC_ScriptableObject NPC;
    [SerializeField] NPC_StateHandler _stateHandler;
    private Pathfinding.AILerp AILerp;
    private Pathfinding.AIDestinationSetter AIDestinationSetter;
    Transform _transform;
    Rigidbody rb;

    Vector3 _currentPosition;
    Vector3 _destination;
    Transform _nextDestination;
    int currentDestination;
    [SerializeField] bool _atDestination;
    bool _shouldMove;

    float timer;
    float duration = 5f;
    private void Awake()
    {
        _transform = transform;
        rb = GetComponent<Rigidbody>();
        AILerp = GetComponent<Pathfinding.AILerp>();
        AIDestinationSetter = GetComponent<Pathfinding.AIDestinationSetter>();
        //_stateHandler = NPC_StateHandler.instance;
    }
    private void Start()
    {
        NPC.runDestination = false;
        NPC.atDestination = true;


        NPC.lastDestinationPosition = transform.position;
        NPC.Index = 0;
        _shouldMove = true;
    }
    /*private void Update()
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
    }*/
    private void FixedUpdate()
    {
        if (!_shouldMove)
            return;
        
        timer += Time.fixedDeltaTime;
        
        float pct = timer / duration;

        Vector3 moveTowards = _transform.position;
        moveTowards = Vector3.MoveTowards(_transform.position, NPC.Destination[NPC.Index], NPC.MoveSpeed * Time.fixedDeltaTime);

        //_transform.position = Vector3.Lerp(NPC.lastDestinationPosition, NPC.Destination[NPC.Index], pct);
        //_transform.position = Vector3.MoveTowards(_transform.position, NPC.Destination[NPC.Index], NPC.MoveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(moveTowards);


    }
    private void Update()
    {
        if (!_shouldMove)
        {
            _stateHandler.UpdateNPCState(NPC_StateHandler.NPCSTATE.STANDING);
            return;
        }
        CheckDistance(NPC.Destination[NPC.Index]);

        _stateHandler.UpdateNPCState(NPC_StateHandler.NPCSTATE.WALKING);

    }
    
    private void CheckDistance(Vector3 destination)
    {
        if (Vector3.Distance(_transform.position, destination) <= 0.1f)
        {
            //at destination
            //check if we need to fire event

            UpdateValues();
        }
    }
    private void UpdateValues()
    {
        NPC.lastDestinationPosition = _transform.position;
        timer = 0;
        if (NPC.Index >= NPC.Destination.Length - 1)
        {
            _shouldMove = false;
            return;
        }


        NPC.Index++;
    }
}


