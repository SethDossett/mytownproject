using UnityEngine;
using MyTownProject.Events;
using MyTownProject.Core;

namespace MyTownProject.UI
{
    public class PauseMenu : MonoBehaviour
    {
        public MainEventChannelSO MainEventChannelSO;
        [SerializeField] private GameObject pauseMenu;

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
            GameManager.instance.UpdateState(GameManager.GameState.GAME_PAUSED);

            if (!pauseMenu.activeInHierarchy)
                pauseMenu.SetActive(true);

        }
        private void Resume()
        {
            GameManager.instance.UpdateState(GameManager.GameState.GAME_PLAYING);

            if (pauseMenu.activeInHierarchy)
                pauseMenu.SetActive(false);
        }
    }
}