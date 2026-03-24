using UnityEngine;

public class HideTutorial : MonoBehaviour
{
    public GameObject openingObject;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (openingObject != null)
            {
                openingObject.SetActive(false); // Ẩn đi
                Debug.Log("Đã ẩn hướng dẫn tại điểm đích");
            }

            // Tự hủy Trigger này để không chạy lại lần sau
            Destroy(gameObject);
        }
    }
}