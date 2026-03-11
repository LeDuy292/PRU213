using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHP = 100;
    private int currentHP;

    [SerializeField] private int expReward = 20;

    private EnemyManager enemyManager;

    void Start()
    {
        currentHP = maxHP;

        enemyManager = Object.FindFirstObjectByType<EnemyManager>();
        if (enemyManager != null)
        {
            enemyManager.RegisterEnemy();
        }
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
        if (enemyManager != null)
        {
            enemyManager.EnemyDied(expReward); // truyền EXP
        }
        
        Destroy(gameObject);
    }
}