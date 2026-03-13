using UnityEngine;

public class ProjectileBlizzardVortex : MonoBehaviour
{
    [SerializeField] public float speed = 4f;  // Tốc độ chậm hơn nhiều (so với đạn thường 8f)

    private Vector2 direction;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;

     

        // Tự hủy sau 18 giây (lâu hơn vì tốc độ chậm)
        Destroy(gameObject, 18f);
    }

    private void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[BlizzardVortex] Trigger với: Tag={other.tag}, Name={other.gameObject.name}");
        if (other.CompareTag("Player"))
        {
            Debug.Log("[BlizzardVortex] → Trúng PLAYER! Gây 30 damage.");
            PlayerHealth ph = other.GetComponent<PlayerHealth>();
            if (ph != null) ph.TakeDamage(100);
            Destroy(gameObject);
        }
    }
}