using UnityEngine;
public class Arrow : MonoBehaviour
{
    public float speed = 8f;
    public float lifeTime = 3f;
    public int damage = 10;

    void Start()
    {

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Di chuyển mũi tên theo trục phải (local right)

        transform.Translate(
            Vector2.right * speed * Time.deltaTime
        );
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
        else
        {
            // Va chạm với object KHÔNG phải Player
            Destroy(gameObject);
        }
    }

    // ================== VA CHẠM DẠNG COLLISION ==================
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Gọi hàm TryDamage với object vừa va chạm
        TryDamage(collision.gameObject);
    }

    // ================== VA CHẠM DẠNG TRIGGER ==================
    void OnTriggerEnter2D(Collider2D other)
    {
        // Nếu trigger trúng Player
        if (other.CompareTag("Player"))
        {
            TryDamage(other.gameObject);
        }
        // Nếu trúng Ground (layer Ground)
        else if (other.gameObject.layer ==
                 LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject); // Hủy mũi tên khi chạm đất
        }
    }
}