using UnityEngine;
using System;

public class UI_EventMaster : MonoBehaviour
{
    public delegate void UI_Events();
    public static event Action<string> interactionTextOn;
    public event UI_Events interactionTextOff;
    public event UI_Events hideAll;
    public event UI_Events unhideAll;


    public void InteractionTextON(string interactionName)
    {
        interactionTextOn?.Invoke(interactionName);
    }
    public void InteractionTextOFF()
    {
        interactionTextOff?.Invoke();
    }
}
