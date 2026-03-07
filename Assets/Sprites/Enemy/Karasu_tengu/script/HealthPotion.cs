using UnityEngine;


[CreateAssetMenu(menuName = "Inventory/Health Potion")]
public class HealthPotion : Item
{
    [Header("Health Potion Info")]
    public int healAmount = 20;      // Số HP hồi phục

    
    public override void Use()
    { 
        // Tìm player trong scene
        PlayerHealth player = FindObjectOfType<PlayerHealth>();
        if (player != null)
        {
            // Gọi method Heal() của player
            player.Heal(healAmount);
            Debug.Log($"Hồi phục {healAmount} HP");
        }
        else
        {
            Debug.LogWarning("Không tìm thấy PlayerHealth component!");
        }
    }
}
