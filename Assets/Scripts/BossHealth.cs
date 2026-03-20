using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [Header("Boss Health")]
    [SerializeField] private int maxHP = 3000;
    private int currentHP;

    [Header("Optional - Effects")]
    [SerializeField] private GameObject portalPrefab;
    [SerializeField] private GameObject scenePortal; // Đối tượng portal có sẵn trong scene

    private Boss_BorealController bossController;




    private void Awake()
    {
        currentHP = maxHP;
    }

    private void Start()
    {
        // Tìm ở chính nó hoặc cha (đề phòng collider ở đối tượng con)
        bossController = GetComponentInParent<Boss_BorealController>();

        if (bossController == null)
        {
            Debug.LogError("[BossHealth] Không tìm thấy Boss_BorealController trên GameObject này hoặc cha!");
        }


        Debug.Log($"Boss khởi tạo với {maxHP} HP");

        // Ẩn portal nếu được gán sẵn trong scene
        if (scenePortal != null)
        {
            scenePortal.SetActive(false);
        }
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

        if (portalPrefab != null)
        {
            Instantiate(portalPrefab, transform.position, Quaternion.identity);
        }

        // Hiện portal nếu có sẵn trong scene
        if (scenePortal != null)
        {
            scenePortal.SetActive(true);
        }

        enabled = false;

    }


    // Getter cho BossHealthBarUI
    public int GetCurrentHealth() => currentHP;
    public int GetMaxHealth() => maxHP;
    public bool IsDead() => currentHP <= 0;
}