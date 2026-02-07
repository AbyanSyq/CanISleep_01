using UnityEngine;
using System.Collections;

public enum EnemyState
{
    Idle,
    Patrol,
    Attack
}

public class EnemyBehavior : MonoBehaviour
{
    Rigidbody2D rb;

    public Collider2D checkRadius;

    public float stateCooldown = 3f;
    public float patrolSpeed = 2f;

    public float attackCooldown = 2f;

    bool attackOnCooldown = false;

    public bool isAttacking = false;
    bool isStateChangingCooldown = false;

    public LayerMask playerLayer;

    Animator animator;

    float patrolDirection = 1f; // 1 = kanan, -1 = kiri

    public EnemyState currentState = EnemyState.Idle;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // cooldown ganti state
        if (!isAttacking && !isStateChangingCooldown)
        {
            StartCoroutine(WaitForStateChangingCooldown(stateCooldown));
        }

        CheckPlayer();
        HandleState();
    }

    void CheckPlayer()
    {

    }

    public void SetState(EnemyState newState)
    {
        currentState = newState;

        // Random arah hanya saat masuk Patrol
        if (newState == EnemyState.Patrol)
        {
            patrolDirection = Random.value < 0.5f ? -1f : 1f;
        }
    }

    void HandleState()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                animator.SetBool("isWalking", false);
                animator.SetBool("isAttacking", false);
                break;

            case EnemyState.Patrol:
                rb.linearVelocity = new Vector2(patrolSpeed * patrolDirection, rb.linearVelocity.y);
                animator.SetBool("isWalking", true);
                break;

            case EnemyState.Attack:

                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

                if (!attackOnCooldown && !isAttacking)
                {
                    StartCoroutine(DoAttack());
                }

                break;

        }
    }

    public IEnumerator WaitForStateChangingCooldown(float cooldown)
    {
        isStateChangingCooldown = true;

        yield return new WaitForSeconds(cooldown);

        SetState(GetRandomEnemyState<EnemyState>());

        isStateChangingCooldown = false;
    }

    public static T GetRandomEnemyState<T>()
    {
        T[] values = (T[])System.Enum.GetValues(typeof(T));
        return values[Random.Range(0, values.Length - 1)];
    }

    public void Attack()
    {
        // handle attack

    }

    IEnumerator DoAttack()
    {
        isAttacking = true;
        attackOnCooldown = true;

        animator.SetBool("isAttacking", true);

        // tunggu animasi attack (contoh 0.5 detik)

        Attack(); // panggil logic attack kamu

        animator.SetBool("isAttacking", false);

        // cooldown attack
        yield return new WaitForSeconds(attackCooldown);

        attackOnCooldown = false;
        isAttacking = false;
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            SetState(EnemyState.Attack);
        }


    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            SetState(EnemyState.Idle);
        }
    }

    public IEnumerator WaitForAttackCooldown(float cooldown)
    {
        attackOnCooldown = true;
        yield return new WaitForSeconds(cooldown);
        attackOnCooldown = false;
    }

}
