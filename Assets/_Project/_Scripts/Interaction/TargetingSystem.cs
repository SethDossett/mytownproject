using UnityEngine.InputSystem;
using UnityEngine;
using MyTownProject.Core;
using MyTownProject.Events;
using MyTownProject.Cameras;
using KinematicCharacterController.Examples;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;

namespace MyTownProject.Interaction
{
    public class TargetingSystem : MonoBehaviour
    {
        [Header("Controls")]
        private NewControls _inputActions;
        private InputAction _interact;
        private InputAction _cameraInput;
        private InputAction _LeftTriggerInput;

        [Header("Events")]
        [SerializeField] UIEventChannelSO uiEventChannel;
        [SerializeField] FloatEventSO RecenterCamX;
        [SerializeField] FloatEventSO RecenterCamY;
        [SerializeField] TransformEventSO _targetingEvent;
        [SerializeField] TransformEventSO _changeTargetEvent;
        [SerializeField] GeneralEventSO _unTargetingEvent;
        [SerializeField] AudioEventSO _audioEvent;

        [Header("References")]
        [SerializeField] Transform currentTarget;
        [SerializeField] Transform _closestTarget = null;
        [SerializeField] TheCharacterController CC;
        private IInteractable _interactable;
        Transform cam;
        CharacterState currentCharacterState;
        private GameStateManager.GameState _game_Playing_State;

        [Header("Settings")]
        [SerializeField] bool zeroVert_Look;
        [SerializeField] float noticeZone = 10;
        [SerializeField] float lookAtSmoothing = 2;
        [Tooltip("Angle_Degree")][SerializeField] float maxNoticeAngle = 60;
        [SerializeField] float crossHair_Scale = 0.1f;
        float maxDistance = 25f;


        [Header("Values")]
        [SerializeField] bool canRaycast = false;
        [SerializeField] bool findByAngle;
        [SerializeField] LayerMask targetLayers;
        [SerializeField] int _NPCIndex;
        public bool _enemyLocked;
        bool _isTalking;
        float currentYOffset;
        [SerializeField] Vector3 pos;
        bool _freeLookCameraOff;
        bool _resettingTarget;
        Vector3 _npcRayPoint = new Vector3(0, 1.2f, 0);
        Vector3 _playerRayPoint = new Vector3(0, 1.5f, 0);
        
        float _timer = 0;
        bool _startTimer;
        [Range(0, 0.5f)][SerializeField] float _nextTargetWindow = 0.25f;
        [SerializeField] float dis;

        [Header("Audio")]
        [SerializeField] EventReference _recenterCameraSFX;
        [SerializeField] EventReference _LockOnSFX;
        [SerializeField] EventReference _LockOffSFX;


        [Header("Targets")]
        [SerializeField] Collider[] nearbyTargets;
        [SerializeField] List<Transform> remainingTargets = new List<Transform>();
        [SerializeField] List<Transform> AvailableTargets = new List<Transform>();
        [SerializeField] List<Transform> AvailableObjects = new List<Transform>();

       
        private void OnEnable()
        {
            SetInitialReferences();
            GameStateManager.OnGameStateChanged += CheckGameState;
            TheCharacterController.OnPlayerStateChanged += CheckPlayerState;
            _inputActions = new NewControls();
            _interact = _inputActions.GamePlay.Interact;
            _cameraInput = _inputActions.GamePlay.Camera;
            _LeftTriggerInput = _inputActions.GamePlay.LeftTrigger;
            _interact.Enable();
            _cameraInput.Enable();
            _LeftTriggerInput.Enable();
            _LeftTriggerInput.performed += CheckForRecenterInput;
            _LeftTriggerInput.canceled += CheckForInputRelease;
        }
        private void OnDisable()
        {
            GameStateManager.OnGameStateChanged -= CheckGameState;
            TheCharacterController.OnPlayerStateChanged -= CheckPlayerState;
            _interact.Disable();
            _cameraInput.Disable();
            _LeftTriggerInput.Disable();
            _LeftTriggerInput.canceled -= CheckForRecenterInput;
            _LeftTriggerInput.canceled -= CheckForInputRelease;
        }
        private void Awake(){
            CC = GetComponent<TheCharacterController>();
            _game_Playing_State = GameStateManager.GameState.GAME_PLAYING;
            cam = Camera.main.transform;
        }
        private void Start()
        { 
            
        }
        void SetInitialReferences(){
            canRaycast = true;
            _startTimer = false;
            _isTalking = false;
        }
        void CheckGameState(GameStateManager.GameState state)
        {
            if(state == _game_Playing_State){
                _interact.Enable();
                _cameraInput.Enable();
                _LeftTriggerInput.Enable();
                canRaycast = true;
            }
            else{
                _interact.Disable();
                _cameraInput.Disable();
                _LeftTriggerInput.Disable();
                canRaycast = false;
            }
        }
        void CheckPlayerState(CharacterState state)
        {
            currentCharacterState = state;
            if (state == CharacterState.Default)
                canRaycast = true;
            else if(state == CharacterState.Targeting)
                canRaycast = true;
            else
                canRaycast = false;    
        }
        private void Update()
        {
            if (!canRaycast) return;

            CC._hasTargetToLockOn = _enemyLocked;
            if(currentTarget && _freeLookCameraOff){ // might not want, it is janky, transitioning from kcc to freelook cam.
                if(_cameraInput.ReadValue<Vector2>().magnitude > 0){
                    ChangeCamera();
                    RecenterCamera(0, 0.5f, 1);
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
            if (_LeftTriggerInput.WasPressedThisFrame())
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
                if (currentTarget == null && _closestTarget == null) {
                    CC.TransitionToState(CharacterState.Targeting);
                    RecenterCamX.ThreeFloats(0, 0.1f, 1);
                    RecenterCamY.ThreeFloats(0, 0.1f, 1);
                    _audioEvent.RaiseEvent2(_recenterCameraSFX, transform.position);
                    uiEventChannel.RaiseBarsOn(0.1f);
                } 


            }
            //Keyboard.current.shiftKey.wasReleasedThisFrame
            if (_LeftTriggerInput.WasReleasedThisFrame())
            {
                if (currentTarget != null)
                    _startTimer = true;
            }

            if (!_enemyLocked)
            {
                //If We Are not Locked On Search for interactables
                SearchForInteractables();
                // If interactables found, then find closest target
                if(AvailableTargets.Count > 0) FindClosestTarget(); 
            }
            if (_enemyLocked)
            {
                if (!TargetOnRange()) StartCoroutine(FindNextTarget());
                LookAtTarget();
            }

        }
        void CheckTimer()
        {
            if (_startTimer)
            {
                _timer += Time.deltaTime;
            }


            if (_timer > _nextTargetWindow)
            {
                uiEventChannel.RaiseBarsOff(0.1f);
                if(CC.CurrentCharacterState != CharacterState.Talking) CC.TransitionToState(CharacterState.Default);
                ResetTarget();
            }
        }
        [SerializeField] float _angle;
        void SearchForInteractables()
        {
            nearbyTargets = Physics.OverlapSphere(transform.position, noticeZone, targetLayers);
            float closestAngle = maxNoticeAngle;
            float closestDis = maxDistance;
            Transform closestTarget = null;

            if(nearbyTargets.Length <= 0){
                return;
            }

            //Check to see what targets are available
            foreach(var target in nearbyTargets){
                Transform t = target.transform;
                
                IInteractable interactable = t.gameObject.GetComponent<IInteractable>();
                //Does canbetargeted matter if this pick up item, can be interacted but not targeted
                //Maybe have a 2nd list for items inradius that cant be targeted

                //Is this interactable able to be targted?
                if(interactable == null || !interactable.IsVisible){
                    AvailableTargets.Remove(t);
                    //print("Removed by initial");
                    continue;
                }
                if(interactable == null || !interactable.IsVisible || !interactable.CanBeTargeted || interactable.BeenTargeted){
                    AvailableTargets.Remove(t);
                    //print("Removed by initial");
                    continue;
                }
                // Is Target Blocked out of sight?
                if(Blocked(t.position + _npcRayPoint)){
                    AvailableTargets.Remove(t);
                    //print("Removed by blocked");
                    continue;
                }
                // Is Target too far away?
                if(GetDistance(transform, t) > interactable.MaxNoticeRange){
                    AvailableTargets.Remove(t);
                    //print("Removed by distance");
                    continue;
                }
                //Need Check Does player sight/view angle matter ex: for picking up items in field
                // player view or interactable view doesnt seem to matter with enemies
                if(interactable.DoesAngleMatter){
                    // Is the angle of interactable within our players max view?
                    if(GetAngle(t.position, transform.position, transform.forward) > maxNoticeAngle){
                        AvailableTargets.Remove(t);
                        //print("Removed by player view");
                        continue;
                    }   
                    //Is the angle of our player within our interactables max view?
                    if(GetAngle(transform.position, t.position, t.forward) > interactable.MaxNoticeAngle){
                        AvailableTargets.Remove(t);
                        //print("Removed by npc view");
                        continue;
                    }
                }
                //If list does not contain interactable, then add
                if(!AvailableTargets.Contains(t)){
                    AvailableTargets.Add(t);
                    //print("perfect");
                }
                    
            }


            if (findByAngle) //Dont Need, Depricated
            {
                if (nearbyTargets.Length <= 0) return;
                for (int i = 0; i < nearbyTargets.Length; i++)
                {

                    Vector3 dir = nearbyTargets[i].transform.position - cam.position;
                    dir.y = 0;
                    _angle = Vector3.Angle(cam.forward, dir);

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
                    float ang = GetAngle(t.position, transform.position, transform.forward);
                    IInteractable interactable = t.gameObject.GetComponent<IInteractable>();
                    if (interactable.CanBeTargeted && interactable.IsVisible){
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
            //This was old way to place UI icon at center of target
            float h1 = closestTarget.GetComponent<CapsuleCollider>().height;
            float h2 = closestTarget.localScale.y;
            float h = h1 * h2;
            float half_h = (h / 2) / 2;
            currentYOffset = h - half_h;
            if (zeroVert_Look && currentYOffset > 1.6f && currentYOffset < 1.6f * 3) currentYOffset = 1.6f;
            Vector3 tarPos = closestTarget.position + new Vector3(0, currentYOffset, 0);
            if(Blocked(closestTarget.position + _npcRayPoint)){
                closestTarget.gameObject.GetComponent<IInteractable>().SetHovered(false);
                if(_closestTarget){
                    _closestTarget = null;
                    ResetTarget();
                }
                return;
            } 
            if(GetDistance(transform, closestTarget) > closestTarget.GetComponent<IInteractable>().MaxNoticeRange){
                //print(GetDistance(transform, closestTarget));
                _closestTarget = null;
                return; // not working right with findByAngle
            }
            if(GetAngle(closestTarget.position,transform.position,transform.forward) > maxNoticeAngle){
                //print(GetAngle(closestTarget.position,transform.position,transform.forward, 0));
                _closestTarget = null;
                return;
            }
            

            //_closestTarget = closestTarget;
            //_closestTarget.gameObject.GetComponent<IInteractable>().SetHovered(true);

        }

        void FindClosestTarget(){
            float closestDis = maxDistance;
            Transform closestTarget = null;

            foreach(Transform target in AvailableTargets){
                float dis = GetDistance(transform, target);
                if (dis < closestDis){
                    closestTarget = target;
                    closestDis = dis; 
                }

            }
            if(!closestTarget){
                _closestTarget = null;
                return;
            }
            _closestTarget = closestTarget;
            _closestTarget.gameObject.GetComponent<IInteractable>().SetHovered(true);
        }
        bool StillClosestTarget(Transform t){
            IInteractable interactable = t.GetComponent<IInteractable>();
            if (!interactable.IsVisible)
            {
                return false;
            }
            if (Blocked(t.position + _npcRayPoint)){ //
                interactable.SetHovered(false);
                 return false;
            }
            if(GetAngle(t.position,transform.position,transform.forward) > maxNoticeAngle){
                //HideHover(t);
                 return false;
            }
            if(GetDistance(transform, t) > interactable.MaxNoticeRange){
                //HideHover(t);
                return false;
            } 

            return true;
        }

        bool Blocked(Vector3 t)
        {
            RaycastHit hit;
            if (Physics.Linecast(transform.position + _playerRayPoint, t, out hit))
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
        float GetAngle(Vector3 loc1, Vector3 loc2, Vector3 forwardDir, bool lockYAxis = false){
            Vector3 dir = loc1 - loc2;
            if(lockYAxis) dir.y = 0;
            float angle = Vector3.Angle(forwardDir, dir);
            return angle;
        }

        void FoundTarget()
        {
            print("Found");
            _audioEvent.RaiseEvent2(_LockOnSFX, currentTarget.position);
            _targetingEvent.RaiseEvent(currentTarget);
            uiEventChannel.RaiseBarsOn(0.1f);
            CC.TransitionToState(CharacterState.Targeting);
            currentTarget.gameObject.GetComponent<IInteractable>().SetTargeted(true);
            //HideHover(currentTarget);
            //currentTarget.gameObject.GetComponent<IInteractable>().SetTargeted(true)(); //Make Events that fire for UI Targeted


            //turning off to see if i can better camera
            //ChangeCamera(true);

            _enemyLocked = true;
            _closestTarget.GetComponent<IInteractable>().SetBeenTargeted(true); //Set NPC as been targeted.

            for (int i = 0; i < nearbyTargets.Length; i++)
            {
                Transform t = nearbyTargets[i].transform;
                remainingTargets.Add(t);
                print(GetDistance(transform,t) + t.gameObject.name);
                if (t.gameObject.GetComponent<IInteractable>().BeenTargeted == true)
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
            currentTarget.gameObject.GetComponent<IInteractable>().SetTargeted(false);
            if (remainingTargets.Count <= 0)
            {
                ResetTarget();
                yield break;
            }
            else
            {
                float closestDis = 10f; // change to max range
                Transform closetT = null;
                foreach (var target in nearbyTargets)
                {
                    print(GetAngle(transform.position, target.transform.position, transform.forward));
                    if (target.gameObject.GetComponent<IInteractable>().BeenTargeted == false)
                    {
                        float disP = GetDistance(transform, target.transform);
                        if (disP <= target.transform.GetComponent<IInteractable>().MaxNoticeRange)
                        {
                            float disN = GetDistance(currentTarget, target.transform);
                            if (disN < closestDis)
                            {
                                closetT = target.transform;
                                closestDis = disN;
                            }
                        }

                    }

                }
                if (!closetT) {
                    ResetTarget();
                    yield break;
                }

                _audioEvent.RaiseEvent2(_LockOnSFX, currentTarget.position);
                currentTarget = closetT;
                currentTarget.gameObject.GetComponent<IInteractable>().SetTargeted(true);
                CC._target = currentTarget;
                //currentTarget.gameObject.GetComponent<IInteractable>().SetTargeted(true)();
                closetT.gameObject.GetComponent<IInteractable>().SetBeenTargeted(true);
                _changeTargetEvent.RaiseEvent(currentTarget);
            }

            yield return null;

        }
        void ResetTarget()
        {
            print("reset");
            if(!_isTalking) _audioEvent.RaiseEvent2(_LockOffSFX, currentTarget.position);
            //uiEventChannel.RaiseBarsOff(0.1f);
            //currentTarget.gameObject.GetComponent<NPC_Interact>().HideTargeted();
            currentTarget.gameObject.GetComponent<IInteractable>().SetTargeted(false);
            _unTargetingEvent.RaiseEvent();
            //if(CC.CurrentCharacterState != CharacterState.Talking) CC.TransitionToState(CharacterState.Default);
            _startTimer = false;
            _timer = 0;

            //turning off to see if i can better camera
            //ChangeCamera();
            currentTarget = null;
            _closestTarget = null;
            _enemyLocked = false;
            _NPCIndex = 0;
            BeenTargetedReset();
        }

        void BeenTargetedReset()
        {
            foreach (var npc in nearbyTargets)
            {
                IInteractable m = npc.GetComponent<IInteractable>(); // set npc back to not being targeted
                m.SetBeenTargeted(false);
                //m.UnTargeted();
                //HideHover(m.gameObject.transform);
            }
            remainingTargets.Clear();
        }
        // i dont think this is need any longer
        private void LookAtTarget()
        {
            if (currentTarget == null)
            {
                ResetTarget();
                return;
            }
        }
        private void IconControl(){

            if(_closestTarget != null){
                foreach(var npc in nearbyTargets){
                    if(npc.gameObject.transform != _closestTarget){
                        //HideHover(npc.gameObject.transform);
                        npc.gameObject.GetComponent<IInteractable>().SetHovered(false);
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
                    npc.gameObject.GetComponent<IInteractable>().SetHovered(false);
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
                _isTalking = true;
                CC._target = t;
                CC.TransitionToState(CharacterState.Talking);
                if(currentTarget) ResetTarget();

                //turning off to see if i can better camera
                //ChangeCamera();
                
                _interactable.OnInteract(this);
                _isTalking = false;
                return;
            } 

        }
        void ClearIInteractable(){
            if (_interactable != null) _interactable = null;
            uiEventChannel.HideTextInteract();
        }
        void ChangeCamera(bool targetingCam = false){
            if(!targetingCam){
                _freeLookCameraOff = false;
                cam.gameObject.GetComponent<Cinemachine.CinemachineBrain>().enabled = true;
                cam.gameObject.GetComponent<KinematicCharacterController.Examples.ExampleCharacterCamera>().isTargeting = false;

            }
            else{
                _freeLookCameraOff = true;
                cam.gameObject.GetComponent<Cinemachine.CinemachineBrain>().enabled = false;
                cam.gameObject.GetComponent<KinematicCharacterController.Examples.ExampleCharacterCamera>().isTargeting = true;

            }
        }
        void RecenterCamera(float waitTime, float recenteringTime, float disableRecenter){
            RecenterCamX.ThreeFloats(waitTime, recenteringTime, disableRecenter);
            RecenterCamY.ThreeFloats(waitTime, recenteringTime, disableRecenter);
        }
        void CheckForRecenterInput(InputAction.CallbackContext ctx){
            if(canRaycast) return;
            //Need a Recenter CoolDown
            if(currentCharacterState == CharacterState.Default)
            {
                CC.TransitionToState(CharacterState.Targeting);
            }
            RecenterCamera(0, 0.1f, 1);
            _audioEvent.RaiseEvent2(_recenterCameraSFX, transform.position);
            uiEventChannel.RaiseBarsOn(0.1f);
        }
        void CheckForInputRelease(InputAction.CallbackContext ctx){
            if(_enemyLocked) return;
            if (currentCharacterState == CharacterState.Targeting)
            {
                CC.TransitionToState(CharacterState.Default);
            }
            uiEventChannel.RaiseBarsOff(0.1f);
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            
            //Gizmos.DrawWireSphere(_interactionPoint.position, _interactionPointRadius);
            
            Gizmos.DrawWireSphere(transform.position, noticeZone);
            
        }

    }
}