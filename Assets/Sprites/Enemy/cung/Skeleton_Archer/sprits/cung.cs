using UnityEngine;
using System.Collections;

public class Cung : MonoBehaviour
{
    // ================== PATROL ==================

    [Header("Patrol")]
    public float patrolSpeed = 2f;
    public float patrolDistance = 3f;
    public float idleTimeAtEdge = 1.5f;

    // ================== ARCHER ==================

    [Header("Archer")]
    public float shootRange = 14f;
    public float shootCooldown = 1.8f;
    public float arrowSpeed = 10f;
    public float pauseAfterLostTime = 1f;

    // ================== ENVIRONMENT ==================

    [Header("Environment")]
    public float checkAhead = 1f;
    public float checkDown = 1f;
    public GameObject arrowPrefab;
    public Transform shootPoint;

    // ================== BIẾN NỘI BỘ ==================

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

        // Tìm GameObject có Tag = Player
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

        // Nếu player đã chết
        if (playerHealth != null && playerHealth.IsDead)
        {
            Patrol();
            return;
        }

        // Tính khoảng cách từ enemy tới player
        float dist = Vector2.Distance(
            transform.position,
            player.position
        );

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

        // ===== KHÔNG THẤY PLAYER =====
        Patrol();
    }

    // ================== PATROL ==================
    void Patrol()
    {
        if (isWaiting) return;

        // Tạo điểm bắn ray phía trước mặt
        Vector2 checkPos = transform.position;
        checkPos.x += direction * checkAhead;

        // Bắn ray xuống dưới để kiểm tra có đất không
        RaycastHit2D hit = Physics2D.Raycast(
            checkPos,
            Vector2.down,
            checkDown
        );

        Debug.DrawRay(
            checkPos,
            Vector2.down * checkDown,
            Color.green
        );

        if (hit.collider == null)
        {
            StartCoroutine(IdleAndTurn());
            return;
        }

        anim.SetBool("iswalk", true);

        // Di chuyển enemy theo hướng hiện tại
        transform.Translate(
            Vector2.right * direction * patrolSpeed * Time.deltaTime,
            Space.World
        );

        // Lật sprite theo hướng
        transform.localScale = new Vector3(direction, 1, 1);

        // Nếu vượt quá giới hạn tuần tra
        if (direction > 0 &&
            transform.position.x >= startPos.x + patrolDistance)
            StartCoroutine(IdleAndTurn());

        else if (direction < 0 &&
                 transform.position.x <= startPos.x - patrolDistance)
            StartCoroutine(IdleAndTurn());
    }

    // ================== DỪNG & QUAY ĐẦU ==================
    IEnumerator IdleAndTurn()
    {
        isWaiting = true;
        anim.SetBool("iswalk", false);

        yield return new WaitForSeconds(idleTimeAtEdge);

        direction *= -1;
        isWaiting = false;
    }

    // ================== QUAY MẶT VỀ PLAYER ==================
    void FacePlayer()
    {
        float dir = Mathf.Sign(
            player.position.x - transform.position.x
        );                                  // Xác định player ở trái hay phải

        if (dir != 0)
            transform.localScale = new Vector3(dir, 1, 1);
    }

    // ================== SHOOT ==================
    IEnumerator Shoot()
    {
        isShooting = true;

        anim.SetTrigger("attack");

        yield return new WaitForSeconds(0.3f);

        if (arrowPrefab != null &&
            shootPoint != null)
        {
            float dir = Mathf.Sign(transform.localScale.x); // Hướng bắn

            Quaternion rotation =
                (dir > 0)
                ? Quaternion.identity
                : Quaternion.Euler(0, 0, 180); // Quay mũi tên

            GameObject arrow = Instantiate(
                arrowPrefab,
                shootPoint.position,
                rotation
            );

            Rigidbody2D rbArrow =
                arrow.GetComponent<Rigidbody2D>();

            if (rbArrow != null)
            {
                rbArrow.linearVelocity =
                    arrow.transform.right * arrowSpeed;
            }
        }

        yield return new WaitForSeconds(shootCooldown);
        isShooting = false;
    }
    // ================== VẼ TẦM BẮN ==================
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;           // Màu đỏ
        Gizmos.DrawWireSphere(
            transform.position,
            shootRange
        );                                  // Vẽ vòng tầm bắn
    }
}