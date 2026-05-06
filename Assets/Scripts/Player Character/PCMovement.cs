using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCMovement : MonoBehaviour
{
    public GamepadCursor cursor;

    public Transform orientation;
    public float moveSpeed = 5f;
    public float crouchSpeed = 2.5f;
    public float standHeight = 2f;
    public float crouchHeight = 1f;

    public bool IsCrouching { get; private set; }

    public LayerMask groundLayer;
    public float jumpHeight = 2f;
    public float groundCheckDistance = 0.1f;

    public float deadSpeed = 0;
    public bool isDead = false;

    private Rigidbody rb;
    private CapsuleCollider col;
    private float storeSpeed;
    private float standCenterY;
    private float crouchCenterY;
    private bool isGrounded;

    void Start()
    {
        cursor.SetCursorActive(false);

        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();

        storeSpeed = moveSpeed;

        // Store standing collider values
        standHeight = col.height;
        standCenterY = col.center.y;

        // Calculate crouch values
        crouchCenterY = standCenterY - (standHeight - crouchHeight) / 2f;
    }

    void FixedUpdate()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, (col.height / 2f) + groundCheckDistance, groundLayer);

        if (!isDead)
        {
            moveSpeed = storeSpeed;

            // Check for jump
            if (InputManager.Instance.IsJumping() && isGrounded)
                Jump();

            // Handle crouch
            HandleCrouch();

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
        else if (isDead)
            moveSpeed = deadSpeed;
    }

    void Jump()
    {
        float jumpVelocity = Mathf.Sqrt(2f * Mathf.Abs(Physics.gravity.y) * jumpHeight);
        rb.velocity = new Vector3(rb.velocity.x, jumpVelocity, rb.velocity.z);
    }

    void HandleCrouch()
    {
        if (InputManager.Instance.IsCrouching())
        {
            if (!IsCrouching)
                EnterCrouch();
        }
        else
        {
            if (IsCrouching && CanStand())
                ExitCrouch();
        }
    }

    void EnterCrouch()
    {
        IsCrouching = true;

        moveSpeed = crouchSpeed;

        col.height = crouchHeight;
        col.center = new Vector3(col.center.x, crouchCenterY, col.center.z);
    }

    void ExitCrouch()
    {
        IsCrouching = false;

        moveSpeed = storeSpeed;

        col.height = standHeight;
        col.center = new Vector3(col.center.x, standCenterY, col.center.z);
    }

    bool CanStand()
    {
        return !Physics.Raycast(transform.position, Vector3.up, standHeight);
    }
}
