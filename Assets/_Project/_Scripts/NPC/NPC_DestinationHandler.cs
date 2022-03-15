using UnityEngine;
using UnityEngine.Events;
using MyTownProject.Events;
using System.Collections;

namespace MyTownProject.NPC
{
    public class NPC_DestinationHandler : MonoBehaviour
    {
        public static UnityAction<DestinationPathsSO> UpdateScene;

        [Header("References")]
        [SerializeField] NPC_ScriptableObject NPC;
        private DestinationPathsSO Path;
        [SerializeField] NPC_StateHandler _stateHandler;
        [SerializeField] StateChangerEventSO _stateChanger;
        //private Pathfinding.AILerp AILerp;
        //private Pathfinding.AIDestinationSetter AIDestinationSetter;
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
        }*/ //old method for moving using A*. keeping just in case.

        void ChangeState() => _stateChanger.RaiseNPCStateVoid();

        private void OnEnable()
        {
            NPC.OnMove += MoveTowardDestination;
        }
        private void OnDisable()
        {
            NPC.OnMove -= MoveTowardDestination;
        }
        private void Awake()
        {
            _transform = transform;
            rb = GetComponent<Rigidbody>();
            //AILerp = GetComponent<Pathfinding.AILerp>();
            //AIDestinationSetter = GetComponent<Pathfinding.AIDestinationSetter>();
        }
        private void Start()
        {
            SetUp();
            _currentPosition = NPC.currentPosition; // may need rotation as well for after loading game after save.
            _transform.position = _currentPosition;
        }

        private void SetUp()
        {
            if (NPC.destinationPaths == null || NPC.destinationPaths.Length == 0)
                return;

            Path = NPC.destinationPaths[NPC.currentDestinationIndex];
            NPC.currentScene = Path.thisPathScene;
            ChangeState();
        }
        private void MoveTowardDestination()
        {
            if(!NPC.moveTowardsDestination)
                NPC.moveTowardsDestination = true;

            SetPath();
        }
        private void FixedUpdate()
        {
            if (!_shouldMove)
                return;

            DoMove();
        }
        private void Update()
        {
            _shouldMove = NPC.moveTowardsDestination;
            NPC.currentPosition = _currentPosition;
            
            if (!_shouldMove)
                return;

            CheckDistance(Path.path[Path.index]);
        }
        private void SetPath()
        {
            Path = NPC.destinationPaths[NPC.currentDestinationIndex];
            NPC.currentScene = Path.thisPathScene;
            _currentPosition = Path.startPosition;
            _transform.position = _currentPosition;
            //unhide npc ChangeVisibility
            Path.index = 0;
            _atDestination = false; // just for debug
            ChangeState();
        }
        private void DoMove()
        {
            //_moveTowards = _currentPosition;
            //Quaternion _rotation = Quaternion.LookRotation(rb.velocity, Vector3.up);
            //_rotation.x = 0;
            //_rotation.z = 0;
            _currentPosition = Vector3.MoveTowards(_currentPosition, Path.path[Path.index], NPC.MoveSpeed * Time.fixedDeltaTime);

            //rb.position = _currentPosition;
           //rb.MovePosition(_currentPosition);
           //rb.rotation = Quaternion.RotateTowards(rb.rotation, _rotation, 200 * Time.fixedDeltaTime);
        }
        private void CheckDistance(Vector3 destination)
        {
            if (Vector3.Distance(_currentPosition, destination) <= 0.1f)
            {
                UpdateValues();
            }
        }
        private void UpdateValues()
        {
            if (Path.index >= Path.path.Length - 1) //at end of array
            {
                NPC.moveTowardsDestination = false;
                _atDestination = true; // just for debug
                StartCoroutine(DestinationReached());
                return;
            }

            Path.index++;
        }
        
        IEnumerator DestinationReached()
        {
            if (Path.continueMoving)
            {
                if (NPC.currentDestinationIndex < NPC.destinationPaths.Length - 1)
                {
                    NPC.currentDestinationIndex++;
                    if (Path.needToTeleport)
                    {
                        //hide npc
                        NPC.currentScene = Path.nextSceneAfterDestination;
                        //UpdateScene?.Invoke(Path);
                        yield return new WaitForEndOfFrame();
                    }

                    SetPath();
                    NPC.moveTowardsDestination = true;
                    ChangeState();
                }
                else
                {
                    NPC.moveTowardsDestination = false;
                    ChangeState();
                }
            }
            else
            {
                NPC.moveTowardsDestination = false;
                ChangeState();
            }
            yield break;
        }

        

    }
}