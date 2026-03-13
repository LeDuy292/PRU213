using UnityEngine;

public class FallReset : MonoBehaviour
{
    [Header("Kéo GameObject RespawnPoint vào đây")]
    public Transform respawnPoint;

    // Hàm OnTriggerEnter2D tự động kích hoạt khi có một Collider (có Rigidbody2D) đi vào vùng Is Trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra xem đối tượng va chạm có mang Tag "Player" không
        if (collision.CompareTag("Player"))
        {
            // Dịch chuyển vị trí của Player về điểm hồi sinh
            collision.transform.position = respawnPoint.position;

            // Xóa gia tốc rơi tự do của nhân vật để khi hồi sinh không bị rớt xuống tiếp
            Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.linearVelocity = Vector2.zero;
            }

            Debug.Log("Nhân vật đã rớt xuống vực và được reset vị trí!");
        }
    }
}