using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger_ShowScanner : MonoBehaviour
{
    [SerializeField] GameObject MapContent;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Yup, its the player.");
            MapContent.SetActive(true);
            Destroy(this);
        }
    }
}
