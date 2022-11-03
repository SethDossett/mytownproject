using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class MainMenuManager : MonoBehaviour
{
    int lastCurrentScene;
    [SerializeField] GameObject firstButton;

    private void Awake()
    {
        StartCoroutine(SetFirstSelection());
    }
    public void EnterGame()
    {
        lastCurrentScene = 0;

        //SceneController.SwitchScene(lastCurrentScene);
    }
    IEnumerator SetFirstSelection()
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(firstButton);
        yield break;
    }
}
