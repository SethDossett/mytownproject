using UnityEngine.InputSystem;
using UnityEngine;
using MyTownProject.Core;
using MyTownProject.Events;
using MyTownProject.Cameras;
using KinematicCharacterController.Examples;
using System.Collections;
using System.Collections.Generic;

namespace MyTownProject.Interaction
{
    public class PlayerRacasting : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] UIEventChannelSO uiEventChannel;
        [SerializeField] FloatEventSO RecenterCamX;
        [SerializeField] FloatEventSO RecenterCamY;
        [SerializeField] TheCharacterController CC;
        private IInteractable _interactable;
        private NewControls _inputActions;
        private InputAction _interact;
        private InputAction _cameraInput;
        private InputAction _crouchInput;
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
        [SerializeField] TransformEventSO _targetingEvent;
        [SerializeField] TransformEventSO _changeTargetEvent;
        [SerializeField] GeneralEventSO _unTargetingEvent;


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
        bool _enemyLocked;
        bool _isTalking;
        float currentYOffset;
        [SerializeField] Vector3 pos;
        
        float _timer = 0;
        bool _startTimer;
        [Range(0, 0.5f)][SerializeField] float _nextTargetWindow = 0.25f;
        [SerializeField] float dis;

        [Header("Targets")]
        [SerializeField] Collider[] nearbyTargets;
        [SerializeField] List<Transform> remainingTargets = new List<Transform>();

        #endregion
        private void OnEnable()
        {
            GameStateManager.OnGameStateChanged += CheckGameState;
            TheCharacterController.OnPlayerStateChanged += CheckPlayerState;
            _inputActions = new NewControls();
            _interact = _inputActions.GamePlay.Interact;
            _cameraInput = _inputActions.GamePlay.Camera;
            _crouchInput = _inputActions.GamePlay.Crouch;
            _interact.Enable();
            _cameraInput.Enable();
            _crouchInput.Enable();
        }
        private void OnDisable()
        {
            GameStateManager.OnGameStateChanged -= CheckGameState;
            TheCharacterController.OnPlayerStateChanged -= CheckPlayerState;
            _interact.Disable();
            _cameraInput.Disable();
            _crouchInput.Disable();
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
        void CheckGameState(GameStateManager.GameState state)
        {
            if(state == _game_Playing_State)
                canRaycast = true;
            else
                canRaycast = false;
        }
        void CheckPlayerState(CharacterState state)
        {
            if (state == CharacterState.Climbing)
                canRaycast = false;
            else if (state == CharacterState.Default)
                canRaycast = true;
            else
                canRaycast= true;

        }
        private void Update()
        {
            if (!canRaycast)
                return;
            

            //CheckForInteractable();//old version
            //Interactor();//old version

            camFollow.lockedTarget = _enemyLocked;
            //defMovement.lockMovement = enemyLocked;
            if(currentTarget){
                if(_cameraInput.ReadValue<Vector2>().magnitude > 0){
                    cam.gameObject.GetComponent<Cinemachine.CinemachineBrain>().enabled = true;
                    RecenterCamX.RaiseEvent2(0,0.5f);
                    RecenterCamY.RaiseEvent2(0, 0.5f);
                }
            }
            
            IconControl();
            CheckTimer();
            CheckForIInteractable();
            

            if(_closestTarget != null){

                if(!StillClosestTarget(_closestTarget)){
                    //_closestTarget.gameObject.GetComponent<NPC_Interact>()._hovered = false;
                    _closestTarget = null;
                }
            }
            //Keyboard.current.shiftKey.wasPressedThisFrame
            if (_crouchInput.WasPressedThisFrame())
            {
                print("Fire");
                if (_startTimer == true && remainingTargets.Count > 0)
                {
                    if (_timer <= _nextTargetWindow)
                        StartCoroutine(FindNextTarget());
                    return;
                }
                if(_closestTarget != null){
                    pos = _closestTarget.position;
                    currentTarget = _closestTarget;
                }
                if (currentTarget)
                {
                    CC._target = currentTarget;
                     // so we dont lose closest target when switching
                    if(_enemyLocked) StartCoroutine(FindNextTarget()); else FoundTarget();
                }
                if (currentTarget == null && _closestTarget == null) CC._canCrouch = true;


            }
            //Keyboard.current.shiftKey.wasReleasedThisFrame
            if (_crouchInput.WasReleasedThisFrame())
            {
                CC._canCrouch = false;
                if (currentTarget != null)
                    _startTimer = true;
            }

            if (!_enemyLocked)
            {
                CheckForNPCS();
            }
            if (_enemyLocked)
            {
                if (!TargetOnRange()) StartCoroutine(FindNextTarget());
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

                if (!_interactable.CanBeInteractedWith || _hitinfo.distance > _interactable.MaxNoticeRange)
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
        //private void Interactor()
        //{
        //    _numFound = Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interactionPointRadius, _colliders, _interactableMask);
//
        //    
        //    if (_numFound > 0)
        //    {
        //        _interactable = _colliders[0].GetComponentInParent<IInteractable>();
        //        
        //        if (_interactable == null) return;
//
        //        uiEventChannel.ShowTextInteract(_interactable.Prompt);
//
        //        if (_interact.WasPerformedThisFrame()) _interactable.OnInteract(this);
        //    }
        //    else
        //    {
        //        if (_interactable != null) _interactable = null;
//
        //        uiEventChannel.HideTextInteract();
        //    }
        //}
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
                    Transform t = nearbyTargets[i].transform;
                    float dis = GetDistance(transform, t);
                    float ang = GetAngle(t.position, transform.position, transform.forward, 0);
                    if(t.gameObject.GetComponent<IInteractable>().CanBeTargeted){
                        if(ang <= maxNoticeAngle){
                            if (dis < closestDis){
                                closestTarget = t;
                                closestDis = dis; 
                                _NPCIndex = i;
                            }
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
            if(Blocked(closestTarget.position + Vector3.up * 1.2f)){
                closestTarget.gameObject.GetComponent<NPC_Interact>()._hovered = false;
                if(_closestTarget){
                    _closestTarget = null;
                    ResetTarget();
                }
                print("Blocked");
                return;
            } 
            if(GetDistance(transform, closestTarget) > closestTarget.GetComponent<IInteractable>().MaxNoticeRange){
                //print(GetDistance(transform, closestTarget));
                _closestTarget = null;
                return; // not working right with findByAngle
            }
            if(GetAngle(closestTarget.position,transform.position,transform.forward, 0) > maxNoticeAngle){
                //print(GetAngle(closestTarget.position,transform.position,transform.forward, 0));
                _closestTarget = null;
                return;
            }
            //closestTarget.gameObject.GetComponent<NPC_Interact>().Hovered(); // being called over and over, or 
            if (_closestTarget != null) {
                //_closestTarget.gameObject.GetComponent<NPC_Interact>()._hovered = false;
            }

            _closestTarget = closestTarget;
            _closestTarget.gameObject.GetComponent<NPC_Interact>()._hovered = true;

        }
        bool StillClosestTarget(Transform t){
            if(Blocked(t.position + Vector3.up * 1.2f)){ //
                t.gameObject.GetComponent<NPC_Interact>()._hovered = false;
                print("Blocked");
                 return false;
            }
            if(GetAngle(t.position,transform.position,transform.forward, 0) > maxNoticeAngle){
                //HideHover(t);
                 return false;
            }
            if(GetDistance(transform, t) > t.gameObject.GetComponent<IInteractable>().MaxNoticeRange){
                //HideHover(t);
                return false;
            } 

            return true;
        }

        bool Blocked(Vector3 t)
        {
            RaycastHit hit;
            if (Physics.Linecast(transform.position + Vector3.up * 1.5f, t, out hit))
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
            _targetingEvent.RaiseEvent(currentTarget);
            CC.TransitionToState(CharacterState.Targeting);
            currentTarget.gameObject.GetComponent<NPC_Interact>()._targeted = true;
            //HideHover(currentTarget);
            //currentTarget.gameObject.GetComponent<NPC_Interact>().Targeted(); //Make Events that fire for UI Targeted
            lockOnCanvas.gameObject.SetActive(true);
            //anim.SetLayerWeight(1, 1);
            //cinemachineAnimator.Play("TargetingCamera01");
            cam.gameObject.GetComponent<Cinemachine.CinemachineBrain>().enabled = false; // needs to be event fired
            cam.gameObject.GetComponent<KinematicCharacterController.Examples.ExampleCharacterCamera>().isTargeting = true;
            _enemyLocked = true;
            _closestTarget.GetComponent<NPC_Interact>().SetTargeted(); //Set NPC as been targeted.

            for (int i = 0; i < nearbyTargets.Length; i++)
            {
                Transform t = nearbyTargets[i].transform;
                remainingTargets.Add(t);
                print(GetDistance(transform,t) + t.gameObject.name);
                if (t.gameObject.GetComponent<NPC_Interact>().beenTargeted == true)
                {
                    remainingTargets.Remove(t);
                }
                if (GetDistance(transform, t) > t.GetComponent<IInteractable>().MaxNoticeRange)
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
            //HideHover(currentTarget);
            currentTarget.gameObject.GetComponent<NPC_Interact>()._targeted = false;
            if (remainingTargets.Count <= 0) ResetTarget();
            else
            {
                float closestDis = 10f; // change to max range
                Transform closetT = null;
                foreach(var target in nearbyTargets){
                    print(GetAngle(transform.position, target.transform.position, transform.forward, 0));
                    if(target.gameObject.GetComponent<NPC_Interact>().beenTargeted == false){
                        float disP = GetDistance(transform, target.transform);
                        if(disP <= target.transform.GetComponent<IInteractable>().MaxNoticeRange){
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
                currentTarget.gameObject.GetComponent<NPC_Interact>()._targeted = true;
                CC._target = currentTarget;
                //currentTarget.gameObject.GetComponent<NPC_Interact>().Targeted();
                closetT.gameObject.GetComponent<NPC_Interact>().beenTargeted = true;
                _changeTargetEvent.RaiseEvent(currentTarget);
            }

            yield return null;

        }
        void ResetTarget()
        {
            print("reset");
            //currentTarget.gameObject.GetComponent<NPC_Interact>().HideTargeted();
            currentTarget.gameObject.GetComponent<NPC_Interact>()._targeted = false;
            _unTargetingEvent.RaiseEvent();
            if(CC.CurrentCharacterState != CharacterState.Talking) CC.TransitionToState(CharacterState.Default);
            _startTimer = false;
            _timer = 0;
            cam.gameObject.GetComponent<Cinemachine.CinemachineBrain>().enabled = true;
            lockOnCanvas.gameObject.SetActive(false);
            currentTarget = null;
            _closestTarget = null;
            _enemyLocked = false;
            //anim.SetLayerWeight(1, 0);
            cinemachineAnimator.Play("PlayerFreeLook01");
            _NPCIndex = 0;
            BeenTargetedReset();
        }

        void BeenTargetedReset()
        {
            foreach (var npc in nearbyTargets)
            {
                NPC_Interact m = npc.GetComponent<NPC_Interact>(); // set npc back to not being targeted
                m.UnsetTargeted();
                //m.UnTargeted();
                //HideHover(m.gameObject.transform);
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
            //currentTarget.GetComponent<NPC_Interact>().Targeted();
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
                    if(npc.gameObject.transform != _closestTarget){
                        //HideHover(npc.gameObject.transform);
                        npc.gameObject.GetComponent<NPC_Interact>()._hovered = false;
                    }
                    else{
                        //npc.gameObject.GetComponent<NPC_Interact>()._hovered = false;
                        //npc.gameObject.GetComponent<NPC_Interact>().Hovered();
                    } 
                } 
            }
            else
            {
                foreach (var npc in nearbyTargets)
                {
                    
                    npc.gameObject.GetComponent<NPC_Interact>()._hovered = false;
                    
                }
                
            }

            if(currentTarget != null){
                foreach(var npc in nearbyTargets){
                    if(npc.gameObject.transform != currentTarget){
                        //npc.gameObject.GetComponent<NPC_Interact>().HideTargeted();
                    }
                } 
            }
            
        }
        bool tarr;
        bool _hovering;
        bool _targeting;
        void HideHover(Transform t){
            if(!t.gameObject.GetComponent<NPC_Interact>()._hovered) return;
            
            //t.gameObject.GetComponent<NPC_Interact>().HideHover();
            t.gameObject.GetComponent<NPC_Interact>()._hovered = true;

            print($"Hide Hover" + t.gameObject.name);
         }
        private void LockOnCanvas()
        {
            lockOnCanvas.position = pos;
            lockOnCanvas.localScale = Vector3.one * ((cam.position - pos).magnitude * crossHair_Scale);
        }

        void CheckForIInteractable(){
            
            if(currentTarget == null){
                if(_closestTarget == null){
                    ClearIInteractable();
                    return;
                }
                else TestForInteraction(_closestTarget);
            }
            else TestForInteraction(currentTarget);
        }

        void TestForInteraction(Transform t){
            _interactable = t.gameObject.GetComponent<IInteractable>();
            if (_interactable == null){
                ClearIInteractable();
                return;
            }
            if(!_interactable.CanBeInteractedWith){
                ClearIInteractable();
                return;
            }
            if(GetDistance(transform, t) > _interactable.MaxInteractRange){
                ClearIInteractable();
                return;
            } 
            
            uiEventChannel.ShowTextInteract(_interactable.Prompt);
            if (_interact.WasPerformedThisFrame()){
                Debug.Log($"interacted with {t.gameObject.name}");
                CC.TransitionToState(CharacterState.Talking);
                if(currentTarget) ResetTarget();
                cam.gameObject.GetComponent<Cinemachine.CinemachineBrain>().enabled = true;
                cam.gameObject.GetComponent<KinematicCharacterController.Examples.ExampleCharacterCamera>().isTargeting = false;
                _interactable.OnInteract(this);
                _isTalking = true;
                return;
            } 

        }
        void ClearIInteractable(){
            if (_interactable != null) _interactable = null;
            uiEventChannel.HideTextInteract();
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