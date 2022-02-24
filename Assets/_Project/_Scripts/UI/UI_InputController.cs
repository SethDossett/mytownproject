using UnityEngine;
using UnityEngine.InputSystem;
using MyTownProject.Events;
using MyTownProject.Core;

namespace MyTownProject.UI
{
    public class UI_InputController : MonoBehaviour
    {
        [SerializeField] MainEventChannelSO MainEventChannelSO;
        [SerializeField] DialogueEventsSO DialogueEventsSO;
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
            DialogueEventsSO.Submit();
        }
    }
}