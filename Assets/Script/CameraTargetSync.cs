using UnityEngine;
using Unity.Cinemachine; 

public class CameraTargetSync : MonoBehaviour
{
    public void SetTarget(Transform playerTransform)
    {
        if (playerTransform == null) return;

        var cam6 = GetComponent<CinemachineCamera>();
        if (cam6 == null) cam6 = GetComponentInChildren<CinemachineCamera>();
        
        if (cam6 != null)
        {
            cam6.Follow = playerTransform;
            Debug.Log($"[CameraSync] Đã gán {playerTransform.name} làm mục tiêu theo dõi.");
            return;
        }

        Debug.LogWarning("[CameraSync] Không tìm thấy component Cinemachine trên " + gameObject.name);
    }

    private void Start()
    {
        // Ưu tiên dùng instance của PlayerController để không bị nhầm với Boss
        if (PlayerController.instance != null)
        {
            SetTarget(PlayerController.instance.transform);
        }
        else
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                SetTarget(player.transform);
            }
        }
    }
}
