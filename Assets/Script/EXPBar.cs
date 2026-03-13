using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EXPBar : MonoBehaviour
{
    public Image fillImage; 
    public TextMeshProUGUI levelText;  
    public TextMeshProUGUI expText;    

    [Header("Animation")]
    public float lerpSpeed = 5f;
    private float targetFillAmount;
    private float maxExp;

    void Start()
    {
        // Khởi tạo targetFillAmount theo giá trị hiện tại của ảnh để tránh bị nhảy về 0 lúc đầu
        if (fillImage != null)
        {
            targetFillAmount = fillImage.fillAmount;
        }
    }

    void Update()
    {
        if (fillImage != null)
        {
            float oldAmount = fillImage.fillAmount;
            fillImage.fillAmount = Mathf.Lerp(fillImage.fillAmount, targetFillAmount, Time.deltaTime * lerpSpeed);
            
            // Nếu lerp đang chạy, có thể bật log này để debug (nên tắt khi xong)
            // if (Mathf.Abs(oldAmount - fillImage.fillAmount) > 0.001f) Debug.Log($"[EXPBar] Animating: {fillImage.fillAmount} -> {targetFillAmount}");
        }
    }

    public void SetMaxExp(int exp)
    {
        maxExp = exp;
        Debug.Log($"[EXPBar] SetMaxExp called -> New Max: {maxExp}");
    }

    public void SetExp(int exp)
    {
        if (maxExp > 0)
        {
            targetFillAmount = (float)exp / maxExp;
            Debug.Log($"[EXPBar] SetExp: {exp}/{maxExp} -> Target Fill: {targetFillAmount}");
        }
        else
        {
            Debug.LogWarning("[EXPBar] Cảnh báo: maxExp đang bằng 0, không thể tính tỉ lệ fill!");
        }

        if (expText != null)
        {
            expText.text = $"{exp} / {maxExp}";
        }
    }

    public void SetLevel(int level)
    {
        if (levelText != null)
        {
            levelText.text = "Lv. " + level;
        }
    }
}