using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item; // ScriptableObject chứa dữ liệu item

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra nếu chạm vào Player
        if (collision.CompareTag("Player"))
        {
            if (InventoryManager.Instance != null)
            {
                bool wasPickedUp = InventoryManager.Instance.AddItem(item);
                
                if (wasPickedUp)
                {
                    Debug.Log($"Nhặt được: {item.itemName}");
                    Destroy(gameObject); // Biến mất sau khi nhặt
                }
            }
            else
            {
                Debug.LogWarning("Không tìm thấy InventoryManager trong Scene!");
            }
        }
    }
}
