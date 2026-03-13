using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] public float speed = 8f;
    private Vector2 direction;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;

        // 🔥 Xoay sprite theo hướng bay 
        // Giả sử sprite đạn trong prefab hướng về bên PHẢI (0 độ)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle + 0f);

        // Tự hủy sau 5 giây
        Destroy(gameObject, 5f);
    }

    private void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth ph = other.GetComponent<PlayerHealth>();
            if (ph != null) ph.TakeDamage(15);   // chỉnh số damage tùy ý
            Destroy(gameObject);
        }
    }
}