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
        if (other.CompareTag("Enemy"))   // Boss cũng phải có tag "Enemy"
        {
            // Hỗ trợ cả enemy nhỏ lẫn Boss
            EnemyHealth normalEnemy = other.GetComponent<EnemyHealth>();
            if (normalEnemy != null)
            {
                normalEnemy.TakeDamage(damage);
                return;
            }

            BossHealth boss = other.GetComponent<BossHealth>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
                return;
            }
        }
    }
}
