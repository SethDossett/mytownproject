using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_DialogueHandler : MonoBehaviour
{
    [SerializeField] MainEventChannelSO evnt;
    [SerializeField] private TextMeshProUGUI _dialogueText;

    private void OnEnable()
    {
        evnt.OnTalk += EnterDialogueMode;
    }
    private void OnDisable()
    {
        evnt.OnTalk -= EnterDialogueMode;
    }

    void EnterDialogueMode(GameObject npc, TextAsset inkJSON)
    {
        //currentStory = Story(inkJSON.text);
          
    }
}
