using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float jump = 10.0f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private GameObject slashEffectPrefab;
    [SerializeField] private GameObject dragonEffectPrefab;
    [SerializeField] private GameObject momEffectPrefab;
    [SerializeField] private GameObject dragonJudgmentEffectPrefab;
    [SerializeField] private int normalAttackDamage = 10;
    [SerializeField] private int skill1Damage = 25;
    [SerializeField] private int skill2Damage = 40;
    [SerializeField] private int skill3Damage = 60;
    [SerializeField] private Transform attackPoint;
    [Header("Cooldowns")]
    [SerializeField] private float normalCooldown = 0.3f;
    [SerializeField] private float skill1Cooldown = 6f;
    [SerializeField] private float skill2Cooldown = 10f;
    [SerializeField] private float skill3Cooldown = 15f;

    private float normalTimer;
    private float skill1Timer;
    private float skill2Timer;
    private float skill3Timer;

    private bool isAttacking;
    private float attackTimer;
    private Animator animator;
    private Rigidbody2D rb;
    private bool isGrounded;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleJump();
        UpdateAnimation();
        HandleAttack();

        normalTimer -= Time.deltaTime;
        skill1Timer -= Time.deltaTime;
        skill2Timer -= Time.deltaTime;
        skill3Timer -= Time.deltaTime;

        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
                isAttacking = false;
        }
    }

    private void HandleAttack()
    {
        if (isAttacking) return;

        if (Input.GetKeyDown(KeyCode.J) && normalTimer <= 0)
        {
            normalTimer = normalCooldown;
            DoAttack("attack");
        }
        else if (Input.GetKeyDown(KeyCode.Q) && skill1Timer <= 0)
        {
            skill1Timer = skill1Cooldown;
            DoAttack("Skill1");
        }
        else if (Input.GetKeyDown(KeyCode.W) && skill2Timer <= 0)
        {
            skill2Timer = skill2Cooldown;
            DoAttack("Skill2");
        }
        else if (Input.GetKeyDown(KeyCode.E) && skill3Timer <= 0)
        {
            if (FindFocusEnemy() == null)
            {
                Debug.Log("❌ Không có enemy để dùng Skill 3");
                return;
            }

            skill3Timer = skill3Cooldown;
            DoAttack("Skill3");
        }
    }


    private void DoAttack(string triggerName)
    {
        isAttacking = true;
        animator.ResetTrigger(triggerName); // an toàn
        animator.SetTrigger(triggerName);
    }



    private void HandleMovement()
    {
        if (isAttacking)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);

        if (moveInput > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);

    }
    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump")&&isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jump);
        }
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }
    private void UpdateAnimation()
    {
        bool isRunning = Math.Abs(rb.linearVelocity.x) > 0.1f;
        bool isJumping = !isGrounded;
        animator.SetBool("IsRunning", isRunning);
        animator.SetBool("IsJumping", isJumping);
    }
    public void SpawnSlashEffect()
    {
        if (slashEffectPrefab == null || attackPoint == null) return;

        GameObject effect = Instantiate(
            slashEffectPrefab,
            attackPoint.position,
            Quaternion.identity
        );

        Vector3 scale = Vector3.one;

        // chỉ quay hướng theo player, KHÔNG đảo ngược
        scale.x = Mathf.Sign(transform.localScale.x);

        effect.transform.localScale = scale;

        SkillDamage dmg = effect.GetComponent<SkillDamage>();
        if (dmg != null)
        {
            dmg.damage = normalAttackDamage;
        }
    }


    public void SpawnDragonEffect()
    {
        if (dragonEffectPrefab == null || attackPoint == null) return;

        GameObject effect = Instantiate(
            dragonEffectPrefab,
            attackPoint.position,
            Quaternion.identity
        );

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        scale.x *= 4f;
        effect.transform.localScale = scale;

        // 👉 DAMAGE SKILL 1
        SkillDamage dmg = effect.GetComponent<SkillDamage>();
        if (dmg != null)
        {
            dmg.damage = skill1Damage;
        }
    }
    public void SpawnMomEffect()
    {
        if (momEffectPrefab == null) return;
        float dir = Mathf.Sign(transform.localScale.x);

        Vector3 offset = new Vector3(0.8f * dir, 0.3f, 0f);

        GameObject effect = Instantiate(
            momEffectPrefab,
            transform.position + offset,
            Quaternion.identity
        );

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        scale.x *= 3f;
        scale.y *= 2f;

        effect.transform.localScale = scale;

        SkillDamage dmg = effect.GetComponent<SkillDamage>();
        if (dmg != null)
        {
            dmg.damage = skill2Damage;
        }
    }


    public void SpawndragonJudgmentEffect()
    {
        if (dragonJudgmentEffectPrefab == null) return;

        Transform target = FindFocusEnemy();

        if (target == null)
        {
            Debug.Log("❌ Không có enemy focus");
            return;
        }

        GameObject effect = Instantiate(
            dragonJudgmentEffectPrefab,
            target.position, // 👈 SPAWN TRÊN ĐẦU ENEMY
            Quaternion.identity
        );

        Vector3 scale = Vector3.one * 2f;
        effect.transform.localScale = scale;

        // DAMAGE
        SkillDamage dmg = effect.GetComponent<SkillDamage>();
        if (dmg != null)
        {
            dmg.damage = skill3Damage;
        }
    }

    private Transform FindFocusEnemy(float range = 8f)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range);

        Transform closestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            float dirToEnemy = hit.transform.position.x - transform.position.x;

            // chỉ lấy enemy phía trước mặt
            if (Mathf.Sign(dirToEnemy) != Mathf.Sign(transform.localScale.x))
                continue;

            float distance = Vector2.Distance(transform.position, hit.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestEnemy = hit.transform;
            }
        }

        return closestEnemy;
    }

}
