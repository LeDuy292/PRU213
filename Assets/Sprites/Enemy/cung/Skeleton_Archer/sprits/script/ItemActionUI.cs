using UnityEngine;

public class ItemActionUI : MonoBehaviour
{
    public static ItemActionUI Instance;

    private InventorySlot currentSlot;

    void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void Show(InventorySlot slot)
    {
        currentSlot = slot;
        gameObject.SetActive(true);
    }

    public void UseItem()
    {
        if (currentSlot != null)
            currentSlot.UseItem();

        gameObject.SetActive(false);
    }

    public void EquipItem()
    {
        if (currentSlot != null)
            currentSlot.EquipItem();

        gameObject.SetActive(false);
    }

    public void DropItem()
    {
        if (currentSlot != null)
            currentSlot.DropItem();

        gameObject.SetActive(false);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}