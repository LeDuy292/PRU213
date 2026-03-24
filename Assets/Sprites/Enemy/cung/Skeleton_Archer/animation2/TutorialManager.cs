using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public GameObject targetObject; // Kéo 'move' vào đây
    public bool shouldActivate;     // Tích vào nếu muốn BẬT, bỏ tích nếu muốn TẮT

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (targetObject != null)
            {
                targetObject.SetActive(shouldActivate); // Bật hoặc Tắt dấu tích
                Debug.Log("Đã " + (shouldActivate ? "Bật" : "Tắt") + " " + targetObject.name);
            }
        }
    }
}