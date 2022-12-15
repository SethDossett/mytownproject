using UnityEngine;
using MyTownProject.Events;
using MyTownProject.Core;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections;
using MyTownProject.SO;

namespace MyTownProject.UI
{
    public class PauseMenu : PageMenuBase
    {
        [Header("Pause Menu Variables")]
        [SerializeField] StateChangerEventSO StateChanger;
        [SerializeField] GeneralEventSO GameSaved;
        [SerializeField] SceneSO MainMenuScene;
        [SerializeField] GameObject pauseMenu;
        GameState currentGameState;

        private InputAction exit;
        private InputAction submit;

        bool _paused;
        private void OnEnable()
        {
            GameStateManager.OnGameStateChanged += ChangedGameState;

            exit = InputActions.UI.Exit;
            submit = InputActions.UI.Submit;
            

            exit.performed += StartButtonPressed;
            submit.performed += SubmitButtonPressed;
            

            MainEventsChannel.OnGamePaused += Pause;
        }
        private void OnDisable()
        {
            GameStateManager.OnGameStateChanged -= ChangedGameState;

            exit.performed -= StartButtonPressed;
            submit.performed -= SubmitButtonPressed;
            

            MainEventsChannel.OnGamePaused -= Pause;
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
        public override void ChangeScene()
        {
            InputManager.DisableControls(InputManager.inputActions.UI);
            EventSystem.current.currentInputModule.enabled = false;
            StartCoroutine(ChangeScenes());
        }

        IEnumerator ChangeScenes()
        {
            UIEvents.FadeTo(Color.black, 1f);
            yield return new WaitForSecondsRealtime(1f);
 
            MainEventsChannel.RaiseEventChangeScene(MainMenuScene);
            yield break;
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
            
            Controller.SelectButton();
        }
        public void Resume()
        {
            _paused = false;
            MainEventsChannel.RaiseEventUnPaused();
            if (pauseMenu.activeInHierarchy)
                pauseMenu.SetActive(false);
        }
        public void SaveGame()
        {
            SaveControllerType.RaiseEvent();
        }
        
    }
}