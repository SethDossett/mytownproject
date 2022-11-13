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
        bool _isSaving;

        private void OnEnable()
        {
            saveGameEvent.OnRaiseEvent += StartSave;
            loadGameEvent.OnRaiseEvent += Load;
        }
        private void OnDisable()
        {
            saveGameEvent.OnRaiseEvent -= StartSave;
            loadGameEvent.OnRaiseEvent -= Load;

        }
        private void Awake()
        {
            Load();
            Debug.Log("Loaded States");
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
        void StartSave()
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

                    WriteFileAsync(Application.persistentDataPath + "/" + state.GetUID() + ".save", json);

                    yield return new WaitForEndOfFrame();
                }
            }
            _isSaving = false;
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
                }
            }
        }
    }
}