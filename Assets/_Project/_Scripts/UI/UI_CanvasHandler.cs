using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyTownProject.Events;
using MyTownProject.Core;

namespace MyTownProject.UI
{
    public class UI_CanvasHandler : MonoBehaviour
    {
        private UI_EventMaster ui_eventMaster;
        [SerializeField] MainEventChannelSO eventChannel;

        [SerializeField] private GameObject _gameUI;
        [SerializeField] private GameObject _clock;
        [SerializeField] private GameObject _buttons;
        [SerializeField] private GameObject _dialogue;
        [SerializeField] private TextMeshProUGUI _interactionText;


        [SerializeField] private float _cycleLength = 2f;

        private void OnEnable()
        {
            ui_eventMaster = GameObject.Find("EventMaster").GetComponent<UI_EventMaster>();
            GameManager.OnGameStateChanged += CheckGameState;
            eventChannel.OnTalk += TalkingWithNPC;
            eventChannel.OnGameUnPaused += ExitDialogue;
            eventChannel.OnSubmit += ContinueIconSubmit;
            UI_EventMaster.interactionTextOn += ShowInteractionText;
            ui_eventMaster.interactionTextOff += HideInteractionText;
        }


        private void OnDisable()
        {
            GameManager.OnGameStateChanged -= CheckGameState;
            eventChannel.OnTalk -= TalkingWithNPC;
            eventChannel.OnGameUnPaused -= ExitDialogue;
            eventChannel.OnSubmit += ContinueIconSubmit;
            UI_EventMaster.interactionTextOn -= ShowInteractionText;
            ui_eventMaster.interactionTextOff -= HideInteractionText;
        }

        private void CheckGameState(GameManager.GameState state)
        {
            if (state == GameManager.GameState.GAME_PLAYING)
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
            _interactionText.text = interactionName;
        }
        private void HideInteractionText()
        {
            _interactionText.text = "";
        }
        #endregion

        #region Talking
        void TalkingWithNPC(GameObject npc, TextAsset inkJSON)
        {
            if (!_dialogue.activeInHierarchy)
                _dialogue.SetActive(true);


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
    }
}