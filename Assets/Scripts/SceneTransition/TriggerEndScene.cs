using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerEndScene : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("collide");
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player");
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene("EndScene", LoadSceneMode.Single);
        }
    }
}