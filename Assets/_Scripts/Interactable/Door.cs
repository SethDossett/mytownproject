using KinematicCharacterController.Examples;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class Door : MonoBehaviour, IInteractable
{
    [field: SerializeField] public float MaxRange { get; private set; }


    TheCharacterController cc;
    public UnityAction<TheCharacterController> OnCharacterTeleport;
    [SerializeField] private Transform _destination;
    [SerializeField] private GameObject _player;
    public bool isBeingTeleportedTo { get; set; }
    private bool _needToTeleport;
    [SerializeField] private bool _isLocked = false;
    
    [Header("Animations")]
    [SerializeField] private Animator _animatorRight;
    [SerializeField] private Animator _animatorLeft;

    private TextMeshProUGUI interactionText;


    [SerializeField] UnityEvent _interactionEvent;
    [SerializeField] private bool _canInteract = true;

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
        GameManager.instance.UpdateState(GameManager.GameState.CUTSCENE);

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
        GameManager.instance.UpdateState(GameManager.GameState.GAME_PLAYING);
        yield break;
    }
    private void DoLockedDoor()
    {
        _canInteract = true;
        Debug.Log("locked");
        
    }


}
