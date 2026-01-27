using UnityEngine;
using System.Collections;

public class Cung : MonoBehaviour
{
    [Header("Patrol")]
    public float patrolSpeed = 2f;
    public float patrolDistance = 3f;     // đi tuần rộng hơn
    public float idleTimeAtEdge = 1.5f;

    [Header("Archer")]
    public float shootRange = 14f;         // bắn xa

    public float shootCooldown = 1.8f;
    public float arrowSpeed = 10f;         // Tốc độ bay của tên
    public float pauseAfterLostTime = 1f;

    [Header("Environment")]
    public float checkAhead = 1f;      // Khoảng cách check phía trước
    public float checkDown = 1f;       // Khoảng cách check xuống dưới
    public GameObject arrowPrefab;
    public Transform shootPoint;

    private Vector2 startPos;
    private int direction = 1;

    private bool isWaiting;
    private bool isPausingAfterLost;
    private bool isShooting;

    private Transform player;
    private PlayerHealth playerHealth;
    private Animator anim;

    void Start()
    {
        startPos = transform.position;
        anim = GetComponent<Animator>();

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
    }

    void Update()
    {
        if (player == null) return;

        // Nếu player đã chết thì ngừng tấn công, chỉ patrol
        if (playerHealth != null && playerHealth.IsDead)
        {
            Patrol();
            return;
        }

        float dist = Vector2.Distance(transform.position, player.position);

        // ===== PLAYER TRONG TẦM BẮN =====
        if (dist <= shootRange)
        {
            anim.SetBool("iswalk", false);
            isWaiting = false;

            FacePlayer();

            if (!isShooting)
                StartCoroutine(Shoot());

            return;
        }

        // ===== PLAYER RA KHỎI TẦM =====
        // Nếu không bắn nữa thì đi tuần
        Patrol();

        // ===== PATROL =====
        Patrol();
    }

    // ================== PATROL ==================
    void Patrol()
    {
        if (isWaiting) return;

        // --- KIỂM TRA VỰC THẲM (GROUND CHECK) ---
        // Vị trí bắt đầu bắn tia Ray (ngay trước mặt theo hưởng di chuyển)
        Vector2 checkPos = transform.position;
        checkPos.x += direction * checkAhead;

        // Bắn tia Ray xuống dưới để check mặt đất
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, checkDown);
        Debug.DrawRay(checkPos, Vector2.down * checkDown, Color.green);

        // Nếu KHÔNG chạm gì (collider == null) => Là vực thẳm
        if (hit.collider == null)
        {
            StartCoroutine(IdleAndTurn());
            return;
        }

        // =======================================

        anim.SetBool("iswalk", true);

        transform.Translate(Vector2.right * direction * patrolSpeed * Time.deltaTime, Space.World);
        transform.localScale = new Vector3(direction, 1, 1);

        // Chỉ quay đầu nếu đi quá giới hạn VÀ đang đi ra xa
        if (direction > 0 && transform.position.x >= startPos.x + patrolDistance)
            StartCoroutine(IdleAndTurn());
        else if (direction < 0 && transform.position.x <= startPos.x - patrolDistance)
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

    // ================== QUAY MẶT ==================
    void FacePlayer()
    {
        float dir = Mathf.Sign(player.position.x - transform.position.x);
        if (dir != 0)
            transform.localScale = new Vector3(dir, 1, 1);
    }

    // ================== PAUSE AFTER LOST ==================


    // ================== SHOOT ==================
    IEnumerator Shoot()
    {
        isShooting = true;

        anim.SetTrigger("attack");

        yield return new WaitForSeconds(0.3f);

        if (arrowPrefab != null && shootPoint != null)
        {
            float dir = Mathf.Sign(transform.localScale.x);
            
            // Xoay mũi tên theo hướng bắn (Quay trái thì xoay 180 độ)
            Quaternion rotation = (dir > 0) ? Quaternion.identity : Quaternion.Euler(0, 0, 180);

            GameObject arrow = Instantiate(
                arrowPrefab,
                shootPoint.position,
                rotation
            );

            // Gán vận tốc bay cho mũi tên
            Rigidbody2D rbArrow = arrow.GetComponent<Rigidbody2D>();
            if (rbArrow != null)
            {
                // Bay theo hướng trục phải (đỏ) của mũi tên
                rbArrow.linearVelocity = arrow.transform.right * arrowSpeed;
            }
        }

        yield return new WaitForSeconds(shootCooldown);
        isShooting = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shootRange);
    }
}
