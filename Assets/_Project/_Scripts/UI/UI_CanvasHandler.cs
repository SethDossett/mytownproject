using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyTownProject.Events;
using MyTownProject.Core;
using System.Collections.Generic;

namespace MyTownProject.UI
{
    public class UI_CanvasHandler : MonoBehaviour
    {
        [Header("Events Channels")]
        [SerializeField] UIEventChannelSO uIEventChannel;
        [SerializeField] MainEventChannelSO mainEventChannel;
        [SerializeField] DialogueEventsSO dialogueEvents;

        [Header("UI Objects")]
        [SerializeField] private GameObject _gameUI;
        [SerializeField] private GameObject _clock;
        [SerializeField] private GameObject _buttons;
        [SerializeField] private GameObject _dialogue;
        [SerializeField] private TextMeshProUGUI _interactionText;

        [Header("Tween Values")]
        [SerializeField] private float _cycleLength = 2f;

        [Header("Prompt Properties")]
        [SerializeField] Prompt _currentPrompt;
        [SerializeField] List<Prompt> prompts = new List<Prompt>();
        int _highestPriority = 0;
        const string _empty = "";

        private void OnEnable()
        {
            GameStateManager.OnGameStateChanged += CheckGameState;
            dialogueEvents.onEnter += TalkingWithNPC;
            mainEventChannel.OnGameUnPaused += ExitDialogue;
            mainEventChannel.OnSubmit += ContinueIconSubmit;
            uIEventChannel.OnShowTextInteract += ShowInteractionText;
            uIEventChannel.OnHideTextInteract += HideInteractionText;
            uIEventChannel.OnChangePrompt += ChangePromptPriority;

        }
        private void OnDisable()
        {
            GameStateManager.OnGameStateChanged -= CheckGameState;
            dialogueEvents.onEnter -= TalkingWithNPC;
            mainEventChannel.OnGameUnPaused -= ExitDialogue;
            mainEventChannel.OnSubmit += ContinueIconSubmit;
            uIEventChannel.OnShowTextInteract -= ShowInteractionText;
            uIEventChannel.OnHideTextInteract -= HideInteractionText;
            uIEventChannel.OnChangePrompt -= ChangePromptPriority;
        }

        void Awake(){
            ChangePromptPriority(PromptName.Null, 1);
        }
        private void CheckGameState(GameState state)
        {
            if (state == GameState.GAME_PLAYING)
            {
                UI_UnhideAll();
                if (_dialogue.activeInHierarchy)
                    _dialogue.SetActive(false);
                return;
            }
            else
            {
                UI_HideAll();
                return;
            }


        }
        #region Control All UI Elements
        public void UI_HideAll()
        {
            _buttons.transform.DOLocalMoveY(275f, _cycleLength).SetEase(Ease.InSine).SetUpdate(true);
            _clock.transform.DOLocalMoveY(-470f, _cycleLength).SetEase(Ease.InSine).SetUpdate(true);
        }
        public void UI_UnhideAll()
        {
            _buttons.transform.DOLocalMoveY(0f, _cycleLength).SetEase(Ease.OutSine).SetUpdate(true);
            _clock.transform.DOLocalMoveY(-195f, _cycleLength).SetEase(Ease.OutSine).SetUpdate(true);
        }
        public void UI_FadeAll()
        {
            _buttons.GetComponentInChildren<Image>().DOFade(0f, _cycleLength).SetUpdate(true);

        }
        public void UI_UnFadeAll()
        {
            _buttons.GetComponentInChildren<Image>().DOFade(1f, _cycleLength).SetUpdate(true);

        }
        #endregion

        #region Control Individual UI Elements
        private void ShowInteractionText(string interactionName)
        {
            if (_interactionText.text != interactionName)
                _interactionText.text = interactionName;
        }
        private void HideInteractionText()
        {
            if(_interactionText.text != "")
                _interactionText.text = "";
        }
        #endregion

        #region Talking
        void TalkingWithNPC(GameObject npc, TextAsset inkJSON)
        {
            if (!_dialogue.activeInHierarchy)
                _dialogue.SetActive(true);

            uIEventChannel.RaiseBarsOff(0.1f);
        }
        void ContinueIconSubmit()
        {

        }
        void ExitDialogue() // migth not need
        {
            //if (_dialogue.activeInHierarchy)
            //_dialogue.SetActive(false);
        }


        #endregion
        
        void ChangePromptPriority(PromptName name, int priority){
            int currentHighestPriority = _highestPriority;
            Prompt topPrompt = null;
            //Go Through Every prompt we have in Enum List
            foreach(Prompt prompt in prompts){
                //Change Priority Value
                if(prompt.name == name){
                    //If trying to change priority but its already the same then return out of function;
                    if(priority == prompt.priority) return;
                    prompt.priority = priority;
                }
                //Find which prompt we should show
                if(prompt.priority > currentHighestPriority){
                    topPrompt = prompt;
                    currentHighestPriority = prompt.priority;
                }    
            }
            //Show Top Priority
            if(topPrompt != _currentPrompt){
                _currentPrompt = topPrompt;
                SetPrompt();
            }
        }

        void SetPrompt(){
            _interactionText.text = _currentPrompt.text;
        }
    }
}