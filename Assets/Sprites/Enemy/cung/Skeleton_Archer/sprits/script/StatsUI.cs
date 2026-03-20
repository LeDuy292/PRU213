using UnityEngine;
using TMPro;

public class StatsUI : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public PlayerStats playerStats;
    public PlayerController playerController;
    public TextMeshProUGUI txtHealth;
    public TextMeshProUGUI txtLevel;
    public TextMeshProUGUI txtEnergy;
    public TextMeshProUGUI txtSpeed;
    public TextMeshProUGUI txtDamage;

    void Update()
    {
        if (playerHealth == null) playerHealth = UnityEngine.Object.FindFirstObjectByType<PlayerHealth>();
        if (playerStats == null) playerStats = UnityEngine.Object.FindFirstObjectByType<PlayerStats>();
        if (playerController == null) playerController = UnityEngine.Object.FindFirstObjectByType<PlayerController>();

        if (playerHealth == null || playerStats == null || playerController == null) return;

        txtHealth.text = "Máu: "
            + playerHealth.GetCurrentHealth()
            + "/"
            + playerHealth.GetMaxHealth();

        txtLevel.text = "Cấp: " + playerStats.level;
        txtEnergy.text = "EXE: "
            + playerStats.currentEnergy
            + "/"
            + playerStats.maxEnergy;

        txtSpeed.text = "Tốc độ: " + playerController.Speed;
        txtDamage.text = "Tấn công: " + playerController.Attack;
    }
}