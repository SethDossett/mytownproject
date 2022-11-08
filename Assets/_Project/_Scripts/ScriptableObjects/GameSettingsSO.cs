﻿using UnityEngine;
using MyTownProject.Core;

namespace MyTownProject.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/GameSettings")]
    public class GameSettingsSO : ScriptableObject
    {
        public ControllerType ControllerType;
        public SceneSO CurrentScene;
        public bool StartOfGame;

    }
}
