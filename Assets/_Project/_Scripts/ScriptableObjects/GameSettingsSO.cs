using UnityEngine;
using MyTownProject.Core;

namespace MyTownProject.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/GameSettings")]
    public class GameSettingsSO : ScriptableObject
    {
        public ControllerType ControllerType;
        public SceneSO SceneToEnterIn;
        public bool StartOfGame;
        public bool UseDebugSpawnPosition;
        [Range(0f, 1f)] public float MasterVolume;
        [Range(0f, 1f)] public float MusicVolume;
        [Range(0f, 1f)] public float SFXVolume;

    }
}
