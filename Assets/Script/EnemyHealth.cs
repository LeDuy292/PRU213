using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHP = 100;
    private int currentHP;
    [SerializeField] private EnemyManager enemyManager;

    void Start()
    {
        currentHP = maxHP;
        enemyManager = FindObjectOfType<EnemyManager>();
        enemyManager.RegisterEnemy();
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("Enemy take damage: " + damage);

        currentHP -= damage;

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
        enemyManager.EnemyDied();   // 🔥 DÒNG QUAN TRỌNG

    }
}
