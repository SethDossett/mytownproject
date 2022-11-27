using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyTownProject.Events;
using MyTownProject.Enviroment;
using MyTownProject.Core;
using MyTownProject.SO;
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
        #region  UI Elements
        [SerializeField]
        private GameObject _gameUI, _debugClock, _clock, _buttons,
         _dialogue, _explaination, _interactionPrompt, _rightTriggerPrompt;
        [SerializeField]
        private TextMeshProUGUI _interactionText, _interactionPromptText,
         _rightTriggerText, _explainationText;
        private CanvasGroup _promptCG, _dialogueCG, _explainationCG;
        #endregion

        [Header("Events Channels")]
        [SerializeField] UIEventChannelSO uIEventChannel;
        [SerializeField] MainEventChannelSO mainEventChannel;
        [SerializeField] DialogueEventsSO dialogueEvents;
        [SerializeField] ActionSO openDoorEvent;

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
            openDoorEvent.OnOpenDoor += OpenDoor;

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
            openDoorEvent.OnOpenDoor -= OpenDoor;
        }

        void Awake()
        {
            ChangePromptPriority(PromptName.Null, 1);
            SetInitialReferences();
        }
        void SetInitialReferences()
        {
            _promptCG = _interactionPrompt.GetComponent<CanvasGroup>();
            _dialogueCG = _dialogue.GetComponent<CanvasGroup>();
            _explainationCG = _explaination.GetComponent<CanvasGroup>();
        }
        private void CheckGameState(GameState state)
        {
            if (state == GameState.GAME_PLAYING)
            {
                // Might need to go under exit dialogue, so this is not called when not exiting.
                StartCoroutine(ExitDialogueUI());
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
            _clock.transform.DOLocalMoveY(455f, _cycleLength).SetEase(Ease.InSine).SetUpdate(true);
            _debugClock.transform.DOLocalMoveY(-470f, _cycleLength).SetEase(Ease.InSine).SetUpdate(true);
        }
        public void UI_UnhideAll()
        {
            _buttons.transform.DOLocalMoveY(0f, _cycleLength).SetEase(Ease.OutSine).SetUpdate(true);
            _clock.transform.DOLocalMoveY(180f, _cycleLength).SetEase(Ease.OutSine).SetUpdate(true);
            _debugClock.transform.DOLocalMoveY(-195f, _cycleLength).SetEase(Ease.OutSine).SetUpdate(true);
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

        #region Actions
        void OpenDoor(DoorType doorType, GameObject obj)
        {
            StartCoroutine(InteractedWithPrompt());
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
            yield return InteractedWithPrompt();

            _dialogue.SetActive(true);
            _dialogueCG.DOFade(1, 0.3f).SetUpdate(true);

            print("Done with Coroutine");
            yield break;
        }
        IEnumerator ExitDialogueUI()
        {
            // cashe canvas groups
            yield return _dialogueCG.DOFade(0, 0.3f).SetUpdate(true).WaitForCompletion();
            UI_UnhideAll();
            if (_currentPrompt.name != PromptName.Null)
            {
                EnablePrompt();
            }
            _dialogue.SetActive(false);

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
                    EnablePrompt();
                    _interactionPrompt.transform.DOScale(Vector3.one, 0.5f); //can make anim curve
                }
            }

            _interactionText.text = _currentPrompt.text;
            _interactionPromptText.text = _currentPrompt.text;
        }
        IEnumerator InteractedWithPrompt()
        {
            yield return _interactionPrompt.transform.DOPunchScale(Vector3.one * 1.5f, 0.3f).SetUpdate(true).WaitForCompletion();

            yield return _promptCG.DOFade(0, 0.5f).SetUpdate(true);

            _interactionPrompt.SetActive(false);

            yield break;
        }
        void EnablePrompt()
        {
            _interactionPrompt.SetActive(true);
            _promptCG.alpha = 1;
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
            _explainationCG.DOFade(0.9f, 1f).SetUpdate(true);

            yield return new WaitForSecondsRealtime(duration);
            _explainationCG.DOFade(0f, 1f).SetUpdate(true);
            _bubbleOnScreen = false;
            yield break;
        }
    }
}