using UnityEngine.SceneManagement;
using UnityEngine;
using MyTownProject.Events;
using MyTownProject.SO;
using MyTownProject.Interaction;
using MyTownProject.Enviroment;
using System.Collections.Generic;
using System.Collections;

namespace MyTownProject.Core
{
    public enum CurrentScene
    {
        MainMenu, CityScene, Hospital, TerrainConept, TestScene
    }
    public class SceneController : MonoBehaviour
    {
        public static SceneController instance;
        public CurrentScene GamesCurrentScene;
        [SerializeField] MainEventChannelSO mainEventChannel;
        [SerializeField] UIEventChannelSO uIEventChannel;
        [SerializeField] StateChangerEventSO stateChangerEvent;
        [SerializeField] ActionSO teleportPlayer;
        [SerializeField] TransformEventSO EnteredNewScene;


        private void OnEnable()
        {
            SceneManager.activeSceneChanged += SceneChanged;
            mainEventChannel.OnChangeScene += SwitchScene;
        }
        private void OnDisable()
        {
            SceneManager.activeSceneChanged -= SceneChanged;
            mainEventChannel.OnChangeScene -= SwitchScene;
        }
        private void Awake()
        {
            if (instance != null && instance != this)
                Destroy(gameObject);
            else
                instance = this;

            DontDestroyOnLoad(gameObject);
        }
        void SwitchScene(SceneSO sceneSO)
        {
            //teleportPlayer.TeleportObject(sceneSO.playerLocation, sceneSO.playerRotation);

            var progress = SceneManager.LoadSceneAsync(sceneSO.sceneIndex, LoadSceneMode.Single);

            progress.completed += op => StartCoroutine(EnterScene(sceneSO));

        }

        IEnumerator EnterScene(SceneSO sceneSO)
        {
            if (sceneSO.EnteredThroughDoor)
            {
                GameObject obj = GameObject.Find("DoorsInScene");
                obj.GetComponent<DoorsInScene>().FindAllDoorsInScene();
                yield return new WaitForEndOfFrame();
                print("2nd");
                Transform exitedDoor = null;
                Door exitedDoorScript = null;
                foreach (GameObject door in sceneSO.DoorsInScene)
                {
                    Door script = door.GetComponent<Door>();
                    if (script.DoorIndex == sceneSO.DoorIndex)
                    {
                        exitedDoor = door.transform;
                        exitedDoorScript = script;
                        break;
                    }
                }

                if (exitedDoor == null) Debug.LogError("NO CORRESPONDING DOOR IN THIS SCENE");
                else EnteredNewScene.RaiseEvent(exitedDoor);
                if (exitedDoorScript != null)
                {
                    Vector3 offset = exitedDoorScript.InteractionPointOffset;
                    sceneSO.playerLocation = exitedDoor.TransformPoint(new Vector3(offset.x, 0, 0.3f));
                    sceneSO.playerRotation = exitedDoor.rotation;
                    teleportPlayer.TeleportObject(sceneSO.playerLocation, sceneSO.playerRotation);
                }
            }


            //Reset Scene Event, to recenter camera, slide in UI, etc.
            print("Completed Eneter Scene");

            //stateChangerEvent.RaiseEventGame(GameState.CUTSCENE);
            Time.timeScale = 1;
            // If CutScene to be played Play now.
            yield return new WaitForSecondsRealtime(1f);
            Time.timeScale = 0;
            uIEventChannel.RaiseFadeIn(Color.black, 1f);
            yield return new WaitForSecondsRealtime(0.25f);
            uIEventChannel.RaiseBarsOff(2f);
            yield return new WaitForSecondsRealtime(1f);
            //stateChangerEvent.RaiseEventGame(GameState.GAME_PLAYING);
            yield break;
        }

        void SceneChanged(Scene previous, Scene current)
        {
            GamesCurrentScene = (CurrentScene)current.buildIndex;
        }
    }
}