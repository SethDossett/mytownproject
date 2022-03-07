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
        [field: SerializeField] public float MaxRange { get; private set; }
        [field: SerializeField] public bool CanBeInteractedWith { get; private set; }

        [Header("ScriptableObjects")]
        [SerializeField] MainEventChannelSO mainEventChannel;
        [SerializeField] InteractionEventSO _interactionDoor;
        [SerializeField] UIEventChannelSO uIEventChannel;
        [SerializeField] SceneSO sceneSO;
        [SerializeField] StateChangerEventSO stateChangerEvent;

        [Header("References")]
        string _npcTag = "NPC";

        [Header("Values")]
        [SerializeField] private bool _isLocked = false;
        [SerializeField] private bool _canInteract = true;
        private bool _needToTeleport;
        
        [SerializeField] int _nextSceneIndex;


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
        public void OnInteract()
        {
            OpenDoor();
            _canInteract = false;
            return;
        }

        public void OnFocus(string interactionName)
        {
            if (!_canInteract)
            {
                uIEventChannel.HideTextInteract();
                return;
            }
            interactionName = "Open";
            uIEventChannel.ShowTextInteract(interactionName);
        }

        public void OnLoseFocus()
        {
            uIEventChannel.HideTextInteract();
        }
    
        public void OpenDoor()
        {
            if (!_isLocked)
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

            uIEventChannel.RaiseBarsOn();
            yield return new WaitForSecondsRealtime(1f);
            _animatorRight.Play(crackdoorR);
            _animatorLeft.Play(crackdoorL);
            uIEventChannel.RaiseFadeOut(Color.black, 1f);
            yield return new WaitForSecondsRealtime(1f);
            
            _canInteract = true;
            mainEventChannel.RaiseEventChangeScene(_nextSceneIndex, sceneSO);
        
        }
        private void DoLockedDoor()
        {
            _canInteract = true;
            Debug.Log("locked");
        
        }
        private async void OnTriggerEnter(Collider other) // trigger door open event
        {
            CanBeInteractedWith = false;
            if (other.gameObject.CompareTag(_npcTag)){
                _animatorRight.Play(opendoorR);
                _animatorLeft.Play(opendoorL);
            }
            await Task.Delay(1500);

            CanBeInteractedWith = true;
            if (other.gameObject.CompareTag(_npcTag))
            {
                _animatorRight.SetTrigger(closeDoor);
                _animatorLeft.SetTrigger(closeDoor);
            }

        }
        private void OnTriggerExit(Collider other) // trigger door close event
        {
            
        }

    }

}
