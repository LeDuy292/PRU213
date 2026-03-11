using UnityEngine;

/// <summary>
/// Định nghĩa các loại item trong game
/// </summary>
public enum ItemType
{
    Consumable,    // Item tiêu thụ (potion, food)
    Weapon,        // Vũ khí (sword, bow)
    Helmet,        // Mũ (helmet, hat)
    Armor,         // Giáp (chestplate)
    Boots,         // Giày (boots)
    Ring           // Nhẫn (ring)
}


[CreateAssetMenu(menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    [Header("Thông tin cơ bản")]
    public string itemName;           // Tên hiển thị của item
    public Sprite icon;               // Icon hiển thị trong UI
    public ItemType itemType;          // Loại item

    [Header("Equipment Stats")]
    public int bonusHealth;           // +HP khi trang bị
    public float bonusMoveSpeed;      // +Tốc độ di chuyển khi trang bị
    
    [Header("Damage Bonus")]
    public float bonusNormalDamagePercent;   // +% damage đánh thường
    public float bonusSkillDamagePercent;    // +% damage skill

    
    public virtual void Use()
    {
        // Nếu không phải item tiêu thụ, thử trang bị
        if (itemType != ItemType.Consumable)
        {
            EquipmentManager.Instance.Equip(this);
            return;
        }

        // Item tiêu thụ sẽ override method này
        Debug.Log("Dùng item thường: " + itemName);
    }
}