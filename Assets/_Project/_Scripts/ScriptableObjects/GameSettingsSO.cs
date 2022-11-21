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

    }
}
