using UnityEngine;
using UnityEngine.UI;

public class BossHealthBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BossHealth bossHealth;
    [SerializeField] private Image hpFill;

    [Header("Smooth Animation")]
    [SerializeField] private bool useSmoothFill = true;
    [SerializeField] private float smoothSpeed = 10f;   // càng cao càng mượt nhanh

    private float targetFill = 1f;

    private void Start()
    {
        if (bossHealth == null)
        {
            Debug.LogError("[BossHealthBarUI] Chưa gán BossHealth!");
            return;
        }
        if (hpFill == null)
        {
            Debug.LogError("[BossHealthBarUI] Chưa gán Image hpFill!");
            return;
        }

        // Khởi tạo đúng 100% lần đầu
        hpFill.fillAmount = 1f;
        targetFill = 1f;
    }

    private void Update()
    {
        if (bossHealth == null || hpFill == null) return;

        float current = bossHealth.GetCurrentHealth();
        float max = bossHealth.GetMaxHealth();

        if (max <= 0) return;

        float newFill = current / (float)max;

        if (useSmoothFill)
        {
            targetFill = newFill;
            hpFill.fillAmount = Mathf.Lerp(hpFill.fillAmount, targetFill, Time.deltaTime * smoothSpeed);
        }
        else
        {
            hpFill.fillAmount = newFill;
        }
    }
}