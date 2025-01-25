using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] private SceneController sceneController;

    public void StartGame()
    {
        // SceneManager.LoadScene("Game");
        sceneController.LoadScene("Game");
    }

    public void BackToMenu()
    {
        // SceneManager.LoadScene("StartMenu");
        sceneController.LoadScene("StartMenu");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}