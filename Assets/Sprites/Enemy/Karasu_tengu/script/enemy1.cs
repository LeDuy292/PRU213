using UnityEngine;
using System.Collections;

public class EnemyAI1 : MonoBehaviour
{
    // ===== TUẦN TRA =====
    [Header("Patrol")]
    public float patrolSpeed = 2f;
    public float patrolDistance = 3f;
    public float idleTimeAtEdge = 2f;

    // ===== PHÁT HIỆN PLAYER =====
    [Header("Detect Player")]
    public float detectRange = 5f;
    public float chaseSpeed = 2.5f;
    public float pauseAfterLostTime = 1f;

    // ===== TẤN CÔNG =====
    [Header("Attack")]
    public float attackCooldown = 1.2f;
    public int attackDamage = 15;

    // ===== BIẾN NỘI BỘ =====
    private Vector2 startPos;
    private int direction = 1;

    private bool isWaiting = false;
    private bool isChasing = false;
    private bool isPausingAfterLost = false;
    private bool isTouchingPlayer = false;
    private bool isAttacking = false;

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

        // Tìm player theo Tag
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

        // ===== PLAYER ĐÃ CHẾT =====
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

        // ===== ĐANG CHẠM PLAYER → ĐÁNH =====
        if (isTouchingPlayer)
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetBool("iswalk", false);

            if (!isAttacking)
                StartCoroutine(Attack());

            return;
        }

        // Tính khoảng cách tới player
        float dist = Vector2.Distance(
            transform.position,
            player.position
        );

        // ===== PLAYER TRONG TẦM PHÁT HIỆN =====
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

        // ===== KHÔNG THẤY PLAYER → PATROL =====
        Patrol();
    }

    // ================== PATROL ==================
    void Patrol()
    {
        if (isWaiting) return;

        anim.SetBool("iswalk", true);

        // Di chuyển theo hướng hiện tại
        transform.Translate(
            Vector2.right * direction * patrolSpeed * Time.deltaTime
        );

        // Lật sprite theo hướng
        transform.localScale = new Vector3(direction, 1, 1);

        // Nếu đi quá xa vị trí ban đầu
        if (Mathf.Abs(transform.position.x - startPos.x) >= patrolDistance)
            StartCoroutine(IdleAndTurn());
    }


    IEnumerator IdleAndTurn()
    {
        isWaiting = true;
        anim.SetBool("iswalk", false);

        yield return new WaitForSeconds(idleTimeAtEdge);

        direction *= -1;
        isWaiting = false;
    }

    // ================== CHASE ==================
    void ChasePlayer()
    {
        anim.SetBool("iswalk", true);


        Vector2 target = new Vector2(
            player.position.x,
            transform.position.y
        );

        // Di chuyển dần về phía player
        transform.position = Vector2.MoveTowards(
            transform.position,
            target,
            chaseSpeed * Time.deltaTime
        );

        // Quay mặt về phía player
        float dir = Mathf.Sign(
            player.position.x - transform.position.x
        );
        if (dir != 0)
            transform.localScale = new Vector3(dir, 1, 1);
    }

    // ================== PAUSE AFTER LOST ==================
    IEnumerator PauseAfterLost()
    {
        isPausingAfterLost = true;
        anim.SetBool("iswalk", false);

        yield return new WaitForSeconds(pauseAfterLostTime);

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

        // Đợi tới frame đánh trong animation
        yield return new WaitForSeconds(0.3f);

        // Gây damage nếu player còn sống
        if (player != null &&
            playerHealth != null &&
            !playerHealth.IsDead)
        {
            playerHealth.TakeDamage(attackDamage);
            Debug.Log($"Enemy dealt {attackDamage} damage to player!");
        }

        // Đợi hết thời gian hồi chiêu
        yield return new WaitForSeconds(
            attackCooldown - 0.3f
        );

        isAttacking = false;
    }

    // ================== COLLISION va cham ==================
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
        Gizmos.DrawWireSphere(
            transform.position,
            detectRange
        );                                   // Vẽ vòng phát hiện player
    }
}
