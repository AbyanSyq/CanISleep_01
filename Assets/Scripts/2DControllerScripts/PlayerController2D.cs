using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInputHandler2D))]
[RequireComponent(typeof(Animator))] // Menambahkan requirement Animator
public class PlayerController2D : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float acceleration = 7f;
    [SerializeField] private float deceleration = 7f;
    [SerializeField] private float velPower = 0.9f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 14f;
    [SerializeField] private float jumpCutMultiplier = 0.5f;
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBufferTime = 0.1f;
    [SerializeField] private float gravityScale = 3f;
    [SerializeField] private float fallGravityMultiplier = 1.5f;

    [Header("Checks")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.2f);
    [SerializeField] private LayerMask groundLayer;

    [Header("Animation Settings")]
    [SerializeField] private string yVelocityFloatName = "yVelocity";
    [SerializeField] private string isGroundedBoolName = "isGrounded";
    [SerializeField] private string speedFloatName = "speed";

    [Header("Audio Settings")]
    [SerializeField] private PlayerAudioHandler playerAudioHandler;


    // References
    private Rigidbody2D rb;
    private PlayerInputHandler2D input;
    private Animator anim; // Referensi Animator

    // State Variables
    private bool isGrounded;
    private bool isFacingRight = true;

    // Timers
    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInputHandler2D>();
        anim = GetComponent<Animator>(); // Ambil komponen Animator
        playerAudioHandler = GetComponent<PlayerAudioHandler>();
    }

    private void Update()
    {
        UpdateTimers();
        CheckInputLogic();
        UpdateGravityScale();
        
        // Panggil fungsi animasi setiap frame
        UpdateAnimationState();
    }

    private void FixedUpdate()
    {
        CheckGround();
        Move();
        
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            Jump();
        }
    }

    #region Movement Logic

    private void Move()
    {
        float targetSpeed = input.move.x * moveSpeed;
        float speedDif = targetSpeed - rb.linearVelocity.x; 
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);

        rb.AddForce(movement * Vector2.right);

        if (input.move.x != 0)
        {
            CheckFlip(input.move.x > 0);
        }
    }

    private void CheckFlip(bool movingRight)
    {
        if (movingRight != isFacingRight)
        {
            isFacingRight = !isFacingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    #endregion

    #region Jump Logic

    private void UpdateTimers()
    {
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (input.jump) 
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
    }
    
    private void CheckInputLogic()
    {
        if (!input.jump && rb.linearVelocity.y > 0)
        {
             rb.AddForce(Vector2.down * rb.linearVelocity.y * (1 - jumpCutMultiplier), ForceMode2D.Impulse);
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        jumpBufferCounter = 0;
        coyoteTimeCounter = 0;

        // --- ANIMATION TRIGGER ---
        // Memicu animasi lompat seketika
        anim.SetTrigger("jump"); 
        playerAudioHandler.PlayJumpingSound();
    }

    private void UpdateGravityScale()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = gravityScale * fallGravityMultiplier;
        }
        else
        {
            rb.gravityScale = gravityScale;
        }
    }

    #endregion

    #region Physics Checks

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
        }
    }

    #endregion

    #region Animation Logic

    private void UpdateAnimationState()
    {
        // 1. Mengirim status Grounded
        // Digunakan untuk transisi dari Any State -> Land, atau Jump -> Fall -> Land
        anim.SetBool(isGroundedBoolName, isGrounded);

        // 2. Mengirim Kecepatan Horizontal (Speed)
        // Gunakan Mathf.Abs agar nilainya selalu positif (0 sampai max speed)
        // Ini untuk Blend Tree: Idle (0) -> Walk (misal 2) -> Run (misal 8)
        anim.SetFloat(speedFloatName, Mathf.Abs(rb.linearVelocity.x));

        // 3. Mengirim Kecepatan Vertikal (yVelocity)
        // Berguna untuk membedakan animasi 'Naik' (Jump Up) dan 'Turun' (Fall)
        anim.SetFloat(yVelocityFloatName, rb.linearVelocity.y);
    }

    #endregion
}