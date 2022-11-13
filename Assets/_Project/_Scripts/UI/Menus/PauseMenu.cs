using UnityEngine;
using System.Collections;
using MyTownProject.Events;
using MyTownProject.Core;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System;

namespace MyTownProject.UI
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] MainEventChannelSO MainEventChannelSO;
        [SerializeField] StateChangerEventSO StateChanger;
        [SerializeField] GameObject pauseMenu;
        [SerializeField] GameObject firstButton;
        GameState currentGameState;
        [SerializeField] MenuController controller;
        public MainMenuState CurrentMenuState { get; private set; }
        public static event Action<MainMenuState, MainMenuState> OnPauseMenuStateChanged;

        private NewControls _inputActions;
        private InputAction exit;
        private InputAction submit;
        private InputAction leftTrigger;
        private InputAction rightTrigger;

        bool _paused;
        private void OnEnable()
        {
            GameStateManager.OnGameStateChanged += ChangedGameState;

            _inputActions = InputManager.inputActions;
            exit = _inputActions.UI.Exit;
            submit = _inputActions.UI.Submit;
            leftTrigger = _inputActions.UI.LeftTrigger;
            rightTrigger = _inputActions.UI.RightTrigger;


            exit.performed += StartButtonPressed;
            submit.performed += SubmitButtonPressed;
            leftTrigger.performed += LeftTriggerReleased;
            rightTrigger.performed += RightTriggerReleased;


            MainEventChannelSO.OnGamePaused += Pause;
        }
        private void OnDisable()
        {
            GameStateManager.OnGameStateChanged -= ChangedGameState;

            exit.performed -= StartButtonPressed;
            submit.performed -= SubmitButtonPressed;
            leftTrigger.performed -= LeftTriggerReleased;
            rightTrigger.performed -= RightTriggerReleased;

            MainEventChannelSO.OnGamePaused -= Pause;
        }
        private void ChangedGameState(GameState state)
        {
            currentGameState = state;

            if (state == GameState.GAME_PLAYING)
            {
                exit.Disable();
                submit.Disable();
            }
            else if (state == GameState.GAME_PAUSED)
            {
                exit.Enable();
                submit.Enable();
            }
            else if (state == GameState.CUTSCENE)
            {
                exit.Enable();
                submit.Enable();
            }
        }
        private void StartButtonPressed(InputAction.CallbackContext obj)
        {
            if (currentGameState == GameState.GAME_PAUSED)
                Resume();
        }
        private void SubmitButtonPressed(InputAction.CallbackContext obj)
        {
            print("Submit Pressed");
        }
        private void LeftTriggerReleased(InputAction.CallbackContext obj)
        {
            if (controller.InputDisabled) return;
            print("LeftTriggerPerformed");
            int index = (int)CurrentMenuState - 1;
            if (index < 0) index = 3;

            MainMenuState nextState = (MainMenuState)index;
            TransitionToState(nextState);
            controller.MovePage(-1);
        }
        private void RightTriggerReleased(InputAction.CallbackContext obj)
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

            OnPauseMenuStateChanged?.Invoke(newState, tmpInitialState);
        }
        void OnStateEnter(MainMenuState toState, MainMenuState fromState)
        {

        }
        private void Pause()
        {
            _paused = true;
            StartCoroutine(SetFirstSelection());
            if (!pauseMenu.activeInHierarchy)
                pauseMenu.SetActive(true);
        }
        public void Resume()
        {
            _paused = false;
            MainEventChannelSO.RaiseEventUnPaused();
            if (pauseMenu.activeInHierarchy)
                pauseMenu.SetActive(false);
        }

        IEnumerator SetFirstSelection()
        {
            EventSystem.current.SetSelectedGameObject(null);
            yield return new WaitForEndOfFrame();
            EventSystem.current.SetSelectedGameObject(firstButton);
            yield break;
        }

        IEnumerator CheckInputs()
        {
            while (_paused)
            {
                //if(leftTrigger.ReadValue<float>() == 0)
                //print(leftTrigger.ReadValue<float>());
                yield return null;
            }
            yield break;
        }
    }
}