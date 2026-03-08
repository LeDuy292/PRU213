using UnityEngine;

public class BossMeleeAttack : MonoBehaviour
{
    [Header("Range & Timing")]
    [SerializeField] public float meleeRange = 1.5f;
    [SerializeField] private float attackDelay = 0.4f;
    [SerializeField] private float meleeCooldown = 1.5f;

    [HideInInspector] public bool isTouchingPlayer;
    private float attackTimer;
    private float lastMeleeTime;

    private Boss_BorealController boss;

    private void Awake()
    {
        boss = GetComponent<Boss_BorealController>();
    }

    public void HandleMeleeLogic()
    {
        boss.ChangeState(Boss_BorealController.State.Melee);
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackDelay && Time.time >= lastMeleeTime + meleeCooldown)
        {
            StartMelee();
        }
    }

    private void StartMelee()
    {
        lastMeleeTime = Time.time;
        attackTimer = 0f;
        boss.TriggerAnimator("Melee"); // hoặc "Skill"
        Invoke(nameof(EndMelee), 0.6f);
    }

    private void EndMelee()
    {
        boss.ChangeState(Boss_BorealController.State.Idle);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer = false;
        }
    }
}