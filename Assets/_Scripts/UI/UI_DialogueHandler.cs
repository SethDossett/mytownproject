using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using DG.Tweening;
using UnityEngine.EventSystems;
using System;

public class UI_DialogueHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] MainEventChannelSO evnt;
    [SerializeField] DialogueEventsSO DialogueEvents;
    [SerializeField] TextMeshProUGUI _dialogueText;
    [SerializeField] TextMeshProUGUI _speakerText;
    [SerializeField] GameObject continueIcon;
    [SerializeField] GameObject[] _choices;
    TextMeshProUGUI[] _choicesText;
    DialogueVariables _dialogueVariables;
    [SerializeField] TextAsset loadGlobalsJSON;
    private Story currentStory;
    Animator _animator;

    [Header("Values")]
    [SerializeField] float typingSpeed = 0.05f;
    bool _displayingLine = false;   
    bool _skipToEndOfLine = false;

    [Header("Tags")]
    private const string SPEAKER_TAG = "speaker";
    private const string EMOTE_TAG = "emote";
    private const string ICON_TAG = "icon";

    [Header("Anim")]
    int idle = Animator.StringToHash("Idle");

    private void OnEnable()
    {
        evnt.OnTalk += EnterDialogueMode;
        evnt.OnSubmit += SubmitButton;
        _dialogueVariables = new DialogueVariables(loadGlobalsJSON);
    }
    private void OnDisable()
    {
        evnt.OnTalk -= EnterDialogueMode;
        evnt.OnSubmit -= SubmitButton;
    }
    private void Start()
    {
        SetUpReferences();
    }
    private void SetUpReferences()
    {
        _choicesText = new TextMeshProUGUI[_choices.Length];
        int index = 0;
        foreach (GameObject choice in _choices)
        {
            _choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }
    }
    void EnterDialogueMode(GameObject npc, TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);

        _dialogueVariables.StartListening(currentStory);
        DialogueEvents.Enter();

        _animator = npc.GetComponent<Animator>();

        ResetTagValues();

        ContinueStory();
    }

    private void ResetTagValues() { // reset to defaults so info doesnt carry over from last npc.
        _dialogueText.text = "???";
        _animator.Play(idle);
    }

    void SubmitButton()
    {
        if(_displayingLine == true)
        {
            _skipToEndOfLine = true;
            return;
        }
        else
        {
            if (currentStory.currentChoices.Count == 0)
                ContinueStory();
            else return;
        }

    }
    void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            StartCoroutine(DisplayLine(currentStory.Continue()));

            ShowChoices();

            HandleTags(currentStory.currentTags);
            
        }
        else
        {
            StartCoroutine(ExitDialogueMode());
        }
    }
    IEnumerator DisplayLine(string line)
    {
        _displayingLine = true;
        _dialogueText.text = line;
        _dialogueText.maxVisibleCharacters = 0;
        Submit();

        bool isAddingRichTextTag = false;

        foreach(char letter in line.ToCharArray())
        {
            if (_skipToEndOfLine)
            {
                _dialogueText.maxVisibleCharacters = line.Length;
                _skipToEndOfLine = false;

                break;
            }

            if(letter == '<' || isAddingRichTextTag)
            {
                isAddingRichTextTag = true;
                if (letter == '>')
                    isAddingRichTextTag = false;

            }
            else
            {
                _dialogueText.maxVisibleCharacters++;
                yield return new WaitForSecondsRealtime(typingSpeed);

            }

        }

        continueIcon.transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.InExpo).SetDelay(0.05f).SetUpdate(true);
        _displayingLine = false;
    }
    void HandleTags(List<string> currentTags)
    {
        foreach (string tag in currentTags)
        {
            string[] splitTags = tag.Split(':');
            if (splitTags.Length != 2) Debug.LogError($"Could not parse {tag} correctly!");

            string tagKey = splitTags[0].Trim();
            string tagValue = splitTags[1].Trim();

            switch (tagKey)
            {
                case SPEAKER_TAG:
                    _speakerText.text = tagValue;
                    break;
                case EMOTE_TAG:
                    _animator.Play(tagValue);
                    break;
                case ICON_TAG:
                    Debug.Log("icon=" + tagValue);
                    break;
                default:
                    break;
            }


        }
    }
    void ShowChoices()
    {
        List<Choice> currentchoices = currentStory.currentChoices;
        if(currentchoices.Count > _choices.Length) {
            Debug.LogError("More Choices given than UI can support. Number of Choices given: " + currentchoices.Count);
        }

        int index = 0;
        foreach(Choice choice in currentchoices)
        {
            _choices[index].SetActive(true);
            _choicesText[index].text = choice.text;
            index++;
        }

        for(int i = index; i < _choices.Length; i++)
        {
            _choices[i].gameObject.SetActive(false);
        }

        StartCoroutine(SelectFirstChoice());    

    }
    IEnumerator SelectFirstChoice() {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(_choices[0].gameObject);
    }

    public void MakeChoice(int choiceIndex)
    {
        currentStory.ChooseChoiceIndex(choiceIndex);
        StartCoroutine(TimeBetweenChoices());
        
    }
    IEnumerator TimeBetweenChoices()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        ContinueStory();
    }
    void Submit()
    {
        continueIcon.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBounce).SetUpdate(true);
        
    }

    public Ink.Runtime.Object GetVariableState(string variableName)
    {
        Ink.Runtime.Object variableValue = null;
        _dialogueVariables.variables.TryGetValue(variableName, out variableValue);
        if(variableValue == null) Debug.LogError("Ink Variable was found to be null" + variableName);
        return variableValue;
    }
    IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        _dialogueText.text = "";
        _dialogueVariables.StopListening(currentStory);
        DialogueEvents.Exit();
        evnt.RaiseEventUnPaused();
    }
}