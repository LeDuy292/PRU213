using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    private bool isDead = false;

    // Property để các script khác kiểm tra
    public bool IsDead => isDead;

    void Start()
    {
       
        currentHealth = maxHealth;
        isDead = false;
    }

    // ================== NHẬN DAMAGE ==================
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"Player took {damage} damage. Current Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // ================== HỒI MÁU (ITEM DÙNG Ở ĐÂY) ==================
    public void Heal(int amount)
    {
        if (isDead) return; // chết rồi thì không hồi

        currentHealth += amount;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        Debug.Log($"Player healed {amount}. Current Health: {currentHealth}");
        
        // Hiển thị text trên màn hình
        ShowHealText(amount);
    }

    private void ShowHealText(int amount)
    {
        // Tạo UI text hiển thị hồi máu
        GameObject healText = new GameObject("HealText");
        healText.transform.position = transform.position + Vector3.up * 2f;
        
        TextMesh textMesh = healText.AddComponent<TextMesh>();
        textMesh.text = $"+{amount} HP";
        textMesh.fontSize = 30;
        textMesh.color = Color.green;
        textMesh.anchor = TextAnchor.MiddleCenter;
        
        // Di chuyển text lên trên và mờ dần
        StartCoroutine(FadeAndMoveText(healText));
    }

    private System.Collections.IEnumerator FadeAndMoveText(GameObject textObj)
    {
        TextMesh textMesh = textObj.GetComponent<TextMesh>();
        Vector3 startPos = textObj.transform.position;
        Vector3 endPos = startPos + Vector3.up * 2f;
        
        float duration = 1.5f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            
            // Di chuyển lên trên
            textObj.transform.position = Vector3.Lerp(startPos, endPos, progress);
            
            // Mờ dần
            Color color = textMesh.color;
            color.a = 1f - progress;
            textMesh.color = color;
            
            yield return null;
        }
        
        Destroy(textObj);
    }

    // ================== CHẾT ==================
    private void Die()
    {
        if (isDead) return;
        
        isDead = true;
        Debug.Log("Player Died!");
        
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;
        
        Animator animator = GetComponent<Animator>();
        if (animator != null && animator.HasState(0, Animator.StringToHash("Die")))
        {
            animator.SetTrigger("Die");
            Destroy(gameObject, 1.5f);
        }
        else
        {
            Destroy(gameObject, 0.5f);
        }
    }
    public int GetCurrentHealth()
{
    return currentHealth;
}

public int GetMaxHealth()
{
    return maxHealth;
}
    public void AddBonusHealth(int amount)
    {
        maxHealth += amount;
        currentHealth += amount;

        Debug.Log("Bonus HP +" + amount);
        Debug.Log("New Max HP: " + maxHealth);
    }

    public void RemoveBonusHealth(int amount)
    {
        maxHealth -= amount;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        Debug.Log("Removed Bonus HP -" + amount);
    }
}
