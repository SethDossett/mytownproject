using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MyTownProject.Core
{
    public class InputManager : MonoBehaviour
    {
        public static NewControls inputActions = new NewControls();
        public static event Action<InputActionMap> actionMapChange;

        private void Start()
        {
            ToggleActionMap(inputActions.GamePlay);
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