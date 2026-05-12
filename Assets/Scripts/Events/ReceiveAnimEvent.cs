using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceiveAnimEvent : MonoBehaviour
{
    public PCMovement pcMove;
    public Camera MainCamera;

    public GameObject ventMine;
    Animation mineAnim;

    public GameObject ventAnimTrigger;

    public GameObject ventCamera;
    private void Start()
    {
        mineAnim = ventMine.GetComponent<Animation>();
    }


    public void StartMineMove()
    {
        mineAnim.Play();
        ventMine.GetComponent<AudioSource>().Play();
    }

    public void EndMineMove()
    {
        ventCamera.GetComponent<Camera>().enabled = false;
        MainCamera.GetComponent<Camera>().enabled = true;

        pcMove.frozen = false;

        if (ventAnimTrigger) Destroy(ventAnimTrigger);
        Destroy(ventCamera);
        Destroy(ventMine);
    }
}
