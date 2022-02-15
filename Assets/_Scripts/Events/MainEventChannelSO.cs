using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "ScriptableObject/Event/maineventSO")]
public class MainEventChannelSO : ScriptableObject
{
    public UnityAction OnGamePaused;
    public UnityAction OnGameUnPaused;
    public UnityAction<GameObject, TextAsset> OnTalk;

    public void RaiseEventPaused()
    {
        OnGamePaused?.Invoke();
    }
    public void RaiseEventUnPaused()
    {
        OnGameUnPaused?.Invoke();
    }
    public void RaiseEventTalk(GameObject npc, TextAsset inkJSON)
    {
        OnTalk?.Invoke(npc,inkJSON);
    }
}
