using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "ScriptableObject/Event/maineventSO")]
public class MainEventChannelSO : ScriptableObject
{
    public UnityAction OnGamePaused;
    public UnityAction OnGameUnPaused;
    public UnityAction OnTalk;

    public void RaiseEventPaused()
    {
        OnGamePaused?.Invoke();
    }
    public void RaiseEventUnPaused()
    {
        OnGameUnPaused?.Invoke();
    }
    public void RaiseEventTalk()
    {
        OnTalk?.Invoke();
    }
}
