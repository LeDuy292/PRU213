using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Health Potion")]
public class HealthPotion : Item
{
    public int healAmount = 20;

    public override void Use()
    { 
        // Tìm player và gọi Heal()
        PlayerHealth player = FindObjectOfType<PlayerHealth>();
        if (player != null)
        {
            player.Heal(healAmount);
        }
    }
}
