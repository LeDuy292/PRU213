using UnityEngine;

public enum ItemType
{
    Consumable,
    Weapon,
    Helmet,
    Armor,
    Boots,
    Ring
}

[CreateAssetMenu(menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public ItemType itemType;

    [Header("Equipment")]
    public int bonusHealth;
  
    public float bonusMoveSpeed;   // 👈 thêm dòng này
    [Header("Damage Bonus")]
    public float bonusNormalDamagePercent;
    public float bonusSkillDamagePercent;

    public virtual void Use()
    {
        if (itemType != ItemType.Consumable)
        {
            EquipmentManager.Instance.Equip(this);
            return;
        }

        Debug.Log("Dùng item thường: " + itemName);
    }
}