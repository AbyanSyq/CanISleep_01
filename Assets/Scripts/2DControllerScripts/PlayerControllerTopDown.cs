using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInputHandler2D))]
public class PlayerControllerTopDown : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float acceleration = 50f; // Seberapa cepat mencapai max speed
    [SerializeField] private float deceleration = 30f; // Seberapa cepat berhenti (friction feel)

    [Header("Visual Settings")]
    [SerializeField] private bool rotateTowardsMove = false; // Jika true: karakter memutar badan (tank/shooter)
    [SerializeField] private bool flipSprite = true; // Jika true: karakter flip kiri/kanan (RPG klasik)

    // References
    private Rigidbody2D rb;
    private PlayerInputHandler2D input;
    private Vector2 currentVelocity; // Untuk caching velocity
    
    // State
    private Vector2 lastMoveDir;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInputHandler2D>();
    }

    private void FixedUpdate()
    {
        Move();
        HandleRotation();
    }

    private void Move()
    {
        // 1. Tentukan target speed berdasarkan input sprint
        float targetSpeed = input.sprint ? sprintSpeed : walkSpeed;
        
        // 2. Jika tidak ada input, target speed jadi 0
        // Input system biasanya sudah menormalisasi vector, tapi kita pastikan lagi saat diagonal
        Vector2 targetVector = input.move.normalized * targetSpeed;

        if (input.move == Vector2.zero)
        {
            targetVector = Vector2.zero;
        }

        // 3. Kalkulasi pergerakan fisik (Force-based Movement)
        // Kita menggunakan AddForce untuk mencapai target velocity agar ada 'bobot' gerakannya
        
        // Unity 6+ menggunakan 'linearVelocity', Unity lama menggunakan 'velocity'.
        // Gunakan rb.velocity jika error di versi lama.
        Vector2 velocityDifference = targetVector - rb.linearVelocity; 
        
        // Tentukan apakah kita sedang akselerasi atau deselerasi (berhenti)
        float accelRate = (input.move.magnitude > 0.01f) ? acceleration : deceleration;

        // Terapkan force
        Vector2 movementForce = velocityDifference * accelRate;
        
        rb.AddForce(movementForce);
    }

    private void HandleRotation()
    {
        if (input.move.magnitude < 0.01f) return;

        // Opsi 1: Flip Sprite (Cocok untuk game pixel art 2D / RPG klasik)
        if (flipSprite)
        {
            if (input.move.x > 0) transform.localScale = new Vector3(1, 1, 1);
            else if (input.move.x < 0) transform.localScale = new Vector3(-1, 1, 1);
        }

        // Opsi 2: Rotate Transform (Cocok untuk top-down shooter / mobil / pesawat)
        if (rotateTowardsMove)
        {
            float angle = Mathf.Atan2(input.move.y, input.move.x) * Mathf.Rad2Deg;
            // Gunakan rotasi smooth
            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 15f * Time.deltaTime);
        }
        
        lastMoveDir = input.move;
    }

    // Berguna untuk Animation Controller nanti
    public bool IsMoving()
    {
        return input.move.magnitude > 0.01f;
    }
    
    public Vector2 GetLastDirection()
    {
        return lastMoveDir;
    }
}