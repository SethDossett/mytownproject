using UnityEngine;
using Cinemachine;
using MyTownProject.Events;
using MyTownProject.NPC;
using MyTownProject.UI;
using MyTownProject.Core;
using TMPro;

namespace MyTownProject.Interaction
{
    public class NPC_Interact : MonoBehaviour, IInteractable
    {
        [field: SerializeField] public bool IsVisible { get; set; }
        [field: SerializeField] public float MaxNoticeRange { get; private set; }
        [field: SerializeField] public float MaxInteractRange { get; private set; }
        [field: SerializeField] public bool CanBeInteractedWith { get; private set; }
        [field: SerializeField] public bool CanBeTargeted { get; private set; }
        [field: SerializeField] public string Prompt { get; private set; }

        [SerializeField] private TextAsset inkJSON;
        bool _hasInteracted = false;
        bool _isFocusing = false;
        public bool _hovered; // if hovered false then dont hide hover
        public bool _targeted;


        [Header("References")]
        //[SerializeField] private CinemachineTargetGroup _targetGroup;
        [SerializeField] private NPC_ScriptableObject npc;
        [SerializeField] DialogueEventsSO dialogueEvents;

    #region new variables
        [SerializeField] GameObject _hoverIcon;
        [SerializeField] GameObject _targetIcon;
        [SerializeField] GameObject _interactionUI;
        [SerializeField] TextMeshProUGUI _NPCName;
        [SerializeField] GameObject _titlName;
        [SerializeField] string _name;
        [SerializeField] bool ShowName = false;
        int hshow = 0;
        int tshow = 1;
    #endregion

        public bool beenTargeted;
        private void OnEnable()
        {
            GameStateManager.OnGameStateChanged += ChangedGameState;
        }
        private void OnDisable()
        {
            GameStateManager.OnGameStateChanged -= ChangedGameState;
        }
        public void OnFocus(string interactionName)
        {
            if (_isFocusing) return;
            Debug.Log($"Focusing on {gameObject.name}");
            _isFocusing = true;
        }

        public void OnInteract(TargetingSystem player)
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

        void Start()
        {
            if (!_interactionUI.activeInHierarchy)
                _interactionUI.SetActive(true);

        }

        void ChangedGameState(GameStateManager.GameState state){
            if(state != GameStateManager.GameState.GAME_PLAYING){
                TurnOffAllIcons();
            }
            else{
                if (!_interactionUI.activeInHierarchy)
                _interactionUI.SetActive(true);
            }
        }
        void Update()
        {
            if(!_interactionUI.activeInHierarchy) return;

            if (_targeted)
            {
                _hovered = false;
                if (_hoverIcon.activeInHierarchy)
                    _hoverIcon.SetActive(false);
                if (_titlName.activeInHierarchy)
                    _titlName.SetActive(false);
                if (!_targetIcon.activeInHierarchy)
                    _targetIcon.SetActive(true);
                
            }
            else
            {
                if (_targetIcon.activeInHierarchy)
                    _targetIcon.SetActive(false);

                if (_hovered)
                {
                    if (!_hoverIcon.activeInHierarchy)
                    {
                        print("HoverTurnedOn " + gameObject.name);
                        _hoverIcon.SetActive(true);
                    }

                    if (ShowName)
                    {
                        if (!_titlName.activeInHierarchy)
                            _titlName.SetActive(true);
                        _NPCName.text = _name;
                    }
                }
                else
                {
                    
                    if (_hoverIcon.activeInHierarchy)
                    {
                        print("HoverTurnedOff " + gameObject.name);
                        _hoverIcon.SetActive(false);
                    }
                    if (_titlName.activeInHierarchy)
                        _titlName.SetActive(false);
                }
            }
        }

        public void SetTargeted(){
            if(!beenTargeted)
                beenTargeted = true;
            
        }

        public void UnsetTargeted(){
            if(beenTargeted)
                beenTargeted = false;
        }

        void TurnOffAllIcons(){
            if (_hoverIcon.activeInHierarchy)
                _hoverIcon.SetActive(false);
            if (_titlName.activeInHierarchy)
                _titlName.SetActive(false);
            if (_targetIcon.activeInHierarchy)
                _targetIcon.SetActive(false);
            if (_interactionUI.activeInHierarchy)
                _interactionUI.SetActive(false);
        }

        
    }
}