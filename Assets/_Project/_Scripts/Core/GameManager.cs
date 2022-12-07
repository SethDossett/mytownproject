using UnityEngine;
using MyTownProject.Events;
using MyTownProject.SO;
using UnityEngine.InputSystem;
using System.Collections;
using KinematicCharacterController;

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
        [SerializeField] GeneralEventSO TurnOnTimeScaleZeroTick;
        [SerializeField] StateChangerEventSO StateChanger;

        KinematicCharacterSystem _kccSystem;

        [SerializeField] bool setFrameRate;
        [SerializeField] int targetFrameRate;
        InputAction action;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        static void OnBeforeSplashScreen()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        private void OnEnable()
        {
            TurnOnTimeScaleZeroTick.OnRaiseEvent += CheckTimeScale;
        }
        private void OnDisable()
        {
            TurnOnTimeScaleZeroTick.OnRaiseEvent -= CheckTimeScale;
        }
        private void Awake()
        {
            if (GameObject.Find("New Game Manager")) Destroy(gameObject);

            CheckFrameRate();
        }
        void CheckFrameRate()
        {
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
        void Update()
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                if(Screen.fullScreen)
                    Screen.fullScreen = false;
            }
        }
        void CheckTimeScale()
        {
            _kccSystem = KinematicCharacterSystem.GetInstance();

            if (KinematicCharacterSystem.Settings.AutoSimulation)
            {
                KinematicCharacterSystem.Settings.AutoSimulation = false;
                _kccSystem.TimeScaleZeroTick();
            }
            else
            {
                KinematicCharacterSystem.Settings.AutoSimulation = true;
            }
            print($"Auto Simulation {KinematicCharacterSystem.Settings.AutoSimulation}");
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