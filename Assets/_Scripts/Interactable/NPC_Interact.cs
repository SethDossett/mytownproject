using UnityEngine;
using Cinemachine;

public class NPC_Interact : MonoBehaviour, IInteractable
{
     [field: SerializeField] public float MaxRange { get; private set; }

    //[SerializeField] private const float _maxRange = 2.5f;

    [Header("References")]
    [SerializeField] private CinemachineTargetGroup _targetGroup;
    [SerializeField] private NPC_ScriptableObject npc_scriptableObject;
    public MainEventChannelSO MainEventChannelSO;
    private UI_EventMaster ui_eventMaster;

    private void OnEnable()
    {
        GameObject eventMaster = GameObject.Find("EventMaster");
        ui_eventMaster = eventMaster.GetComponent<UI_EventMaster>();
    }
    private void OnDisable()
    {

    }

    public void OnFocus(string interactionName)
    {
        interactionName = "Talk";
        ui_eventMaster.InteractionTextON(interactionName);
        Debug.Log($"Focusing on {gameObject.name}");
    }

    public void OnInteract()
    {
        Debug.Log($"Interacting with {gameObject.name}");
        _targetGroup.m_Targets[1].target = this.transform;
        GameManager.instance.UpdateState(GameManager.GameState.CUTSCENE);
        ui_eventMaster.InteractionTextOFF();
        Speak();
        return;
    }
    public void OnLoseFocus()
    {
        ui_eventMaster.InteractionTextOFF();
        Debug.Log($"Lost Focus on {gameObject.name}");
    }

    private void Speak()
    {
        MainEventChannelSO.RaiseEventTalk();
        NPC_StateHandler.instance.UpdateNPCState(NPC_StateHandler.NPCSTATE.TALKING);
    }
}
