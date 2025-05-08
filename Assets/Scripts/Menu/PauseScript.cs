using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{
    public static bool GameIsPaused;
    public GameObject PauseMenuUI;
    
    //Объект игрока, чтобы во время паузы не реагировал на мышь
    public GameObject player;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        PauseMenuUI.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        //player.GetComponent<CameraMove>().enabled = true;
        player.SetActive(true);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Pause()
    {
        PauseMenuUI.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        //player.GetComponent<CameraMove>().enabled = false;
        player.SetActive(false);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void LoadMainMenu()
    {
        GameIsPaused = false;
        Time.timeScale = 1f;
        SceneManager.UnloadSceneAsync("SampleScene");
        SceneManager.LoadSceneAsync("Menu");
    }

    public void QuitGame()
    {
        Debug.Log("Exit");
        Application.Quit();
    }
}
