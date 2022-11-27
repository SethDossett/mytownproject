using MyTownProject.SO;
using MyTownProject.Events;
using MyTownProject.Audio;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MyTownProject.UI
{
    //Change this to be on One Script instead of every individual button.
    public class SettingsValues : MonoBehaviour
    {
        public enum SettingToChange
        {
            MasterVolume, MusicVolume, SFXVolume
        }
        [field: SerializeField] public SettingToChange SelectedSettingToChange { get; private set; }
        [SerializeField] GameSettingsSO gameSettings;
        [SerializeField] GeneralEventSO UpdateSettings;
        [SerializeField] AudioEventSO UpdateBus;

        [SerializeField] Slider _slider;
        [SerializeField] TextMeshProUGUI _percentage;

        void Awake()
        {
            UpdateSettings.OnRaiseEvent += SetInitialSetting;
            SetInitialSetting();
        }
        void OnDestroy()
        {
            UpdateSettings.OnRaiseEvent -= SetInitialSetting;
        }
        void SetInitialSetting()
        {
            if (SelectedSettingToChange == SettingToChange.MasterVolume)
            {
                _slider.value = gameSettings.MasterVolume;
            }
            else if (SelectedSettingToChange == SettingToChange.MusicVolume)
            {
                _slider.value = gameSettings.MusicVolume;
            }
            else if (SelectedSettingToChange == SettingToChange.SFXVolume)
            {
                _slider.value = gameSettings.SFXVolume;
            }
            
            SetPercentageValue();

        }
        public void ChangeSetting()
        {
            float sliderValue = _slider.value;

            if (SelectedSettingToChange == SettingToChange.MasterVolume)
            {
                gameSettings.MasterVolume = sliderValue;
                UpdateBus.ChangeBus(MixerBusController.MasterBus, sliderValue);
            }
            else if (SelectedSettingToChange == SettingToChange.MusicVolume)
            {
                gameSettings.MusicVolume = sliderValue;
                UpdateBus.ChangeBus(MixerBusController.MusicBus, sliderValue);
            }
            else if (SelectedSettingToChange == SettingToChange.SFXVolume)
            {
                gameSettings.SFXVolume = sliderValue;
                UpdateBus.ChangeBus(MixerBusController.SFXBus, sliderValue);
            }

            SetPercentageValue();
        }
        void SetPercentageValue()
        {
            int value = Mathf.RoundToInt(_slider.value * 100);
            _percentage.text = $"{value}%";
        }

    }
}