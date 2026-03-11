using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public InventorySlot[] slots;

    void Awake()
    {
        Instance = this;
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
}