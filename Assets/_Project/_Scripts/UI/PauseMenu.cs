using UnityEngine;
using System.Collections;
using MyTownProject.Events;
using MyTownProject.Core;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace MyTownProject.UI
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] MainEventChannelSO MainEventChannelSO;
        [SerializeField] StateChangerEventSO StateChanger;
        [SerializeField] GameObject pauseMenu;
        [SerializeField] GameObject firstButton;
        GameState currentGameState;

        private NewControls _inputActions;
        private InputAction exit;
        private InputAction submit;
        private void OnEnable()
        {
            GameStateManager.OnGameStateChanged += ChangedGameState;

            _inputActions = InputManager.inputActions;
            exit = _inputActions.UI.Exit;
            submit = _inputActions.UI.Submit;
            
            exit.performed += StartButtonPressed;
            submit.performed += SubmitButtonPressed;

            MainEventChannelSO.OnGamePaused += Pause;
            MainEventChannelSO.OnGameUnPaused += Resume;
        }
        private void OnDisable()
        {
            GameStateManager.OnGameStateChanged -= ChangedGameState;

            exit.performed -= StartButtonPressed;
            submit.performed -= SubmitButtonPressed;
            
            MainEventChannelSO.OnGamePaused -= Pause;
            MainEventChannelSO.OnGameUnPaused -= Resume;
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
            if(currentGameState == GameState.GAME_PAUSED)
                MainEventChannelSO.RaiseEventUnPaused();
        }
        private void SubmitButtonPressed(InputAction.CallbackContext obj)
        {
            print("Submit Pressed");
        }
        private void Pause()
        {
            StartCoroutine(SetFirstSelection());
            if (!pauseMenu.activeInHierarchy)
                pauseMenu.SetActive(true);
        }
        private void Resume()
        {
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
    }
}