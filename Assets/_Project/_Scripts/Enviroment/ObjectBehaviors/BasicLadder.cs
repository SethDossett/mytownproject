using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using MyTownProject.Core;
using MyTownProject.Events;
using KinematicCharacterController.Examples;

namespace MyTownProject.Enviroment
{
    public class BasicLadder : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] FloatEventSO RecenterCamX;
        [SerializeField] FloatEventSO RecenterCamY;
        [SerializeField] GeneralEventSO DisableCamRecter;
        [SerializeField] GeneralEventSO EnableControls;
        [SerializeField] GeneralEventSO DisableControls;
        private NewControls inputActions;
        InputAction _cameraInput;
        GameObject _player;
        TheCharacterController CC;

        int _anim_GetOnTop = Animator.StringToHash("GetOnLadderTop");
        int _anim_GetOffTop = Animator.StringToHash("GetOffTop");

        [Header("Values")]
        [SerializeField] float _ladderHeight;
        [SerializeField] float _heightOnLadder;
        [SerializeField] float _minClimbAngle = -0.95f;
        [SerializeField] float _maxDistance;
        [SerializeField] Vector3 _ladderPosOffset;
        Vector3 _ladderForward;
        float _startHeight;
        BoxCollider _ladderCollider;

        [Header("TopOfLadder")]
        [SerializeField] float _TopLadderRadius;
        [SerializeField] Vector3 _SphereCastOffsetTop;
        [SerializeField] Vector3 _StartPosTop = new Vector3(0.35f, 2.5f, 0f);

        [Header("BottomOfLadder")]
        [SerializeField] float _BottomLadderRadius;
        [SerializeField] Vector3 _StartPosBottom;
        [SerializeField] Vector3 _SphereCastOffsetBottom;

        [Header("Overlap Sphere")]
        [SerializeField] LayerMask _playerLayer;
        Collider[] cols = new Collider[3];
        float _timer = 0;
        float _timeBuffer = 0.1f;
        float _dot;
        float inputDot;
        float _dis;
        int _numFoundBottom;
        int _numFoundTop;

        [Header("Checks")]
        [SerializeField] bool _onLadder;
        bool _checkForPlayer;
        bool _exitLadderCalled;

        [SerializeField] float inputMag;

        private void Awake()
        {
            inputActions = InputManager.inputActions;
            _ladderCollider = GetComponent<BoxCollider>();
            _cameraInput = inputActions.GamePlay.Camera;
            _cameraInput.performed += CamInput;
        }
        private void OnDestroy()
        {
            _cameraInput.performed -= CamInput;
        }
        private void Start()
        {
            _checkForPlayer = true;
            _ladderPosOffset = new Vector3(0, 0.3f, 0);
            _ladderPosOffset = _ladderPosOffset + (transform.right * 0.35f);
            _StartPosBottom = _ladderPosOffset;
            _StartPosTop = _StartPosBottom + new Vector3(0, _ladderHeight - 1.4f, 0);

            _SphereCastOffsetBottom = _StartPosBottom;
            print(_SphereCastOffsetBottom);
            _SphereCastOffsetTop = new Vector3(0, _ladderHeight, 0);
            _SphereCastOffsetTop = _SphereCastOffsetTop + (-transform.right * 0.5f);
            print(_SphereCastOffsetTop);

        }
        void CamInput(InputAction.CallbackContext ctx)
        {
            if (_onLadder)
                DisableCamRecter.RaiseEvent();
        }
        private void Update()
        {
            //inputMag = inputActions.GamePlay.Move.ReadValue<Vector2>().magnitude;
            if (!_checkForPlayer) return;

            if (!_onLadder)
                NotOnLadder();
            else
                OnLadder();


            //print(inputActions.GamePlay.Move.ReadValue<Vector2>());
        }
        void NotOnLadder()
        {
            if (_exitLadderCalled) return;
            _numFoundBottom = Physics.OverlapSphereNonAlloc(transform.position + _SphereCastOffsetBottom, _BottomLadderRadius, cols, _playerLayer);
            _numFoundTop = Physics.OverlapSphereNonAlloc(transform.position + _SphereCastOffsetTop, _TopLadderRadius, cols, _playerLayer);

            if (_numFoundBottom > 0)
            {
                if (_player == null) _player = cols[0].transform.gameObject;
                if (CC == null) CC = _player.GetComponent<TheCharacterController>();
                _ladderForward = transform.right;
                _dis = Vector3.Distance(transform.position, cols[0].transform.position);
                if (_dis > _maxDistance) return;

                _dot = Vector3.Dot(_ladderForward, cols[0].transform.forward);
                if (_dot > _minClimbAngle) return;

                Vector3 value = CC._moveInputVector;
                inputDot = Vector3.Dot(value, _ladderForward);

                if (inputDot <= _minClimbAngle)
                {
                    _timer += Time.unscaledDeltaTime;

                    if (_timer >= _timeBuffer)
                    {
                        SwitchToClimbingState(true);
                        _timer = 0;
                        return;
                    }
                }
                else _timer = 0;
            }
            if (_numFoundTop > 0)
            {
                if (_player == null) _player = cols[0].transform.gameObject;
                if (CC == null) CC = _player.GetComponent<TheCharacterController>();
                _ladderForward = transform.right;
                _dis = Vector3.Distance(transform.position + _SphereCastOffsetTop, cols[0].transform.position);
                if (_dis > _maxDistance) return;

                _dot = Vector3.Dot(_ladderForward, cols[0].transform.forward);
                if (_dot < -_minClimbAngle) return;


                Vector3 value = CC._moveInputVector;
                inputDot = Vector3.Dot(value, _ladderForward);
                print(-_minClimbAngle);
                if (inputDot >= -_minClimbAngle)
                {
                    _exitLadderCalled = true;
                    StartCoroutine(GetOnLadderTop());
                }
            }
        }
        void OnLadder()
        {
            if (CC.CurrentRootName == P_StateNames.Default) SwitchToDefaltState();
            if (!_onLadder) return;
            if (_exitLadderCalled) return;
            _heightOnLadder = _player.transform.position.y - _startHeight;

            if (_heightOnLadder <= 0.1f)
            {
                Vector2 value = inputActions.GamePlay.Move.ReadValue<Vector2>().normalized;
                if (value.y < -0.95f)
                {
                    _timer += Time.unscaledDeltaTime;

                    if (_timer >= _timeBuffer)
                    {
                        print("offLadder");
                        SwitchToDefaltState();
                        _timer = 0;
                        return;
                    }
                }
                else _timer = 0;
            }

            if (_heightOnLadder >= _ladderHeight - 1)
            {
                Vector2 value = inputActions.GamePlay.Move.ReadValue<Vector2>().normalized;
                if (value.y > 0.95f)
                {
                    _exitLadderCalled = true;
                    StartCoroutine(GetOffLadderTop());
                }
            }
        }
        void SwitchToClimbingState(bool onBottom)
        {
            //When player is getting on the Bottom of the Ladder
            if (onBottom)
            {
                _startHeight = transform.position.y;
                _onLadder = true;
                CC.CurrentState.SwitchStates(CC.CurrentFactory.GetBaseState(P_StateNames.ClimbLadder));
                _ladderCollider.enabled = false;
                CC.NewCenteredPosition = transform.position + _StartPosBottom;
                CC.NewLadderRotation = Quaternion.LookRotation(-_ladderForward, Vector3.up);
                RecenterCamX.ThreeFloats(0, 0.35f, 1);
                RecenterCamY.ThreeFloats(0, 0.35f, 1);
            }
            else
            {//When player is getting on the Top of the Ladder
                _ladderForward = transform.right;
                _startHeight = transform.position.y;
                CC.CurrentState.SwitchStates(CC.CurrentFactory.GetBaseState(P_StateNames.ClimbLadder));
                CC.NewCenteredPosition = transform.position + _StartPosTop;
                CC.NewLadderRotation = Quaternion.LookRotation(-_ladderForward, Vector3.up);
                _ladderCollider.enabled = false;
            }

        }
        void SwitchToDefaltState()
        {
            if (CC.CurrentRootName != P_StateNames.Default) 
                CC.CurrentState.SwitchStates(CC.CurrentFactory.GetBaseState(P_StateNames.Default));
            _onLadder = false;
            _heightOnLadder = 0;
            _ladderCollider.enabled = true;
            _exitLadderCalled = false;
        }

        IEnumerator GetOnLadderTop()
        {
            inputActions.Disable();
            DisableControls.RaiseEvent();
            float dis = Vector3.Distance(CC.Motor.TransientPosition, transform.position + Vector3.up * _ladderHeight);
            if (dis != 0f)
            {
                CC.Motor.SetPosition(transform.position + Vector3.up * _ladderHeight);//could cash
            }
            yield return new WaitForEndOfFrame();
            SwitchToClimbingState(false);
            _player.GetComponent<Animator>().CrossFadeInFixedTime(_anim_GetOnTop, 0.2f, 0);
            _onLadder = true;
            yield return new WaitForSecondsRealtime(0.2f);
            RecenterCamX.ThreeFloats(0, 0.35f, 1);
            RecenterCamY.ThreeFloats(0, 0.35f, 1);
            //yield return new WaitForSecondsRealtime(0.35f);
            _exitLadderCalled = false;
            inputActions.Enable();
            EnableControls.RaiseEvent();
            yield break;
        }
        IEnumerator GetOffLadderTop()
        {
            CC._gettingOnOffObstacle = true;
            _player.GetComponent<Animator>().CrossFade(_anim_GetOffTop, 0, 0);

            yield return new WaitForSecondsRealtime(0.75f);
            SwitchToDefaltState();
            CC.Motor.SetPosition(transform.position + _SphereCastOffsetTop);// could be setTransientPosition?
            CC._gettingOnOffObstacle = false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position + _SphereCastOffsetBottom, _BottomLadderRadius);
            Gizmos.DrawWireSphere(transform.position + _SphereCastOffsetTop, _TopLadderRadius);
            Gizmos.DrawRay(transform.position, _ladderForward * 5f);
            Gizmos.DrawRay(transform.position, Vector3.up * _ladderHeight);

        }
    }
}

