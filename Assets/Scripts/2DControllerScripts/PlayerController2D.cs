using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInputHandler2D))] // Memastikan input handler ada
public class PlayerController2D : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float acceleration = 7f;
    [SerializeField] private float deceleration = 7f;
    [SerializeField] private float velPower = 0.9f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 14f;
    [SerializeField] private float jumpCutMultiplier = 0.5f; // Pengurangan gaya saat tombol dilepas
    [SerializeField] private float coyoteTime = 0.1f; // Waktu toleransi jatuh
    [SerializeField] private float jumpBufferTime = 0.1f; // Waktu toleransi tekan tombol sebelum mendarat
    [SerializeField] private float gravityScale = 3f;
    [SerializeField] private float fallGravityMultiplier = 1.5f;

    [Header("Checks")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.2f);
    [SerializeField] private LayerMask groundLayer;

    // References
    private Rigidbody2D rb;
    private PlayerInputHandler2D input;

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
    }

    private void Update()
    {
        // Update Logic & Timers di sini (Frame dependent)
        UpdateTimers();
        CheckInputLogic();
        UpdateGravityScale();
    }

    private void FixedUpdate()
    {
        // Update Physics di sini (Fixed time step)
        CheckGround();
        Move();
        
        // Handle Jump Execution in Physics step for consistency
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            Jump();
        }
    }

    #region Movement Logic

    private void Move()
    {
        // 1. Hitung target kecepatan
        float targetSpeed = input.move.x * moveSpeed;

        // 2. Hitung selisih kecepatan (force yang dibutuhkan)
        float speedDif = targetSpeed - rb.linearVelocity.x; // Unity 6 pakai linearVelocity, Unity lama pakai velocity

        // 3. Tentukan rate akselerasi vs deselerasi
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;

        // 4. Aplikasikan movement force
        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);

        rb.AddForce(movement * Vector2.right);

        // 5. Flip Visual
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
        // Coyote Time
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Jump Buffer
        if (input.jump) // Input 'jump' dari handler bernilai true saat ditekan
        {
            // Karena Input System event-based, kita butuh trigger manual
            // Logika ini asumsi input.jump jadi true sesaat (bisa dimodifikasi di handler)
            // Atau kita bisa pakai event based langsung.
            // Untuk simplifikasi dengan handler yang ada, kita cek state:
             
             // NOTE: Idealnya Jump Buffer di-set saat event 'started', tapi di sini kita pakai state check
             // Kita perlu mereset input jump di handler atau mendeteksi 'press' baru.
             // Agar simpel, mari kita anggap handler memberikan state "IsPressed".
             
             // Hack kecil agar buffer jalan dengan state bool:
             jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
    }
    
    // Modifikasi kecil: Kita butuh trigger "OnJump" dari handler sebenarnya lebih baik daripada bool state.
    // Tapi karena kamu pakai bool di handler, kita adaptasi:
    private void CheckInputLogic()
    {
        // Cut Jump (Variable Height)
        // Jika tombol dilepas saat sedang naik, potong kecepatan vertikal
        if (!input.jump && rb.linearVelocity.y > 0)
        {
             rb.AddForce(Vector2.down * rb.linearVelocity.y * (1 - jumpCutMultiplier), ForceMode2D.Impulse);
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // Reset y velocity agar lompatan konsisten
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        jumpBufferCounter = 0; // Reset buffer
        coyoteTimeCounter = 0; // Reset coyote (agar tidak bisa double jump di udara)
    }

    private void UpdateGravityScale()
    {
        // Membuat jatuh terasa lebih "berat" dan cepat dibanding saat naik
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
}