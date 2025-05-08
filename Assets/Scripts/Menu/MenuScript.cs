using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public static bool GameIsPaused;

    public void Play()
    {
        SceneManager.UnloadSceneAsync("Menu");
        SceneManager.LoadSceneAsync("SampleScene");
    }

    public void QuitGame()
    {
        Debug.Log("exit");
        Application.Quit();
    }
}
