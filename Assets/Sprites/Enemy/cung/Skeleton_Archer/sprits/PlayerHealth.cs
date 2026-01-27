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

    public void TakeDamage(int damage)
    {
        // Nếu đã chết thì không nhận damage nữa
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"Player took {damage} damage. Current Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return; // Tránh gọi Die nhiều lần
        
        isDead = true;
        Debug.Log("Player Died!");
        
        // Tắt collider để enemy không tấn công nữa
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        
        // Tắt movement
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;
        
        // Chơi animation chết nếu có
        Animator animator = GetComponent<Animator>();
        if (animator != null && animator.HasState(0, Animator.StringToHash("Die")))
        {
            animator.SetTrigger("Die");
            // Destroy sau 1.5 giây để animation chơi xong
            Destroy(gameObject, 1.5f);
        }
        else
        {
            // Không có animation thì destroy ngay
            Destroy(gameObject, 0.5f);
        }
    }
}
