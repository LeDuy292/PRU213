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