using UnityEngine;
using UnityEngine.UI;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager Instance;

    [Header("UI Slots")]
    public Image weaponSlot;
    public Image helmetSlot;
    public Image armorSlot;
    public Image bootsSlot;
    public Image ringSlot;

    private Item currentWeapon;
    private Item currentHelmet;
    private Item currentArmor;
    private Item currentBoots;
    private Item currentRing;

    private PlayerHealth playerHealth;
    private PlayerController playerController; // ✅ thêm dòng này

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        playerHealth = Object.FindFirstObjectByType<PlayerHealth>();
        playerController = Object.FindFirstObjectByType<PlayerController>(); // ✅ thêm dòng này
    }

    // ================= EQUIP =================
    public bool Equip(Item item)
    {
        switch (item.itemType)
        {
            case ItemType.Weapon:
                return EquipToSlot(item, ref currentWeapon, weaponSlot);

            case ItemType.Armor:
                return EquipToSlot(item, ref currentArmor, armorSlot);

            case ItemType.Helmet:
                return EquipToSlot(item, ref currentHelmet, helmetSlot);

            case ItemType.Boots:
                return EquipToSlot(item, ref currentBoots, bootsSlot);

            case ItemType.Ring:
                return EquipToSlot(item, ref currentRing, ringSlot);
        }

        return false;
    }

    bool EquipToSlot(Item newItem, ref Item currentItem, Image slotImage)
    {
        if (currentItem != null)
        {
            Item oldItem = currentItem;

            RemoveBonus(oldItem);

            currentItem = newItem;

            slotImage.sprite = newItem.icon;
            slotImage.enabled = true;

            AddBonus(newItem);

            InventoryManager.Instance.AddItem(oldItem);

            return true;
        }

        currentItem = newItem;
        slotImage.sprite = newItem.icon;
        slotImage.enabled = true;

        AddBonus(newItem);

        return true;
    }

    // ================= UNEQUIP =================
    public void Unequip(ItemType type)
    {
        switch (type)
        {
            case ItemType.Weapon:
                UnequipFromSlot(ref currentWeapon, weaponSlot);
                break;

            case ItemType.Helmet:
                UnequipFromSlot(ref currentHelmet, helmetSlot);
                break;

            case ItemType.Armor:
                UnequipFromSlot(ref currentArmor, armorSlot);
                break;

            case ItemType.Boots:
                UnequipFromSlot(ref currentBoots, bootsSlot);
                break;

            case ItemType.Ring:
                UnequipFromSlot(ref currentRing, ringSlot);
                break;
        }
    }

    void UnequipFromSlot(ref Item currentItem, Image slotImage)
    {
        if (currentItem == null)
            return;

        bool added = InventoryManager.Instance.AddItem(currentItem);

        if (!added)
        {
            Debug.Log("Inventory full! Cannot unequip.");
            return;
        }

        RemoveBonus(currentItem);

        currentItem = null;

        if (slotImage != null)
        {
            slotImage.sprite = null;
            slotImage.enabled = false;
        }
    }

    // ================= BONUS SYSTEM =================
    private void AddBonus(Item item)
    {
        if (item.bonusHealth > 0)
        {
            playerHealth.AddBonusHealth(item.bonusHealth);
        }

        if (item.bonusMoveSpeed != 0)
        {
            playerController.AddSpeed(item.bonusMoveSpeed);
        }
        if (item.bonusNormalDamagePercent != 0)
        {
            playerController.AddNormalDamagePercent(item.bonusNormalDamagePercent);
        }

        if (item.bonusSkillDamagePercent != 0)
        {
            playerController.AddSkillDamagePercent(item.bonusSkillDamagePercent);
        }
    }

    private void RemoveBonus(Item item)
    {
        if (item.bonusHealth > 0)
        {
            playerHealth.RemoveBonusHealth(item.bonusHealth);
        }

        if (item.bonusMoveSpeed != 0)
        {
            playerController.RemoveSpeed(item.bonusMoveSpeed);
        }
        if (item.bonusNormalDamagePercent != 0)
        {
            playerController.AddNormalDamagePercent(item.bonusNormalDamagePercent);
        }

        if (item.bonusSkillDamagePercent != 0)
        {
            playerController.AddSkillDamagePercent(item.bonusSkillDamagePercent);
        }
    }
}