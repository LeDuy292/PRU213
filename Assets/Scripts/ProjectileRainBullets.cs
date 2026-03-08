using UnityEngine;

public class ProjectileRainBullets : MonoBehaviour
{
    [SerializeField] public float speed = 1f; // Tốc độ ban đầu (rơi nhanh)

    [Header("Rotation Fix for Rain")]
    [SerializeField] [Range(-360f, 360f)] public float rainRotationOffset = 0; // ← Offset linh hoạt, chỉnh trong Inspector

    private Vector2 direction;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;

        // Tính góc hướng bay
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Áp dụng offset đặc biệt cho mưa đạn (xoay để đầu hướng xuống)
        float finalAngle = angle + rainRotationOffset;

        transform.rotation = Quaternion.Euler(0f, 0f, finalAngle);

        // Tự hủy sau 6 giây
        Destroy(gameObject, 6f);
    }

    private void Update()
    {
        // Di chuyển theo hướng (thường là Vector2.down)
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }
}