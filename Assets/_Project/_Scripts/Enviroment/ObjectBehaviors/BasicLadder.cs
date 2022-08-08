using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using KinematicCharacterController.Examples;

namespace MyTownProject.Enviroment{
    public class BasicLadder : MonoBehaviour
    {   
        [SerializeField] float _minClimbAngle;
        [SerializeField] float _ladderHeight;
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

            EnterLadder();
            
            
            
        }
        void EnterLadder(){
            _numFound = Physics.OverlapSphereNonAlloc(transform.position + _SphereCastOffset, _enterLadderRadius, cols, _playerLayer); 

            if(_numFound > 0){
                float dot = Vector3.Dot(transform.forward, cols[0].transform.forward);
                if(dot <= _minClimbAngle){
                    float value = inputActions.GamePlay.Move.ReadValue<Vector2>().magnitude;
                    print(value);
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
        }
        void ExitLadder(){
            
        }
        void SwitchToClimbingState(GameObject player){
            _player = player;
           TheCharacterController CC = player.GetComponent<TheCharacterController>();
           //_checkForPlayer = false;
           _onLadder = true;
           CC.TransitionToState(CharacterState.Climbing);
           _ladderCollider.enabled = false;
           //play ladder jump into anim
           CC._newCenteredPosition = transform.position + _ladderPosOffset;
           print("SwitchedToClimbing");
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position + _SphereCastOffset, _enterLadderRadius);
        }







    }
}

