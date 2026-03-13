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

    public int bonusHealth;
    public float bonusMoveSpeed;

    public float bonusNormalDamagePercent;
    public float bonusSkillDamagePercent;

    public GameObject worldPrefab;

    public virtual void Use()
    {
        if (itemType == ItemType.Consumable)
        {
            Debug.Log("Use item: " + itemName);
            return;
        }

        EquipmentManager.Instance.Equip(this);
    }
}