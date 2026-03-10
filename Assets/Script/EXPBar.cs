using UnityEngine;
using UnityEngine.UI;

public class EXPBar : MonoBehaviour
{
    public Image fillImage; // Hỗ trợ Image fill giống HealthBar

    private float maxExp;

    public void SetMaxExp(int exp)
    {
        maxExp = exp;
        Debug.Log($"[EXPBar] SetMaxExp called -> New Max: {maxExp}");
    }

    public void SetExp(int exp)
    {
        Debug.Log($"[EXPBar] SetExp called -> Current EXP: {exp} / {maxExp}");
        if (fillImage != null && maxExp > 0)
        {
            fillImage.fillAmount = (float)exp / maxExp;
            Debug.Log($"[EXPBar] fillAmount updated -> {fillImage.fillAmount}");
        }
        else
        {
            if (fillImage == null) Debug.LogWarning("[EXPBar] LỖI: fillImage chưa được gắn trong Inspector!");
            if (maxExp <= 0) Debug.LogWarning("[EXPBar] LỖI: maxExp đang <= 0. Chưa gọi SetMaxExp()?");
        }
    }
}