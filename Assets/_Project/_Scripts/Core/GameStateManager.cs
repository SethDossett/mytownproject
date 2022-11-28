using System;
using System.Collections;
using UnityEngine;
using MyTownProject.Events;

namespace MyTownProject.Core
{
    public enum GameState
    {
        GAME_PLAYING, GAME_PAUSED, CUTSCENE
    }
    public class GameStateManager : MonoBehaviour
    {
        #region State of Game
        public static event Action<GameState> OnGameStateChanged;
        public GameState gameState;

        public void UpdateState(GameState newState)
        {
            StopAllCoroutines();
            gameState = newState;

            switch (newState)
            {
                case GameState.GAME_PLAYING:
                    StartCoroutine(HandleGamePlaying());
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


        #endregion

        #region State Logic

        public static GameStateManager instance;
        private void Awake()
        {
            instance = this;
        }
        private void Start()
        {
            //UpdateState(GameState.GAME_PLAYING);
        }

        private void HandleCutscene()
        {
            Debug.Log($"State = {gameState}");
            Time.timeScale = 0f;
            InputManager.ToggleActionMap(InputManager.inputActions.UI);
            //myPlayerInput.GetComponent<ExamplePlayer>().enabled = false; //Turning off ExamplePlayerScript
        }

        private void HandleGamePaused()
        {
            Debug.Log($"State = {gameState}");
            Time.timeScale = 0f;
            InputManager.ToggleActionMap(InputManager.inputActions.UI);
            //myPlayerInput.GetComponent<ExamplePlayer>().enabled = false; //Turning off ExamplePlayerScript
        }

        IEnumerator HandleGamePlaying()
        {
            Debug.Log($"State = {gameState}");
            Time.timeScale = 1f;
            yield return new WaitForSecondsRealtime(0.4f);
            InputManager.ToggleActionMap(InputManager.inputActions.GamePlay);
            //myPlayerInput.GetComponent<ExamplePlayer>().enabled = true; //Turning on ExamplePlayerScript
            yield break;
        }
        #endregion

        #region Changing State

        [SerializeField] StateChangerEventSO stateChanger;
        [SerializeField] DialogueEventsSO dialogueEvents;
        [SerializeField] MainEventChannelSO mainEventChannel;
        [SerializeField] GeneralEventSO StartOfGame;
        void OnEnable()
        {
            StartOfGame.OnRaiseEvent += EnterCutsceneState; // Probably should be changed to event fired when cutscene should be triggered
            stateChanger.OnGameState += UpdateState;
            dialogueEvents.onEnter += EnteringDialogue;
            dialogueEvents.onExit += EnterGamePlayingState;
            mainEventChannel.OnGamePaused += EnterGamePausedState;
            mainEventChannel.OnGameUnPaused += EnterGamePlayingState;
        }
        void OnDisable()
        {
            StartOfGame.OnRaiseEvent -= EnterCutsceneState;
            stateChanger.OnGameState -= UpdateState;
            dialogueEvents.onEnter -= EnteringDialogue;
            dialogueEvents.onExit -= EnterGamePlayingState;
            mainEventChannel.OnGamePaused -= EnterGamePausedState;
            mainEventChannel.OnGameUnPaused -= EnterGamePlayingState;
        }

        void EnteringDialogue(GameObject obj, TextAsset json)
        {
            EnterCutsceneState();
        }
        void EnterGamePlayingState()
        {
            if (gameState != GameState.GAME_PLAYING) UpdateState(GameState.GAME_PLAYING);
        }
        void EnterGamePausedState()
        {
            if (gameState != GameState.GAME_PAUSED) UpdateState(GameState.GAME_PAUSED);
        }
        void EnterCutsceneState()
        {
            if (gameState != GameState.CUTSCENE) UpdateState(GameState.CUTSCENE);
        }



        #endregion







    }
}