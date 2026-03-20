using UnityEngine;

public class SkillDamage : MonoBehaviour
{
    public int damage;          // damage thay đổi
    public float lifeTime = 0.5f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Chỉ dùng tag Enemy để tránh lỗi nếu user chưa tạo tag Boss
        if (other.CompareTag("Enemy"))
        {
            // Thử lấy EnemyHealth trước (cho quái thường)
            EnemyHealth normalEnemy = other.GetComponentInParent<EnemyHealth>();
            if (normalEnemy != null)
            {
                normalEnemy.TakeDamage(damage);
                return;
            }

            // Thử lấy BossHealth (cho Boss)
            BossHealth boss = other.GetComponentInParent<BossHealth>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
                return;
            }
            
            Debug.LogWarning($"[SkillDamage] Va chạm với {other.name} nhưng không tìm thấy health script!");
        }
    }
}


