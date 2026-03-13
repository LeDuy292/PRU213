using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item;

    public float pickupDelay = 2f; // mặc định: quái rơi
    private bool canPickup = false;

    void Start()
    {
        Invoke(nameof(EnablePickup), pickupDelay);
    }

    void EnablePickup()
    {
        canPickup = true;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canPickup) return;

        if (collision.CompareTag("Player"))
        {
            bool picked = InventoryManager.Instance.AddItem(item);

            if (picked)
            {
                Destroy(gameObject);
            }
        }
    }
}