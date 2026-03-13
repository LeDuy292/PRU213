using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item; // ScriptableObject chứa dữ liệu item
    public float pickupDelay = 1f; // mặc định: quái rơi
    private bool canPickup = false;

    void Start()
    {
        // Chờ 1 thời gian trước khi cho phép quái nhặt (để item bay ra từ quái)
        Invoke(nameof(EnablePickup), pickupDelay);
    }

    void EnablePickup()
    {
        canPickup = true;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Nếu hiện tại chưa nhặt được thì thoát
        if (!canPickup) return;

        // Kiểm tra nếu chạm vào Player
        if (collision.CompareTag("Player"))
        {
            if (InventoryManager.Instance != null)
            {
                // Thêm vào inventory
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