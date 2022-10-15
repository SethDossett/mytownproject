using System.Threading.Tasks;
using System.Collections;
using UnityEngine;
using TMPro;
using MyTownProject.UI;
using MyTownProject.Core;
using MyTownProject.Events;
using MyTownProject.SO;

namespace MyTownProject.Interaction
{
    public class Door : MonoBehaviour, IInteractable
    {
        [field: SerializeField] public bool IsVisible { get; set; }
        [field: SerializeField] public float MaxNoticeRange { get; private set; }
        [field: SerializeField] public float MaxNoticeAngle { get; private set; }
        [field: SerializeField] public bool DoesAngleMatter { get; private set; }
        [field: SerializeField] public float MaxInteractRange { get; private set; }
        [field: SerializeField] public bool CanBeInteractedWith { get; private set; }
        [field: SerializeField] public bool CanBeTargeted { get; private set; }
        [field: SerializeField] public string Prompt { get; private set; }
        [field: SerializeField] public bool Hovered { get; set; }
        [field: SerializeField] public bool Targeted { get; set; }
        [field: SerializeField] public bool BeenTargeted { get; set; }


        public enum DoorType
        {
            RightDoorIn, LeftDoorIn, RightDoorOut, LeftDoorOut
        }

        [Header("Door Specific")]
        [SerializeField] DoorType CurrentDoorType;
        [SerializeField] bool _locked = false;

        [Header("ScriptableObjects")]
        [SerializeField] MainEventChannelSO mainEventChannel;
        [SerializeField] InteractionEventSO _interactionDoor;
        [SerializeField] UIEventChannelSO uIEventChannel;
        [SerializeField] SceneSO nextScene;
        [SerializeField] StateChangerEventSO stateChangerEvent;

        [Header("References")]
        string _npcTag = "NPC";

        [Header("Values")]
        bool _hasInteracted = false;
        bool _isFocusing = false;
        private bool _needToTeleport;

        [Header("Animations")]
        [SerializeField] private Animator _animatorRight;
        [SerializeField] private Animator _animatorLeft;
        int crackdoorR = Animator.StringToHash("Door_Open_Crack_01");
        int crackdoorL = Animator.StringToHash("Door_Open_Crack_02");
        int opendoorR = Animator.StringToHash("Door_WideOpen_01");
        int opendoorL = Animator.StringToHash("Door_WideOpen_02");
        int closeDoor = Animator.StringToHash("closeDoor");



        
        private void OnEnable()
        {
        
        }
        private void OnDisable()
        {
        
        }

        public void OnFocus(string interactionName)
        {
            if (_isFocusing) return;
            
            _isFocusing = true;
        }

        public void OnInteract(TargetingSystem player)
        {
            if (_hasInteracted) return;
            OpenDoor();
            _hasInteracted = true;
            return;
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
            stateChangerEvent.RaiseEventGame(GameStateManager.GameState.CUTSCENE);

            uIEventChannel.RaiseBarsOn(2f);
            yield return new WaitForSecondsRealtime(1f);
            _animatorRight.Play(crackdoorR);
            _animatorLeft.Play(crackdoorL);
            uIEventChannel.RaiseFadeOut(Color.black, 1f);
            yield return new WaitForSecondsRealtime(1f);
            
            _hasInteracted = true;
            mainEventChannel.RaiseEventChangeScene(nextScene);
        
        }
        private void DoLockedDoor()
        {
            _hasInteracted = true;
            Debug.Log("locked");
        
        }
        private async void OnTriggerEnter(Collider other) // trigger door open event
        {
            
            if (other.gameObject.CompareTag(_npcTag)){
                CanBeInteractedWith = false;
                _animatorRight.Play(opendoorR);
                _animatorLeft.Play(opendoorL);
            }
            await Task.Delay(1500);

            
            if (other.gameObject.CompareTag(_npcTag)){
                CanBeInteractedWith = true;
                _animatorRight.SetTrigger(closeDoor);
                _animatorLeft.SetTrigger(closeDoor);
            }

        }
        private void OnTriggerExit(Collider other) // trigger door close event
        {
            
        }

    }

}
