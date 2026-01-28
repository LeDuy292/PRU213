using UnityEngine;
using UnityEngine.UI;

public class SkillCooldownUI : MonoBehaviour
{
    [SerializeField] private Image cooldownImage;
    private float cooldownTime;
    private float timer;
    private bool isCooling;

    public void StartCooldown(float time)
    {
        cooldownTime = time;
        timer = time;
        isCooling = true;
        cooldownImage.fillAmount = 1;
    }

    private void Update()
    {
        if (!isCooling) return;

        timer -= Time.deltaTime;
        cooldownImage.fillAmount = timer / cooldownTime;

        if (timer <= 0)
        {
            cooldownImage.fillAmount = 0;
            isCooling = false;
        }
    }

    public bool IsReady()
    {
        return !isCooling;
    }
}
