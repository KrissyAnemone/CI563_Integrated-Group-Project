using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCCamMovement : MonoBehaviour
{
    public Camera cam;
    public Transform ori;
    public float mouseSensitivity = 50f;

    private float xRotation = 0f;
    private float yRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Calculate mouse pos
        yRotation += mouseX;
        xRotation -= mouseY;

        // Clamping cam
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Rotate cam
        cam.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        ori.rotation = Quaternion.Euler(xRotation, yRotation, 0);
    }
}
