using UnityEngine;
using TMPro;

public class EnemyManager : MonoBehaviour
{
    public int enemyCount;
    public GameObject teleport;
    // Xóa playerExp, playerLevel, expToNextLevel, playerAttack, playerHP, levelText, expBar ở đây vì đã có PlayerLevel quản lý
    // levelUpEffect cũng chuyển sang PlayerLevel

    private PlayerController playerController;
    private PlayerHealth playerHealth;

    private void Start()
    {
        if (teleport != null) 
        {
            teleport.SetActive(false);
        }

        playerController = Object.FindFirstObjectByType<PlayerController>();
        playerHealth = Object.FindFirstObjectByType<PlayerHealth>();
    }

    public void RegisterEnemy()
    {
        enemyCount++;
        Debug.Log("Enemy Spawn → count = " + enemyCount);
    }

    public void EnemyDied(int exp)
    {
        enemyCount--;
        AddExp(exp);

        if (enemyCount <= 0 && teleport != null)
        {
            teleport.SetActive(true);
        }
    }

    void AddExp(int exp)
    {
        // Giao việc cho PlayerLevel quản lý
        PlayerLevel playerLvl = Object.FindFirstObjectByType<PlayerLevel>();
        if (playerLvl != null)
        {
            playerLvl.GainExp(exp);
        }
        else
        {
            Debug.LogWarning("[EnemyManager] LỖI: Không tìm thấy script PlayerLevel trên Player!");
        }
    }
}