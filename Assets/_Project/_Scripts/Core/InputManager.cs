using System;
using UnityEngine.InputSystem;

namespace MyTownProject.Core
{
    public static class InputManager
    {
        public static NewControls inputActions = new NewControls();
        public static event Action<InputActionMap> actionMapChange;


        public static void EnableControls(InputActionMap actionMap) => actionMap.Enable();

        public static void DisableControls(InputActionMap actionMap) => actionMap.Disable();

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