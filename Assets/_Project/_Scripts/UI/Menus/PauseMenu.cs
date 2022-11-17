using UnityEngine;
using System.Collections;
using MyTownProject.Events;
using MyTownProject.Core;
using MyTownProject.SO;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System;

namespace MyTownProject.UI
{
    public class PauseMenu : PageMenuBase
    {
        [SerializeField] MainEventChannelSO MainEventChannelSO;
        [SerializeField] StateChangerEventSO StateChanger;
        [SerializeField] GameSettingsSO pauseGameSettings;
        [SerializeField] GameObject pauseMenu;
        GameState currentGameState;

        private NewControls _pauseMenuActions;
        private InputAction exit;
        private InputAction submit;

        bool _paused;
        private void OnEnable()
        {
            GameStateManager.OnGameStateChanged += ChangedGameState;

            _pauseMenuActions = InputManager.inputActions;
            exit = _pauseMenuActions.UI.Exit;
            submit = _pauseMenuActions.UI.Submit;
            


            exit.performed += StartButtonPressed;
            submit.performed += SubmitButtonPressed;
            

            MainEventChannelSO.OnGamePaused += Pause;
        }
        private void OnDisable()
        {
            GameStateManager.OnGameStateChanged -= ChangedGameState;

            exit.performed -= StartButtonPressed;
            submit.performed -= SubmitButtonPressed;
            

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
        
        private void Pause()
        {
            _paused = true;
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

        
    }
}