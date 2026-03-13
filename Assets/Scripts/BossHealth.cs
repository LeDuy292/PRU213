using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [Header("Boss Health")]
    [SerializeField] private int maxHP = 3000;
    private int currentHP;

    [Header("Optional - Effects")]
    [SerializeField] private float destroyDelayAfterDeath = 3f;

    private Boss_BorealController bossController;

    private void Awake()
    {
        currentHP = maxHP;
    }

    private void Start()
    {
        bossController = GetComponent<Boss_BorealController>();

        if (bossController == null)
        {
            Debug.LogError("[BossHealth] Không tìm thấy Boss_BorealController trên cùng GameObject!");
        }

        Debug.Log($"Boss khởi tạo với {maxHP} HP");
    }

    public void TakeDamage(int damage)
    {
        if (currentHP <= 0) return;

        Debug.Log($"Boss nhận damage: {damage}");

        currentHP = Mathf.Max(0, currentHP - damage);

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("=== BOSS BOREAL ĐÃ CHẾT ===");

        if (bossController != null)
        {
            bossController.Die();
            bossController.TriggerAnimator("Die");
        }

        enabled = false;
        // Destroy(gameObject, destroyDelayAfterDeath); // mở ra nếu muốn tự hủy
    }

    // Getter cho BossHealthBarUI
    public int GetCurrentHealth() => currentHP;
    public int GetMaxHealth() => maxHP;
    public bool IsDead() => currentHP <= 0;
}