using UnityEngine;

public class Arrow : MonoBehaviour
{
    [Header("Arrow Settings")]
    public float speed = 8f;
    public float lifeTime = 3f;
    public int damage = 10;

    private Vector2 direction;

    void Start()
    {
        // Tìm player
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            // Tính hướng tới player
            direction = (player.transform.position - transform.position).normalized;

            // Xoay mũi tên theo hướng bay
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            // Nếu không có player → bắn sang phải
            direction = Vector2.right;
        }

        // Tự hủy sau vài giây
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Bay theo hướng đã xác định
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    // ================== GÂY DAMAGE ==================
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null && !playerHealth.IsDead)
            {
                playerHealth.TakeDamage(damage);
            }

            // Hủy mũi tên khi trúng player
            Destroy(gameObject);
        }
    }
}