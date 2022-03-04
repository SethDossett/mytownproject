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
    public class SceneController: MonoBehaviour
    {
        [SerializeField] MainEventChannelSO mainEventChannel;
        [SerializeField] UIEventChannelSO uIEventChannel;
        [SerializeField] StateChangerEventSO stateChangerEvent;

        [Header("MovePlayer")]
        [SerializeField] GameObject _player;
        TheCharacterController cc;
        public UnityAction<TheCharacterController> OnCharacterTeleport;
        public bool isBeingTeleportedTo { get; set; }

        private void OnEnable()
        {
            mainEventChannel.OnChangeScene += SwitchScene;
        }
        private void OnDisable()
        {
            mainEventChannel.OnChangeScene -= SwitchScene;
        }

        private void Start()
        {
            cc = _player.GetComponent<TheCharacterController>();
            
        }
        void SwitchScene(int index, SceneSO sceneSO)
        {
            MovePlayerToScene(sceneSO);


            var progress = SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);
            
            progress.completed += op => StartCoroutine(EnterScene());



        }

        private void MovePlayerToScene(SceneSO sceneSO)
        {
            if (!isBeingTeleportedTo)
            {
                if (cc)
                {
                    cc.Motor.SetPositionAndRotation(sceneSO.playerLocation, sceneSO.playerRotation);

                    if (OnCharacterTeleport != null)
                    {
                        OnCharacterTeleport(cc);
                    }
                    this.isBeingTeleportedTo = true;
                }
            }

            isBeingTeleportedTo = false;
        }

        IEnumerator EnterScene()
        {

            yield return new WaitForSecondsRealtime(1f);
            uIEventChannel.RaiseFadeIn(Color.black, 1f);
            yield return new WaitForSecondsRealtime(0.25f);
            uIEventChannel.RaiseBarsOff();
            yield return new WaitForSecondsRealtime(0.5f);
            stateChangerEvent.RaiseEvent(GameStateManager.GameState.GAME_PLAYING);
            yield break;
        }

    }
}