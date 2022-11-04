using UnityEngine;
using System.Collections;
using System;
using MyTownProject.Core;
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

        private void Awake()
        {
            _inputActions = InputManager.inputActions;
            InputManager.ToggleActionMap(_inputActions.UI);
            _inputActions.UI.LeftTrigger.performed += LeftTriggerInput;
            _inputActions.UI.RightTrigger.performed += RightTriggerInput;
        }
        private void OnDestroy()
        {
            _inputActions.UI.LeftTrigger.performed -= LeftTriggerInput;
            _inputActions.UI.RightTrigger.performed -= RightTriggerInput;
        }
        public void EnterGame()
        {
            lastCurrentScene = 0;

            //SceneController.SwitchScene(lastCurrentScene);
        }
        void LeftTriggerInput(InputAction.CallbackContext ctx)
        {
            if(controller.InputDisabled) return;
            print("LeftTriggerPerformed");
            int index = (int)CurrentMenuState - 1;
            if(index < 0) index = 3;

            MainMenuState nextState = (MainMenuState)index;
            TransitionToState(nextState);
            controller.MovePage(-1);
        }
        void RightTriggerInput(InputAction.CallbackContext ctx)
        {
            if(controller.InputDisabled) return;
            print("RightTriggerPerformed");
            int index = (int)CurrentMenuState + 1;
            if(index > 3) index = 0;

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

        private void OnStateEnter(MainMenuState toState, MainMenuState fromState)
        {

        }

        
    }
}
