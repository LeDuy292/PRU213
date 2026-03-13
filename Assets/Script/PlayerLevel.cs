using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    [Header("Stats")]
    public int level = 1;
    public int currentExp = 0;
    public int expToNextLevel = 100;

    [Header("Components")]
    public EXPBar[] expBars; // Các thanh EXP trên HUD
    public ParticleSystem levelUpEffect;

    private PlayerController playerController;
    private PlayerHealth playerHealth;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        playerHealth = GetComponent<PlayerHealth>();

        // Khởi tạo HUD ban đầu
        if (expBars == null || expBars.Length == 0)
        {
            Debug.LogError("[PlayerLevel] LỖI: Bạn chưa kéo thanh EXP vào danh sách Exp Bars trên Player!");
        }
        else
        {
            Debug.Log($"[PlayerLevel] Đã tìm thấy {expBars.Length} thanh EXP để cập nhật.");
        }

        UpdateUI();
    }

    public void GainExp(int amount)
    {
        currentExp += amount;
        Debug.Log("Nhận EXP: " + amount);
        
        while (currentExp >= expToNextLevel)
        {
            LevelUp();
        }

        // Cập nhật thanh EXP trên HUD
        foreach (var bar in expBars)
        {
            if (bar != null) bar.SetExp(currentExp);
        }
    }

    void LevelUp()
    {
        level++;
        currentExp -= expToNextLevel;
        expToNextLevel += 50; 

        // Tăng chỉ số Player
        if (playerHealth != null)
        {
            playerHealth.AddBonusHealth(20); 
            playerHealth.Heal(playerHealth.GetMaxHealth()); 
        }
        
        if (playerController != null)
        {
            playerController.AddBaseDamage(5); 
        }

        // Hiệu ứng và UI
        if (levelUpEffect != null) levelUpEffect.Play();
        
        UpdateUI();
        Debug.Log("LEVEL UP! Level hiện tại: " + level);
    }

    void UpdateUI()
    {
        foreach (var bar in expBars)
        {
            if (bar != null)
            {
                bar.SetMaxExp(expToNextLevel);
                bar.SetExp(currentExp);
                bar.SetLevel(level);
            }
        }
    }
}