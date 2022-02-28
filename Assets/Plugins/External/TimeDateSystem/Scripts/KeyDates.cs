using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DPUtils.Systems.DateTime;

[CreateAssetMenu(menuName = "Time Date System/Key Dates")]
public class KeyDates : ScriptableObject
{
    public DateTime KeyDate;
    public bool Yearly;
    public Sprite thumbnail;
    public string Description;
}
