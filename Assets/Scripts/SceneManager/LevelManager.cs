using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public void LoadLevel(string sceneName)
    {
        Debug.Log("New Level load: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }
	

    public void quitGame()
    {
        Debug.Log("Exiting Game");
        Application.Quit();
    }
}
