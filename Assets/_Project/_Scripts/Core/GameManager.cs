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
        [SerializeField] ActionSO TurnOnTimeScaleZeroTick;
        [SerializeField] UIEventChannelSO UIEvents;
        [SerializeField] StateChangerEventSO StateChanger;

        KinematicCharacterSystem _kccSystem = null;
        InputAction action;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        static void OnBeforeSplashScreen()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        private void OnEnable()
        {
            TurnOnTimeScaleZeroTick.OnTimeScaleZeroTick += CheckTimeScale;
        }
        private void OnDisable()
        {
            TurnOnTimeScaleZeroTick.OnTimeScaleZeroTick -= CheckTimeScale;
        }
        private void Awake()
        {
            if (GameObject.Find("New Game Manager")) Destroy(gameObject);
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

                if (Screen.fullScreen)
                    Screen.fullScreen = false;
            }
        }
        void CheckTimeScale(float duration, bool loop)
        {
            if (!_kccSystem) _kccSystem = KinematicCharacterSystem.GetInstance();

            if (KinematicCharacterSystem.Settings.AutoSimulation)
            {
                KinematicCharacterSystem.Settings.AutoSimulation = false;
                _kccSystem.TimeScaleZeroTick();

                if (loop) return;
                else StartCoroutine(HandleTick(duration));
            }
            else
            {
                KinematicCharacterSystem.Settings.AutoSimulation = true;
            }
            print($"Auto Simulation {KinematicCharacterSystem.Settings.AutoSimulation}");
        }
        IEnumerator HandleTick(float duration)
        {
            yield return new WaitForSecondsRealtime(duration);
            KinematicCharacterSystem.Settings.AutoSimulation = true;
            print($"Auto Simulation {KinematicCharacterSystem.Settings.AutoSimulation}");
            yield break;
        }

        IEnumerator EnterScene()
        {
            if (settings.StartOfGame)
            {
                settings.StartOfGame = false;
                StartOfGame.RaiseEvent();
                print("START OF GAME");

                UIEvents.ShowExplaination(new Vector2(250, 50f), 5f, "Controls are in the Pause Menu, Press E or Start for Menu");
            }
            // if has cutscene play,
            // after return to play mode

            yield break;

        }

    }
}