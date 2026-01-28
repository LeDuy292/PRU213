using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider2D))]
public class Boss_BorealController : MonoBehaviour
{
    // ================= RANGE =================
    [Header("Range")]
    [SerializeField] private float meleeRange = 1.5f;
    [SerializeField] private float shootRange = 6f;

    // ================= MOVEMENT =================
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;

    // ================= TIMING =================
    [Header("Timing")]
    [SerializeField] private float attackDelay = 0.4f;
    [SerializeField] private float meleeCooldown = 1.5f;
    [SerializeField] private float shootCooldown = 3f;

    // ================= MELEE =================
    [Header("Melee")]
    [SerializeField] private float meleeDuration = 0.6f;

    // ================= SHOOT =================
    [Header("Shoot (Fireball)")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float shootRate = 0.5f;
    [SerializeField] private int bulletsPerShoot = 5;
    [SerializeField] private float minShootDistance = 3f;

    // ================= PRIVATE =================
    private Rigidbody2D rb;
    private Animator animator;
    private Transform player;

    private float moveX;
    private int facingDirection = 1;

    private float attackTimer;
    private float lastMeleeTime;
    private float lastShootSkillTime;
    private float lastShootTime;

    private int bulletsShot;
    private bool isShooting;
    private bool isDie;
    private bool isTouchingPlayer;

    // ================= STATE =================
    private enum State
    {
        Idle,
        Chase,
        Melee,
        Shoot
    }

    private State currentState = State.Idle;

    // ================= UNITY =================
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0;
        rb.freezeRotation = true;
    }

    private void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    private void Update()
    {
        if (isDie || player == null) return;

        // Đang tung skill → đứng yên
        if (currentState == State.Melee || currentState == State.Shoot)
        {
            moveX = 0;
            UpdateAnimation();
            return;
        }

        float deltaX = player.position.x - transform.position.x;
        float distance = Mathf.Abs(deltaX);
        int dir = deltaX > 0 ? 1 : -1;

        Flip(dir);

        // ================= MELEE =================
        if (distance <= meleeRange || isTouchingPlayer)
        {
            moveX = 0;
            attackTimer += Time.deltaTime;

            if (attackTimer >= attackDelay &&
                Time.time >= lastMeleeTime + meleeCooldown)
            {
                StartMelee();
            }

            UpdateAnimation();
            return;
        }

        // ================= SHOOT =================
        if (distance <= shootRange && distance >= minShootDistance)
        {
            moveX = 0;

            if (!isShooting && Time.time >= lastShootSkillTime + shootCooldown)
            {
                StartShoot();
            }

            UpdateAnimation();
            return;
        }

        // ================= CHASE =================
        currentState = State.Chase;
        moveX = dir * moveSpeed;
        attackTimer = 0;

        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveX, 0);
    }

    // ================= MELEE =================
    private void StartMelee()
    {
        currentState = State.Melee;
        lastMeleeTime = Time.time;
        attackTimer = 0;

        Invoke(nameof(EndMelee), meleeDuration);
    }

    private void EndMelee()
    {
        currentState = State.Idle;
    }

    // ================= SHOOT =================
    private void StartShoot()
    {
        isShooting = true;
        currentState = State.Shoot;
        lastShootSkillTime = Time.time;

        bulletsShot = 0;
        lastShootTime = -999f;
    }

    private void Fireball()
    {
        if (bulletsShot >= bulletsPerShoot)
        {
            EndShoot();
            return;
        }

        if (Time.time < lastShootTime + shootRate) return;

        lastShootTime = Time.time;
        bulletsShot++;

        Vector2 dir = (player.position - firePoint.position).normalized;

        GameObject bullet = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.identity
        );

        Projectile proj = bullet.GetComponent<Projectile>();
        if (proj != null)
            proj.SetDirection(dir);
    }

    private void EndShoot()
    {
        isShooting = false;
        currentState = State.Idle;
    }

    // ================= COLLISION =================
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer = true;
            moveX = 0;
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer = false;
        }
    }

    // ================= FLIP =================
    private void Flip(int dir)
    {
        if (dir == facingDirection) return;

        facingDirection = dir;
        Vector3 scale = transform.localScale;
        scale.x = facingDirection;
        transform.localScale = scale;
    }

    // ================= ANIMATION =================
    private void UpdateAnimation()
    {
        bool isRunning = currentState == State.Chase && Mathf.Abs(moveX) > 0.1f;

        animator.SetBool("IsRunning", isRunning);
        animator.SetBool("IsSkill", currentState == State.Melee);
        animator.SetBool("IsDie", isDie);

        if (currentState == State.Shoot)
        {
            Fireball();
        }
    }

    // ================= DIE =================
    public void Die()
    {
        isDie = true;
        rb.linearVelocity = Vector2.zero;
    }
}
