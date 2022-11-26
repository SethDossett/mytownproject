using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using MyTownProject.Core;
using MyTownProject.SO;
using UnityEngine.EventSystems;

namespace MyTownProject.UI
{
    public class MainMenuManager : PageMenuBase
    {
        [Header("Main Menu Variables")]

        int lastCurrentScene;

        public void EnterGame()
        {
            InputManager.DisableControls(InputManager.inputActions.UI);
            EventSystem.current.currentInputModule.enabled = false;
            lastCurrentScene = 0;
            StartCoroutine(ChangeScenes());
        }

        IEnumerator ChangeScenes()
        {
            UIEvents.FadeTo(Color.black, 1f);
            yield return new WaitForSecondsRealtime(1f);

            //This is just for now, so that start of game is not coming out of door, 
            //need this to change if we want to start game coming out of last door
            GameSettings.SceneToEnterIn.EnteredThroughDoor = false;
            GameSettings.StartOfGame = true;
            SceneSO scene = GameSettings.SceneToEnterIn;
            scene.playerLocation = scene.NoDoorStartPos;
            scene.playerRotation = scene.NoDoorStartRot;
            MainEventsChannel.RaiseEventChangeScene(scene);
            yield break;
        }
        public override void NaviagtionInput(InputAction.CallbackContext ctx)
        {
            Vector2 inputValue = ctx.ReadValue<Vector2>();
            GameObject currentSelected = EventSystem.current.currentSelectedGameObject;
            Slider slider = currentSelected.GetComponentInChildren<Slider>();

            if (slider)
            {
                if (inputValue.x >= 0.5f)
                {
                    slider.value = Mathf.Min(slider.value + 0.05f, 1);
                }
                else if (inputValue.x <= -0.5f)
                {
                    slider.value = Mathf.Max(slider.value - 0.05f, 0);
                }
            }

        }


    }
}
