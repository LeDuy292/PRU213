using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
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
    [SerializeField] private float shootSkillCooldown = 3f;

    // ================= MELEE =================
    [Header("Melee Skill")]
    [SerializeField] private float meleeDuration = 0.6f;

    // ================= SHOOT =================
    [Header("Shoot Skill")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float shootRate = 0.25f;
    [SerializeField] private float shootDuration = 1.2f;

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

    private bool isDie;

    // ================= STATE =================
    private enum SkillState
    {
        None,
        Melee,
        Shoot
    }

    private SkillState currentSkill = SkillState.None;

    // ================= UNITY =================
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0;
    }

    private void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    private void Update()
    {
        if (isDie || player == null) return;

        float deltaX = player.position.x - transform.position.x;
        float distance = Mathf.Abs(deltaX);
        int dir = deltaX > 0 ? 1 : -1;

        Flip(dir);

        // 🔒 ĐANG DÙNG SKILL
        if (currentSkill != SkillState.None)
        {
            HandleSkill();
            UpdateAnimation();
            return;
        }

        // 🔴 ƯU TIÊN 1: CẬN CHIẾN
        if (distance <= meleeRange)
        {
            moveX = 0;
            attackTimer += Time.deltaTime;

            if (attackTimer >= attackDelay &&
                Time.time >= lastMeleeTime + meleeCooldown)
            {
                UseMeleeSkill();
            }

            UpdateAnimation();
            return;
        }

        // 🔵 ƯU TIÊN 2: BẮN XA (<= shootRange)
        if (distance <= shootRange)
        {
            moveX = 0;

            if (Time.time >= lastShootSkillTime + shootSkillCooldown)
            {
                UseShootSkill();
            }

            UpdateAnimation();
            return;
        }

        // 🏃 NGOÀI TẦM → ĐUỔI
        moveX = dir * moveSpeed;
        attackTimer = 0;

        UpdateAnimation();
    }



    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveX, 0);
    }

    // ================= MELEE =================
    private void UseMeleeSkill()
    {
        currentSkill = SkillState.Melee;
        lastMeleeTime = Time.time;
        attackTimer = 0;

        Invoke(nameof(ResetSkill), meleeDuration);
    }

    // ================= SHOOT =================
    private void UseShootSkill()
    {
        currentSkill = SkillState.Shoot;
        lastShootSkillTime = Time.time;
        lastShootTime = 0f;

        Invoke(nameof(ResetSkill), shootDuration);
    }

    private void HandleSkill()
    {
        moveX = 0;

        if (currentSkill == SkillState.Shoot)
        {
            AutoShoot();
        }
    }

    private void AutoShoot()
    {
        if (Time.time < lastShootTime + shootRate) return;

        lastShootTime = Time.time;

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

    // ================= RESET =================
    private void ResetSkill()
    {
        currentSkill = SkillState.None;
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
        animator.SetBool("IsRunning", Mathf.Abs(moveX) > 0.1f);
        animator.SetBool("IsMelee", currentSkill == SkillState.Melee);
        animator.SetBool("IsShoot", currentSkill == SkillState.Shoot);
        animator.SetBool("IsDie", isDie);
    }

    // ================= DIE =================
    public void Die()
    {
        isDie = true;
        rb.linearVelocity = Vector2.zero;
    }
}
