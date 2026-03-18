using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform orientation;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Direction variables
        Vector3 moveDirection = Vector3.zero;
        Vector3 forward = orientation.forward;
        Vector3 right = orientation.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        // Input
        if (InputManager.Instance.IsMovingUp())
            moveDirection += forward;

        if (InputManager.Instance.IsMovingDown())
            moveDirection -= forward;

        if (InputManager.Instance.IsMovingLeft())
            moveDirection -= right;

        if (InputManager.Instance.IsMovingRight())
            moveDirection += right;

        moveDirection.Normalize();

        Vector3 velocity = moveDirection * moveSpeed;
        velocity.y = rb.velocity.y;

        rb.velocity = velocity;
    }
}
