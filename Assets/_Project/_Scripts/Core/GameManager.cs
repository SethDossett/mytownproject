using UnityEngine;
using MyTownProject.UI;
using System.Collections;

namespace MyTownProject.Core
{
    public class GameManager : MonoBehaviour
    {
        private void Awake()
        {
            if(GameObject.Find("New Game Manager")) Destroy(gameObject);
                
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