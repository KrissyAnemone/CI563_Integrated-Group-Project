using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceiveAnimEvent : MonoBehaviour
{
    public PCMovement pcMove;
    public Camera MainCamera;

    public GameObject ventMine;
    Animation mineAnim;

    public GameObject ventCamera;
    private void Start()
    {
        mineAnim = ventMine.GetComponent<Animation>();
    }


    public void StartMineMove()
    {
        mineAnim.Play();
    }

    public void EndMineMove()
    {
        ventCamera.GetComponent<Camera>().enabled = false;
        MainCamera.GetComponent<Camera>().enabled = true;

        pcMove.frozen = false;

        Destroy(ventCamera);
        Destroy(ventMine);
    }
}
