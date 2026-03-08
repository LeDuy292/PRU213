using UnityEngine;

public class BossMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeedForm1 = 3f;
    public float moveSpeedForm2 = 4.5f;

    [HideInInspector] public float moveX;
    private int facingDirection = 1;

    public int FacingDirection => facingDirection;

    private Boss_BorealController boss;
    private BossFormManager form;

    private void Awake()
    {
        boss = GetComponent<Boss_BorealController>();
        form = GetComponent<BossFormManager>();
    }

    public void StopMovement()
    {
        moveX = 0f;
    }

    public void HandleNormalMovementAndDecideState()
    {
        if (!boss.InitialPhase.hasFinishedInitialShootPhase)
        {
            moveX = 0f;
            boss.ChangeState(Boss_BorealController.State.Idle);
            return;
        }

        float deltaX = boss.player.position.x - transform.position.x;
        float distance = Mathf.Abs(deltaX);

        if (distance <= boss.Melee.meleeRange || boss.Melee.isTouchingPlayer)
        {
            moveX = 0f;
            boss.Melee.HandleMeleeLogic();
            return;
        }

        // Chase
        boss.ChangeState(Boss_BorealController.State.Chase);
        boss.ShootChase.TryShootDuringChase(distance);

        moveX = Mathf.Sign(deltaX) * form.CurrentMoveSpeed;
    }

    public void FlipTowardsPlayer()
    {
        if (boss.player == null) return;

        int dir = boss.player.position.x > transform.position.x ? 1 : -1;
        if (dir == facingDirection) return;

        facingDirection = dir;
        Vector3 scale = transform.localScale;
        scale.x = facingDirection;
        transform.localScale = scale;
    }

    private void Update()
    {
        FlipTowardsPlayer();
    }
}