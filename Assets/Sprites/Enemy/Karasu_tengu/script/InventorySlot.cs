using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    public Button button;

    [Header("Debug")]
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

        if (icon != null)
        {
            icon.sprite = item.icon;
            icon.enabled = true;
        }
    }

    void OnClick()
    {
        if (currentItem == null) return;

        if (ItemActionUI.Instance != null)
        {
            ItemActionUI.Instance.Show(this);
        }
    }

    // DÙNG ITEM (Potion)
    public void UseItem()
    {
        if (currentItem == null) return;

        if (currentItem.itemType == ItemType.Consumable)
        {
            currentItem.Use();
            RemoveItem();
        }
    }

    // EQUIP ITEM
    public void EquipItem()
    {
        if (currentItem == null) return;

        if (currentItem.itemType == ItemType.Consumable)
        {
            Debug.Log("Consumable không thể equip!");
            return;
        }

        if (EquipmentManager.Instance == null)
        {
            Debug.LogError("EquipmentManager not found in scene!");
            return;
        }

        bool equipped = EquipmentManager.Instance.Equip(currentItem);

        if (equipped)
        {
            RemoveItem();
        }
    }

    // DROP ITEM
    public void DropItem()
    {
        if (currentItem == null) return;

        if (InventoryManager.Instance == null)
        {
            Debug.LogError("InventoryManager not found!");
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("Player not found!");
            return;
        }

        Vector3 dropPos = player.transform.position;

        InventoryManager.Instance.DropItem(currentItem, dropPos);

        RemoveItem();
    }

    public void RemoveItem()
    {
        currentItem = null;

        if (icon != null)
        {
            icon.sprite = null;
            icon.enabled = false;
        }
    }

    public bool IsEmpty()
    {
        return currentItem == null;
    }

    public Item GetItem()
    {
        return currentItem;
    }
}