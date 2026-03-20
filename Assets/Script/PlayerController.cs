using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // ==================== MOVEMENT ====================

    [Header("Movement")]
    [SerializeField] private float speed = 5f;
    public float Speed => speed + bonusSpeed;

    private float originalSpeed;
    private float bonusSpeed = 0f;

    [SerializeField] private float jump = 10f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;

    private bool isGrounded;
    private bool isAttacking;

    // ==================== DAMAGE ====================

    [Header("Base Damage")]
    [SerializeField] private int normalAttackDamage = 10;
    [SerializeField] private int skill1Damage = 25;
    [SerializeField] private int skill2Damage = 40;
    [SerializeField] private int skill3Damage = 60;

    private float normalDamageBonusPercent = 0f;
    private float skillDamageBonusPercent = 0f;

    public int Attack => GetNormalDamage();

    // ==================== EFFECT ====================

    [Header("Effects")]
    [SerializeField] private GameObject slashEffectPrefab;
    [SerializeField] private GameObject dragonEffectPrefab;
    [SerializeField] private GameObject momEffectPrefab;
    [SerializeField] private GameObject dragonJudgmentEffectPrefab;
    [SerializeField] private Transform attackPoint;

    // ==================== COOLDOWN ====================

    [Header("Cooldowns")]
    [SerializeField] private SkillCooldownUI skill1UI;
    [SerializeField] private SkillCooldownUI skill2UI;
    [SerializeField] private SkillCooldownUI skill3UI;

    [SerializeField] private float normalCooldown = 0.3f;
    [SerializeField] private float skill1Cooldown = 6f;
    [SerializeField] private float skill2Cooldown = 10f;
    [SerializeField] private float skill3Cooldown = 15f;

    private float normalTimer;
    private float skill1Timer;
    private float skill2Timer;
    private float skill3Timer;

    // ==================== COMPONENT ====================

    private Animator animator;
    private Rigidbody2D rb;

    public static PlayerController instance;

    // ==================== UNITY ====================

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        originalSpeed = speed;

        // Kết nối UI ngay từ scene đầu tiên
        ReconnectUI();
    }


    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ReconnectUI();
        StopCoroutine(nameof(ReconnectCameraDelay));
        StartCoroutine(ReconnectCameraDelay());
    }

    private void ReconnectUI()
    {
        SkillCooldownUI[] allSkills = UnityEngine.Object.FindObjectsByType<SkillCooldownUI>(FindObjectsSortMode.None);
        
        foreach (var s in allSkills)
        {
            // Kiểm tra tên đối tượng để gán đúng slot
            string n = s.name.ToLower();
            if (n.Contains("skill1")) skill1UI = s;
            else if (n.Contains("skill2")) skill2UI = s;
            else if (n.Contains("skill3")) skill3UI = s;
        }

        Debug.Log($"[PlayerController] Đã kết nối lại {allSkills.Length} UI Skills.");
    }

    private IEnumerator ReconnectCameraDelay()
    {
        // Chờ 1-2 frame để các object trong scene mới được Awake/Start
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.1f);

        CameraTargetSync camSync = null;

        // 1. Ưu tiên tìm theo tên GameObject camera phổ biến trước
        string[] camNames = { "CinemachineCamera", "Virtual Camera", "CM vcam1", "Main Camera" };
        GameObject camObj = null;

        foreach (string name in camNames)
        {
            camObj = GameObject.Find(name);
            if (camObj != null) break;
        }

        if (camObj != null)
        {
            camSync = camObj.GetComponent<CameraTargetSync>();
            if (camSync == null)
                camSync = camObj.AddComponent<CameraTargetSync>();
        }
        else
        {
            // 2. Nếu không thấy tên cụ thể, mới tìm bất kỳ script nào trong scene (nhưng bỏ qua chính Player)
            CameraTargetSync[] allSyncs = UnityEngine.Object.FindObjectsByType<CameraTargetSync>(FindObjectsSortMode.None);
            foreach (var s in allSyncs)
            {
                if (s.gameObject != gameObject) // Không phải Player
                {
                    camSync = s;
                    break;
                }
            }
        }

        if (camSync != null)
        {
            camSync.SetTarget(transform);
            Debug.Log("[PlayerController] Camera connected to Player successfully!");
        }
        else
        {
            Debug.LogWarning("[PlayerController] Không tìm thấy Camera nào để gán Target!");
        }
    }

    void Update()
    {

        UpdateAttackState();

        HandleMovement();
        HandleJump();
        UpdateAnimation();
        HandleAttack();

        normalTimer -= Time.deltaTime;
        skill1Timer -= Time.deltaTime;
        skill2Timer -= Time.deltaTime;
        skill3Timer -= Time.deltaTime;
    }

    // ==================== FIX ATTACK STATE ====================

    private void UpdateAttackState()
    {
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        if (state.IsTag("Attack"))
            isAttacking = true;
        else
            isAttacking = false;
    }

    // ==================== MOVEMENT ====================

    private void HandleMovement()
    {
        float moveInput = Input.GetAxis("Horizontal");

        if (isAttacking)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        rb.linearVelocity = new Vector2(moveInput * Speed, rb.linearVelocity.y);

        if (moveInput > 0)
            transform.localScale = Vector3.one;
        else if (moveInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void HandleJump()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            0.2f,
            groundLayer
        );

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jump);
        }
    }

    private void UpdateAnimation()
    {
        bool isRunning = Math.Abs(rb.linearVelocity.x) > 0.1f;

        animator.SetBool("IsRunning", isRunning);
        animator.SetBool("IsJumping", !isGrounded);
    }

    // ==================== ATTACK ====================

    private void HandleAttack()
    {
        if (isAttacking) return;

        if (Input.GetKeyDown(KeyCode.J) && normalTimer <= 0)
        {
            normalTimer = normalCooldown;
            animator.SetTrigger("attack");
        }
        else if (Input.GetKeyDown(KeyCode.Q) && skill1Timer <= 0)
        {
            skill1Timer = skill1Cooldown;
            animator.SetTrigger("Skill1");
            skill1UI?.StartCooldown(skill1Cooldown);
        }
        else if (Input.GetKeyDown(KeyCode.W) && skill2Timer <= 0)
        {
            skill2Timer = skill2Cooldown;
            animator.SetTrigger("Skill2");
            skill2UI?.StartCooldown(skill2Cooldown);
        }
        else if (Input.GetKeyDown(KeyCode.E) && skill3Timer <= 0)
        {
            if (FindFocusEnemy() == null)
            {
                Debug.Log("❌ Không có enemy để dùng Skill 3");
                return;
            }

            skill3Timer = skill3Cooldown;
            animator.SetTrigger("Skill3");
            skill3UI?.StartCooldown(skill3Cooldown);
        }
    }

    // ==================== DAMAGE CALC ====================

    public int GetNormalDamage()
    {
        return Mathf.RoundToInt(
            normalAttackDamage * (1 + normalDamageBonusPercent / 100f)
        );
    }

    public int GetSkillDamage(int baseDamage)
    {
        return Mathf.RoundToInt(
            baseDamage * (1 + skillDamageBonusPercent / 100f)
        );
    }

    // ==================== BONUS ====================

    public void AddBaseDamage(int amount)
    {
        normalAttackDamage += amount;
        skill1Damage += amount;
        skill2Damage += amount;
        skill3Damage += amount;
    }

    public void AddSpeed(float value) => bonusSpeed += value;
    public void RemoveSpeed(float value) => bonusSpeed -= value;

    public void AddNormalDamagePercent(float percent)
        => normalDamageBonusPercent += percent;

    public void RemoveNormalDamagePercent(float percent)
        => normalDamageBonusPercent -= percent;

    public void AddSkillDamagePercent(float percent)
        => skillDamageBonusPercent += percent;

    public void RemoveSkillDamagePercent(float percent)
        => skillDamageBonusPercent -= percent;

    // ==================== SPEED BOOST ====================

    public void BoostSpeed(float amount, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(SpeedBoostCoroutine(amount, duration));
    }

    private IEnumerator SpeedBoostCoroutine(float amount, float duration)
    {
        speed += amount;
        yield return new WaitForSeconds(duration);
        speed = originalSpeed;
    }

    // ==================== SPAWN EFFECT ====================

    public void SpawnSlashEffect()
    {
        if (!slashEffectPrefab || !attackPoint) return;

        GameObject effect = Instantiate(
            slashEffectPrefab,
            attackPoint.position,
            Quaternion.identity
        );

        SkillDamage dmg = effect.GetComponent<SkillDamage>();
        if (dmg != null)
            dmg.damage = GetNormalDamage();
    }

    public void SpawnDragonEffect()
    {
        if (!dragonEffectPrefab || !attackPoint) return;

        GameObject effect = Instantiate(
            dragonEffectPrefab,
            attackPoint.position,
            Quaternion.identity
        );

        SkillDamage dmg = effect.GetComponent<SkillDamage>();
        if (dmg != null)
            dmg.damage = GetSkillDamage(skill1Damage);
    }

    public void SpawnMomEffect()
    {
        if (!momEffectPrefab) return;

        GameObject effect = Instantiate(
            momEffectPrefab,
            transform.position,
            Quaternion.identity
        );

        SkillDamage dmg = effect.GetComponent<SkillDamage>();
        if (dmg != null)
            dmg.damage = GetSkillDamage(skill2Damage);
    }

    public void SpawndragonJudgmentEffect()
    {
        if (!dragonJudgmentEffectPrefab) return;

        Transform target = FindFocusEnemy();
        if (target == null) return;

        GameObject effect = Instantiate(
            dragonJudgmentEffectPrefab,
            target.position,
            Quaternion.identity
        );

        SkillDamage dmg = effect.GetComponent<SkillDamage>();
        if (dmg != null)
            dmg.damage = GetSkillDamage(skill3Damage);
    }

    private Transform FindFocusEnemy(float range = 8f)
    {
        Collider2D[] hits =
            Physics2D.OverlapCircleAll(transform.position, range);

        Transform closestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            float distance =
                Vector2.Distance(transform.position, hit.transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestEnemy = hit.transform;
            }
        }

        return closestEnemy;
    }
}