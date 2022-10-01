using UnityEngine;
using MyTownProject.UI;
using System.Collections;

namespace MyTownProject.Core
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] bool setFrameRate;
        [SerializeField] int targetFrameRate;
        private void Awake()
        {
            if(GameObject.Find("New Game Manager")) Destroy(gameObject);
            if(setFrameRate)
                Application.targetFrameRate = targetFrameRate;
            else
                Application.targetFrameRate = -1;
        }

        void Start()
        {
            DontDestroyOnLoad(gameObject);
            gameObject.name = "New Game Manager";
            StartCoroutine(EnterScene());
        }

        IEnumerator EnterScene()
        {
            // if has cutscene play,
            // after return to play mode
            yield break;
            
        }

    }
}