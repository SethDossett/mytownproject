using UnityEngine;
using KinematicCharacterController.Examples;
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
        [field: SerializeField] public bool CanBeInteractedWith { get; private set; }
        [field: SerializeField] public bool CanBeTargeted { get; private set; }
        [field: SerializeField] public bool Hovered { get; set; }
        [field: SerializeField] public bool Targeted { get; set; }
        [field: SerializeField] public bool BeenTargeted { get; set; }
        [field: SerializeField] public bool DoesAngleMatter { get; private set; }
        [field: SerializeField] public bool ExtraRayCheck { get; private set; }
        [field: SerializeField] public float MaxNoticeAngle { get; private set; }
        [field: SerializeField] public float MaxNoticeRange { get; private set; }
        [field: SerializeField] public float MaxInteractRange { get; private set; }
        [field: SerializeField] public PromptName PromptName { get; private set; }
        [field: SerializeField] public Vector3 InteractionPointOffset { get; private set; }




        [Header("References")]
        [SerializeField] private TextAsset inkJSON;
        //[SerializeField] private CinemachineTargetGroup _targetGroup;
        [SerializeField] private NPC_ScriptableObject npc;
        [SerializeField] DialogueEventsSO dialogueEvents;


        #region new variables
        [Header("Script Specific")]
        bool _hasInteracted = false;
        bool _isFocusing = false;
        [SerializeField] GameObject _hoverIcon;
        [SerializeField] GameObject _targetIcon;
        [SerializeField] GameObject _interactionUI;
        [SerializeField] TextMeshProUGUI _NPCName;
        [SerializeField] GameObject _titleName;
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
            player.TransitionCharacterState(CharacterState.Talking);

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
            InteractionPointOffset = new Vector3(0, 1.2f, 0);
            if (!_interactionUI.activeInHierarchy)
                _interactionUI.SetActive(true);

        }

        void ChangedGameState(GameStateManager.GameState state)
        {
            if (state != GameStateManager.GameState.GAME_PLAYING)
            {
                TurnOffAllIcons();
            }
            else
            {
                _interactionUI.SetActive(true);
            }
        }
        void Update()
        {
            if (!_interactionUI.activeInHierarchy) return;

            HandleIcons();
        }

        void HandleIcons()
        {
            if (Targeted)
            {//If We have targeted NPC we want to hide HoverIcon and TitleIcon
                Hovered = false;
                if (_hoverIcon.activeInHierarchy)
                    _hoverIcon.SetActive(false);
                if (_titleName.activeInHierarchy)
                    _titleName.SetActive(false);
                if (!_targetIcon.activeInHierarchy)
                    _targetIcon.SetActive(true);

            }
            else
            {//If We are not Targeting NPC, We Want to hide of TargetIcon
                if (_targetIcon.activeInHierarchy)
                    _targetIcon.SetActive(false);
                // Check and see if We should activate HoverIcon
                if (Hovered) //Show Hover and Title
                {
                    if (!_hoverIcon.activeInHierarchy)
                    {
                        print("HoverTurnedOn " + gameObject.name);
                        _hoverIcon.SetActive(true);
                    }
                    // If we want to show name, we turn bool true in Inspector
                    if (ShowName)
                    {
                        if (!_titleName.activeInHierarchy)
                            _titleName.SetActive(true);
                        _NPCName.text = _name;
                    }
                }
                else //Hide Hover and Title
                {

                    if (_hoverIcon.activeInHierarchy)
                    {
                        print("HoverTurnedOff " + gameObject.name);
                        _hoverIcon.SetActive(false);
                    }
                    if (_titleName.activeInHierarchy)
                        _titleName.SetActive(false);
                }
            }

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
        public void UnsetTargeted()
        {
            beenTargeted = false;
        }

        void TurnOffAllIcons()
        {
            if (_hoverIcon.activeInHierarchy)
                _hoverIcon.SetActive(false);
            if (_titleName.activeInHierarchy)
                _titleName.SetActive(false);
            if (_targetIcon.activeInHierarchy)
                _targetIcon.SetActive(false);
            if (_interactionUI.activeInHierarchy)
                _interactionUI.SetActive(false);
        }


    }
}