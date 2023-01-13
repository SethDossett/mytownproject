using UnityEngine;
using MyTownProject.Core;
using MyTownProject.Events;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace MyTownProject.Utility{

    public class DebugController : MonoBehaviour{

        [SerializeField] DebugSettingsSO _debugSettings;
        [SerializeField] GeneralEventSO _generalDebugUpdate;
        bool _showConsole = false;
        bool _showHelp = false;
        string _input;
        Vector2 _scroll;

        #region DebugCommands
        public static DebugCommand HUD_TOGGLE;
        public static DebugCommand<int, int> SET_TIME;
        public static DebugCommand<int> SET_FRAMERATE;
        public static DebugCommand HELP;

        #endregion

        public List<object> CommandList;

        void Awake(){
            HUD_TOGGLE = new DebugCommand("hud_toggle", "Toggle In-Game UI Invisible.", "hud_toggle", () => {
                _debugSettings.HudToggle = !_debugSettings.HudToggle;
                _generalDebugUpdate.RaiseEvent();
            });

            SET_TIME = new DebugCommand<int, int>("set_time", "Change the in Game Time and Update Positions", "set_time <hour:minute>", (hour, minute) => {
                TimeManager.DateTime = new DateTime(TimeManager.DateTime.Date, hour, minute);
            });

            SET_FRAMERATE = new DebugCommand<int>("set_framerate", " [-1] = Off, [-2] Screen Refresh Rate", "set_framerate <goal_framerate>", (rate) => {
                if(rate < -2) return;
                else if(rate == -2) Application.targetFrameRate = Screen.currentResolution.refreshRate;
                else Application.targetFrameRate = rate; 
            });

            HELP = new DebugCommand("help", "Show Full List of Commands", "help", () => {
                _showHelp = true;
            });

            CommandList = new List<object>{
                HUD_TOGGLE,
                SET_TIME,
                SET_FRAMERATE,
                HELP
            };
        }

        void Update()
        {
            if (Keyboard.current.tabKey.wasPressedThisFrame)
            {
                OnToggleDebugMenu();
            }
            if (Keyboard.current.enterKey.wasPressedThisFrame)
            {
                if(!_showConsole) return;
                HandleInput();
                _input = "";
            }
            
        }

        void OnToggleDebugMenu(){
            _showConsole = !_showConsole;
            if(_showConsole) {
                InputManager.DisableControls(InputManager.inputActions.GamePlay);
                _showHelp = false;
            }
            else {
                InputManager.EnableControls(InputManager.inputActions.GamePlay);
            }
        }

        void OnGUI() {
            if(!_showConsole) return;

            float y = 0f;

            if(_showHelp){
                GUI.Box(new Rect(0, y, Screen.width, 100), "");
                Rect viewport = new Rect(0,0, Screen.width - 30, 20 * CommandList.Count);
                _scroll = GUI.BeginScrollView(new Rect(0, y + 5f, Screen.width, 90), _scroll, viewport);

                for (int i = 0; i < CommandList.Count; i++)
                {
                    DebugCommandBase command = CommandList[i] as DebugCommandBase;
                    string label = $"{command.CommandFormat} - {command.CommandDescription}";
                    Rect rectLabel = new Rect(5, 20 * i, viewport.width - 100, 20);
                    GUI.Label(rectLabel, label);
                }

                GUI.EndScrollView();
                y += 100;
            }

            GUI.Box(new Rect(0, y, Screen.width, 30), "");
            GUI.backgroundColor = new Color(0,0,0,0);

            _input = GUI.TextField(new Rect(10f, y + 5f, Screen.width - 20f, 20f), _input);
        }


        void HandleInput(){

            string[] properties = _input.Split(' ');

            for (int i = 0; i < CommandList.Count; i++)
            {
                DebugCommandBase commandBase = CommandList[i] as DebugCommandBase;

                if(_input.Contains(commandBase.CommandId)){
                    if(CommandList[i] as DebugCommand != null){
                        //Cast to this type, then invoke the command
                        (CommandList[i] as DebugCommand).Invoke();
                    }
                    else if(CommandList[i] as DebugCommand<int> != null){
                        (CommandList[i] as DebugCommand<int>).Invoke(int.Parse(properties[1]));
                    }
                    else if(CommandList[i] as DebugCommand<int, int> != null){
                        string[] intProperties = properties[1].Split(':');
                        (CommandList[i] as DebugCommand<int, int>).Invoke(int.Parse(intProperties[0]), int.Parse(intProperties[1]));
                    }

                } 
            }

        }

    }


}