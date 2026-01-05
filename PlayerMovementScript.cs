using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 7f;
    public float acceleration = 70f;
    public float deceleration = 90f;
    [Range(0f, 1f)]
    public float airControlMultiplier = 0.25f;

    [Header("Jump")]
    public float jumpForce = 18f;
    public float maxJumpTime = 0.22f;
    public AnimationCurve jumpCurve;
    public float jumpReleaseMultiplier = 3f;
    public float fallMultiplier = 3f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.1f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isJumping;
    private float jumpTimeElapsed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Default jump curve if none is set
        if (jumpCurve == null || jumpCurve.length == 0)
        {
            jumpCurve = new AnimationCurve(
                new Keyframe(0f, 1f),
                new Keyframe(1f, 0f)
            );
        }
    }

    void Update()
    {
        CheckGround();
        HandleJumpInput();
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleGravity();
    }

    void HandleMovement()
    {
        float input = Input.GetAxisRaw("Horizontal");
        float targetSpeed = input * moveSpeed;
        float speedDiff = targetSpeed - rb.velocity.x;

        float accelRate = Mathf.Abs(targetSpeed) > 0.01f
            ? acceleration
            : deceleration;

        if (!isGrounded)
            accelRate *= airControlMultiplier;

        float movement = accelRate * speedDiff * Time.fixedDeltaTime;
        rb.velocity = new Vector2(rb.velocity.x + movement, rb.velocity.y);
    }

    void HandleJumpInput()
    {
        // Start jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            isJumping = true;
            jumpTimeElapsed = 0f;
            rb.velocity = new Vector2(rb.velocity.x, 0f);
        }

        // Continue jump with easing
        if (Input.GetKey(KeyCode.Space) && isJumping)
        {
            if (jumpTimeElapsed < maxJumpTime)
            {
                float t = jumpTimeElapsed / maxJumpTime;
                float curveValue = jumpCurve.Evaluate(t);

                rb.velocity = new Vector2(
                    rb.velocity.x,
                    jumpForce * curveValue
                );

                jumpTimeElapsed += Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        // Hard jump cut on release
        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;
            if (rb.velocity.y > 0f)
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.35f);
        }
    }

    void HandleGravity()
    {
        if (rb.velocity.y < 0f)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y *
                           (fallMultiplier - 1f) * Time.fixedDeltaTime;
        }
        else if (!Input.GetKey(KeyCode.Space) && rb.velocity.y > 0f)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y *
                           (jumpReleaseMultiplier - 1f) * Time.fixedDeltaTime;
        }
    }

    void CheckGround()
    {
        isGrounded = Physics2D.Raycast(
            groundCheck.position,
            Vector2.down,
            groundDistance,
            groundLayer
        );
    }

    void OnDrawGizmos()
    {
        if (!groundCheck) return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(
            groundCheck.position,
            groundCheck.position + Vector3.down * groundDistance
        );
    }
}
