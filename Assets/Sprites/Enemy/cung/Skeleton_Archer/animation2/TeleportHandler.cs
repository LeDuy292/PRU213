using UnityEngine;

public class Teleport1 : MonoBehaviour
{
    [Header("Điểm đến (Kéo Object đích vào đây)")]
    public Transform destination;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra nếu là Player chạm vào cổng
        if (other.CompareTag("Player"))
        {
            // Dịch chuyển vị trí của Player tới vị trí của điểm đích
            other.transform.position = destination.position;

            Debug.Log("Đã dịch chuyển tới: " + destination.name);
        }
    }
}