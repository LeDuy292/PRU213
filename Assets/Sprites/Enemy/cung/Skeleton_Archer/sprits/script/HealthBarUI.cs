using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public Image hpFill;

    void Update()
    {
        if (playerHealth == null) return;

        float current = playerHealth.GetCurrentHealth();
        float max = playerHealth.GetMaxHealth();

        hpFill.fillAmount = current / max;
    }
}