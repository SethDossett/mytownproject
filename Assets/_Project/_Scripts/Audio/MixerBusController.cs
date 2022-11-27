using MyTownProject.Events;
using MyTownProject.SO;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

namespace MyTownProject.Audio
{
    public class MixerBusController : MonoBehaviour
    {
        [SerializeField] AudioEventSO _updateBus;
        [SerializeField] GeneralEventSO _updateSettings;
        [SerializeField] GeneralEventSO _gameLoaded;
        [SerializeField] GameSettingsSO _gameSettings;
        public static Bus MasterBus;
        public static Bus MusicBus;
        public static Bus SFXBus;
        public static Bus LowPass;

        private void Awake()
        {
            MasterBus = FMODUnity.RuntimeManager.GetBus("bus:/");
            MusicBus = FMODUnity.RuntimeManager.GetBus("bus:/Music");
            SFXBus = FMODUnity.RuntimeManager.GetBus("bus:/SFX");
            LowPass = FMODUnity.RuntimeManager.GetBus("bus:/LowPass");

            _gameLoaded.OnRaiseEvent += UpdateAllValues;
            _updateBus.onChangeBus += ChangeBusValue;
            _updateBus.onPauseBus += PauseBusValue;
            _updateSettings.OnRaiseEvent += UpdateAllValues;
        }
        private void OnDestroy()
        {
            _gameLoaded.OnRaiseEvent -= UpdateAllValues;
            _updateBus.onChangeBus -= ChangeBusValue;
            _updateBus.onPauseBus -= PauseBusValue;
            _updateSettings.OnRaiseEvent -= UpdateAllValues;
        }

        void ChangeBusValue(Bus bus, float value)
        {
            bus.setVolume(value);
        }
        void PauseBusValue(Bus bus, bool value)
        {
            bus.setPaused(value);
        }

        void UpdateAllValues()
        {
            MasterBus.setVolume(_gameSettings.MasterVolume);
            MusicBus.setVolume(_gameSettings.MusicVolume);
            SFXBus.setVolume(_gameSettings.SFXVolume);
        }

    }
}