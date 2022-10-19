using System;
using UnityEngine;
using UnityEngine.InputSystem;
using MyTownProject.Events;

namespace MyTownProject.Core
{
    public class InputManager : MonoBehaviour
    {
        public static NewControls inputActions = new NewControls();
        public static event Action<InputActionMap> actionMapChange;

        [SerializeField] GeneralEventSO _enableControls;
        [SerializeField] GeneralEventSO _disableControls;

        private void OnEnable()
        {
            GameStateManager.OnGameStateChanged += ChangedGameState;
            _disableControls.OnRaiseEvent += DisableControls;
            _enableControls.OnRaiseEvent += EnableControls;
        }
        private void OnDisable()
        {
            GameStateManager.OnGameStateChanged -= ChangedGameState;
            _disableControls.OnRaiseEvent -= DisableControls;
            _enableControls.OnRaiseEvent -= EnableControls;
        }
        void EnableControls() => inputActions.Enable();
       
        void DisableControls() => inputActions.Disable();
        
        private void ChangedGameState(GameStateManager.GameState state)
        {
            if (state == GameStateManager.GameState.GAME_PLAYING)
            {
                inputActions.UI.Disable();
                inputActions.GamePlay.Enable();
                ToggleActionMap(inputActions.GamePlay);
            }
            else if (state == GameStateManager.GameState.GAME_PAUSED)
            {
                inputActions.GamePlay.Disable();
                inputActions.UI.Enable();
                ToggleActionMap(inputActions.UI);
            }
            else if (state == GameStateManager.GameState.CUTSCENE)
            {
                inputActions.GamePlay.Disable();
                inputActions.UI.Enable();
                ToggleActionMap(inputActions.UI);
            }
        }

        public static void ToggleActionMap(InputActionMap actionMap)
        {
            if (actionMap.enabled)
                return;

            inputActions.Disable();
            actionMapChange?.Invoke(actionMap);
            actionMap.Enable();
        }
    }
}