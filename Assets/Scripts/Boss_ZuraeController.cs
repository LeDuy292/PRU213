using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class Boss_ZuraeController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;

    [Header("Patrol")]
    [SerializeField] private float patrolRadius = 5f;

    [Header("Detect Player")]
    [SerializeField] private float detectRadius = 6f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Transform detectPoint;

    [Header("Skill")]
    [SerializeField] private float skillCooldown = 2f;
    [SerializeField] private float skillDuration = 0.6f;

    private Rigidbody2D rb;
    private Animator animator;
    private Transform player;

    private Vector2 startPosition;
    private int patrolDirection = 1;
    private int facingDirection = 1;

    private float moveX;
    private bool isSkill;
    private bool isDie;
    private float lastSkillTime;

    // ================= UNITY =================
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        startPosition = transform.position;
    }

    private void Update()
    {
        if (isDie) return;

        // Đang tung skill → đứng yên
        if (isSkill)
        {
            moveX = 0;
            UpdateAnimation();
            return;
        }

        DetectPlayer();

        if (player != null)
            ChaseAndAttack();
        else
            Patrol();

        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveX, rb.linearVelocity.y);
    }

    // ================= PATROL =================
    private void Patrol()
    {
        float distance = transform.position.x - startPosition.x;

        if (Mathf.Abs(distance) >= patrolRadius)
            patrolDirection *= -1;

        moveX = patrolDirection * moveSpeed;
        Flip(patrolDirection);
    }

    // ================= DETECT =================
    private void DetectPlayer()
    {
        Collider2D hit = Physics2D.OverlapCircle(
            detectPoint.position,
            detectRadius,
            playerLayer
        );

        player = hit != null ? hit.transform : null;
    }

    // ================= CHASE + ATTACK =================
    private void ChaseAndAttack()
    {
        float distanceX = Mathf.Abs(player.position.x - transform.position.x);
        int direction = player.position.x > transform.position.x ? 1 : -1;

        Flip(direction);

        if (distanceX > attackRange)
        {
            moveX = direction * moveSpeed;
        }
        else
        {
            moveX = 0;
            TryUseSkill();
        }
    }

    // ================= SKILL =================
    private void TryUseSkill()
    {
        if (Time.time < lastSkillTime + skillCooldown) return;

        isSkill = true;
        lastSkillTime = Time.time;
        Invoke(nameof(ResetSkill), skillDuration);
    }

    private void ResetSkill()
    {
        isSkill = false;
    }

    // ================= FLIP =================
    private void Flip(int direction)
    {
        if (direction == facingDirection) return;

        facingDirection = direction;
        Vector3 scale = transform.localScale;
        scale.x = facingDirection;
        transform.localScale = scale;
    }

    // ================= ANIMATION =================
    private void UpdateAnimation()
    {
        animator.SetBool("IsRunning", Mathf.Abs(moveX) > 0.1f);
        animator.SetBool("IsSkill", isSkill);
        animator.SetBool("IsDie", isDie);
    }

    // ================= DIE =================
    public void Die()
    {
        isDie = true;
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
    }

    // ================= DEBUG =================
    private void OnDrawGizmosSelected()
    {
        if (detectPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(detectPoint.position, detectRadius);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Vector2 center = Application.isPlaying ? startPosition : (Vector2)transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(
            center + Vector2.left * patrolRadius,
            center + Vector2.right * patrolRadius
        );
    }
}
