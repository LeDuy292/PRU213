using UnityEngine;

[CreateAssetMenu(fileName = "NewConsumable", menuName = "Inventory/Consumable")]
public class ConsumableItem : Item
{
    public int healAmount;

    public override void Use()
    {
        // Nếu không phải Consumable thì không được dùng kiểu hồi máu
        if (itemType != ItemType.Consumable)
        {
            Debug.Log("Đây là trang bị, không phải consumable: " + itemName);
            return;
        }

        PlayerHealth player = FindObjectOfType<PlayerHealth>();

        if (player != null)
        {
            player.Heal(healAmount);
            Debug.Log("Đã dùng: " + itemName);
        }
    }
}