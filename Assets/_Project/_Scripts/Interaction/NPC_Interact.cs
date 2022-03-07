using UnityEngine;
using Cinemachine;
using MyTownProject.Events;
using MyTownProject.NPC;
using MyTownProject.UI;
using MyTownProject.Core;

namespace MyTownProject.Interaction
{
    public class NPC_Interact : MonoBehaviour, IInteractable
    {
        [field: SerializeField] public float MaxRange { get; private set; }

        [field: SerializeField] public bool CanBeInteractedWith { get; private set; }

        [SerializeField] private TextAsset inkJSON;


        [Header("References")]
        [SerializeField] private CinemachineTargetGroup _targetGroup;
        [SerializeField] private NPC_ScriptableObject npc;
        [SerializeField] DialogueEventsSO dialogueEvents;
        [SerializeField] UIEventChannelSO uIEventChannel;
        [SerializeField] StateChangerEventSO stateChanger;

        private void OnEnable()
        {
        }
        private void OnDisable()
        {

        }

        public void OnFocus(string interactionName)
        {
            interactionName = "Talk";
            uIEventChannel.ShowTextInteract(interactionName);
            Debug.Log($"Focusing on {gameObject.name}");
        }

        public void OnInteract()
        {
            Debug.Log($"Interacting with {gameObject.name}");
            _targetGroup.m_Targets[1].target = transform;
            stateChanger.RaiseEventGame(GameStateManager.GameState.CUTSCENE);
            uIEventChannel.HideTextInteract();
            Speak();
            return;
        }
        public void OnLoseFocus()
        {
            uIEventChannel.HideTextInteract();
            Debug.Log($"Lost Focus on {gameObject.name}");
        }

        private void Speak()
        {
            dialogueEvents.Enter(gameObject, inkJSON);
        }
    }
}