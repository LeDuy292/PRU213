using UnityEngine;

public class EquipmentSlot : MonoBehaviour
{
    public ItemType slotType;

    public void OnClickSlot()
    {
        EquipmentManager.Instance.Unequip(slotType);
    }
}