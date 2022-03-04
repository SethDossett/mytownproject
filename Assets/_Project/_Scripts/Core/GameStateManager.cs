using System;
using UnityEngine;
using UnityEngine.InputSystem;
using KinematicCharacterController.Examples;
using MyTownProject.Events;

namespace MyTownProject.Core
{
    public class GameStateManager : MonoBehaviour
    {
        #region State of Game
        public static event Action<GameState> OnGameStateChanged;
        public GameState gameState;

        public void UpdateState(GameState newState)
        {
            gameState = newState;

            switch (newState)
            {
                case GameState.GAME_PLAYING:
                    HandleGamePlaying();
                    break;
                case GameState.GAME_PAUSED:
                    HandleGamePaused();
                    break;
                case GameState.CUTSCENE:
                    HandleCutscene();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);

            }
            OnGameStateChanged?.Invoke(newState);
        }

        public enum GameState
        {
            GAME_PLAYING, GAME_PAUSED, CUTSCENE
        }

        #endregion

        #region State Logic

        public static GameStateManager instance;
        private PlayerInput playerInput;
        [SerializeField] GameObject myPlayerInput;
        private void Awake()
        {
            instance = this;
        }
        private void Start()
        {
            //playerInput = myPlayerInput.GetComponent<PlayerInput>();
            UpdateState(GameState.CUTSCENE);
        }

        private void HandleCutscene()
        {
            Debug.Log($"State = {gameState}");
            Time.timeScale = 0f;
            InputManager.ToggleActionMap(InputManager.inputActions.UI);
            myPlayerInput.GetComponent<ExamplePlayer>().enabled = false; //Turning off ExamplePlayerScript
        }

        private void HandleGamePaused()
        {
            Debug.Log($"State = {gameState}");
            Time.timeScale = 0f;
            InputManager.ToggleActionMap(InputManager.inputActions.UI);
            myPlayerInput.GetComponent<ExamplePlayer>().enabled = false; //Turning off ExamplePlayerScript
        }

        private void HandleGamePlaying()
        {
            Debug.Log($"State = {gameState}");
            Time.timeScale = 1f;
            InputManager.ToggleActionMap(InputManager.inputActions.GamePlay);
            myPlayerInput.GetComponent<ExamplePlayer>().enabled = true; //Turning on ExamplePlayerScript

        }
        #endregion

        #region Changing State

        [SerializeField] StateChangerEventSO stateChanger;
        void OnEnable()
        {
            stateChanger.OnGameState += UpdateState;
        }
        void OnDisable()
        {
            stateChanger.OnGameState -= UpdateState;
        }


        #endregion







    }
}