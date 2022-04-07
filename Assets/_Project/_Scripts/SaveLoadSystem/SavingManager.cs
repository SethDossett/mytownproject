using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;

namespace MyTownProject.SaveLoadSystem
{
    public class SavingManager : MonoBehaviour
    {
        private void Start()
        {
            Load();
            Debug.Log("Loaded States");
        }
        void Update()
        {
            if (Keyboard.current.kKey.wasPressedThisFrame)
            {
                StartCoroutine(Save());
                Debug.Log("Saved States");
            }
            if (Keyboard.current.lKey.wasPressedThisFrame)
            {
                Load();
                Debug.Log("Loaded States");
            }
        }

        IEnumerator Save()
        {
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