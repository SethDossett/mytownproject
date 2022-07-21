using UnityEngine.InputSystem;
using UnityEngine;
using MyTownProject.Core;
using MyTownProject.Events;
using KinematicCharacterController.Examples;
using System.Collections;
using System.Collections.Generic;

namespace MyTownProject.Interaction
{
    public class PlayerRacasting : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] UIEventChannelSO uiEventChannel;
        [SerializeField] TheCharacterController CC;
        private IInteractable _interactable;
        private NewControls _inputActions;
        private InputAction _interact;
        private GameStateManager.GameState _game_Playing_State;
        private RaycastHit _hitinfo;
        private Transform _transform;
        [SerializeField] Transform _interactionPoint;
        

        [Header("Values")]
        [SerializeField] float _interactionPointRadius;
        [SerializeField] LayerMask _interactableMask;
        [SerializeField] private float _interactRayLength = 5f;
        [SerializeField] private Vector3 _offset;
        [SerializeField] bool canRaycast = false;
        private readonly Collider[] _colliders = new Collider[3];
        [SerializeField] private int _numFound;

        #region New Lock On
        [Header("References")]
        [SerializeField] Transform currentTarget;
        [SerializeField] Transform _closestTarget = null;
        [SerializeField] CameraFollow camFollow;
        [SerializeField] Transform lockOnCanvas;
        Animator anim;
        Transform cam;
        //DefMovement defMovement;
        [SerializeField] Transform enemyTarget_Locator;


        [Tooltip("StateDrivenMethod for Switching Cameras")]
        [SerializeField] Animator cinemachineAnimator;

        [Header("Settings")]
        [SerializeField] bool zeroVert_Look;
        [SerializeField] float noticeZone = 10;
        [SerializeField] float lookAtSmoothing = 2;
        [Tooltip("Angle_Degree")][SerializeField] float maxNoticeAngle = 60;
        [SerializeField] float crossHair_Scale = 0.1f;
        float maxDistance = 25f;
        


        [Header("Values")]
        [SerializeField] bool findByAngle;
        [SerializeField] LayerMask targetLayers;
        [SerializeField] int _NPCIndex;
        bool enemyLocked;
        float currentYOffset;
        Vector3 pos;
        
        float _timer = 0;
        bool _startTimer;
        float _nextTargetWindow;
        [SerializeField] float dis;

        [Header("Targets")]
        [SerializeField] Collider[] nearbyTargets;
        [SerializeField] List<Transform> remainingTargets = new List<Transform>();

        #endregion
        private void OnEnable()
        {
            GameStateManager.OnGameStateChanged += CheckState;
            _inputActions = new NewControls();
            _interact = _inputActions.GamePlay.Interact;
            _interact.Enable();
        }
        private void OnDisable()
        {
            GameStateManager.OnGameStateChanged -= CheckState;
            _interact.Disable();
        }
        private void Start()
        {
            CC = GetComponent<TheCharacterController>();
            _game_Playing_State = GameStateManager.GameState.GAME_PLAYING;
            _transform = transform;
            anim = GetComponent<Animator>();
            cam = Camera.main.transform;
            lockOnCanvas.gameObject.SetActive(false);
            _startTimer = false;
        }
        void CheckState(GameStateManager.GameState state)
        {
            if(state == _game_Playing_State)
                canRaycast = true;
            else
                canRaycast = false;
        }
        private void Update()
        {
            if (!canRaycast)
                return;

            //CheckForInteractable();//old version
            //Interactor();//old version

            camFollow.lockedTarget = enemyLocked;
            //defMovement.lockMovement = enemyLocked;


            IconControl();
            CheckTimer();

            if (Keyboard.current.shiftKey.wasPressedThisFrame)
            {
                print("Fire");
                if (_startTimer == true && remainingTargets.Count > 0)
                {
                    if (_timer <= _nextTargetWindow)
                        StartCoroutine(FindNextTarget());
                    return;
                }

                if(_closestTarget != null)
                    currentTarget = _closestTarget;
                if (currentTarget)
                {
                    //If there is already a target, Reset.
                    print("eNTER Talk mode");
                    if(enemyLocked) StartCoroutine(FindNextTarget()); else FoundTarget();
                }

            }

            if (Keyboard.current.shiftKey.wasReleasedThisFrame)
            {
                if (currentTarget != null)
                    _startTimer = true;
            }

            if (!enemyLocked)
            {
                CheckForNPCS();
            }
            if (enemyLocked)
            {
                if (!TargetOnRange()) ResetTarget();
                LookAtTarget();
            }

        }
        #region Old Raycast Method
        private void CheckForInteractable()

        {
            Ray ray = new Ray(_transform.position + _offset, _transform.forward);
            Debug.DrawRay(ray.origin, ray.direction * _interactRayLength);
            Debug.DrawRay(ray.origin, ray.direction * 2f, Color.green);
            if (Physics.Raycast(ray, out _hitinfo, _interactRayLength))
            {
                
                _interactable = _hitinfo.collider.GetComponent<IInteractable>();
                if(_interactable == null)
                {
                    if (_interactable != null)
                    {
                        _interactable.OnLoseFocus();
                        uiEventChannel.HideTextInteract();
                        _interactable = null;
                        return;
                    }
                    return;
                }

                if (!_interactable.CanBeInteractedWith || _hitinfo.distance > _interactable.MaxRange)
                {
                    if (_interactable != null)
                    {
                        _interactable.OnLoseFocus();
                        uiEventChannel.HideTextInteract();
                        _interactable = null;
                        return;
                    }
                    return;
                }

                uiEventChannel.ShowTextInteract(_interactable.Prompt);
                //currentTarget.OnFocus(_hitinfo.collider.gameObject.name);

                if (_interact.WasPerformedThisFrame()) _interactable.OnInteract(this);

            }
            else
            {
                if (_interactable != null) _interactable = null;
                
                uiEventChannel.HideTextInteract();
            }
        }
        #endregion
        #region Old Overlap Sphere Method
        private void Interactor()
        {
            _numFound = Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interactionPointRadius, _colliders, _interactableMask);

            
            if (_numFound > 0)
            {
                _interactable = _colliders[0].GetComponentInParent<IInteractable>();
                
                if (_interactable == null) return;

                uiEventChannel.ShowTextInteract(_interactable.Prompt);

                if (_interact.WasPerformedThisFrame()) _interactable.OnInteract(this);
            }
            else
            {
                if (_interactable != null) _interactable = null;

                uiEventChannel.HideTextInteract();
            }
        }
        #endregion
        void CheckTimer()
        {
            if (_startTimer)
            {
                _timer += Time.deltaTime;
            }


            if (_timer > _nextTargetWindow)
            {
                ResetTarget();
            }
        }
        private void CheckForNPCS()
        {
            nearbyTargets = Physics.OverlapSphere(transform.position, noticeZone, targetLayers);
            float closestAngle = maxNoticeAngle;
            float closestDis = maxDistance;
            Transform closestTarget = null;
            if (findByAngle)
            {
                if (nearbyTargets.Length <= 0) return;
                for (int i = 0; i < nearbyTargets.Length; i++)
                {

                    Vector3 dir = nearbyTargets[i].transform.position - cam.position;
                    dir.y = 0;
                    float _angle = Vector3.Angle(cam.forward, dir);

                    if (_angle < closestAngle)
                    {
                        closestTarget = nearbyTargets[i].transform;
                        closestAngle = _angle;
                        _NPCIndex = i;
                    }

                }
            }
            else
            {
                if (nearbyTargets.Length <= 0) return;
                for (int i = 0; i < nearbyTargets.Length; i++)
                {
                    float dis = GetDistance(transform, nearbyTargets[i].transform);
                    float ang = GetAngle(nearbyTargets[i].transform.position, transform.position,transform.forward, 1);
                    if(ang <= maxNoticeAngle){
                        if (dis < closestDis){
                        closestTarget = nearbyTargets[i].transform;
                        closestDis = dis; 
                        _NPCIndex = i;
                        }
                }

                }

            }

            if (!closestTarget) return;
            float h1 = closestTarget.GetComponent<CapsuleCollider>().height;
            float h2 = closestTarget.localScale.y;
            float h = h1 * h2;
            float half_h = (h / 2) / 2;
            currentYOffset = h - half_h;
            if (zeroVert_Look && currentYOffset > 1.6f && currentYOffset < 1.6f * 3) currentYOffset = 1.6f;
            Vector3 tarPos = closestTarget.position + new Vector3(0, currentYOffset, 0);
            if(Blocked(tarPos)){
                if(_closestTarget){
                    _closestTarget = null;
                    ResetTarget();
                }
                return;
            } 
            if(GetDistance(transform, closestTarget) > closestTarget.GetComponent<NPC_Interact>().MaxRange){
                    print(GetDistance(transform, closestTarget));
                    return; // not working right with findByAngle
            }
            _closestTarget = closestTarget;

        }

        bool Blocked(Vector3 t)
        {
            RaycastHit hit;
            if (Physics.Linecast(transform.position + Vector3.up * 0.5f, t, out hit))
            {
                if (!hit.transform.CompareTag("NPC")) return true;
            }
            return false;
        }
        
        bool TargetOnRange()
        {
            dis = (transform.position - pos).magnitude;
            if (dis > noticeZone) return false; else return true;
        }
        float GetDistance(Transform from, Transform to)
        {
            float distance = (from.position - to.position).magnitude;

            return distance;
        }
        float GetAngle(Vector3 loc1, Vector3 loc2, Vector3 forwardDir, int lockYAxis){
            Vector3 dir = loc1 - loc2;
            if(lockYAxis == 1) dir.y = 0;
            float angle = Vector3.Angle(forwardDir, dir);
            return angle;
        }

        void FoundTarget()
        {
            print("Found");
            CC.TransitionToState(CharacterState.Targeting);
            currentTarget.GetComponent<NPCManager>().Targeted(); //Make Events that fire for UI Targeted
            lockOnCanvas.gameObject.SetActive(true);
            anim.SetLayerWeight(1, 1);
            cinemachineAnimator.Play("TargetingCamera01");
            enemyLocked = true;
            _closestTarget.GetComponent<NPCManager>().SetTargeted(); //Set NPC as been targeted.

            for (int i = 0; i < nearbyTargets.Length; i++)
            {
                Transform t = nearbyTargets[i].transform;
                remainingTargets.Add(t);
                if (t.gameObject.GetComponent<NPCManager>().beenTargeted == true)
                {
                    remainingTargets.Remove(t);
                }
                if (GetDistance(transform, t) > t.GetComponent<NPC_Interact>().MaxRange)
                {
                    remainingTargets.Remove(t);
                }

            }
        }
        IEnumerator FindNextTarget()
        {
            print("FindNext");
            _startTimer = false;
            _timer = 0;

            if (remainingTargets.Count <= 0) ResetTarget();
            else
            {
                // MAKE IT FIND NEXT CLOSEST
                // AND NOT BE OUT OF MAXDISTANCE
                //Need To Check if New TArgets have come in or Out!!!!!!!!!!!!!!!!!!!!!!!!!!
                float closestDis = 10f; // change to max range
                Transform closetT = null;
                foreach(var target in nearbyTargets){
                    if(target.gameObject.GetComponent<NPCManager>().beenTargeted == false){
                        float disP = GetDistance(transform, target.transform);
                        if(disP <= target.transform.GetComponent<NPC_Interact>().MaxRange){
                            float disN = GetDistance(currentTarget, target.transform);
                            if(disN < closestDis){
                                closetT = target.transform;
                                closestDis = disN;
                            }
                        }
                        
                    }
                
                }
                if (!closetT) ResetTarget();

                currentTarget = closetT;
                closetT.gameObject.GetComponent<NPCManager>().beenTargeted = true;
            }

            yield return null;

        }
        void ResetTarget()
        {
            print("reset");
            CC.TransitionToState(CharacterState.Default);
            _startTimer = false;
            _timer = 0;
            //nearbyTargets[i].GetComponent<NPCManager>().Hovered();
            lockOnCanvas.gameObject.SetActive(false);
            currentTarget = null;
            _closestTarget = null;
            enemyLocked = false;
            anim.SetLayerWeight(1, 0);
            cinemachineAnimator.Play("PlayerFreeLook01");
            _NPCIndex = 0;
            BeenTargetedReset();
        }

        void BeenTargetedReset()
        {
            foreach (var npc in nearbyTargets)
            {
                NPCManager m = npc.GetComponent<NPCManager>(); // set npc back to not being targeted
                m.UnsetTargeted();
                m.UnTargeted();
                m.HideHover();
            }
            remainingTargets.Clear();
        }
        private void LookAtTarget()
        {
            if (currentTarget == null)
            {
                ResetTarget();
                return;
            }
            //currentTarget.GetComponent<NPCManager>().Targeted();
            pos = currentTarget.position + new Vector3(0, currentYOffset, 0);
           
            enemyTarget_Locator.position = pos;
            Vector3 dir = currentTarget.position - transform.position;
            dir.y = 0;
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * lookAtSmoothing);
        }
        private void IconControl(){
        

            if(_closestTarget != null){
                foreach(var npc in nearbyTargets){
                if(npc.gameObject.transform == _closestTarget && tarr != true){
                    npc.gameObject.GetComponent<NPCManager>().Hovered();
                }
                else npc.gameObject.GetComponent<NPCManager>().HideHover();
                }   
            }
            else{
                foreach(var npc in nearbyTargets){
                    npc.gameObject.GetComponent<NPCManager>().HideHover();
                } 

            }
            // NOT WORKING RIGHT ?
            if(currentTarget != null){
                foreach(var npc in nearbyTargets){
                    if(npc.gameObject.transform == currentTarget && tarr == false){
                        npc.gameObject.GetComponent<NPCManager>().Targeted();
                        tarr = true;
                    }
                    else {
                        npc.gameObject.GetComponent<NPCManager>().HideHover();
                        tarr = false;
                    }   
                }
            }
        }
    bool tarr;

        private void LockOnCanvas()
        {
            lockOnCanvas.position = pos;
            lockOnCanvas.localScale = Vector3.one * ((cam.position - pos).magnitude * crossHair_Scale);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            //Gizmos.DrawWireSphere(transform.position + _offset + transform.forward * _hitinfo.distance, 1.5f);
            Gizmos.DrawWireSphere(_interactionPoint.position, _interactionPointRadius);
            
            Gizmos.DrawWireSphere(transform.position, noticeZone);
            
            
        }

    }
}