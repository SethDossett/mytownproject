using UnityEngine;
using System;
using MyTownProject.Core;
using MyTownProject.Events;
using MyTownProject.SO;
using UnityEngine.InputSystem;

namespace MyTownProject.UI
{
    public enum MenuState
    {
        Front, Options, Controls, Settings
    }
    public class PageMenuBase : MonoBehaviour
    {

        NewControls _inputActions;
        int lastCurrentScene;
        [SerializeField] GameObject firstButton;
        public MenuState CurrentMenuState { get; private set; }
        public static event Action<MenuState, MenuState> OnMenuStateChanged;
        [SerializeField] MenuController controller;
        [SerializeField] GameSettingsSO settings;
        [SerializeField] UIEventChannelSO UIEvents;
        [SerializeField] GeneralEventSO saveControllerType;

        private void Awake()
        {
            _inputActions = InputManager.inputActions;
            InputManager.ToggleActionMap(_inputActions.UI);
            _inputActions.UI.LeftTrigger.performed += LeftTriggerInput;
            _inputActions.UI.RightTrigger.performed += RightTriggerInput;
            UIEvents.OnChangeControllerType += ChangeController;

            TransitionToState(MenuState.Front);
        }
        private void OnDestroy()
        {
            _inputActions.UI.LeftTrigger.performed -= LeftTriggerInput;
            _inputActions.UI.RightTrigger.performed -= RightTriggerInput;
            UIEvents.OnChangeControllerType -= ChangeController;
        }
        public void EnterGame()
        {
            lastCurrentScene = 0;

            //SceneController.SwitchScene(lastCurrentScene);
        }
        public virtual void LeftTriggerInput(InputAction.CallbackContext ctx)
        {
            if (controller.InputDisabled) return;
            print("LeftTriggerPerformed");
            int index = (int)CurrentMenuState - 1;
            if (index < 0) index = 3;

            MenuState nextState = (MenuState)index;
            TransitionToState(nextState);
            controller.MovePage(-1);
        }
        public virtual void RightTriggerInput(InputAction.CallbackContext ctx)
        {
            if (controller.InputDisabled) return;
            print("RightTriggerPerformed");
            int index = (int)CurrentMenuState + 1;
            if (index > 3) index = 0;

            MenuState nextState = (MenuState)index;
            TransitionToState(nextState);
            controller.MovePage(1);
        }

        public virtual void TransitionToState(MenuState newState)
        {
            MenuState tmpInitialState = CurrentMenuState;
            CurrentMenuState = newState;
            OnStateEnter(newState, tmpInitialState);
            print("Transition to " + newState);

            OnMenuStateChanged?.Invoke(newState, tmpInitialState);
        }

        void OnStateEnter(MenuState toState, MenuState fromState)
        {

        }

        void ChangeController(ControllerType controllerType)
        {
            settings.ControllerType = controllerType;
            saveControllerType.RaiseEvent();
        }
    }
}


