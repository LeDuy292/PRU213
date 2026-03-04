using UnityEngine;

public class InventoryTester : MonoBehaviour
{
    public InventorySlot slot1;
    public InventorySlot slot2;

    public Item healItem;
    public Item speedItem;
    public Item testItem;
    void Start()
    {
        slot1.AddItem(testItem );   // Slot 1 = Heal
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            slot1.UseItem();  // dùng heal
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            slot2.UseItem();  // dùng speed
        }
    }
}