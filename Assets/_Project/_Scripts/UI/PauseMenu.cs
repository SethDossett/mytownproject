using UnityEngine;
using MyTownProject.Events;
using MyTownProject.Core;

namespace MyTownProject.UI
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] MainEventChannelSO MainEventChannelSO;
        [SerializeField] StateChangerEventSO StateChanger;
        [SerializeField] GameObject pauseMenu;

        private void OnEnable()
        {
            MainEventChannelSO.OnGamePaused += Pause;
            MainEventChannelSO.OnGameUnPaused += Resume;
        }
        private void OnDisable()
        {
            MainEventChannelSO.OnGamePaused -= Pause;
            MainEventChannelSO.OnGameUnPaused -= Resume;
        }
        private void Start()
        {
            //pauseMenu = GameObject.Find("PauseMenu");
        }
        private void Pause()
        {
            StateChanger.RaiseEventGame(GameStateManager.GameState.GAME_PAUSED);

            if (!pauseMenu.activeInHierarchy)
                pauseMenu.SetActive(true);

        }
        private void Resume()
        {
            StateChanger.RaiseEventGame(GameStateManager.GameState.GAME_PLAYING);

            if (pauseMenu.activeInHierarchy)
                pauseMenu.SetActive(false);
        }

        
    }
}