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
        FullScreen, Resolution, Vsync, Brightness, MasterVolume, MusicVolume, SFXVolume
    }
    public class SettingsController : MonoBehaviour
    {
        NewControls _inputActions;
        [SerializeField] bool _forceFixedResolutions;
        public List<ResItem> Resolutions = new List<ResItem>();
        [SerializeField] int _selectedResolution;
        [SerializeField] bool _isFullScreen;
        bool _foundResolution;
        Slider _currentSlider;

        [SerializeField] Toggle _toggle;
        [SerializeField] TextMeshProUGUI _resolutionsLabel;
        [SerializeField] GameObject _currentSelectedGameObject;

        private void OnEnable()
        {
            InitializeResolution();
            // change this to event of starting up game
            _inputActions = InputManager.inputActions;
            _inputActions.UI.Navigate.performed += NavigateInput;
        }
        private void OnDisable()
        {
            _inputActions.UI.Navigate.performed -= NavigateInput;
        }
        void InitializeResolution()
        {
            _isFullScreen = Screen.fullScreen;
            _toggle.isOn = _isFullScreen;
            _foundResolution = false;
            for (int i = 0; i < Resolutions.Count; i++)
            {
                if (Screen.width == Resolutions[i].Horizontal && Screen.height == Resolutions[i].Vertical)
                {
                    _foundResolution = true;
                    _selectedResolution = i;
                    UpdateResLabel();
                    break;
                }
            }
            if (!_forceFixedResolutions)
            {
                if (!_foundResolution)
                {
                    ResItem newRes = new ResItem(Screen.width, Screen.height);

                    Resolutions.Add(newRes);
                    _selectedResolution = Resolutions.Count - 1;
                    UpdateResLabel();
                }
            }
        }
        public void NavigateInput(InputAction.CallbackContext ctx)
        {
            _currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
            SettingObject obj = _currentSelectedGameObject.GetComponent<SettingObject>();
            if (!obj) return;
            SelectedObjectType type = obj.objectType;

            Vector2 value = ctx.ReadValue<Vector2>();
            // Put these else ifs in the same order on canvas, so it checks them in order
            if (type == SelectedObjectType.FullScreen) FullScreenSelected();
            else if (type == SelectedObjectType.Resolution) ResolutionSelected(value.x);
            else if (type == SelectedObjectType.Vsync) VsyncSelected(value.x);
            else if (type == SelectedObjectType.Brightness) BrightnessSelected(value.x);
            else VolumeSelected(type, value.x);
        }
        void FullScreenSelected() { }
        public void UpdateFullScreen()
        {
            _isFullScreen = _toggle.isOn;
            Screen.fullScreen = _isFullScreen;
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

            public ResItem(int horizontal, int vertical)
            {
                this.Horizontal = horizontal;
                this.Vertical = vertical;
            }
        }

    }
}