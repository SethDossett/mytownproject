using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using KinematicCharacterController.Examples;

namespace MyTownProject.Enviroment{
    public class BasicLadder : MonoBehaviour
    {   
        [SerializeField] float _minClimbAngle;
        [SerializeField] float _maxDistance;
        [SerializeField] float _ladderHeight;
        [SerializeField] float _heightOnLadder;
        float _startHeight;
        float _dot;
        float _dis;
        int _numFound;
        [SerializeField] float _enterLadderRadius;
        [SerializeField] Vector3 _SphereCastOffset;
        [SerializeField] Vector3 _ladderPosOffset;
        Collider[] cols = new Collider[3];

        MeshCollider _ladderCollider;

        private NewControls inputActions;
        [SerializeField] LayerMask _playerLayer;
        float _timer = 0;
        float _timeBuffer = 0.1f;
        bool _checkForPlayer;
        bool _onLadder;

       GameObject _player;
       TheCharacterController CC;

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
        }
        private void Update() {
            if(!_checkForPlayer) return;

            if(!_onLadder)
                EnterLadder();
            else 
                ExitLadder();    
            
            
            //print(inputActions.GamePlay.Move.ReadValue<Vector2>());
        }
        void EnterLadder(){
            _numFound = Physics.OverlapSphereNonAlloc(transform.position + _SphereCastOffset, _enterLadderRadius, cols, _playerLayer); 

            if(_numFound > 0){
                _dis = Vector3.Distance(transform.position, cols[0].transform.position);
                if(_dis > _maxDistance) return;

                _dot = Vector3.Dot(transform.forward, cols[0].transform.forward);
                if(_dot > _minClimbAngle || _dot < -_minClimbAngle) return;
                
                float value = inputActions.GamePlay.Move.ReadValue<Vector2>().magnitude;
                if(value >= 0.8f){
                    _timer += Time.unscaledDeltaTime;

                    if(_timer >= _timeBuffer){
                        SwitchToClimbingState(cols[0].transform.gameObject);
                        _timer = 0;
                        return;
                    }
                }
                else _timer = 0;
            }
        }
        void ExitLadder(){
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


            
        }
        void SwitchToClimbingState(GameObject player){
            _startHeight = player.transform.position.y;
            _player = player;
           CC = player.GetComponent<TheCharacterController>();
           //_checkForPlayer = false;
           _onLadder = true;
           CC.TransitionToState(CharacterState.Climbing);
           _ladderCollider.enabled = false;
           //play ladder jump into anim
           CC._newCenteredPosition = transform.position + _ladderPosOffset;
           print("SwitchedToClimbing");
        }
        void SwitchToDefaltState(){
            CC.TransitionToState(CharacterState.Default);
            _onLadder = false;
            _ladderCollider.enabled = true;
            print("SwitchedToDefault");
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position + _SphereCastOffset, _enterLadderRadius);
        }







    }
}

