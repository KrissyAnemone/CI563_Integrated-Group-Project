using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public enum InputDevice
    {
        KeyboardMouse,
        Controller
    }

    public InputDevice CurrentDevice { get; private set; } = InputDevice.KeyboardMouse;

    [Header("User Binding")]
    public KeyCode forward = KeyCode.W;
    public KeyCode backward = KeyCode.S;
    public KeyCode left = KeyCode.A;
    public KeyCode right = KeyCode.D;

    public KeyCode jump = KeyCode.Space;
    public KeyCode crouch = KeyCode.LeftControl;
    public KeyCode scanner = KeyCode.Tab;

    [Header("Controller Axes")]
    public string horizontalAxis = "Horizontal";
    public string verticalAxis = "Vertical";
    public string jumpButton = "Jump";
    public string crouchButton = "Crouch";
    public string scannerButton = "Scanner";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        DetectInputDevice();

        //Debug.Log(CurrentDevice);
    }

    void DetectInputDevice()
    {
        float deadzone = 0.2f;

        float h = Input.GetAxis(horizontalAxis);
        float v = Input.GetAxis(verticalAxis);

        // Controller detection 
        if (Mathf.Abs(h) > deadzone || Mathf.Abs(v) > deadzone)
        {
            CurrentDevice = InputDevice.Controller;
            return;
        }

        // Detect joystick buttons
        for (int i = 0; i < 20; i++)
        {
            if (Input.GetKey("joystick button " + i))
            {
                CurrentDevice = InputDevice.Controller;
                return;
            }
        }

        // Keyboard detection
        if (Input.GetKey(forward) || Input.GetKey(backward) || Input.GetKey(left) || Input.GetKey(right) || Input.GetKey(jump) || Input.GetKey(crouch) || Input.GetKey(scanner))
            CurrentDevice = InputDevice.KeyboardMouse;
    }

    // Movement
    public bool IsMovingUp()
    {
        if (CurrentDevice == InputDevice.Controller)
            return Input.GetAxis(verticalAxis) > 0.1f;

        return Input.GetKey(forward);
    }

    public bool IsMovingDown()
    {
        if (CurrentDevice == InputDevice.Controller)
            return Input.GetAxis(verticalAxis) < -0.1f;

        return Input.GetKey(backward);
    }

    public bool IsMovingLeft()
    {
        if (CurrentDevice == InputDevice.Controller)
            return Input.GetAxis(horizontalAxis) < -0.1f;

        return Input.GetKey(left);
    }

    public bool IsMovingRight()
    {
        if (CurrentDevice == InputDevice.Controller)
            return Input.GetAxis(horizontalAxis) > 0.1f;

        return Input.GetKey(right);
    }

    // Jump
    public bool IsJumping()
    {
        if (CurrentDevice == InputDevice.Controller)
            return Input.GetButton(jumpButton);

        return Input.GetKey(jump);
    }

    // Crouch
    public bool IsCrouching()
    {
        if (CurrentDevice == InputDevice.Controller)
            return Input.GetButton(crouchButton);

        return Input.GetKey(crouch);
    }

    // Scanner
    public bool IsScannerDown()
    {
        if (CurrentDevice == InputDevice.Controller)
            return Input.GetButtonDown(scannerButton);

        return Input.GetKeyDown(scanner);
    }

    public bool IsScannerUp()
    {
        if (CurrentDevice == InputDevice.Controller)
            return Input.GetButtonUp(scannerButton);

        return Input.GetKeyUp(scanner);
    }
}