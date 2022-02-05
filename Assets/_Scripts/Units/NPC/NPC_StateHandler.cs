using UnityEngine;
using System;

public class NPC_StateHandler : MonoBehaviour
{
    public static NPC_StateHandler instance;
    public NPCSTATE npcState;
    public static event Action<NPCSTATE> OnNPCStateChange;

    Animator _animator;
    int _isWalking = Animator.StringToHash("isWalking");


    private void Awake()
    {
        instance = this;
        _animator = GetComponent<Animator>();

    }
    #region State of NPC
    public void UpdateNPCState(NPCSTATE newState)
    {
        npcState = newState;

        switch (newState)
        {
            case NPCSTATE.STANDING:
                HandleStanding();
                break;
            case NPCSTATE.WALKING:
                HandleWalking();
                break;
            case NPCSTATE.TALKING:
                HandleTalking();
                break;
            case NPCSTATE.WORKING:
                HandleWorking();
                break;
            default:
                break;
        }

        OnNPCStateChange?.Invoke(newState);
    }

    private void HandleWorking()
    {
        _animator.SetBool(_isWalking, false);
    }

    private void HandleTalking()
    {
        _animator.SetBool(_isWalking, false);
        Debug.Log("Talking to Player");
    }

    private void HandleWalking()
    {
        _animator.SetBool(_isWalking, true);
        Debug.Log("Walking");
    }

    private void HandleStanding()
    {
        _animator.SetBool(_isWalking, false);
    }

    public enum NPCSTATE
    {
        STANDING, WALKING, TALKING, WORKING
    }
    #endregion
}
