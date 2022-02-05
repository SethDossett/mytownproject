using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("References")]
    public MainEventChannelSO mainEventChannelSO;
    private CinemachineFreeLook _freeLookCamera_01;
    private CinemachineVirtualCamera _virtualCamera_01;
    [SerializeField] private CinemachineTargetGroup _targetGroup_01;
    private Animator _animator;

    [Header("Animation")]
    int playerFreeLook01 = Animator.StringToHash("PlayerFreeLook01");
    int dialogueVCam01 = Animator.StringToHash("DialogueVirtalCamera01");

    private void OnEnable()
    {
        mainEventChannelSO.OnTalk += SwitchToDialogue;
    }
    private void OnDisable()
    {
        mainEventChannelSO.OnTalk -= SwitchToDialogue;
    }
    private void Start()
    {
        _animator = GetComponent<Animator>();
    }
    private void SwitchToDialogue()
    {
        _animator.Play(dialogueVCam01);
    }
}
