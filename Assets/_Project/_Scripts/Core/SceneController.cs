using UnityEngine.SceneManagement;
using UnityEngine;
using MyTownProject.Events;
using MyTownProject.SO;
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
            if(instance != null && instance != this)
                Destroy(gameObject);
            else
                instance = this;

            DontDestroyOnLoad(gameObject);
        }
        void SwitchScene(SceneSO sceneSO)
        {
            teleportPlayer.TeleportObject(sceneSO.playerLocation, sceneSO.playerRotation);

            var progress = SceneManager.LoadSceneAsync(sceneSO.sceneIndex, LoadSceneMode.Single);

            progress.completed += op => StartCoroutine(EnterScene());

        }

        IEnumerator EnterScene()
        {
            //Reset Scene Event, to recenter camera, slide in UI, etc.
            print("Completed Eneter Scene");
            stateChangerEvent.RaiseEventGame(GameState.CUTSCENE);
            Time.timeScale = 1;
            // If CutScene to be played Play now.
            yield return new WaitForSecondsRealtime(1f);
            Time.timeScale = 0;
            uIEventChannel.RaiseFadeIn(Color.black, 1f);
            yield return new WaitForSecondsRealtime(0.25f);
            uIEventChannel.RaiseBarsOff(2f);
            yield return new WaitForSecondsRealtime(0.5f);
            stateChangerEvent.RaiseEventGame(GameState.GAME_PLAYING);
            yield break;
        }

        void SceneChanged(Scene previous, Scene current)
        {
            GamesCurrentScene = (CurrentScene)current.buildIndex;
        }
    }
}