using UnityEngine;

public class TeleportToBoss : MonoBehaviour
{
    [SerializeField] private Transform bossSpawnPoint;
    [SerializeField] private GameObject bossMap;

    private bool used = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (used) return;

        if (other.CompareTag("Player"))
        {
            used = true;

            // bật map boss
            if (bossMap != null)
                bossMap.SetActive(true);

            // dịch chuyển player
            other.transform.position = bossSpawnPoint.position;
        }
    }
}
