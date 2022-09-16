using UnityEngine;
using System.Collections;
using MyTownProject.Events;
using MyTownProject.Core;
using UnityEngine.EventSystems;

namespace MyTownProject.UI
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] MainEventChannelSO MainEventChannelSO;
        [SerializeField] StateChangerEventSO StateChanger;
        [SerializeField] GameObject pauseMenu;
        [SerializeField] GameObject firstButton;

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
            StartCoroutine(SetFirstSelection());
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

        IEnumerator SetFirstSelection()
        {
            EventSystem.current.SetSelectedGameObject(null);
            yield return new WaitForEndOfFrame();
            EventSystem.current.SetSelectedGameObject(firstButton);
            yield break;
        }
    }
}