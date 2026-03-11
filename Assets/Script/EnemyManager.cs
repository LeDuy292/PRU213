using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public int enemyCount;
    public GameObject teleport;
    public EXPBar expBar;
    public int playerExp;
    public int playerLevel = 1;
    public int expToNextLevel = 100;
    public ParticleSystem levelUpEffect;
    public int playerAttack = 10;
    public int playerHP = 100;
    public UnityEngine.UI.Text levelText; // Hiển thị số cấp độ

    private PlayerController playerController;
    private PlayerHealth playerHealth;

    private void Start()
    {
        if (teleport != null) 
        {
            teleport.SetActive(false);
        }
        if (expBar != null)
        {
            expBar.SetMaxExp(expToNextLevel);
            Debug.Log("[EnemyManager] Start: Found expBar, initialized MaxExp.");
        }
        else
        {
            Debug.LogWarning("[EnemyManager] LỖI: expBar đang bị NULL! Bạn chưa kéo EXPBar vào EnemyManager!");
        }

        playerController = Object.FindFirstObjectByType<PlayerController>();
        playerHealth = Object.FindFirstObjectByType<PlayerHealth>();

        UpdateLevelText();
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
        playerExp += exp;
        
        if (expBar != null)
        {
            expBar.SetExp(playerExp);
            Debug.Log($"[EnemyManager] Đã gửi playerExp ({playerExp}) cho expBar");
        }
        else
        {
            Debug.LogWarning("[EnemyManager] LỖI: Không truyền điểm cho EXPBar được vì expBar bị NULL.");
        }

        while (playerExp >= expToNextLevel)
        {
            LevelUp();
            if (expBar != null)
            {
                expBar.SetMaxExp(expToNextLevel);
            }
        }

        Debug.Log("Player EXP: " + playerExp);
    }

    void LevelUp()
    {
        playerLevel++;
        playerExp -= expToNextLevel;
        expToNextLevel += 50;

        playerAttack += 5;
        playerHP += 20;

        if (playerHealth != null)
        {
            playerHealth.AddBonusHealth(20);
            playerHealth.Heal(playerHealth.GetMaxHealth());
        }
        
        if (playerController != null)
        {
            playerController.AddBaseDamage(5);
        }

        if (levelUpEffect != null)
        {
            levelUpEffect.Play();
        }
        Debug.Log("LEVEL UP → Level " + playerLevel);
        UpdateLevelText();
    }

    void UpdateLevelText()
    {
        if (levelText != null)
        {
            levelText.text = playerLevel.ToString(); // Chỉ in ra con số, ví dụ "1"
        }
    }
}