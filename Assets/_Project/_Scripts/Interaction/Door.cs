using KinematicCharacterController.Examples;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using MyTownProject.UI;
using MyTownProject.Core;

namespace MyTownProject.Interaction
{
    public class Door : MonoBehaviour, IInteractable
    {
        [field: SerializeField] public float MaxRange { get; private set; }
        [field: SerializeField] public bool CanBeInteractedWith { get; private set; }
    

        public UnityAction<TheCharacterController> OnCharacterTeleport;
        [SerializeField] UnityEvent _interactionEvent;

        [Header("References")]
        [SerializeField] private GameObject _player;
        [SerializeField] private Transform _destination;
        TheCharacterController cc;
        private TextMeshProUGUI interactionText;
        string _npcTag = "NPC";

        [Header("Values")]
        [SerializeField] private bool _isLocked = false;
        [SerializeField] private bool _canInteract = true;
        private bool _needToTeleport;
        public bool isBeingTeleportedTo { get; set; }


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
            _interactionEvent?.Invoke();
            _canInteract = false;
            return;
        }

        public void OnFocus(string interactionName)
        {
            if (!_canInteract)
            {
                interactionText.text = "";
                return;
            }
            interactionName = "Open";
            interactionText.text = interactionName;
        
        }

        public void OnLoseFocus()
        {
            interactionText.text = "";
        }
    
        private void Start()
        {
            cc = _player.GetComponent<TheCharacterController>();
            interactionText = GameObject.Find("InteractionText").GetComponent<TextMeshProUGUI>();
        }
    
        public void OpenDoor()
        {
            Debug.Log("Open");
        }
        public void OpenDoorAndTeleportPlayer()
        {
            if (!_isLocked)
                StartCoroutine(Teleport());
            else
                DoLockedDoor();
        }
        IEnumerator Teleport()
        {
            GameStateManager.instance.UpdateState(GameStateManager.GameState.CUTSCENE);

            CinematicBars.instance.BarsOn();
            yield return new WaitForSecondsRealtime(1f);
            _animatorRight.Play("Door_Open_Crack_01");
            _animatorLeft.Play("Door_Open_Crack_02");
            TransitionHandler.instance.FadeOut();
            yield return new WaitForSecondsRealtime(1f);

            if (!isBeingTeleportedTo)
            {
                if (cc)
                {
                    cc.Motor.SetPositionAndRotation(_destination.transform.position, _destination.transform.localRotation);

                    if (OnCharacterTeleport != null)
                    {
                        OnCharacterTeleport(cc);
                    }
                    this.isBeingTeleportedTo = true;
                }
            }

            isBeingTeleportedTo = false;
            _canInteract = true;
        
            yield return new WaitForSecondsRealtime(1f);
            TransitionHandler.instance.FadeIn();
            yield return new WaitForSecondsRealtime(0.25f);
            CinematicBars.instance.BarsOff();
            yield return new WaitForSecondsRealtime(0.5f);
            GameStateManager.instance.UpdateState(GameStateManager.GameState.GAME_PLAYING);
            yield break;
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
