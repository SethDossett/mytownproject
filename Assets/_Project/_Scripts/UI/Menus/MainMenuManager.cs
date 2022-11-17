using UnityEngine;
using System;
using System.Collections;
using MyTownProject.Core;
using MyTownProject.Events;
using MyTownProject.SO;
using UnityEngine.InputSystem;

namespace MyTownProject.UI
{
    public class MainMenuManager : PageMenuBase
    {
        [Header("Main Menu Variables")]
        [SerializeField] GameSettingsSO menuGameSettings;
        [SerializeField] UIEventChannelSO uIEvents;
        [SerializeField] GeneralEventSO saveControllerType;
        [SerializeField] MainEventChannelSO sceneController;
        [SerializeField] UIEventChannelSO uIEventChannel;

        int lastCurrentScene;
        
        public void EnterGame()
        {
            lastCurrentScene = 0;
            StartCoroutine(ChangeScenes());
        }

        IEnumerator ChangeScenes()
        {
            uIEventChannel.RaiseFadeOut(Color.black, 1f);
            yield return new WaitForSecondsRealtime(1f);

            //This is just for now, so that start of game is not coming out of door, 
            //need this to change if we want to start game coming out of last door
            menuGameSettings.SceneToEnterIn.EnteredThroughDoor = false;
            menuGameSettings.StartOfGame = true;
            SceneSO scene = menuGameSettings.SceneToEnterIn;
            scene.playerLocation = scene.NoDoorStartPos;
            scene.playerRotation = scene.NoDoorStartRot;
            sceneController.RaiseEventChangeScene(scene);
            yield break;
        }
        public override void Awake() {
            base.Awake();
        }

        
    }
}
