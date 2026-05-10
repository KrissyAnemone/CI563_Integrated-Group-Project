using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Trigger_VentAnimation : MonoBehaviour
{
    public PCMovement pcMove;
    public Camera MainCamera;

    public GameObject ventMine;
    Animation mineAnim;

    public GameObject ventCamera;
    Animation cameraAnim;
    // Start is called before the first frame update
    void Start()
    {
        mineAnim = ventMine.GetComponent<Animation>();
        cameraAnim = ventCamera.GetComponent<Animation>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            StartAnimation();
        }
    }

    void StartAnimation()
    {
        MainCamera.enabled = false;
        ventCamera.GetComponent<Camera>().enabled = true;
        ventCamera.GetComponent<Light>().enabled = true;
        pcMove.frozen = true;
        cameraAnim.Play();
    }

}
