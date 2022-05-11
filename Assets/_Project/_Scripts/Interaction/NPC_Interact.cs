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
        [field: SerializeField] public string Prompt { get; private set; }

        [SerializeField] private TextAsset inkJSON;
        bool _hasInteracted = false;
        bool _isFocusing = false;


        [Header("References")]
        //[SerializeField] private CinemachineTargetGroup _targetGroup;
        [SerializeField] private NPC_ScriptableObject npc;
        [SerializeField] DialogueEventsSO dialogueEvents;
        [SerializeField] GeneralEventSO findPlayerRef;
        Transform _playerReference;

        private void OnEnable()
        {
        }
        private void OnDisable()
        {

        }
        public void OnFocus(string interactionName)
        {
            if (_isFocusing) return;
            Debug.Log($"Focusing on {gameObject.name}");
            _isFocusing = true;
        }

        public void OnInteract(PlayerRacasting player)
        {
            //if (_hasInteracted) return;

            Debug.Log($"Interacting with {gameObject.name}");
            //_targetGroup.m_Targets[1].target = transform;
            Speak();

            _hasInteracted = true;
            return;
        }
        public void OnLoseFocus()
        {
            Debug.Log($"Lost Focus on {gameObject.name}");
            _isFocusing = false;
            _hasInteracted = false;
        }

        private void Speak()
        {
            dialogueEvents.Enter(gameObject, inkJSON);
        }
    }
}