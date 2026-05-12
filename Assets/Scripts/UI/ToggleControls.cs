using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleControls : MonoBehaviour
{
    public GameObject gameControlUI;

    private void Start()
    {
        gameControlUI.SetActive(false);
    }

    public void EnableControls()
    {
        gameControlUI.SetActive(true);
    }

    public void DisableControls()
    {
        gameControlUI.SetActive(false);
    }
}
