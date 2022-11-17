using UnityEngine;
using System;
using System.Collections;
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
    public abstract class PageMenuBase : MonoBehaviour
    {

        NewControls _inputActions;
        int _pageCount;
        [SerializeField] GameObject firstButton;
        [field:SerializeField] public MenuState CurrentMenuState { get; private set; }
        public static event Action<MenuState, MenuState> OnMenuStateChanged;
        [SerializeField] MenuController controller;
        public GameSettingsSO GameSettings;
        public UIEventChannelSO UIEvents;
        public GeneralEventSO SaveControllerType;
        public MainEventChannelSO SceneController;
        public UIEventChannelSO UIEventChannel;

        public virtual void Awake()
        {
            _inputActions = InputManager.inputActions;
            InputManager.ToggleActionMap(_inputActions.UI);
            _inputActions.UI.LeftTrigger.performed += LeftTriggerInput;
            _inputActions.UI.RightTrigger.performed += RightTriggerInput;
            UIEvents.OnChangeControllerType += ChangeController;

            TransitionToState(MenuState.Front);
            _pageCount = controller.pages.Count;
        }
        public virtual void OnDestroy()
        {
            _inputActions.UI.LeftTrigger.performed -= LeftTriggerInput;
            _inputActions.UI.RightTrigger.performed -= RightTriggerInput;
            UIEvents.OnChangeControllerType -= ChangeController;
        }

        public virtual void LeftTriggerInput(InputAction.CallbackContext ctx)
        {
            if (controller.InputDisabled) return;
            print("LeftTriggerPerformed");
            int index = (int)CurrentMenuState - 1;
            if (index < 0) index = _pageCount - 1;

            MenuState nextState = (MenuState)index;
            TransitionToState(nextState);
            controller.MovePage(-1);
        }
        public virtual void RightTriggerInput(InputAction.CallbackContext ctx)
        {
            if (controller.InputDisabled) return;
            print("RightTriggerPerformed");
            int index = (int)CurrentMenuState + 1;
            if (index > _pageCount - 1) index = 0;

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

        public virtual void OnStateEnter(MenuState toState, MenuState fromState)
        {

        }

        public virtual void ChangeController(ControllerType controllerType)
        {
            GameSettings.ControllerType = controllerType;
            SaveControllerType.RaiseEvent();
        }


        public virtual void QuitGame()
        {
            // save any game data here
#if UNITY_EDITOR
            // Application.Quit() does not work in the editor so
            // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
            UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
        }

        public virtual void OnApplicationQuit()
        {
            print("QUIT GAME");
        }
    }
}


