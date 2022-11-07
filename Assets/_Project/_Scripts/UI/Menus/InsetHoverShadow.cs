using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using LeTai.TrueShadow;
using MyTownProject.SO;
using MyTownProject.Core;
using MyTownProject.Events;

namespace MyTownProject.UI
{
    public class InsetHoverShadow : MonoBehaviour
    {
        [Header("Inset")]
        TrueShadow shadow;
        [SerializeField] ControllerType _controllerType;
        [SerializeField] GameObject _keyboardImage;
        [SerializeField] GameObject _gamePadImage;
        [SerializeField] GameSettingsSO settingsSO;
        [SerializeField] GeneralEventSO saveControllerType;
        private void OnEnable()
        {
            if (_controllerType == settingsSO.controllerType){
                StartCoroutine(SetFirstSelection(gameObject));
            }
        }
        public void IsSelected(int value)
        {
            shadow = GetComponent<TrueShadow>();
            // Setting the Shadow Inset on or off
            SetAllOpacity(value);
            // If being Hovered, we change Controller Type & save settings
            if (value == 1)
            {
                SetImage();
                settingsSO.controllerType = _controllerType;
                saveControllerType.RaiseEvent();
            }
        }
        void SetAllOpacity(int value)
        {

            if (value == 1)
            {
                shadow.Inset = false;
            }
            else
            {
                shadow.Inset = true;
            }

        }
        void SetImage()
        {
            if (_controllerType == ControllerType.KeyBoard)
            {
                _gamePadImage.SetActive(false);
                _keyboardImage.SetActive(true);
            }
            else
            {
                _keyboardImage.SetActive(false);
                _gamePadImage.SetActive(true);
            }

        }
        IEnumerator SetFirstSelection(GameObject selection)
        {
            EventSystem.current.SetSelectedGameObject(null);
            yield return new WaitForEndOfFrame();
            EventSystem.current.SetSelectedGameObject(selection);
            yield break;
        }
    }
}
