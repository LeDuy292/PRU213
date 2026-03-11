using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    public Button button;
    public Item debugItem;

    private Item currentItem;

    void Start()
    {
        if (icon != null)
            icon.enabled = false;

        if (button != null)
            button.onClick.AddListener(OnClick);

        if (debugItem != null)
            AddItem(debugItem);
    }

    public void AddItem(Item item)
    {
        currentItem = item;
        icon.sprite = item.icon;
        icon.enabled = true;
    }

    void OnClick()
    {
        if (currentItem == null)
            return;

        if (currentItem.itemType == ItemType.Consumable)
        {
            currentItem.Use();
            RemoveItem();
        }
        else
        {
            bool equipped = EquipmentManager.Instance.Equip(currentItem);

            if (equipped)
            {
                RemoveItem();
            }
        }
    }

    public void RemoveItem()
    {
        currentItem = null;
        icon.sprite = null;
        icon.enabled = false;
    }

    public bool IsEmpty()
    {
        return currentItem == null;
    }

    public void UseItem()
    {
        if (currentItem == null) return;

        if (currentItem.itemType != ItemType.Consumable)
        {
            bool equipped = EquipmentManager.Instance.Equip(currentItem);

            if (equipped)
            {
                RemoveItem(); // chỉ xóa khi equip thành công
            }
        }
    }
}