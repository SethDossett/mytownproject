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
    public abstract class PageMenuBase : MonoBehaviour
    {
        [field: SerializeField] public MenuState CurrentMenuState { get; private set; }
        [field: SerializeField] public MenuController Controller { get; private set; }
        [SerializeField] GameObject firstButton;
        [SerializeField] protected GameSettingsSO GameSettings;
        [SerializeField] protected GameSettingsSO DefaultGameSettings;
        [SerializeField] protected UIEventChannelSO UIEvents;
        [SerializeField] protected GeneralEventSO SaveControllerType;
        [SerializeField] protected MainEventChannelSO MainEventsChannel;
        [SerializeField] protected GeneralEventSO UpdateSettings;

        public static event Action<MenuState, MenuState> OnMenuStateChanged;

        int _pageCount;
        NewControls _inputActions;
        public NewControls InputActions { get { return _inputActions; } private set { _inputActions = value; } }

        public virtual void Awake()
        {
            _inputActions = InputManager.inputActions;
            InputManager.ToggleActionMap(_inputActions.UI);
            _inputActions.UI.Navigate.performed += NaviagtionInput;
            _inputActions.UI.LeftTrigger.performed += LeftTriggerInput;
            _inputActions.UI.RightTrigger.performed += RightTriggerInput;
            UIEvents.OnChangeControllerType += ChangeController;

            TransitionToState(MenuState.Front);
            _pageCount = Controller.pages.Count;
        }
        public virtual void OnDestroy()
        {
            _inputActions.UI.Navigate.performed -= NaviagtionInput;
            _inputActions.UI.LeftTrigger.performed -= LeftTriggerInput;
            _inputActions.UI.RightTrigger.performed -= RightTriggerInput;
            UIEvents.OnChangeControllerType -= ChangeController;
        }

        public virtual void NaviagtionInput(InputAction.CallbackContext ctx)
        {

        }
        public virtual void LeftTriggerInput(InputAction.CallbackContext ctx)
        {
            if (Controller.InputDisabled) return;
            print("LeftTriggerPerformed");
            int index = (int)CurrentMenuState - 1;
            if (index < 0) index = _pageCount - 1;

            MenuState nextState = (MenuState)index;
            TransitionToState(nextState);
            Controller.MovePage(-1);
        }
        public virtual void RightTriggerInput(InputAction.CallbackContext ctx)
        {
            if (Controller.InputDisabled) return;
            print("RightTriggerPerformed");
            int index = (int)CurrentMenuState + 1;
            if (index > _pageCount - 1) index = 0;

            MenuState nextState = (MenuState)index;
            TransitionToState(nextState);
            Controller.MovePage(1);
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

        public virtual void ResetGameSettings()
        {
            GameSettings.MasterVolume = DefaultGameSettings.MasterVolume;
            GameSettings.MusicVolume = DefaultGameSettings.MusicVolume;
            GameSettings.SFXVolume = DefaultGameSettings.SFXVolume;

            UpdateSettings.RaiseEvent();
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


