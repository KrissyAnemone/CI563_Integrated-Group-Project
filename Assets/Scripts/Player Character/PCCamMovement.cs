using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCCamMovement : MonoBehaviour
{
    public Camera cam;
    public Transform ori;

    [Header("Mouse")]
    public float mouseSensitivity = 50f;

    [Header("Controller")]
    public float controllerSensitivity = 150f;
    public string controllerLookX = "RightStickX";
    public string controllerLookY = "RightStickY";

    public bool isDead = false;

    private float xRotation = 0f;
    private float yRotation = 0f;

    private bool scannerOpen = false;

    void Start()
    {
        LockCursor();
    }

    void Update()
    {
        if (isDead)
        {
            UnlockCursor();
            return;
        }

        HandleScannerToggle();

        if (scannerOpen)
            return; 

        HandleCameraLook();
    }

    void HandleScannerToggle()
    {
        if (InputManager.Instance.IsScannerDown())
        {
            scannerOpen = true;
            UnlockCursor();
        }

        if (InputManager.Instance.IsScannerUp())
        {
            scannerOpen = false;
            LockCursor();
        }
    }

    void HandleCameraLook()
    {
        float lookX = 0f;
        float lookY = 0f;

        float deadzone = 0.2f;

        if (InputManager.Instance.CurrentDevice == InputManager.InputDevice.Controller)
        {
            float rawX = Input.GetAxis(controllerLookX);
            float rawY = Input.GetAxis(controllerLookY);

            // Apply deadzone
            if (Mathf.Abs(rawX) < deadzone) rawX = 0f;
            if (Mathf.Abs(rawY) < deadzone) rawY = 0f;

            lookX = rawX * controllerSensitivity * Time.deltaTime;
            lookY = rawY * controllerSensitivity * Time.deltaTime;
        }
        else
        {
            lookX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            lookY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        }

        yRotation += lookX;
        xRotation -= lookY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cam.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);

        ori.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}