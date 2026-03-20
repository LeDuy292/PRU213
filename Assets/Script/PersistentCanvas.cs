using UnityEngine;

public class PersistentCanvas : MonoBehaviour
{
    private static PersistentCanvas instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            
            // Kiểm tra xem có phải là Root Canvas không
            Canvas canvas = GetComponent<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("[PersistentCanvas] LỖI: Script này phải được gắn vào đối tượng có component Canvas (thường là Root Canvas)!");
            }

            if (transform.parent != null)
            {
                Debug.LogWarning("[PersistentCanvas] CẢNH BÁO: Đang gắn vào object con. Sẽ tự động tách ra làm Root để DontDestroyOnLoad hoạt động.");
                transform.SetParent(null);
            }
            
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Nếu đã có một Canvas persistent khác, xóa Canvas mới này đi để tránh trùng lặp HUD
            Destroy(gameObject);
        }
    }
}
