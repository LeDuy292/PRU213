using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider2D))]
public class Boss_BorealController : MonoBehaviour
{
    // ================= RANGE =================
    [Header("Range")]
    [SerializeField] private float meleeRange = 1.5f;
    [SerializeField] private float shootTriggerDistance = 3f;

    // ================= MOVEMENT =================
    [Header("Movement")]
    [SerializeField] private float moveSpeedForm1 = 3f;
    [SerializeField] private float moveSpeedForm2 = 4.5f;

    // ================= TIMING =================
    [Header("Timing")]
    [SerializeField] private float attackDelay = 0.4f;
    [SerializeField] private float meleeCooldown = 1.5f;

    // ================= SHOOT DURING CHASE =================
    [Header("Shoot During Chase")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float shootChanceForm1 = 0.2f;
    [SerializeField] private float shootChanceForm2 = 0.45f;
    [SerializeField] private float shootCooldownChaseForm1 = 2.5f;
    [SerializeField] private float shootCooldownChaseForm2 = 1.8f;
    [SerializeField] private float shootDuration = 0.4f;

    // ================= INITIAL SHOOT PHASE =================
    [Header("Initial Shoot Phase (Fireball Barrage)")]
    [SerializeField] private int totalShootWaves = 3;
    [SerializeField] private int bulletsPerWave = 5;
    [SerializeField] private float delayBetweenBullets = 0.3f;
    [SerializeField] private float delayBetweenWaves = 2f;
    [SerializeField] private float delayAfterAllWaves = 1f;

    // ================= FORM 2 SPECIAL ATTACK =================
    [Header("Form 2 - Circular Barrage")]
    [SerializeField] private int circularBullets = 15;              // 15 viên tạo hình tròn
    [SerializeField] private int circularWaves = 3;                 // bắn 3 đợt
    [SerializeField] private float delayBetweenCircularWaves = 1.2f;// thời gian giữa các đợt bắn tròn
    [SerializeField] private float circularBulletSpeedMultiplier = 1.2f; // tốc độ đạn có thể tăng nếu muốn

    // ================= FORM CHANGE =================
    [Header("Form Change")]
    [SerializeField] private KeyCode formChangeKey = KeyCode.F;
    [SerializeField] private bool isForm2 = false;

    // ================= PRIVATE =================
    private Rigidbody2D rb;
    private Animator animator;
    private Transform player;
    private float moveX;
    private int facingDirection = 1;
    private float attackTimer;
    private float lastMeleeTime;
    private bool isTouchingPlayer;
    private bool hasFinishedInitialShootPhase = false;
    private int currentWave = 0;
    private int bulletsShotInWave = 0;
    private float nextActionTime;
    private float lastShootChaseTime = -999f;

    private enum State
    {
        InitialShootPhase,
        Idle,
        Chase,
        Melee,
        ShootChase,
        Form2CircularAttack   // trạng thái mới cho skill bắn tròn Form 2
    }
    private State currentState = State.InitialShootPhase;

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

        if (currentState == State.InitialShootPhase)
        {
            nextActionTime = Time.time;
        }

        UpdateFormParameters();
    }

    private void Update()
    {
        if (player == null) return;

        // Chuyển dạng bằng phím F
        if (Input.GetKeyDown(formChangeKey))
        {
            ToggleForm();
        }

        FlipTowardsPlayer();

        switch (currentState)
        {
            case State.InitialShootPhase:
                HandleInitialShootPhase();
                break;

            case State.Form2CircularAttack:
                // Đứng yên trong lúc bắn tròn
                moveX = 0;
                break;

            case State.ShootChase:
                moveX = 0;
                break;

            default:
                HandleNormalPhase();
                break;
        }

        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        if (currentState != State.InitialShootPhase &&
            currentState != State.ShootChase &&
            currentState != State.Form2CircularAttack)
        {
            rb.linearVelocity = new Vector2(moveX, 0);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    // ================= FORM CHANGE LOGIC =================
    private void ToggleForm()
    {
        isForm2 = !isForm2;
        UpdateFormParameters();

        Debug.Log(isForm2 ? "Boss chuyển sang Form 2!" : "Boss quay về Form 1");

        animator.SetTrigger("FormChange");

        // Khi chuyển sang Form 2 → kích hoạt ngay skill bắn tròn
        if (isForm2)
        {
            StartCoroutine(PerformCircularBarrage());
        }
    }

    private void UpdateFormParameters()
    {
        if (isForm2)
        {
            animator.SetBool("IsForm2", true);
        }
        else
        {
            animator.SetBool("IsForm2", false);
        }
    }

    private float CurrentMoveSpeed => isForm2 ? moveSpeedForm2 : moveSpeedForm1;
    private float CurrentShootChance => isForm2 ? shootChanceForm2 : shootChanceForm1;
    private float CurrentShootCooldownChase => isForm2 ? shootCooldownChaseForm2 : shootCooldownChaseForm1;

    // ================= SKILL BẮN TRÒN FORM 2 =================
    private IEnumerator PerformCircularBarrage()
    {
        currentState = State.Form2CircularAttack;
        moveX = 0;

        // Trigger animation nếu có (ví dụ "CircularAttack")
        animator.SetTrigger("CircularAttack");

        for (int wave = 0; wave < circularWaves; wave++)
        {
            // Bắn 15 viên cùng lúc tạo hình tròn
            for (int i = 0; i < circularBullets; i++)
            {
                float angle = i * (360f / circularBullets);
                Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

                GameObject bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
                Projectile proj = bullet.GetComponent<Projectile>();
                if (proj != null)
                {
                    proj.SetDirection(direction.normalized);
                    // Optional: tăng tốc độ đạn ở Form 2
                    proj.speed *= circularBulletSpeedMultiplier;
                }
            }

            // Chờ giữa các đợt
            yield return new WaitForSeconds(delayBetweenCircularWaves);
        }

        // Hoàn thành skill → quay lại trạng thái bình thường
        currentState = State.Idle;
    }

    // ================= CÁC HÀM CŨ KHÔNG THAY ĐỔI =================
    private void HandleInitialShootPhase()
    {
        moveX = 0;
        if (Time.time < nextActionTime) return;

        if (bulletsShotInWave < bulletsPerWave)
        {
            FireOneBullet();
            bulletsShotInWave++;
            nextActionTime = Time.time + delayBetweenBullets;
        }
        else
        {
            currentWave++;
            bulletsShotInWave = 0;

            if (currentWave >= totalShootWaves)
            {
                Invoke(nameof(FinishInitialShootPhase), delayAfterAllWaves);
                nextActionTime = float.MaxValue;
            }
            else
            {
                nextActionTime = Time.time + delayBetweenWaves;
            }
        }
    }

    private void FinishInitialShootPhase()
    {
        hasFinishedInitialShootPhase = true;
        currentState = State.Idle;
    }

    private void HandleNormalPhase()
    {
        if (!hasFinishedInitialShootPhase)
        {
            moveX = 0;
            currentState = State.Idle;
            return;
        }

        float deltaX = player.position.x - transform.position.x;
        float distance = Mathf.Abs(deltaX);

        if (distance <= meleeRange || isTouchingPlayer)
        {
            moveX = 0;
            attackTimer += Time.deltaTime;

            if (attackTimer >= attackDelay && Time.time >= lastMeleeTime + meleeCooldown)
            {
                StartMelee();
            }

            currentState = State.Melee;
            return;
        }

        currentState = State.Chase;
        attackTimer = 0;

        if (distance >= shootTriggerDistance && Time.time >= lastShootChaseTime + CurrentShootCooldownChase)
        {
            if (Random.value <= CurrentShootChance)
            {
                StartShootChase();
                return;
            }
        }

        moveX = Mathf.Sign(deltaX) * CurrentMoveSpeed;
    }

    private void StartShootChase()
    {
        currentState = State.ShootChase;
        moveX = 0;
        lastShootChaseTime = Time.time;
        animator.SetTrigger("Shoot");
        FireOneBullet();
        Invoke(nameof(EndShootChase), shootDuration);
    }

    private void EndShootChase()
    {
        currentState = State.Chase;
    }

    private void FireOneBullet()
    {
        if (projectilePrefab == null || firePoint == null) return;

        Vector2 dir = (player.position - firePoint.position).normalized;
        GameObject bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Projectile proj = bullet.GetComponent<Projectile>();
        if (proj != null)
        {
            proj.SetDirection(dir);
        }
    }

    private void StartMelee()
    {
        currentState = State.Melee;
        lastMeleeTime = Time.time;
        attackTimer = 0;
        Invoke(nameof(EndMelee), 0.6f);
    }

    private void EndMelee()
    {
        currentState = State.Idle;
    }

    // ================= COLLISION & FLIP & ANIMATION =================
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

    private void FlipTowardsPlayer()
    {
        if (player == null) return;
        int dir = player.position.x > transform.position.x ? 1 : -1;
        if (dir != facingDirection)
        {
            facingDirection = dir;
            Vector3 scale = transform.localScale;
            scale.x = facingDirection;
            transform.localScale = scale;
        }
    }

    private void UpdateAnimation()
    {
        bool isRunning = (currentState == State.Chase || currentState == State.ShootChase)
                         && Mathf.Abs(moveX) > 0.1f;
        animator.SetBool("IsRunning", isRunning);
        animator.SetBool("IsSkill", currentState == State.Melee);
    }

    public void Die()
    {
        rb.linearVelocity = Vector2.zero;
        currentState = State.Idle;
        CancelInvoke();
        StopAllCoroutines();
    }
}