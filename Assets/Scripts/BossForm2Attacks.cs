using UnityEngine;
using System.Collections;

public class BossForm2Attacks : MonoBehaviour
{
    // Circular Barrage
    [Header("Form 2 - Circular Barrage")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private int circularBullets = 15;
    [SerializeField] private int circularWaves = 3;
    [SerializeField] private float delayBetweenCircularWaves = 1.2f;
    [SerializeField] private float circularBulletSpeedMultiplier = 1.2f;


    // Rain of Bullets
    [Header("Form 2 - Rain of Bullets")]
    [SerializeField] private GameObject rainProjectilePrefab;
    [SerializeField] private int rainBulletsPerWave = 20;
    [SerializeField] private int rainSpawnPerTick = 5;          // Sinh 5 viên mỗi lần để tránh lag
    [SerializeField] private float delayBetweenRainTicks = 0.1f;
    [SerializeField] private int rainWaves = 4;
    [SerializeField] private float delayBetweenRainWaves = 1f;
    [SerializeField] private float rainSpawnHeightOffset = 3f;
    [SerializeField] private float rainSpeed = 2f;
    [SerializeField] private float rainCooldown = 20f;
    private float lastRainTime = -999f;

    // Blizzard Vortex
    [Header("Form 2 - Blizzard Vortex")]
    [SerializeField] private GameObject blizzardVortexPrefab;
    [SerializeField] private Transform vortexFirePoint;
    [SerializeField] private float blizzardFirstDelayMin = 15f;
    [SerializeField] private float blizzardFirstDelayMax = 25f;
    [SerializeField] private float blizzardCooldownMin = 15f;
    [SerializeField] private float blizzardCooldownMax = 25f;
    [SerializeField] private float vortexSpeed = 6f;
    private float lastBlizzardTime = -999f;

    private Boss_BorealController boss;
    private BossMovement movement;

    private void Awake()
    {
        boss = GetComponent<Boss_BorealController>();
        movement = GetComponent<BossMovement>();
    }

    public void StartForm2Behavior()
    {
        StartCoroutine(PerformCircularBarrage());
        StartBlizzardCountdown();
    }

    public void CancelForm2Timers()
    {
        CancelInvoke(nameof(TriggerBlizzardVortex));
    }

    // ─── Circular Barrage ────────────────────────────────────────
    private IEnumerator PerformCircularBarrage()
    {
        boss.ChangeState(Boss_BorealController.State.Form2CircularAttack);
        movement.StopMovement();
        boss.TriggerAnimator("CircularAttack");

        for (int wave = 0; wave < circularWaves; wave++)
        {
            for (int i = 0; i < circularBullets; i++)
            {
                // Ví dụ: 15 đạn → mỗi viên cách 24° (360/15 = 24)
                // i=0: 0°, i=1: 24°, i=2: 48°, ..., i=14: 336°
                float angle = i * (360f / circularBullets);

                // Cos(angle) → thành phần X (ngang)
                // Sin(angle) → thành phần Y (dọc)
                // Mathf.Deg2Rad: chuyển độ (° ) sang radian (π/180) vì Cos/Sin dùng radian
                Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
                GameObject bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
                Projectile proj = bullet.GetComponent<Projectile>();
                if (proj != null)
                {
                    proj.SetDirection(dir.normalized);
                    proj.speed *= circularBulletSpeedMultiplier;
                }
            }
            yield return new WaitForSeconds(delayBetweenCircularWaves);
        }

        boss.ChangeState(Boss_BorealController.State.Chase);

        yield return new WaitForSeconds(3f);

        if (Time.time >= lastRainTime + rainCooldown)
        {
            StartCoroutine(PerformRainOfBullets());
        }
        else
        {
            boss.ChangeState(Boss_BorealController.State.Idle);
        }
    }

    // ─── Rain of Bullets ─────────────────────────────────────────
    private IEnumerator PerformRainOfBullets()
    {
        boss.ChangeState(Boss_BorealController.State.Form2RainAttack);
        movement.StopMovement();
        boss.TriggerAnimator("RainAttack");

        Camera cam = Camera.main;

        // 5. Tính chiều rộng màn hình (dành cho Orthographic Camera 2D)
        // orthographicSize = chiều cao màn hình / 2
        // * cam.aspect = tỷ lệ khung hình (width/height) → chiều rộng = height * aspect * 2
        float screenWidth = cam.orthographicSize * cam.aspect * 2f;
        float spawnY = cam.transform.position.y + cam.orthographicSize + rainSpawnHeightOffset;

        for (int wave = 0; wave < rainWaves; wave++)
        {
            int bulletsSpawned = 0;
            while (bulletsSpawned < rainBulletsPerWave)
            {
                int thisTick = Mathf.Min(rainSpawnPerTick, rainBulletsPerWave - bulletsSpawned);
                for (int i = 0; i < thisTick; i++)
                {
                    //Vị trí X random: từ trái màn hình đến phải màn hình
                    float x = Random.Range(cam.transform.position.x - screenWidth / 2f,
                                           cam.transform.position.x + screenWidth / 2f);

                    //Vị trí spawn: (x random, spawnY cao, z=0)
                    Vector3 pos = new Vector3(x, spawnY, 0f);
                    GameObject bullet = Instantiate(rainProjectilePrefab, pos, Quaternion.identity);
                    ProjectileRainBullets proj = bullet.GetComponent<ProjectileRainBullets>();
                    proj?.SetDirection(Vector2.down);
                    proj.speed = rainSpeed + Random.Range(-1f, 1f);
                }
                bulletsSpawned += thisTick;
                yield return new WaitForSeconds(delayBetweenRainTicks);
            }
            yield return new WaitForSeconds(delayBetweenRainWaves);
        }

        lastRainTime = Time.time;

        yield return new WaitForSeconds(5f);
        StartCoroutine(PerformBlizzardVortex());

        boss.ChangeState(Boss_BorealController.State.Idle);
    }

    // ─── Blizzard Vortex ─────────────────────────────────────────
    private void StartBlizzardCountdown()
    {
        float delay = Random.Range(blizzardFirstDelayMin, blizzardFirstDelayMax);
        Invoke(nameof(TriggerBlizzardVortex), delay);
    }

    private void TriggerBlizzardVortex()
    {
        if (boss.currentState != Boss_BorealController.State.Chase) return;
        StartCoroutine(PerformBlizzardVortex());
    }

    private IEnumerator PerformBlizzardVortex()
    {
        boss.ChangeState(Boss_BorealController.State.Form2BlizzardVortex);
        movement.StopMovement();
        boss.TriggerAnimator("BlizzardVortex");

        FireBlizzardVortex();
        yield return new WaitForSeconds(2f);

        float nextCooldown = Random.Range(blizzardCooldownMin, blizzardCooldownMax);
        lastBlizzardTime = Time.time;
        Invoke(nameof(TriggerBlizzardVortex), nextCooldown);

        boss.ChangeState(Boss_BorealController.State.Idle);
    }

    private void FireBlizzardVortex()
    {
        if (blizzardVortexPrefab == null || vortexFirePoint == null) return;

        Vector2 dir = new Vector2(boss.Movement.FacingDirection, 0f).normalized; // cần public hóa facingDirection hoặc lấy từ scale
        GameObject vortex = Instantiate(blizzardVortexPrefab, vortexFirePoint.position, Quaternion.identity);
        ProjectileBlizzardVortex proj = vortex.GetComponent<ProjectileBlizzardVortex>();
        proj?.SetDirection(dir);
        proj.speed = vortexSpeed;
    }
}