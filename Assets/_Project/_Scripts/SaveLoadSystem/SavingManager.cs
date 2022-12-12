using UnityEngine.InputSystem;
using MyTownProject.Events;
using System.Collections;
using System.Collections.Generic;
using System;
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

        List<NPCTransformValues> _stateList = new List<NPCTransformValues>();

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

                    string path = null;

                    //var dir = Application.persistentDataPath + "/" + state.GetUID() + ".save";
                    //path = Application.persistentDataPath + "/" + state.SaveLocation + ".save";
                    path = Path.Combine(Application.persistentDataPath, state.GetUID()+ ".save");
#if UNITY_EDITOR
                    //var assetFolderPath = Application.dataPath + Path.AltDirectorySeparatorChar + "/SaveFolder" + state.SaveLocation + ".save";
                    //path = "Assets/Resources/SaveFolder/" + state.SaveLocation + ".save";
#endif

                    yield return WriteFileAsync(path, json);

                    yield return new WaitForEndOfFrame();

                    GUIUtility.systemCopyBuffer = path;
                    print($"Saved {state.gameObject.name} to Path: {path}");
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
                    string expectedFileLocation = null;
                    //string expectedFileLocation = Application.persistentDataPath + "/" + state.GetUID() + ".save";
                    //string expectedFileLocation = Application.persistentDataPath + "/" + state.SaveLocation + ".save";
                    //string expectedAssetFileLocation = Application.dataPath + Path.AltDirectorySeparatorChar + state.SaveLocation + ".save";
                    expectedFileLocation = Path.Combine(Application.persistentDataPath, state.GetUID()+ ".save");

#if UNITY_EDITOR
                    //var assetFolderPath = Application.dataPath + Path.AltDirectorySeparatorChar + "/SaveFolder" + state.SaveLocation + ".save";
                    //expectedFileLocation = "Assets/Resources/SaveFolder/" + state.SaveLocation + ".save";
#endif

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
        public static class JsonHelper
        {
            public static T[] FromJson<T>(string json)
            {
                Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
                return wrapper.Items;
            }

            public static string ToJson<T>(T[] array)
            {
                Wrapper<T> wrapper = new Wrapper<T>();
                wrapper.Items = array;
                return JsonUtility.ToJson(wrapper);
            }

            public static string ToJson<T>(T[] array, bool prettyPrint)
            {
                Wrapper<T> wrapper = new Wrapper<T>();
                wrapper.Items = array;
                return JsonUtility.ToJson(wrapper, prettyPrint);
            }

            [Serializable]
            private class Wrapper<T>
            {
                public T[] Items;
            }
        }
}