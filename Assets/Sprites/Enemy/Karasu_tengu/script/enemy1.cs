using UnityEngine;
using System.Collections;

public class EnemyAI1 : MonoBehaviour
{
    // ===== TUẦN TRA =====
    [Header("Patrol")]
    public float patrolSpeed = 2f;        // Tốc độ đi tuần
    public float patrolDistance = 3f;     // Khoảng cách đi tuần
    public float idleTimeAtEdge = 2f;     // Thời gian đứng lại khi tới biên

    // ===== PHÁT HIỆN PLAYER =====
    [Header("Detect Player")]
    public float detectRange = 5f;         // Bán kính phát hiện player
    public float chaseSpeed = 2.5f;        // Tốc độ đuổi
    public float pauseAfterLostTime = 1f;  // Đứng khựng khi mất dấu

    // ===== TẤN CÔNG =====
    [Header("Attack")]
    public float attackCooldown = 1.2f;    // Thời gian hồi đánh
    public int attackDamage = 15;          // Sát thương gây ra

    // ===== BIẾN NỘI BỘ =====
    private Vector2 startPos;               // Vị trí bắt đầu tuần tra
    private int direction = 1;              // Hướng di chuyển (1 hoặc -1)

    private bool isWaiting = false;          // Đang đứng chờ
    private bool isChasing = false;          // Đang đuổi
    private bool isPausingAfterLost = false; // Đang đứng khựng
    private bool isTouchingPlayer = false;   // Đang chạm player
    private bool isAttacking = false;        // Đang đánh

    private Transform player;
    private PlayerHealth playerHealth;
    private Animator anim;
    private Rigidbody2D rb;

    void Start()
    {
        startPos = transform.position;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 2;
        rb.freezeRotation = true;

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            player = p.transform;
            playerHealth = p.GetComponent<PlayerHealth>();
        }
        else
        {
            Debug.LogError("❌ Player chưa set Tag = Player");
        }

        anim.SetBool("iswalk", false);
    }

    void Update()
    {
        if (player == null) return;

        // Nếu player đã chết thì ngừng tấn công, chỉ patrol
        if (playerHealth != null && playerHealth.IsDead)
        {
            isTouchingPlayer = false;
            isChasing = false;
            Patrol();
            return;
        }

        // ===== ĐANG ĐÁNH → KHÓA HÀNH ĐỘNG =====
        if (isAttacking)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // ===== CHẠM PLAYER → ĐÁNH =====
        if (isTouchingPlayer)
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetBool("iswalk", false);

            if (!isAttacking)
                StartCoroutine(Attack());

            return;
        }

        float dist = Vector2.Distance(transform.position, player.position);

        // ===== CHASE =====
        if (dist <= detectRange)
        {
            isWaiting = false;
            isChasing = true;
            isPausingAfterLost = false;

            ChasePlayer();
            return;
        }

        // ===== MẤT DẤU PLAYER =====
        if (isChasing && !isPausingAfterLost)
        {
            isChasing = false;
            StartCoroutine(PauseAfterLost());
            return;
        }

        // ===== PATROL =====
        Patrol();
    }

    // ================== PATROL ==================
    void Patrol()
    {
        if (isWaiting) return;

        anim.SetBool("iswalk", true);

        transform.Translate(Vector2.right * direction * patrolSpeed * Time.deltaTime);
        transform.localScale = new Vector3(direction, 1, 1);

        // Nếu đi quá xa vị trí ban đầu → đứng lại & quay đầu
        if (Mathf.Abs(transform.position.x - startPos.x) >= patrolDistance)
            StartCoroutine(IdleAndTurn());
    }

    IEnumerator IdleAndTurn()
    {
        isWaiting = true;
        anim.SetBool("iswalk", false);

        yield return new WaitForSeconds(idleTimeAtEdge);

        direction *= -1; // đổi hướng
        isWaiting = false;
    }

    // ================== CHASE ==================
    void ChasePlayer()
    {
        anim.SetBool("iswalk", true);

        // Chỉ đuổi theo trục X
        Vector2 target = new Vector2(player.position.x, transform.position.y);

        transform.position = Vector2.MoveTowards(
            transform.position,
            target,
            chaseSpeed * Time.deltaTime
        );

        // Quay mặt về player
        float dir = Mathf.Sign(player.position.x - transform.position.x);
        if (dir != 0)
            transform.localScale = new Vector3(dir, 1, 1);
    }

    // ================== PAUSE AFTER LOST ==================
    IEnumerator PauseAfterLost()
    {
        isPausingAfterLost = true;
        anim.SetBool("iswalk", false);

        yield return new WaitForSeconds(pauseAfterLostTime);

        // Reset tuần tra tại vị trí mới
        startPos = transform.position;
        direction = 1;

        isPausingAfterLost = false;
    }

    // ================== ATTACK ==================
    IEnumerator Attack()
    {
        isAttacking = true;

        anim.ResetTrigger("attack");
        anim.SetTrigger("attack");

        // Đợi một chút để animation chơi đến frame đánh
        yield return new WaitForSeconds(0.3f);

        // Gây damage cho player
        if (player != null && playerHealth != null && !playerHealth.IsDead)
        {
            playerHealth.TakeDamage(attackDamage);
            Debug.Log($"Enemy dealt {attackDamage} damage to player!");
        }

        // Đợi hết cooldown
        yield return new WaitForSeconds(attackCooldown - 0.3f);

        isAttacking = false;
    }

    // ================== COLLISION ==================
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer = true;
            anim.SetBool("iswalk", false);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer = false;
        }
    }

    // ================== DEBUG ==================
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }
}
