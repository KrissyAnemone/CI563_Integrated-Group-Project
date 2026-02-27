using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public KeyCode forward = KeyCode.W;
    public KeyCode backward = KeyCode.S;
    public KeyCode left = KeyCode.A;
    public KeyCode right = KeyCode.D;

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

    // Movement
    public bool IsMovingUp() => Input.GetKey(forward);
    public bool IsMovingDown() => Input.GetKey(backward);
    public bool IsMovingLeft() => Input.GetKey(left);
    public bool IsMovingRight() => Input.GetKey(right);
}
