﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using DG.Tweening;
using UnityEngine.EventSystems;

public class UI_DialogueHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] MainEventChannelSO evnt;
    [SerializeField] TextMeshProUGUI _dialogueText;
    [SerializeField] GameObject continueIcon;
    [SerializeField] GameObject[] _choices;
    TextMeshProUGUI[] _choicesText;
    private Story currentStory;
    private Choice choiceSelected;

    [Header("Values")]
    [SerializeField] float typingSpeed = 0.05f;
    bool _displayingLine = false;   
    bool _skipToEndOfLine = false;


    private void OnEnable()
    {
        evnt.OnTalk += EnterDialogueMode;
        evnt.OnSubmit += SubmitButton;
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

        ContinueStory();

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
            ContinueStory();
        }

    }
    void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            StartCoroutine(DisplayLine(currentStory.Continue()));

            if(currentStory.currentChoices.Count != 0)
            {
                ShowChoices();
            }
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

    void ShowChoices()
    {
        //is choice a yes or no or multiple?

        List<Choice> currentchoices = currentStory.currentChoices;
        if(currentchoices.Count > _choices.Length)
        {
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
            _choices[i].SetActive(false);
        }

        StartCoroutine(SelectFirstChoice());    

    }
    IEnumerator SelectFirstChoice() {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(_choices[0]);
    }

    public void MakeChoice(int choiceIndex)
    {
        currentStory.ChooseChoiceIndex(choiceIndex);
    }
    void Submit()
    {
        continueIcon.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBounce).SetUpdate(true);
        
    }
    IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        _dialogueText.text = "";
        evnt.RaiseEventUnPaused();
    }
}
