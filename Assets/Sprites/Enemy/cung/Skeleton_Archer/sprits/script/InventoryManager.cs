using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public InventorySlot[] slots;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Chỉ gọi DontDestroyOnLoad nếu object này là root (không có cha)
            if (transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool AddItem(Item item)
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.IsEmpty())
            {
                slot.AddItem(item);
                return true;
            }
        }

        Debug.Log("Inventory full!");
        return false;
    }

    public void DropItem(Item item, Vector3 position)
    {
        GameObject obj = Instantiate(item.worldPrefab, position, Quaternion.identity);

        ItemPickup pickup = obj.GetComponent<ItemPickup>();

        if (pickup != null)
        {
            pickup.item = item;
            pickup.pickupDelay = 4f; // player vứt → 4 giây
        }
    }
}