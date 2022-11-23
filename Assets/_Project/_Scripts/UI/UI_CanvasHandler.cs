using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyTownProject.Events;
using MyTownProject.Core;
using System.Collections.Generic;
using System.Collections;

namespace MyTownProject.UI
{
    public enum HudElement
    {
        LeftTrigger, RightTrigger, InteractPrompt
    }
    public class UI_CanvasHandler : MonoBehaviour
    {
        [Header("Events Channels")]
        [SerializeField] UIEventChannelSO uIEventChannel;
        [SerializeField] MainEventChannelSO mainEventChannel;
        [SerializeField] DialogueEventsSO dialogueEvents;

        [Header("UI Objects")]
        [SerializeField] private GameObject _gameUI;
        [SerializeField] private GameObject _clock;
        [SerializeField] private GameObject _fullClock;
        [SerializeField] private GameObject _buttons;
        [SerializeField] private GameObject _dialogue;
        [SerializeField] private GameObject _explaination;
        [SerializeField] private GameObject _interactionPrompt;
        [SerializeField] private GameObject _rightTriggerPrompt;
        [SerializeField] private TextMeshProUGUI _interactionText, _interactionPromptText, _rightTriggerText, _explainationText;

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
            uIEventChannel.OnShowButtonText += ShowHudElementText;
            uIEventChannel.OnHideButtonText += HideHudElementText;
            uIEventChannel.OnChangePrompt += ChangePromptPriority;
            uIEventChannel.OnShowExplaination += Bubble;

        }
        private void OnDisable()
        {
            GameStateManager.OnGameStateChanged -= CheckGameState;
            dialogueEvents.onEnter -= TalkingWithNPC;
            mainEventChannel.OnGameUnPaused -= ExitDialogue;
            mainEventChannel.OnSubmit += ContinueIconSubmit;
            uIEventChannel.OnShowButtonText -= ShowHudElementText;
            uIEventChannel.OnHideButtonText -= HideHudElementText;
            uIEventChannel.OnChangePrompt -= ChangePromptPriority;
            uIEventChannel.OnShowExplaination -= Bubble;
        }

        void Awake()
        {
            ChangePromptPriority(PromptName.Null, 1);
        }
        private void CheckGameState(GameState state)
        {
            if (state == GameState.GAME_PLAYING)
            {
                UI_UnhideAll();
                CheckCurrentPrompt();
                _dialogue.SetActive(false);
                _dialogue.GetComponent<CanvasGroup>().alpha = 0;
            }
            else
            {
                UI_HideAll();
            }


        }
        #region Control All UI Elements
        public void UI_HideAll()
        {
            _buttons.transform.DOLocalMoveY(275f, _cycleLength).SetEase(Ease.InSine).SetUpdate(true);
            _fullClock.transform.DOLocalMoveY(443f, _cycleLength).SetEase(Ease.InSine).SetUpdate(true);
            _clock.transform.DOLocalMoveY(-470f, _cycleLength).SetEase(Ease.InSine).SetUpdate(true);
        }
        public void UI_UnhideAll()
        {
            _buttons.transform.DOLocalMoveY(0f, _cycleLength).SetEase(Ease.OutSine).SetUpdate(true);
            _fullClock.transform.DOLocalMoveY(172f, _cycleLength).SetEase(Ease.OutSine).SetUpdate(true);
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


        #region Talking
        void TalkingWithNPC(GameObject npc, TextAsset inkJSON)
        {
            StartCoroutine(StartDialogueUI());

            uIEventChannel.RaiseBarsOff(0.1f);
        }
        IEnumerator StartDialogueUI()
        {
            yield return _interactionPrompt.transform.DOPunchScale(Vector3.one * 1.5f, 0.3f).SetUpdate(true).WaitForCompletion();

            yield return _interactionPrompt.GetComponent<CanvasGroup>().DOFade(0, 0.5f).SetUpdate(true);

            _interactionPrompt.SetActive(false);
            _dialogue.SetActive(true);
            _dialogue.GetComponent<CanvasGroup>().DOFade(1, 0.7f).SetUpdate(true);

            print("Done with Coroutine");
            yield break;
        }
        void ContinueIconSubmit()
        {

        }
        void ExitDialogue() // migth not need
        {
        }


        #endregion

        #region Control Individual UI Elements
        private void ShowHudElementText(HudElement hudElement, string text)
        {
            if (hudElement == HudElement.RightTrigger)
                _rightTriggerText.text = text;
        }
        private void HideHudElementText(HudElement hudElement)
        {
            if (hudElement == HudElement.RightTrigger)
                _rightTriggerText.text = _empty;
        }
        #endregion

        // Move to PromptHandler
        void ChangePromptPriority(PromptName name, int priority)
        {
            int currentHighestPriority = _highestPriority;
            Prompt topPrompt = null;
            //Go Through Every prompt we have in Enum List
            foreach (Prompt prompt in prompts)
            {
                //Change Priority Value
                if (prompt.name == name)
                {
                    //If trying to change priority but its already the same then return out of function;
                    if (priority == prompt.priority) return;
                    prompt.priority = priority;
                }
                //Find which prompt we should show
                if (prompt.priority > currentHighestPriority)
                {
                    topPrompt = prompt;
                    currentHighestPriority = prompt.priority;
                }
            }
            //Show Top Priority
            if (topPrompt != _currentPrompt)
            {
                _currentPrompt = topPrompt;
                SetPrompt(_currentPrompt.name);
            }
        }

        void SetPrompt(PromptName name)
        {
            if (name == PromptName.Null)
            {
                if (_interactionPrompt.activeInHierarchy)
                {
                    _interactionPrompt.SetActive(false);
                }
            }
            else
            {
                if (!_interactionPrompt.activeInHierarchy)
                {
                    _interactionPrompt.transform.localScale = Vector3.one * 1.5f;
                    _interactionPrompt.SetActive(true);
                    _interactionPrompt.transform.DOScale(Vector3.one, 0.5f); //can make anim curve
                }
            }

            _interactionText.text = _currentPrompt.text;
            _interactionPromptText.text = _currentPrompt.text;
        }
        void CheckCurrentPrompt()
        {
            print("check prompt " + _currentPrompt.name);
            if (_currentPrompt.name != PromptName.Null)
            {

                _interactionPrompt.SetActive(true);
                _interactionPrompt.GetComponent<CanvasGroup>().alpha = 1;
            }
        }

        bool _bubbleOnScreen;
        Tween _fadeTween;
        void Bubble(Vector2 screenPos, float duration, string message)
        {
            if (_bubbleOnScreen) return;
            StartCoroutine(ShowExplainationBubble(screenPos, duration, message));
        }
        IEnumerator ShowExplainationBubble(Vector2 screenPos, float duration, string message)
        {
            print("SHOW BUBBLE");
            if (_fadeTween != null) _fadeTween.Kill();

            _bubbleOnScreen = true;
            _explaination.transform.localPosition = screenPos;
            _explainationText.text = message;
            _explaination.GetComponent<CanvasGroup>().DOFade(0.9f, 1f).SetUpdate(true);

            yield return new WaitForSecondsRealtime(duration);
            _explaination.GetComponent<CanvasGroup>().DOFade(0f, 1f).SetUpdate(true);
            _bubbleOnScreen = false;
            yield break;
        }
    }
}