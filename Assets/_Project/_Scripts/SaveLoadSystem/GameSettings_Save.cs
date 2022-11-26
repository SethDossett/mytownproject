using MyTownProject.SO;
using MyTownProject.Core;
using MyTownProject.Events;
using UnityEngine;

namespace MyTownProject.SaveLoadSystem
{
    public class GameSettings_Save : State_Save
    {
        [SerializeField] GameSettingsSO settings;
        [SerializeField] GameSettingsSO DefaultSettings;
        [SerializeField] GeneralEventSO UpdateSettings;

        public struct GameSettings
        {
            public ControllerType controllerType;
            public SceneSO SceneToEnterIn;
            public bool StartOfGame;
            public float MasterVolume;
            public float MusicVolume;
            public float SFXVolume;

        }

        public GameSettings saveState = new GameSettings();

        public override string SaveState()
        {
            saveState.controllerType = settings.ControllerType;
            saveState.SceneToEnterIn = settings.SceneToEnterIn;
            saveState.StartOfGame = settings.StartOfGame;
            saveState.MasterVolume = settings.MasterVolume;
            saveState.MusicVolume = settings.MusicVolume;
            saveState.SFXVolume = settings.SFXVolume;



            return JsonUtility.ToJson(saveState);
        }

        public override void LoadState(string loadedJSON)
        {
            saveState = JsonUtility.FromJson<GameSettings>(loadedJSON);

            settings.ControllerType = saveState.controllerType;
            settings.SceneToEnterIn = saveState.SceneToEnterIn;
            settings.StartOfGame = saveState.StartOfGame;
            settings.MasterVolume = saveState.MasterVolume;
            settings.MusicVolume = saveState.MusicVolume;
            settings.SFXVolume = saveState.SFXVolume;
        }

        public override void NoSaveFile()
        {
            settings.ControllerType = DefaultSettings.ControllerType;
            settings.SceneToEnterIn = DefaultSettings.SceneToEnterIn;
            settings.MasterVolume = DefaultSettings.MasterVolume;
            settings.MusicVolume = DefaultSettings.MusicVolume;
            settings.SFXVolume = DefaultSettings.SFXVolume;

            UpdateSettings.RaiseEvent();
        }
    }
}