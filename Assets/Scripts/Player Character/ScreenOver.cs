using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenOver : MonoBehaviour
{
    public string sceneName;

    public void Retry()
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    public void Return()
    {
        // Assuming that the starting screen will always been scene 0
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
}
