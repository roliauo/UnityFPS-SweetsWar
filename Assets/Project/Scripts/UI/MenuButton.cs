using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    public string sceneToLoad = "";

    private void Update()
    {
        
    }

    public void LoadTargetScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    public void ShowOptionsMenu()
    {

    }

    public void QuitGame()
    {
        Debug.Log("quit");
        Application.Quit();
    }

}
