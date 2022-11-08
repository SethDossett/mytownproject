using UnityEngine;
using MyTownProject.Events;
using MyTownProject.SO;
using System.Collections;

namespace MyTownProject.Core
{
    public enum ControllerType
    {
        KeyBoard, GamePad
    }
    public class GameManager : MonoBehaviour
    {
        [field: SerializeField] public ControllerType CurrentControllerType { get; set; }

        [SerializeField] GameSettingsSO settings;
        [SerializeField] GeneralEventSO StartOfGame;
        [SerializeField] bool setFrameRate;
        [SerializeField] int targetFrameRate;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        static void OnBeforeSplashScreen()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        private void Awake()
        {
            if (GameObject.Find("New Game Manager")) Destroy(gameObject);
            if (setFrameRate)
                Application.targetFrameRate = targetFrameRate;
            else
                Application.targetFrameRate = -1;
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            gameObject.name = "New Game Manager";
            StartCoroutine(EnterScene());
        }

        IEnumerator EnterScene()
        {
            if (settings.StartOfGame)
            {
                settings.StartOfGame = false;
                StartOfGame.RaiseEvent();
                print("START OF GAME");
            }
            // if has cutscene play,
            // after return to play mode
            yield break;

        }

    }
}