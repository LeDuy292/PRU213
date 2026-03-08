using UnityEngine;

public class BossInitialShootPhase : MonoBehaviour
{
    [Header("Initial Shoot Phase")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private int totalShootWaves = 3;
    [SerializeField] private int bulletsPerWave = 5;
    [SerializeField] private float delayBetweenBullets = 0.3f;
    [SerializeField] private float delayBetweenWaves = 2f;
    [SerializeField] private float delayAfterAllWaves = 1f;

    private int currentWave;
    private int bulletsShotInWave;
    private float nextActionTime;

    [HideInInspector] public bool hasFinishedInitialShootPhase = false;

    private Boss_BorealController boss;
    private BossShootChase shooter;

    private void Awake()
    {
        boss = GetComponent<Boss_BorealController>();
        shooter = GetComponent<BossShootChase>();
    }

    public void BeginInitialPhase()
    {
        nextActionTime = Time.time;
        currentWave = 0;
        bulletsShotInWave = 0;
    }

    public void UpdateInitialPhase()
    {
        boss.Movement.StopMovement();

        if (Time.time < nextActionTime) return;

        if (bulletsShotInWave < bulletsPerWave)
        {
            shooter.FireOneBullet();
            bulletsShotInWave++;
            nextActionTime = Time.time + delayBetweenBullets;
        }
        else
        {
            currentWave++;
            bulletsShotInWave = 0;

            if (currentWave >= totalShootWaves)
            {
                Invoke(nameof(FinishPhase), delayAfterAllWaves);
                nextActionTime = float.MaxValue;
            }
            else
            {
                nextActionTime = Time.time + delayBetweenWaves;
            }
        }
    }

    private void FinishPhase()
    {
        hasFinishedInitialShootPhase = true;
        boss.ChangeState(Boss_BorealController.State.Idle);
    }
}