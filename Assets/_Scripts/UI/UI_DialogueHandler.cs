using System.Collections;
using UnityEngine;
using TMPro;
using Ink.Runtime;

public class UI_DialogueHandler : MonoBehaviour
{
    [SerializeField] MainEventChannelSO evnt;
    [SerializeField] private TextMeshProUGUI _dialogueText;
    private Story currentStory;
    [SerializeField] float typingSpeed = 0.05f;


    private void OnEnable()
    {
        evnt.OnTalk += EnterDialogueMode;
        evnt.OnSubmit += ContinueStory;
    }
    private void OnDisable()
    {
        evnt.OnTalk -= EnterDialogueMode;
        evnt.OnSubmit -= ContinueStory;
    }

    void EnterDialogueMode(GameObject npc, TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);

        ContinueStory();

    }

    IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        _dialogueText.text = "";
        evnt.RaiseEventUnPaused();
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
        _dialogueText.text = "";

        foreach(char letter in line.ToCharArray())
        {
            _dialogueText.text += letter;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }
    }
}
