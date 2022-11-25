using UnityEngine.UI;
using UnityEngine;
using MyTownProject.Events;
using MyTownProject.Core;
using MyTownProject.SO;
using TMPro;

namespace MyTownProject.UI
{
    public class UIIconSwapper : MonoBehaviour
    {
        [SerializeField] GameSettingsSO gameSettingsSO;
        [SerializeField] UIEventChannelSO changeController;
        Image _image;
        TextMeshProUGUI _text;
        [SerializeField] Sprite _spriteKeyboard;
        [SerializeField] Sprite _spritGamePad;
        [SerializeField] string _textKeyboard;
        [SerializeField] string _textGamepad;
        [SerializeField] bool _changeImage;
        [SerializeField] bool _changeText;


        private void Awake()
        {
            _image = GetComponent<Image>();
            _text = GetComponent<TextMeshProUGUI>();
            ControllerTypeChanged(gameSettingsSO.ControllerType);
            changeController.OnChangeControllerType += ControllerTypeChanged;
        }
        private void OnDestroy()
        {
            changeController.OnChangeControllerType -= ControllerTypeChanged;
        }

        void ControllerTypeChanged(ControllerType controllerType)
        {
            if (_changeImage) ChangeSprite(controllerType);
            if (_changeText) ChangeText(controllerType);
        }
        void ChangeSprite(ControllerType controllerType)
        {
            if (controllerType == ControllerType.KeyBoard)
            {
                _image.sprite = _spriteKeyboard;
            }
            if (controllerType == ControllerType.GamePad)
            {
                _image.sprite = _spritGamePad;
            }
        }
        void ChangeText(ControllerType controllerType)
        {
            if (controllerType == ControllerType.KeyBoard)
            {
                _text.text = _textKeyboard;
            }
            if (controllerType == ControllerType.GamePad)
            {
                _text.text = _textGamepad;
            }
        }
    }
}