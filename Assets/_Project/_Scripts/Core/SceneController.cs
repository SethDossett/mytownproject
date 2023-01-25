using UnityEngine.SceneManagement;
using UnityEngine;
using MyTownProject.Events;
using MyTownProject.SO;
using MyTownProject.Interaction;
using MyTownProject.Enviroment;
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
        public static CurrentScene GamesCurrentScene { get; private set; }
        public static SceneSO CurrentSceneSO { get; private set; }
        [SerializeField] MainEventChannelSO mainEventChannel;
        [SerializeField] UIEventChannelSO uIEventChannel;
        [SerializeField] StateChangerEventSO stateChangerEvent;
        [SerializeField] ActionSO teleportPlayer;
        [SerializeField] TransformEventSO EnteredNewScene;
        [SerializeField] ActionSO ToggleTimeScaleZeroTick;
        [SerializeField] GameSettingsSO gameSettings;
        [SerializeField] MyTownProject.Utility.DebugSettingsSO debugSettings;

        Coroutine _walkCoroutine;

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
            {
                Destroy(gameObject);
                return;
            }
            else
                instance = this;

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (debugSettings.EnterIndividualScene)
            {
                StartCoroutine(EnterScene(debugSettings.SceneToEnter));
                debugSettings.EnterIndividualScene = false;
            }
        }
        void SwitchScene(SceneSO sceneSO)
        {
            debugSettings.EnterIndividualScene = false;
            //teleportPlayer.TeleportObject(sceneSO.playerLocation, sceneSO.playerRotation);

            var progress = SceneManager.LoadSceneAsync(sceneSO.sceneIndex, LoadSceneMode.Single);

            progress.completed += op => StartCoroutine(EnterScene(sceneSO));

        }

        IEnumerator EnterScene(SceneSO sceneSO)
        {
            uIEventChannel.FadeFrom(Color.black, 0);
            CurrentSceneSO = sceneSO;
            stateChangerEvent.RaiseEventGame(GameState.CUTSCENE);
            bool enteredThroughDoor = sceneSO.EnteredThroughDoor;
            if (enteredThroughDoor)
            {
                //GameObject obj = GameObject.Find("DoorsInScene");
                GameObject.FindObjectOfType<DoorsInScene>().FindAllDoorsInScene();

                //obj.GetComponent<DoorsInScene>().FindAllDoorsInScene();
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
                }
            }
            else
            {
                sceneSO.playerLocation = sceneSO.NoDoorStartPos;
                sceneSO.playerRotation = sceneSO.NoDoorStartRot;
            }

            if (debugSettings.UseDebugSpawnPosition && !enteredThroughDoor)
            {
                Transform spawnT = GameObject.Find("DebugSpawnPoint").transform;
                if (spawnT) teleportPlayer.TeleportObject(spawnT.position, spawnT.rotation);
                else teleportPlayer.TeleportObject(sceneSO.playerLocation, sceneSO.playerRotation);
            }
            else teleportPlayer.TeleportObject(sceneSO.playerLocation, sceneSO.playerRotation);

            //Reset Scene Event, to recenter camera, slide in UI, etc.
            print("Completed Eneter Scene");

            // If CutScene to be played Play now.
            yield return new WaitForSecondsRealtime(1f);
            uIEventChannel.FadeFrom(Color.black, 1f);
            if (enteredThroughDoor) ToggleTimeScaleZeroTick.TimeScaleZeroTick(0, true);
            //_walkCoroutine = StartCoroutine(WallkIntoPosition());
            yield return new WaitForSecondsRealtime(0.25f);
            uIEventChannel.RaiseBarsOff(2f);
            yield return new WaitForSecondsRealtime(1f);
            //StopCoroutine(_walkCoroutine);
            if (enteredThroughDoor) ToggleTimeScaleZeroTick.TimeScaleZeroTick(0, false);
            stateChangerEvent.RaiseEventGame(GameState.GAME_PLAYING);

            string sceneName = sceneSO.name;
            print(sceneName);
            if (sceneName != "TestScene" && sceneName != "MainMenu")
                uIEventChannel.ShowExplaination(new Vector2(250, 50f), 5f, "This Scene is Empty");
            yield break;
        }

        void EnterThroughDoor(SceneSO sceneSO)
        {

        }
        IEnumerator WallkIntoPosition()
        {
            while (true)
            {
                //print("Walking into Position");

                yield return null;
            }
        }

        void SceneChanged(Scene previous, Scene current)
        {
            GamesCurrentScene = (CurrentScene)current.buildIndex;
        }
    }
}