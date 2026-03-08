using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider2D))]
public class Boss_BorealController : MonoBehaviour
{
    [Header("────── References ──────")]
    public Transform player;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    // Các component con sẽ inject vào đây
    [HideInInspector] public BossMovement Movement;
    [HideInInspector] public BossMeleeAttack Melee;
    [HideInInspector] public BossShootChase ShootChase;
    [HideInInspector] public BossInitialShootPhase InitialPhase;
    [HideInInspector] public BossForm2Attacks Form2Attacks;
    [HideInInspector] public BossFormManager FormManager;

    public enum State
    {
        InitialShootPhase,
        Idle,
        Chase,
        Melee,
        ShootChase,
        Form2CircularAttack,
        Form2RainAttack,
        Form2BlizzardVortex
    }

    [HideInInspector] public State currentState = State.InitialShootPhase;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0;
        rb.freezeRotation = true;

        // Tìm player
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    private void Start()
    {
        // Tìm và inject các component
        Movement = GetComponent<BossMovement>();
        Melee = GetComponent<BossMeleeAttack>();
        ShootChase = GetComponent<BossShootChase>();
        InitialPhase = GetComponent<BossInitialShootPhase>();
        Form2Attacks = GetComponent<BossForm2Attacks>();
        FormManager = GetComponent<BossFormManager>();

        if (currentState == State.InitialShootPhase)
        {
            InitialPhase?.BeginInitialPhase();
        }
    }

    private void Update()
    {
        if (player == null) return;

        FormManager?.HandleFormChangeInput();

        switch (currentState)
        {
            case State.InitialShootPhase:
                InitialPhase?.UpdateInitialPhase();
                break;

            case State.Form2CircularAttack:
            case State.Form2RainAttack:
            case State.Form2BlizzardVortex:
                Movement?.StopMovement();
                break;

            case State.ShootChase:
                Movement?.StopMovement();
                break;

            default:
                Movement?.HandleNormalMovementAndDecideState();
                break;
        }

        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        if (currentState is State.Chase or State.Melee or State.Idle)
        {
            rb.linearVelocity = new Vector2(Movement?.moveX ?? 0f, 0f);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void UpdateAnimation()
    {
        bool isRunning = (currentState is State.Chase or State.ShootChase)
                         && Mathf.Abs(Movement?.moveX ?? 0f) > 0.1f;

        animator.SetBool("IsRunning", isRunning);
        animator.SetBool("IsSkill", currentState == State.Melee);
    }

    public void Die()
    {
        rb.linearVelocity = Vector2.zero;
        currentState = State.Idle;
        CancelInvoke();
        StopAllCoroutines();

        // Có thể thêm animation chết, particle, v.v.
    }

    // Các hàm helper gọi từ các script con
    public void ChangeState(State newState)
    {
        currentState = newState;
    }

    public void TriggerAnimator(string triggerName)
    {
        animator.SetTrigger(triggerName);
    }

    public void SetAnimatorBool(string name, bool value)
    {
        animator.SetBool(name, value);
    }
}