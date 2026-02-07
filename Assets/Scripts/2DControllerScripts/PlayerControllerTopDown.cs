using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInputHandler2D))]
[RequireComponent(typeof(Animator))] // Menambahkan Animator
public class PlayerControllerTopDown : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float deceleration = 30f;

    [Header("Visual Settings")]
    [SerializeField] private bool rotateTowardsMove = false; 
    [SerializeField] private bool flipSprite = false; // Set false jika punya animasi 4 arah

    [Header("Animation Settings")]
    [SerializeField] private string isMovingBoolName = "IsMoving";
    [SerializeField] private string inputXFloatName = "InputX";
    [SerializeField] private string inputYFloatName = "InputY";
    [SerializeField] private string lastInputXFloatName = "LastInputX"; // Opsional
    [SerializeField] private string lastInputYFloatName = "LastInputY"; // Opsional
    [SerializeField] private string speedFloatName = "Speed";

    // References
    private Rigidbody2D rb;
    private PlayerInputHandler2D input;
    private Animator anim; // Referensi Animator

    // State
    private Vector2 lastMoveDir;
    private bool isMoving;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInputHandler2D>();
        anim = GetComponent<Animator>(); // Ambil komponen Animator
    }

    private void FixedUpdate()
    {
        Move();
        HandleRotation();
        
        // Update animasi di sini atau di Update()
        UpdateAnimationParams();
    }

    private void Move()
    {
        float targetSpeed = input.sprint ? sprintSpeed : walkSpeed;
        
        // Normalisasi input agar gerak diagonal tidak lebih cepat
        Vector2 targetVector = (input.move.magnitude > 0) ? input.move.normalized * targetSpeed : Vector2.zero;

        // Cek apakah ada input gerakan yang signifikan
        isMoving = input.move.magnitude > 0.01f;

        Vector2 velocityDifference = targetVector - rb.linearVelocity; 
        float accelRate = isMoving ? acceleration : deceleration;
        Vector2 movementForce = velocityDifference * accelRate;
        
        rb.AddForce(movementForce);
    }

    private void HandleRotation()
    {
        if (!isMoving) return;

        // Simpan arah terakhir untuk animasi Idle
        lastMoveDir = input.move.normalized;

        if (flipSprite)
        {
            if (input.move.x > 0) transform.localScale = new Vector3(1, 1, 1);
            else if (input.move.x < 0) transform.localScale = new Vector3(-1, 1, 1);
        }

        if (rotateTowardsMove)
        {
            float angle = Mathf.Atan2(input.move.y, input.move.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 15f * Time.deltaTime);
        }
    }

    private void UpdateAnimationParams()
    {
        // 1. Kirim parameter apakah sedang bergerak
        // Kita gunakan magnitude input untuk responsivitas instan
        anim.SetBool(isMovingBoolName, isMoving);

        // 2. Kirim parameter Input X dan Y untuk Blend Tree (Walk/Run)
        // Ini menentukan animasi jalan mana yang diputar (Atas/Bawah/Kiri/Kanan)
        if (isMoving)
        {
            anim.SetFloat(inputXFloatName, input.move.x);
            anim.SetFloat(inputYFloatName, input.move.y);
            
            // Simpan parameter arah terakhir agar saat berhenti, animasi Idle menghadap arah yang benar
            anim.SetFloat(lastInputXFloatName, input.move.x);
            anim.SetFloat(lastInputYFloatName, input.move.y);
        }
        else
        {
            // Opsi: Jika ingin animasi idle otomatis menghadap arah terakhir tanpa logic tambahan di Animator,
            // parameter LastInputX/Y di atas sudah cukup.
        }
        
        // 3. (Opsional) Kirim kecepatan fisik nyata untuk transisi Walk ke Run jika pakai Blend Tree 1D
        anim.SetFloat(speedFloatName, rb.linearVelocity.magnitude);
    }
}