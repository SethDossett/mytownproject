using MyTownProject.Core;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MyTownProject.UI
{
    public enum SelectedObjectType
    {
        Resolution, Vsync, Brightness, MasterVolume, MusicVolume, SFXVolume
    }
    public class SettingsController : MonoBehaviour
    {
        NewControls _inputActions;
        public List<ResItem> Resolutions = new List<ResItem>();
        [SerializeField] int _selectedResolution;
        [SerializeField] bool _isFullScreen;
        Slider _currentSlider;

        [SerializeField] TextMeshProUGUI _resolutionsLabel;
        [SerializeField] GameObject _currentSelectedGameObject;

        private void OnEnable()
        {
            _isFullScreen = Screen.fullScreen;
            UpdateResLabel();
            _inputActions = InputManager.inputActions;
            _inputActions.UI.Navigate.performed += NavigateInput;
        }
        private void OnDisable()
        {
            _inputActions.UI.Navigate.performed -= NavigateInput;
        }
        public void NavigateInput(InputAction.CallbackContext ctx)
        {
            _currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
            SettingObject obj = _currentSelectedGameObject.GetComponent<SettingObject>();
            if (!obj) return;
            SelectedObjectType type = obj.objectType;

            Vector2 value = ctx.ReadValue<Vector2>();
            // Put these else ifs in the same order on canvas, so it checks them in order
            if (type == SelectedObjectType.Resolution) ResolutionSelected(value.x);
            else if (type == SelectedObjectType.Vsync) VsyncSelected(value.x);
            else if (type == SelectedObjectType.Brightness) BrightnessSelected(value.x);
            else VolumeSelected(type, value.x);
        }

        void ResolutionSelected(float xInput)
        {
            print("Resolution Selected");
            //Right Input
            if (xInput >= 0.5f)
            {
                _selectedResolution++;
                if (_selectedResolution > Resolutions.Count - 1)
                    _selectedResolution = 0;

                UpdateResLabel();
            }
            //Left Input
            else if (xInput <= -0.5f)
            {
                _selectedResolution--;
                if (_selectedResolution < 0)
                    _selectedResolution = Resolutions.Count - 1;

                UpdateResLabel();
            }
        }
        void VsyncSelected(float xInput)
        {
            print("Vsync Selected");
        }
        void BrightnessSelected(float xInput)
        {
            print("Brightness Selected");
        }
        void VolumeSelected(SelectedObjectType objectType, float xInput)
        {
            print("Volume Selected");
            _currentSlider = _currentSelectedGameObject.GetComponentInChildren<Slider>();

            if (_currentSlider)
            {
                if (xInput >= 0.5f)
                {
                    _currentSlider.value = Mathf.Min(_currentSlider.value + 0.05f, 1);
                }
                else if (xInput <= -0.5f)
                {
                    _currentSlider.value = Mathf.Max(_currentSlider.value - 0.05f, 0);
                }
            }
        }
        public void UpdateResLabel()
        {
            int h = Resolutions[_selectedResolution].Horizontal;
            int v = Resolutions[_selectedResolution].Vertical;

            _resolutionsLabel.text = h.ToString() + " x " + v.ToString();

            Screen.SetResolution(h, v, _isFullScreen);
        }


        [System.Serializable]
        public struct ResItem
        {
            public int Horizontal, Vertical;
        }

    }
}