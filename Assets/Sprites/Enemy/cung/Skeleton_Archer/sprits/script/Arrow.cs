using UnityEngine;
public class Arrow : MonoBehaviour
{
    public float speed = 8f;
    public float lifeTime = 3f;
    public int damage = 10;
    
    private Vector2 targetDirection;

    void Start()
    {
        // Tìm hướng đến player tại thời điểm tạo mũi tên
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector2 direction = (player.transform.position - transform.position).normalized;
            targetDirection = direction;
            
            // Quay mũi tên về hướng player
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            // Nếu không tìm thấy player, bắn sang phải
            targetDirection = Vector2.right;
        }

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Di chuyển theo hướng đã xác định (không thay đổi)
        transform.Translate(targetDirection * speed * Time.deltaTime);
    }

    // ================== HÀM GÂY DAMAGE ==================
    private void TryDamage(GameObject target)
    {

        if (target.CompareTag("Player"))
        {

            PlayerHealth playerHealth =
                target.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                // Nếu player CHƯA chết
                if (!playerHealth.IsDead)
                {
                    playerHealth.TakeDamage(damage);
                    Destroy(gameObject);
                }
                else
                {
                    // Player đã chết → không gây damage

                    Debug.Log("Arrow ignored dead player");
                }
            }
            else
            {
                // Có Tag Player nhưng không có PlayerHealth
                Destroy(gameObject);
            }
        }
        // Va chạm với object KHÔNG phải Player -> không làm gì (xuyên qua)
    }

    // ================== VA CHẠM DẠNG COLLISION ==================
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Chỉ xử lý va chạm với Player
        if (collision.gameObject.CompareTag("Player"))
        {
            TryDamage(collision.gameObject);
        }
        // Va chạm với object khác -> không làm gì (xuyên qua)
    }

    // ================== VA CHẠM DẠNG TRIGGER ==================
    void OnTriggerEnter2D(Collider2D other)
    {
        // Nếu trigger trúng Player
        if (other.CompareTag("Player"))
        {
            TryDamage(other.gameObject);
        }
        // Va chạm với Ground hoặc object khác -> không làm gì (xuyên qua)
    }
}