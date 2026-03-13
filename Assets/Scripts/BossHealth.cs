using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [Header("Boss Health")]
    [SerializeField] private int maxHP = 3000;           // HP boss lớn (có thể chỉnh)
    private int currentHP;

    [Header("UI (tùy chọn)")]
    [SerializeField] private UnityEngine.UI.Image healthBarFill; // Kéo HealthBar của boss vào đây

    private Boss_BorealController bossController;

    private void Start()
    {
        currentHP = maxHP;
        bossController = GetComponent<Boss_BorealController>();

        if (healthBarFill != null)
            healthBarFill.fillAmount = 1f;

        Debug.Log("BossHealth khởi tạo: " + maxHP + " HP");
    }

    public void TakeDamage(int damage)
    {
        Debug.Log($"Boss nhận damage: {damage}");

        currentHP -= damage;

        // Cập nhật health bar
        if (healthBarFill != null)
            healthBarFill.fillAmount = (float)currentHP / maxHP;

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
            bossController.Die();           // Dừng mọi coroutine, state, movement
            bossController.TriggerAnimator("Die"); // (nếu bạn có animation Die)
        }

        // TODO: Sau này có thể thêm: drop item, mở cổng teleport, win screen...
        // Destroy(gameObject, 2f); // hoặc để animation chết chạy xong mới destroy
    }

    // Getter cho các script khác nếu cần
    public int CurrentHP => currentHP;
    public int MaxHP => maxHP;
}