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
        [field: SerializeField] public float MaxRange { get; private set; }
        [field: SerializeField] public bool CanBeInteractedWith { get; private set; }
        [field: SerializeField] public bool CanBeTargeted { get; private set; }
        [field: SerializeField] public string Prompt { get; private set; }

        [SerializeField] private TextAsset inkJSON;
        bool _hasInteracted = false;
        bool _isFocusing = false;


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

        public void Hovered(){

        if(!_interactionUI.activeInHierarchy){
            _interactionUI.SetActive(true);

        }
        else{
            ShowIcon(hshow);

            if(ShowName){
                if(!_titlName.activeInHierarchy)
                    _titlName.SetActive(true);

                   
            }
            else{
                if(_titlName.activeInHierarchy)
                _titlName.SetActive(false);  
            } 
        }

            _NPCName.text = _name; 
        }
        public void HideHover(){
            if(_interactionUI.activeInHierarchy){
                _interactionUI.SetActive(false);
            }
        }
        public void Targeted(){
            if(!_interactionUI.activeInHierarchy){
                _interactionUI.SetActive(true);
            }

            if(_titlName.activeInHierarchy)
                _titlName.SetActive(false);    
            ShowIcon(tshow);
        }
        public void UnTargeted(){
            if(!_interactionUI.activeInHierarchy)
                _interactionUI.SetActive(true);

            ShowIcon(hshow);
        }


        public void SetTargeted(){
            if(!beenTargeted)
                beenTargeted = true;
            
        }

        public void UnsetTargeted(){
            if(beenTargeted)
                beenTargeted = false;
        }



        private void ShowIcon(int i){
            if(i == hshow){
                if(_targetIcon.activeInHierarchy)
                    _targetIcon.SetActive(false);
                if(!_hoverIcon.activeInHierarchy)
                    _hoverIcon.SetActive(true);
            }
            else{
                if(_hoverIcon.activeInHierarchy)
                    _hoverIcon.SetActive(false);
                if(!_targetIcon.activeInHierarchy)
                    _targetIcon.SetActive(true);
            }
        }
    }
}