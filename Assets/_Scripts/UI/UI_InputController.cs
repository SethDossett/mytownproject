using UnityEngine;
using UnityEngine.InputSystem;

public class UI_InputController : MonoBehaviour
{
    private InputAction exit;
    public MainEventChannelSO MainEventChannelSO;

    private void OnEnable()
    {
        exit = InputManager.inputActions.UI.Exit;
        InputManager.inputActions.UI.Exit.performed += ExitMenu;
    }
    private void OnDisable()
    {
        InputManager.inputActions.UI.Exit.performed -= ExitMenu;
    }
    private void ExitMenu(InputAction.CallbackContext obj)
    {
        MainEventChannelSO.RaiseEventUnPaused();
    }
}
