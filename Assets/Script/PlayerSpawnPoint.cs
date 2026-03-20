using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    private void Start()
    {
        // Nháy mắt tìm Player
        PlayerController player = Object.FindFirstObjectByType<PlayerController>();

        if (player != null)
        {
            // Dịch chuyển Player tới vị trí của SpawnPoint này
            player.transform.position = transform.position;
            Debug.Log($"[SpawnPoint] Đã dịch chuyển Player tới {transform.position}");
        }
        else
        {
            Debug.LogWarning("[SpawnPoint] Không tìm thấy Player trong Scene này!");
        }
    }
}
