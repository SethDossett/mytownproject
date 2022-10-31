using System;
using UnityEngine;
using UnityEngine.InputSystem;
using MyTownProject.Events;

namespace MyTownProject.Core
{
    public class InputManager
    {
        public static NewControls inputActions = new NewControls();
        public static event Action<InputActionMap> actionMapChange;


        public static void EnableControls() => inputActions.Enable();

        public static void DisableControls() => inputActions.Disable();

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