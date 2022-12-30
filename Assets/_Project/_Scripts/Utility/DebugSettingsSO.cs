using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyTownProject.Utility{


[CreateAssetMenu(menuName = "ScriptableObject/DebugSettings")]
public class DebugSettingsSO : ScriptableObject 
{
    [Header("Frame Rate")]
    public bool SetFrameRate;
    public int TargetFrameRate;

    [Header("FPS Counter")]
    public bool SetFPS_Counter;
    public int FPS_RefreshRate;


}
}
