using UnityEngine;

public class BossShootChase : MonoBehaviour
{
    [Header("Shoot During Chase")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] public float shootChanceForm1 = 0.2f;
    [SerializeField] public float shootChanceForm2 = 0.45f;
    [SerializeField] public float shootCooldownChaseForm1 = 2.5f;
    [SerializeField] public float shootCooldownChaseForm2 = 1.8f;
    [SerializeField] private float shootDuration = 0.4f;

    [SerializeField] private float shootTriggerDistance = 3f;

    private float lastShootChaseTime = -999f;
    private Boss_BorealController boss;
    private BossFormManager form;

    private void Awake()
    {
        boss = GetComponent<Boss_BorealController>();
        form = GetComponent<BossFormManager>();
    }

    public void TryShootDuringChase(float distance)
    {
        if (distance < shootTriggerDistance) return;
        if (Time.time < lastShootChaseTime + form.CurrentShootCooldownChase) return;

        if (Random.value <= form.CurrentShootChance)
        {
            StartShootChase();
        }
    }

    private void StartShootChase()
    {
        boss.ChangeState(Boss_BorealController.State.ShootChase);
        lastShootChaseTime = Time.time;
        boss.TriggerAnimator("Shoot");
        FireOneBullet();

        Invoke(nameof(EndShootChase), shootDuration);
    }

    private void EndShootChase()
    {
        boss.ChangeState(Boss_BorealController.State.Chase);
    }

    public void FireOneBullet()
    {
        if (projectilePrefab == null || firePoint == null) return;

        Vector2 dir = (boss.player.position - firePoint.position).normalized;
        GameObject bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Projectile proj = bullet.GetComponent<Projectile>();
        proj?.SetDirection(dir);
    }
}