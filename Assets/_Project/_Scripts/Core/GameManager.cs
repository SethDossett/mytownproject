using UnityEngine;

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
        }

    }
}