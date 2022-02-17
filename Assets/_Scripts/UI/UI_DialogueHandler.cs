using System.Collections;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using DG.Tweening;

public class UI_DialogueHandler : MonoBehaviour
{
    [SerializeField] MainEventChannelSO evnt;
    [SerializeField] TextMeshProUGUI _dialogueText;
    [SerializeField] GameObject continueIcon;
    private Story currentStory;
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

    void EnterDialogueMode(GameObject npc, TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);

        ContinueStory();

    }


    void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            StartCoroutine(DisplayLine(currentStory.Continue()));
        }
        else
        {
            StartCoroutine(ExitDialogueMode());
        }
    }
    IEnumerator DisplayLine(string line)
    {
        _displayingLine = true;
        _dialogueText.text = "";
        Submit();

        //continueIcon.SetActive(false);

        foreach(char letter in line.ToCharArray())
        {
            if (_skipToEndOfLine)
            {
                _dialogueText.text = line;
                _skipToEndOfLine = false;

                break;
            }

            _dialogueText.text += letter;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        continueIcon.transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.InExpo).SetDelay(0.05f).SetUpdate(true);
        _displayingLine = false;
    }

    void Submit()
    {
        continueIcon.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBounce).SetUpdate(true);
        
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
    IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        _dialogueText.text = "";
        evnt.RaiseEventUnPaused();
    }
}
