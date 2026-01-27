using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 8f;
    public float lifeTime = 3f;
    public int damage = 10; // Damage amount

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void TryDamage(GameObject target)
    {
        if (target.CompareTag("Player"))
        {
            PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                // Chỉ gây damage nếu player còn sống
                if (!playerHealth.IsDead)
                {
                    playerHealth.TakeDamage(damage);
                    Destroy(gameObject); // Destroy arrow sau khi hit player sống
                }
                else
                {
                    // Player đã chết, arrow không gây damage và không bị destroy
                    Debug.Log("Arrow ignored dead player");
                }
            }
            else
            {
                // Không tìm thấy PlayerHealth component
                Destroy(gameObject);
            }
        }
        else
        {
            // Hit vào object khác (không phải player)
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        TryDamage(collision.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) 
        {
             TryDamage(other.gameObject);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Ground")) 
        {
             Destroy(gameObject);
        }
    }
}
