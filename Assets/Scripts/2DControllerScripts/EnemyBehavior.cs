using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    public enum EnemyState { Idle, Patrol, Chase, Attack }

    [Header("State Debug")]
    [SerializeField] private EnemyState currentState = EnemyState.Idle;

    [Header("Movement Settings")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 3.5f; // Lebih cepat saat mengejar
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float waitTimeAtPoint = 2f;

    [Header("Combat Settings")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float attackRange = 1.2f; // Jarak stop untuk menyerang
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius = 0.5f;
    [SerializeField] private float attackCooldown = 1.5f; // Menggantikan Coroutine cooldown

    private Transform currentPatrolTarget;
    private Transform playerTarget; // Menyimpan referensi player yang terdeteksi
    private Rigidbody2D rb;
    private Animator anim;
    
    private bool isWaiting = false;
    private float lastAttackTime = 0f; // Timer untuk cooldown

    // Parameter Animasi
    private readonly string ANIM_SPEED = "speed";
    private readonly string ANIM_ATTACK_TRIGGER = "attack";
    private readonly string ANIM_IS_IDLE = "isIdle";

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentPatrolTarget = pointA;
        // Opsional: Lepaskan parent point A/B agar ikut bergerak jika enemy child dari objek lain
        if(pointA != null) pointA.parent = null;
        if(pointB != null) pointB.parent = null;

        currentState = EnemyState.Patrol;
    }

    void Update()
    {
        CheckForPlayer();
        HandleStateMachine();
    }

    private void CheckForPlayer()
    {
        // Deteksi Player
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);

        if (playerCollider != null)
        {
            playerTarget = playerCollider.transform;
            float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);

            if (distanceToPlayer <= attackRange)
            {
                currentState = EnemyState.Attack;
            }
            else
            {
                // Jika terdeteksi tapi diluar jarak serang -> KEJAR
                currentState = EnemyState.Chase;
            }
        }
        else
        {
            // Player hilang/kabur -> Kembali ke logika Patroli
            playerTarget = null;
            if (currentState != EnemyState.Patrol && currentState != EnemyState.Idle)
            {
                // Reset ke patrol jika kehilangan jejak
                currentState = EnemyState.Patrol;
            }
        }
    }

    private void HandleStateMachine()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                StopMovement();
                // TAMBAHAN: Jika tidak sedang nunggu (isWaiting false), paksa jalan patroli
                if (!isWaiting) 
                {
                    currentState = EnemyState.Patrol;
                }
                break;
            case EnemyState.Patrol:
                PatrolLogic();
                break;

            case EnemyState.Chase:
                ChaseLogic();
                break;

            case EnemyState.Attack:
                AttackLogic();
                break;
        }
    }

    // --- LOGIKA MOVEMENT & ACTION ---

    private void PatrolLogic()
    {
        if (isWaiting || currentPatrolTarget == null) return;

        MoveTo(currentPatrolTarget.position, patrolSpeed);

        // FIX 3: Perkecil jarak toleransi (sebelumnya 2.5f terlalu jauh)
        if (Vector2.Distance(transform.position, currentPatrolTarget.position) < 1f)
        {
            StartCoroutine(WaitAtPatrolPoint());
        }
    }

    private void ChaseLogic()
    {
        if (playerTarget == null) return;

        // Bergerak menuju posisi pemain dengan kecepatan lari (Chase Speed)
        MoveTo(playerTarget.position, chaseSpeed);
    }

    private void AttackLogic()
    {
        StopMovement(); // Berhenti bergerak saat menyerang

        if (playerTarget != null)
        {
            LookAtTarget(playerTarget.position); // Pastikan menghadap pemain
        }

        // Cek Cooldown sebelum menyerang lagi
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            anim.SetTrigger(ANIM_ATTACK_TRIGGER);
        }
    }

    // --- HELPER FUNCTIONS ---

    private void MoveTo(Vector2 targetPos, float speed)
    {
        Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * speed, rb.linearVelocity.y);

        // Update Animasi Lari/Jalan
        anim.SetFloat(ANIM_SPEED, Mathf.Abs(rb.linearVelocity.x));
        anim.SetBool(ANIM_IS_IDLE, false);

        LookAtTarget(targetPos);
    }

    private void LookAtTarget(Vector2 targetPos)
    {
        // Logic Flip sederhana
        if (transform.position.x < targetPos.x)
            transform.localScale = new Vector3(-1, 1, 1); // Hadap Kanan
        else if (transform.position.x > targetPos.x)
            transform.localScale = new Vector3(1, 1, 1); // Hadap Kiri
    }

    private void StopMovement()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        anim.SetFloat(ANIM_SPEED, 0);
        anim.SetBool(ANIM_IS_IDLE, true);
    }

    private IEnumerator WaitAtPatrolPoint()
    {
        isWaiting = true;
        StopMovement();
        
        // Pakai state Idle agar animasi berubah ke Idle
        EnemyState previousState = currentState;
        currentState = EnemyState.Idle; 

        yield return new WaitForSeconds(waitTimeAtPoint);

        // Switch target patroli
        currentPatrolTarget = (currentPatrolTarget == pointA) ? pointB : pointA;
        
        currentState = EnemyState.Patrol; // Paksa kembali ke patrol
        isWaiting = false;
    }

    // --- ANIMATION EVENTS ---

    // Dipanggil oleh Animation Event
    public void ApplyDamage()
    {
        // Menggunakan OverlapCircleAll agar bisa kena multi-target (misal player dan NPC teman)
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, playerLayer);

        foreach (Collider2D obj in hitObjects)
        {
            IDamageable damageable = obj.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damageAmount, attackPoint.position);
            }
        }
    }

    // --- DEBUG GIZMOS ---
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange); // Area Deteksi
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange); // Area Mulai Attack

        if (attackPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius); // Area Damage Hit
        }
    }
}