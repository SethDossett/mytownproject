using MyTownProject.SO;
using MyTownProject.Core;
using UnityEngine;

namespace MyTownProject.SaveLoadSystem
{
    public class GameSettings_Save : State_Save
    {
        [SerializeField] GameSettingsSO settings;

        public struct GameSettings
        {
            public ControllerType controllerType;
        }

        public GameSettings saveState = new GameSettings();

        public override string SaveState()
        {
            saveState.controllerType = settings.controllerType;


            return JsonUtility.ToJson(saveState);
        }

        public override void LoadState(string loadedJSON)
        {
            saveState = JsonUtility.FromJson<GameSettings>(loadedJSON);

            settings.controllerType = saveState.controllerType;
        }

        public override bool ShouldSave()
        {
            if(saveState.controllerType == settings.controllerType)
                return false;


            return true;
        }
    }
}