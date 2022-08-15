using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MyTownProject.Events;
using KinematicCharacterController.Examples;

namespace MyTownProject.Enviroment{
    public class BasicLadder : MonoBehaviour
    {   
        [SerializeField] FloatEventSO RecenterCamX;
        [SerializeField] FloatEventSO RecenterCamY;
        [SerializeField] GeneralEventSO DisableCamRecter;
        [SerializeField] float _minClimbAngle = -0.95f;
        [SerializeField] float _maxDistance;
        [SerializeField] float _ladderHeight;
        [SerializeField] float _heightOnLadder;
        Vector3 _ladderForward;
        float _startHeight;
        float _dot;
        float inputDot;
        float _dis;
        int _numFoundBottom;
        int _numFoundTop;
        [SerializeField] float _BottomLadderRadius;
        [SerializeField] float _TopLadderRadius;
        [SerializeField] Vector3 _SphereCastOffsetBottom;
        [SerializeField] Vector3 _SphereCastOffsetTop;
        [SerializeField] Vector3 _ladderPosOffset;
        Collider[] cols = new Collider[3];

        MeshCollider _ladderCollider;

        private NewControls inputActions;
        [SerializeField] LayerMask _playerLayer;
        float _timer = 0;
        float _timeBuffer = 0.1f;
        bool _checkForPlayer;
        [SerializeField] bool _onLadder;

       GameObject _player;
       TheCharacterController CC;
       bool _exitLadderCalled;

        private void OnEnable() {
            inputActions.GamePlay.Enable();
        }
        private void OnDisable() {
            inputActions.GamePlay.Enable();
        }
        private void Awake() {
            inputActions = new NewControls();
            _ladderCollider = GetComponent<MeshCollider>();
            _checkForPlayer = true;
            _ladderForward = transform.right;
        }
        private void Update() {
            if(!_checkForPlayer) return;

            if(!_onLadder)
                EnterLadder();
            else 
                ExitLadder();
                if(inputActions.GamePlay.Camera.ReadValue<Vector2>() != Vector2.zero){
                    DisableCamRecter.RaiseEvent();
                }    
            
            
            //print(inputActions.GamePlay.Move.ReadValue<Vector2>());
        }
        void EnterLadder(){
            _numFoundBottom = Physics.OverlapSphereNonAlloc(transform.position + _SphereCastOffsetBottom, _BottomLadderRadius, cols, _playerLayer); 
            _numFoundTop = Physics.OverlapSphereNonAlloc(transform.position + _SphereCastOffsetTop, _TopLadderRadius, cols, _playerLayer);

            if(_numFoundBottom > 0){
                if(_player == null) _player = cols[0].transform.gameObject;
                if(CC == null) CC = _player.GetComponent<TheCharacterController>();
                _dis = Vector3.Distance(transform.position, cols[0].transform.position);
                if(_dis > _maxDistance) return;

                _dot = Vector3.Dot(_ladderForward, cols[0].transform.forward);
                if(_dot > _minClimbAngle) return;
                
                Vector3 value = CC._moveInputVector;
                inputDot = Vector3.Dot(value, _ladderForward);

                if(inputDot <= _minClimbAngle){
                    _timer += Time.unscaledDeltaTime;

                    if(_timer >= _timeBuffer){
                        SwitchToClimbingState();
                        _timer = 0;
                        return;
                    }
                }
                else _timer = 0;
            }
            if(_numFoundTop > 0){
                if(_player == null) _player = cols[0].transform.gameObject;
                if(CC == null) CC = _player.GetComponent<TheCharacterController>();
                _dis = Vector3.Distance(transform.position + _SphereCastOffsetTop, cols[0].transform.position);
                if(_dis > _maxDistance) return;

                _dot = Vector3.Dot(_ladderForward, cols[0].transform.forward);
                if(_dot < -_minClimbAngle) return;
                
                
                Vector3 value = CC._moveInputVector;
                inputDot = Vector3.Dot(value, _ladderForward);
                print(-_minClimbAngle);
                if(inputDot >= -_minClimbAngle){
                    StartCoroutine(GetOnLadderTop());
                }
            }
        }
        void ExitLadder(){
            if(!_onLadder) return;
            if(_exitLadderCalled) return;
            _heightOnLadder = _player.transform.position.y - _startHeight;

            if(_heightOnLadder <= 0.1f){
                Vector2 value = inputActions.GamePlay.Move.ReadValue<Vector2>();
                if(value == Vector2.down){
                    _timer += Time.unscaledDeltaTime;

                    if(_timer >= _timeBuffer){
                        SwitchToDefaltState();
                        _timer = 0;
                        return;
                    }
                }
                else _timer = 0;
            }

            if(_heightOnLadder >= _ladderHeight-1){
                Vector2 value = inputActions.GamePlay.Move.ReadValue<Vector2>();
                if(value == Vector2.up){
                    
                    StartCoroutine(GetOffLadderTop());
                    _exitLadderCalled = true;
                }
            }


            
        }
        void SwitchToClimbingState(){
            RecenterCamX.OnRaiseEvent2(0,0.35f);
            RecenterCamY.OnRaiseEvent2(0,0.35f);
            _startHeight = _player.transform.position.y;
           //_checkForPlayer = false;
           _onLadder = true;
           CC.TransitionToState(CharacterState.Climbing);
           _ladderCollider.enabled = false;
           //play ladder jump into anim
           Vector3 startLadderPos = transform.position + _ladderPosOffset;
           CC._newCenteredPosition = startLadderPos;
           CC._newLadderRotation = Quaternion.LookRotation(-_ladderForward, Vector3.up);
           print("SwitchedToClimbing");
        }
        void SwitchToDefaltState(){
            CC.TransitionToState(CharacterState.Default);
            _onLadder = false;
            _heightOnLadder = 0;
            _ladderCollider.enabled = true;
            _exitLadderCalled = false;
            print("SwitchedToDefault");
        }

        IEnumerator GetOnLadderTop(){
            print("geton ladder top");
            yield return new WaitForSecondsRealtime(1.15f);
        }
        IEnumerator GetOffLadderTop(){
            CC._gettingOnOffLadder = true;
            _player.GetComponent<Animator>().CrossFade("GetOffTop", 0, 2);

            yield return new WaitForSecondsRealtime(1.15f);
            SwitchToDefaltState();
            CC.Motor.SetPosition(transform.position + _SphereCastOffsetTop);
            CC._gettingOnOffLadder = false;
            print("GetOFFTopOfLadder");
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position + _SphereCastOffsetBottom, _BottomLadderRadius);
            Gizmos.DrawWireSphere(transform.position + _SphereCastOffsetTop, _TopLadderRadius);
            //Gizmos.DrawRay(transform.position,_ladderForward * 5f);
            //Gizmos.DrawRay(transform.position,Vector3.up * _ladderHeight);

        }
    }
}

