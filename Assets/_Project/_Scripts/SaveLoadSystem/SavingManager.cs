using UnityEngine.InputSystem;
using MyTownProject.Events;
using System.Collections;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;

namespace MyTownProject.SaveLoadSystem
{
    public class SavingManager : MonoBehaviour
    {
        [SerializeField] GeneralEventSO saveGameEvent;
        [SerializeField] GeneralEventSO loadGameEvent;
        [SerializeField] GeneralEventSO GameSaved;
        [SerializeField] GeneralEventSO GameLoaded;
        bool _isSaving;

        private void Awake()
        {
            saveGameEvent.OnRaiseEvent += StartSave;
            loadGameEvent.OnRaiseEvent += Load;
        }
        private void OnDestroy()
        {
            saveGameEvent.OnRaiseEvent -= StartSave;
            loadGameEvent.OnRaiseEvent -= Load;

        }
        void Update()
        {
            if (Keyboard.current.kKey.wasPressedThisFrame)
            {
                StartCoroutine(Save());
            }
            if (Keyboard.current.lKey.wasPressedThisFrame)
            {
                Load();

            }
        }
        public void StartSave()
        {
            if (_isSaving)
            {
                Debug.LogWarning("Trying To Call Save() Multiple Times");
                return;
            }
            StartCoroutine(Save());
        }

        IEnumerator Save()
        {
            _isSaving = true;
            Debug.Log("Save States");
            foreach (State_Save state in GameObject.FindObjectsOfType<State_Save>())
            {
                if (state.ShouldSave())
                {
                    yield return new WaitForEndOfFrame();

                    string json = state.SaveState();

                    var dir = Application.persistentDataPath + "/" + state.GetUID() + ".save";

                    yield return WriteFileAsync(dir, json);

                    yield return new WaitForEndOfFrame();

                    GUIUtility.systemCopyBuffer = dir;
                }
            }
            _isSaving = false;
            GameSaved.RaiseEvent();
        }

        public async Task WriteFileAsync(string path, string json)
        {
            using (StreamWriter outputFile = new StreamWriter(path))
            {
                await outputFile.WriteAsync(json);
            }
        }

        void Load()
        {
            Debug.Log("Load States");
            foreach (State_Save state in GameObject.FindObjectsOfType<State_Save>())
            {
                if (state.ShouldLoad())
                {
                    string expectedFileLocation = Application.persistentDataPath + "/" + state.GetUID() + ".save";

                    if (File.Exists(expectedFileLocation))
                    {
                        string json = File.ReadAllText(expectedFileLocation);
                        state.LoadState(json);
                    }
                    else
                    {
                        Debug.LogError($"{state.gameObject.name} save file does not exist!");
                        state.NoSaveFile();
                    }
                }
            }

            GameLoaded.RaiseEvent();
        }
    }
}