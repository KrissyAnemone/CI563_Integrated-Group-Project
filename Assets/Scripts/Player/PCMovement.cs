using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 moveDirection = Vector3.zero;

        if (InputManager.Instance.IsMovingUp())
            moveDirection += transform.forward;

        if (InputManager.Instance.IsMovingDown())
            moveDirection -= transform.forward;

        if (InputManager.Instance.IsMovingLeft())
            moveDirection -= transform.right;

        if (InputManager.Instance.IsMovingRight())
            moveDirection += transform.right;

        moveDirection.Normalize();

        Vector3 velocity = moveDirection * moveSpeed;
        velocity.y = rb.velocity.y;

        rb.velocity = velocity;
    }
}
