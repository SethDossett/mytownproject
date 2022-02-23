using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MyTownProject.Core
{
    public class GameManager : MonoBehaviour
    {
        public GameState gameState;
        public static event Action<GameState> OnGameStateChanged;
        public static GameManager instance;
        private PlayerInput playerInput;
        [SerializeField] GameObject myPlayerInput;
        private void Awake()
        {
            instance = this;
        }
        private void Start()
        {
            playerInput = myPlayerInput.GetComponent<PlayerInput>();
            UpdateState(GameState.GAME_PLAYING);
        }
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

        private void HandleCutscene()
        {
            Debug.Log($"State = {gameState}");
            Time.timeScale = 0f;
            InputManager.ToggleActionMap(InputManager.inputActions.UI);
            myPlayerInput.GetComponent<KinematicCharacterController.Examples.ExamplePlayer>().enabled = false; //Turning off ExamplePlayerScript
        }

        private void HandleGamePaused()
        {
            Debug.Log($"State = {gameState}");
            Time.timeScale = 0f;
            InputManager.ToggleActionMap(InputManager.inputActions.UI);
            myPlayerInput.GetComponent<KinematicCharacterController.Examples.ExamplePlayer>().enabled = false; //Turning off ExamplePlayerScript
        }

        private void HandleGamePlaying()
        {
            Debug.Log($"State = {gameState}");
            Time.timeScale = 1f;
            InputManager.ToggleActionMap(InputManager.inputActions.GamePlay);
            myPlayerInput.GetComponent<KinematicCharacterController.Examples.ExamplePlayer>().enabled = true; //Turning on ExamplePlayerScript

        }

        public enum GameState
        {
            GAME_PLAYING, GAME_PAUSED, CUTSCENE
        }

    }
}