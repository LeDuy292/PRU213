using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    private Vector2 direction;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;

        // 🔥 TÍNH GÓC XOAY THEO HƯỚNG BAY
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle + 180f);

        Destroy(gameObject, 5f);
    }

    private void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }
}
