using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float baseSpeed = 50f;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float maxSpeed = 100f;
    [SerializeField] private float horizontalSpeed = 15f;
    [SerializeField] private float laneWidth = 3f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private float groundDrag = 5f;
    [SerializeField] private float airDrag = 2f;

    [Header("Dash")]
    [SerializeField] private float dashForce = 30f;
    [SerializeField] private float dashDuration = 0.3f;
    [SerializeField] private float dashCooldown = 0.5f;

    [Header("Ground Check")]
    [SerializeField] private float groundDragDistance = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody rb;
    private float currentSpeed;
    private float currentHorizontalInput;
    private bool isGrounded;
    private bool isDashing;
    private float dashTimer;
    private float dashCooldownTimer;
    private Vector3 currentLanePosition;
    private int currentLane = 1; // 0 = left, 1 = center, 2 = right

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentSpeed = baseSpeed;
    }

    private void Update()
    {
        HandleInput();
        CheckGroundStatus();
        UpdateDash();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        ApplyHorizontalMovement();
    }

    private void HandleInput()
    {
        // Horizontal movement input
        currentHorizontalInput = Input.GetAxis("Horizontal");

        // Jump input
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        // Dash input
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTimer <= 0 && !isDashing)
        {
            StartDash();
        }

        // Quick lane switching with A/D
        if (Input.GetKeyDown(KeyCode.A) && currentLane > 0)
        {
            currentLane--;
        }
        if (Input.GetKeyDown(KeyCode.D) && currentLane < 2)
        {
            currentLane++;
        }
    }

    private void ApplyMovement()
    {
        // Accelerate forward
        currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.fixedDeltaTime, maxSpeed);

        // Apply forward velocity
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, currentSpeed);

        // Apply drag
        float dragForce = isGrounded ? groundDrag : airDrag;
        rb.drag = dragForce;
    }

    private void ApplyHorizontalMovement()
    {
        // Smooth horizontal movement
        float targetX = currentLane * laneWidth - laneWidth;
        float currentX = rb.position.x;
        float newX = Mathf.Lerp(currentX, targetX, Time.fixedDeltaTime * horizontalSpeed);

        rb.velocity = new Vector3((newX - currentX) / Time.fixedDeltaTime, rb.velocity.y, rb.velocity.z);
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
    }

    private void StartDash()
    {
        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;
        rb.AddForce(Vector3.forward * dashForce, ForceMode.Impulse);
    }

    private void UpdateDash()
    {
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0)
            {
                isDashing = false;
            }
        }

        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }
    }

    private void CheckGroundStatus()
    {
        RaycastHit hit;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, out hit, groundDragDistance, groundLayer);
    }

    public float GetCurrentSpeed() => currentSpeed;
    public bool IsGrounded() => isGrounded;
    public int GetCurrentLane() => currentLane;
}