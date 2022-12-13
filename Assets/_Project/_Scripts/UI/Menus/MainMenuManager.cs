using UnityEngine;
using System.Collections;
using MyTownProject.Core;
using MyTownProject.Events;
using MyTownProject.SO;
using UnityEngine.EventSystems;

namespace MyTownProject.UI
{
    public class MainMenuManager : PageMenuBase
    {
        [Header("Main Menu Variables")]

        [SerializeField] GeneralEventSO _loadGameEvent;
        [SerializeField] CanvasGroup _canvasGroup;
        [SerializeField] SceneSO MainMenuScene;

        public override void Awake()
        {
            base.Awake();
            GameManager obj = GameObject.FindObjectOfType<GameManager>();
            if (obj) Destroy(obj.gameObject);

            //Hack Set Up just for Build, needs to be redone
            Cursor.visible = false;
        }
        public void Start()
        {
            _loadGameEvent.RaiseEvent();
        }

        public override void ChangeScene()
        {
            InputManager.DisableControls(InputManager.inputActions.UI); //Does This Actually work?
            EventSystem.current.currentInputModule.enabled = false;
            StartCoroutine(ChangeScenes());
        }

        IEnumerator ChangeScenes()
        {
            UIEvents.FadeTo(Color.black, 1f);
            yield return new WaitForSecondsRealtime(1f);

            //This is just for now, so that start of game is not coming out of door, 
            //need this to change if we want to start game coming out of last door
            GameSettings.StartOfGame = true;
            SceneSO scene = GameSettings.SceneToEnterIn;
            if (scene == null)
            {
                print("No Saved Scene, Using Default Scene");
                scene = DefaultGameSettings.SceneToEnterIn;
            }
            scene.EnteredThroughDoor = false;
            scene.playerLocation = scene.NoDoorStartPos;
            scene.playerRotation = scene.NoDoorStartRot;
            MainEventsChannel.RaiseEventChangeScene(scene);
            yield break;
        }

    }
}
