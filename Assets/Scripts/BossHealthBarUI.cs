using UnityEngine;
using UnityEngine.UI;

public class BossHealthBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BossHealth bossHealth;     // Kéo component BossHealth của Boss vào đây
    [SerializeField] private Image hpFill;              // Image fill của thanh máu (phải set Image Type = Filled)

    [Header("Optional - Smooth Fill (mượt hơn)")]
    [SerializeField] private bool smoothFill = true;    // Có làm mượt thanh máu không
    [SerializeField] private float smoothSpeed = 5f;    // Tốc độ mượt (càng cao càng nhanh)

    private float targetFillAmount = 1f;

    private void Start()
    {
        if (bossHealth == null)
        {
            Debug.LogWarning("[BossHealthBarUI] BossHealth chưa được gán! Kéo Boss vào Inspector.");
            return;
        }

        if (hpFill == null)
        {
            Debug.LogWarning("[BossHealthBarUI] hpFill Image chưa được gán!");
            return;
        }

        // Khởi tạo ban đầu
        hpFill.fillAmount = 1f;
        targetFillAmount = 1f;
    }

    private void Update()
    {
        if (bossHealth == null || hpFill == null) return;

        float current = bossHealth.CurrentHP;
        float max = bossHealth.MaxHP;

        if (max <= 0) return; // tránh chia cho 0

        float newFill = current / max;

        if (smoothFill)
        {
            // Làm mượt thanh máu (tránh nhảy đột ngột)
            targetFillAmount = newFill;
            hpFill.fillAmount = Mathf.Lerp(hpFill.fillAmount, targetFillAmount, Time.deltaTime * smoothSpeed);
        }
        else
        {
            // Cập nhật trực tiếp (nhanh, không mượt)
            hpFill.fillAmount = newFill;
        }
    }

    // Optional: Gọi khi boss nhận damage để flash hoặc hiệu ứng (nếu muốn sau này)
    public void FlashHealthBar()
    {
        // Ví dụ: đổi màu tạm thời thành đỏ, sau đó về lại
        // Bạn có thể implement sau nếu cần
    }
}