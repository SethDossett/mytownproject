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
            print("Left Trigger Release");
        }
        private void RightTriggerReleased(InputAction.CallbackContext obj)
        {
            print("Right Trigger Released");
        }
        private void Pause()
        {
            _paused = true;
            StartCoroutine(SetFirstSelection());
            StartCoroutine(CheckInputs());
            if (!pauseMenu.activeInHierarchy)
                pauseMenu.SetActive(true);
        }
        public void Resume()
        {
            _paused = false;
            StopCoroutine(CheckInputs());
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