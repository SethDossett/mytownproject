using MyTownProject.SO;
using MyTownProject.Events;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MyTownProject.UI
{
    public class SettingsValues : MonoBehaviour
    {
        public enum SettingToChange
        {
            MasterVolume, MusicVolume, SFXVolume
        }
        [field: SerializeField] public SettingToChange SelectedSettingToChange { get; private set; }
        [SerializeField] GameSettingsSO gameSettings;
        [SerializeField] GeneralEventSO UpdateSettings;

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
                SetPercentageValue();
            }
            if (SelectedSettingToChange == SettingToChange.MusicVolume)
            {
                _slider.value = gameSettings.MusicVolume;
                SetPercentageValue();
            }
            if (SelectedSettingToChange == SettingToChange.SFXVolume)
            {
                _slider.value = gameSettings.SFXVolume;
                SetPercentageValue();
            }

        }
        public void ChangeSetting()
        {
            if (SelectedSettingToChange == SettingToChange.MasterVolume)
            {
                gameSettings.MasterVolume = _slider.normalizedValue;
                SetPercentageValue();
            }
            else if (SelectedSettingToChange == SettingToChange.MusicVolume)
            {
                gameSettings.MusicVolume = _slider.normalizedValue;
                SetPercentageValue();
            }
            else if (SelectedSettingToChange == SettingToChange.SFXVolume)
            {
                gameSettings.SFXVolume = _slider.normalizedValue;
                SetPercentageValue();
            }
        }
        void SetPercentageValue()
        {
            int value = Mathf.RoundToInt(_slider.value * 100);
            _percentage.text = $"{value}%";
        }

    }
}