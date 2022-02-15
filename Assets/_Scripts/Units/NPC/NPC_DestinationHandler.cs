using UnityEngine;
using UnityEngine.Events;

public class NPC_DestinationHandler : MonoBehaviour
{
    public static UnityAction<DestinationPathsSO> OnDestinationReached;

    [Header("References")]
    [SerializeField] NPC_ScriptableObject NPC;
    private DestinationPathsSO Path;
    [SerializeField] NPC_StateHandler _stateHandler;
    private Pathfinding.AILerp AILerp;
    private Pathfinding.AIDestinationSetter AIDestinationSetter;
    Transform _transform;
    Rigidbody rb;


    [Header("Values")]
    [SerializeField] bool _atDestination;
    Vector3 _moveTowards;
    Vector3 _currentPosition;
    Vector3 _destination;
    Transform _nextDestination;
    int currentDestination;
    
    bool _shouldMove;
    private void OnEnable()
    {
        NPC.move += SetPath;
    }
    private void OnDisable()
    {
        NPC.move -= SetPath;
    }
    private void Awake()
    {
        _transform = transform;
        rb = GetComponent<Rigidbody>();
        AILerp = GetComponent<Pathfinding.AILerp>();
        AIDestinationSetter = GetComponent<Pathfinding.AIDestinationSetter>();
    }
    private void Start()
    {
        NPC.runDestination = false;
        NPC.atDestination = true;


        
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

        _moveTowards = _transform.position;
        Quaternion _rotation = Quaternion.LookRotation(rb.velocity, Vector3.up);
        _rotation.x = 0;
        _rotation.z = 0;
        _moveTowards = Vector3.MoveTowards(_transform.position, Path.path[Path.index], NPC.MoveSpeed * Time.fixedDeltaTime);
       


        rb.MovePosition(_moveTowards);
        rb.rotation = Quaternion.RotateTowards(rb.rotation, _rotation,  200 * Time.fixedDeltaTime);


    }
    private void Update()
    {
        if (!_shouldMove)
        {
            if(_stateHandler.npcState != NPC_StateHandler.NPCSTATE.STANDING)
                _stateHandler.UpdateNPCState(NPC_StateHandler.NPCSTATE.STANDING);
            return;
        }
        
        CheckDistance(Path.path[Path.index]);

        if (_stateHandler.npcState != NPC_StateHandler.NPCSTATE.WALKING)
            _stateHandler.UpdateNPCState(NPC_StateHandler.NPCSTATE.WALKING);

    }
    private void SetPath()
    {
        Path = NPC.destinationPaths[NPC.currentDestinationIndex];
        Path.index = 0;
        _shouldMove = true;
        _atDestination = false; 
    }
    private void CheckDistance(Vector3 destination)
    {
        if (Vector3.Distance(_transform.position, destination) <= 0.1f)
        {
            
            //CheckNewLocation(destination, NPC.significantLocation);
            UpdateValues();
        }
    }
    private void UpdateValues()
    {
        if (Path.index >= Path.path.Length - 1) //at end of array
        {
            _shouldMove = false;
            _atDestination = true;
            OnDestinationReached?.Invoke(Path);
            return;
        }

        Path.index++;
    }
    private void CheckNewLocation(Vector3 pos, Vector3[] sigLoc)
    {
        foreach(var loc in sigLoc)
        {
            if (pos == loc)
            {
                
                _shouldMove = false;
            }

            return;
        }
    }
}


