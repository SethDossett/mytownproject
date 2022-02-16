using UnityEngine;
using UnityEngine.InputSystem;

public class UI_InputController : MonoBehaviour
{
    public MainEventChannelSO MainEventChannelSO;
    private InputAction exit;
    private InputAction submit;

    private void OnEnable()
    {
        exit = InputManager.inputActions.UI.Exit;
        submit = InputManager.inputActions.UI.Submit;
        
        exit.performed += ExitMenu;
        submit.performed += SubmitAction;
    }


    private void OnDisable()
    {
        exit.performed -= ExitMenu;
        submit.performed -= SubmitAction;
    }
    private void ExitMenu(InputAction.CallbackContext obj)
    {
        MainEventChannelSO.RaiseEventUnPaused();
    }

    private void SubmitAction(InputAction.CallbackContext obj)
    {
        MainEventChannelSO.RaiseEventSubmit();
    }
}
