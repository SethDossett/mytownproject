using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using MyTownProject.Events;
using MyTownProject.UI;
using MyTownProject.SO;
using System.Collections;
using KinematicCharacterController.Examples;

namespace MyTownProject.Core
{
    public enum CurrentScene{
        CityScene, Hospital, TerrainConept, TestScene
    }
    public class SceneController: MonoBehaviour
    {
        public CurrentScene GamesCurrentScene;
        [SerializeField] MainEventChannelSO mainEventChannel;
        [SerializeField] UIEventChannelSO uIEventChannel;
        [SerializeField] StateChangerEventSO stateChangerEvent;
        [SerializeField] ActionSO teleportPlayer;


        private void OnEnable()
        {   SceneManager.activeSceneChanged += SceneChanged;
            mainEventChannel.OnChangeScene += SwitchScene;
        }
        private void OnDisable()
        {   
            SceneManager.activeSceneChanged -= SceneChanged;
            mainEventChannel.OnChangeScene -= SwitchScene;
        }

        private void Start()
        {
            
        }
        void SwitchScene(SceneSO sceneSO)
        {
            teleportPlayer.TeleportObject(sceneSO.playerLocation, sceneSO.playerRotation);

            var progress = SceneManager.LoadSceneAsync(sceneSO.sceneIndex, LoadSceneMode.Single);
            
            progress.completed += op => StartCoroutine(EnterScene());

        }

        IEnumerator EnterScene()
        {
            yield return new WaitForSecondsRealtime(1f);
            uIEventChannel.RaiseFadeIn(Color.black, 1f);
            yield return new WaitForSecondsRealtime(0.25f);
            uIEventChannel.RaiseBarsOff(2f);
            yield return new WaitForSecondsRealtime(0.5f);
            stateChangerEvent.RaiseEventGame(GameState.GAME_PLAYING);
            yield break;
        }
        
        void SceneChanged(Scene previous, Scene current){
            GamesCurrentScene = (CurrentScene)current.buildIndex;
        }
    }
}