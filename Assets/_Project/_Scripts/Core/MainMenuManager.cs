using UnityEngine;
using MyTownProject.SaveLoadSystem;

public class MainMenuManager : MonoBehaviour
{
    int lastCurrentScene;

    public void EnterGame()
    {
        lastCurrentScene = 0;

        //SceneController.SwitchScene(lastCurrentScene);
    }
}
