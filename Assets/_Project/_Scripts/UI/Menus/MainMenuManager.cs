using UnityEngine;
using System;
using System.Collections;
using MyTownProject.Core;
using MyTownProject.Events;
using MyTownProject.SO;
using UnityEngine.InputSystem;

namespace MyTownProject.UI
{
    public enum MainMenuState
    {
        Front, Options, Controls, Settings
    }
    public class MainMenuManager : MonoBehaviour
    {
        NewControls _inputActions;
        int lastCurrentScene;
        [SerializeField] GameObject firstButton;
        public MainMenuState CurrentMenuState { get; private set; }
        public static event Action<MainMenuState, MainMenuState> OnMenuStateChanged;
        [SerializeField] MenuController controller;
        [SerializeField] GameSettingsSO settings;
        [SerializeField] UIEventChannelSO UIEvents;
        [SerializeField] GeneralEventSO saveControllerType;
        [SerializeField] MainEventChannelSO sceneController;
        [SerializeField] UIEventChannelSO uIEventChannel;

        private void Awake()
        {
            _inputActions = InputManager.inputActions;
            InputManager.ToggleActionMap(_inputActions.UI);
            _inputActions.UI.LeftTrigger.performed += LeftTriggerInput;
            _inputActions.UI.RightTrigger.performed += RightTriggerInput;
            UIEvents.OnChangeControllerType += ChangeController;

            TransitionToState(MainMenuState.Front);
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
            StartCoroutine(ChangeScenes());
        }

        IEnumerator ChangeScenes()
        {
            uIEventChannel.RaiseFadeOut(Color.black, 1f);
            yield return new WaitForSecondsRealtime(1f);

            sceneController.RaiseEventChangeScene(settings.CurrentScene);
            yield break;
        }
        void LeftTriggerInput(InputAction.CallbackContext ctx)
        {
            if (controller.InputDisabled) return;
            print("LeftTriggerPerformed");
            int index = (int)CurrentMenuState - 1;
            if (index < 0) index = 3;

            MainMenuState nextState = (MainMenuState)index;
            TransitionToState(nextState);
            controller.MovePage(-1);
        }
        void RightTriggerInput(InputAction.CallbackContext ctx)
        {
            if (controller.InputDisabled) return;
            print("RightTriggerPerformed");
            int index = (int)CurrentMenuState + 1;
            if (index > 3) index = 0;

            MainMenuState nextState = (MainMenuState)index;
            TransitionToState(nextState);
            controller.MovePage(1);
        }

        public void TransitionToState(MainMenuState newState)
        {
            MainMenuState tmpInitialState = CurrentMenuState;
            CurrentMenuState = newState;
            OnStateEnter(newState, tmpInitialState);
            print("Transition to " + newState);

            OnMenuStateChanged?.Invoke(newState, tmpInitialState);
        }

        void OnStateEnter(MainMenuState toState, MainMenuState fromState)
        {

        }

        void ChangeController(ControllerType controllerType)
        {
            settings.ControllerType = controllerType;
            saveControllerType.RaiseEvent();
        }
    }
}
