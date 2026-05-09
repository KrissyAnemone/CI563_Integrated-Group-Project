using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCMovement : MonoBehaviour
{
    [Header("Player Movement")]
    public Transform orientation;
    public float moveSpeed = 5f;
    public float crouchSpeed = 2.5f;

    [Header("Crouching")]
    public Transform cameraHolder;
    public float standHeight = 2f;
    public float crouchHeight = .5f;
    public float crouchCameraY = 0.5f;
    public float crouchCameraOffset = 1.6f;
    public float cameraSmooth = 10f;

    public bool IsCrouching { get; private set; }

    [Header("Jumping")]
    public LayerMask groundLayer;
    public float jumpHeight = 2f;
    public float groundCheckDistance = 0.1f;

    [Header("Dead")]
    public float deadSpeed = 0;
    public bool isDead = false;

    private Rigidbody rb;
    private CapsuleCollider col;
    private float storeSpeed;
    private float standCenterY;
    private float crouchCenterY;
    private float defaultCameraY;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();

        storeSpeed = moveSpeed;

        // Store standing collider values
        standHeight = col.height;
        standCenterY = col.center.y;

        // Calculate crouch values
        crouchCenterY = standCenterY - (standHeight - crouchHeight) / 2f;
        defaultCameraY = cameraHolder.localPosition.y;
    }

    void FixedUpdate()
    {
        isGrounded = CheckGrounded();

        if (!isDead)
        {
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

    private bool CheckGrounded()
    {
        RaycastHit hit;

        float rayLength = (col.height / 2f) + groundCheckDistance;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayLength))
        {
            // Reject steep surfaces (walls)
            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);

            return slopeAngle < 45f;
        }

        return false;
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

        HandleCameraHeight();
    }

    void HandleCameraHeight()
    {
        float targetY = IsCrouching ? defaultCameraY - crouchCameraOffset : defaultCameraY;

        Vector3 pos = cameraHolder.localPosition;

        pos.y = Mathf.Lerp(pos.y, targetY, Time.fixedDeltaTime * cameraSmooth);

        cameraHolder.localPosition = pos;
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
        float radius = col.radius * 0.95f;

        Vector3 point1 = transform.position + Vector3.up * radius;
        Vector3 point2 = transform.position + Vector3.up * (standHeight - radius);

        Collider[] hits = Physics.OverlapCapsule(point1, point2, radius);

        foreach (Collider hit in hits)
        {
            if (hit != col)
                return false;
        }

        return true;
    }
}
