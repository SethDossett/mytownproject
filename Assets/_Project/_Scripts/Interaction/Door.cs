using System.Threading.Tasks;
using System.Collections;
using UnityEngine;
using MyTownProject.Core;
using MyTownProject.Events;
using MyTownProject.SO;
using MyTownProject.UI;
using KinematicCharacterController;

namespace MyTownProject.Interaction
{
    public enum DoorType
    {
        PushDoorR, PullDoorR, PushDoorL, PullDoorL
    }
    public class Door : MonoBehaviour, IInteractable
    {
        [field: SerializeField] public bool IsVisible { get; set; }
        [field: SerializeField] public bool CanBeInteractedWith { get; set; }
        [field: SerializeField] public bool CanBeTargeted { get; private set; }
        [field: SerializeField] public bool Hovered { get; set; }
        [field: SerializeField] public bool Targeted { get; set; }
        [field: SerializeField] public bool BeenTargeted { get; set; }
        [field: SerializeField] public bool DoesAngleMatter { get; private set; }
        [field: SerializeField] public bool ExtraRayCheck { get; private set; }
        [field: SerializeField] public float MaxNoticeAngle { get; private set; }
        [field: SerializeField] public float MaxNoticeRange { get; private set; }
        [field: SerializeField] public float MaxInteractRange { get; private set; }
        [field: SerializeField] public PromptName PromptName { get; private set; }
        [field: SerializeField] public Vector3 InteractionPointOffset { get; private set; }



        [Header("Door Specific")]
        [SerializeField] bool _locked = false;
        [SerializeField] Vector3 _centerStandingPoint;

        [Header("Event References")]
        [SerializeField] MainEventChannelSO mainEventChannel;
        [SerializeField] ActionSO openDoorEvent;
        [SerializeField] UIEventChannelSO uIEventChannel;
        [SerializeField] SceneSO nextScene;
        [SerializeField] StateChangerEventSO stateChangerEvent;
        [SerializeField] ActionSO SetPlayerPosRot;
        [SerializeField] GeneralEventSO DisableControls;

        [Header("References")]

        

        [Header("Values")]
        bool _hasInteracted = false;
        bool _isFocusing = false;
        private bool _needToTeleport;
        GameObject _player;

        [Header("Animations")]
        int crackdoorR = Animator.StringToHash("Door_Open_Crack_01");
        int crackdoorL = Animator.StringToHash("Door_Open_Crack_02");
        int opendoorR = Animator.StringToHash("Door_WideOpen_01");
        int opendoorL = Animator.StringToHash("Door_WideOpen_02");
        int closeDoor = Animator.StringToHash("closeDoor");
        int openWide = Animator.StringToHash("OpenWide");
        int closeWide = Animator.StringToHash("CloseWide");

        public void OnFocus(string interactionName)
        {
            if (_isFocusing) return;

            _isFocusing = true;
        }

        public void OnInteract(TargetingSystem player)
        {
            if (_hasInteracted) return;
            _player = player.gameObject;
            OpenDoor();
            _hasInteracted = true;
        }

        public void OnLoseFocus()
        {
            _isFocusing = false;
            _hasInteracted = false;
        }
        public void SetHovered(bool setTrue)
        {
            if (setTrue) Hovered = true;
            else Hovered = false;
        }
        public void SetTargeted(bool setTrue)
        {
            if (setTrue) Targeted = true;
            else Targeted = false;

        }
        public void SetBeenTargeted(bool setTrue)
        {
            if (setTrue) BeenTargeted = true;
            else BeenTargeted = false;
        }
        public void OpenDoor()
        {
            if (!_locked)
            {
                Debug.Log("Open");
                StartCoroutine(Teleport());
            }
            else
                DoLockedDoor();
        }

        IEnumerator Teleport()
        {
            //DisableControls.RaiseEvent();
            //stateChangerEvent.RaiseEventGame(GameStateManager.GameState.CUTSCENE);
            StartCoroutine(SimulateMovement());
            //openDoorEvent.OpenDoor(CurrentDoorType);
            
            //SetPlayerPosRot.OnSetPosRot(transform.position + _centerStandingPoint, 2f, lookRot, false);
            //KinematicCharacterSystem.Settings.AutoSimulation = false; // This might should be running whenever cutscene is state.
            uIEventChannel.RaiseBarsOn(2f);
            
            //_player.transform.position = _player.transform.position + Vector3.forward * 2f;
            yield return new WaitForSecondsRealtime(1f);
            //_animatorRight.Play(crackdoorR);
            //_animatorLeft.Play(crackdoorL);
            uIEventChannel.RaiseFadeOut(Color.black, 1f);
            yield return new WaitForSecondsRealtime(1f);

            _hasInteracted = true;
            mainEventChannel.RaiseEventChangeScene(nextScene);
            //KinematicCharacterSystem.Settings.AutoSimulation = true;

        }
        float _moveValue;
        Vector3 _currentVelocity;
        IEnumerator SimulateMovement()
        {
            _moveValue = 0;
            Vector3 newPos = transform.position + _centerStandingPoint;
            Quaternion lookRot = Quaternion.LookRotation(-transform.forward, Vector3.up);
            KinematicCharacterMotor motor = _player.GetComponent<KinematicCharacterController.Examples.TheCharacterController>().Motor;

            while (_moveValue < 1)
            {

                _moveValue = Mathf.MoveTowards(_moveValue, 1, 2f * Time.unscaledDeltaTime);
                Vector3 dampPos = Vector3.SmoothDamp(_player.transform.position, newPos, ref _currentVelocity, 5f * Time.unscaledDeltaTime, 10f);
                Quaternion lerpRot = Quaternion.Slerp(_player.transform.rotation, lookRot, _moveValue);
                //Vector3 lerpPosition = Vector3.Lerp(_player.transform.position, newPos, _moveValue);
                motor.SetPosition(dampPos);
                motor.SetRotation(lerpRot);
                //motor.LerpPosition(transform.position + _centerStandingPoint, 0.5f);
                yield return null;
            }

            _moveValue = 0;
            print("DoneLerping");
            yield break;
        }
        private void DoLockedDoor()
        {
            _hasInteracted = true;
            Debug.Log("locked");

        }
        

    }

}
