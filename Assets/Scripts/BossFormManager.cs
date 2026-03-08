using UnityEngine;

public class BossFormManager : MonoBehaviour
{
    [Header("Form Change")]
    [SerializeField] private KeyCode formChangeKey = KeyCode.F;
    [SerializeField] private bool isForm2 = false;

    private Boss_BorealController boss;

    private void Awake()
    {
        boss = GetComponent<Boss_BorealController>();
    }

    public void HandleFormChangeInput()
    {
        if (Input.GetKeyDown(formChangeKey))
        {
            ToggleForm();
        }
    }

    private void ToggleForm()
    {
        isForm2 = !isForm2;
        boss.SetAnimatorBool("IsForm2", isForm2);
        Debug.Log(isForm2 ? "Boss → Form 2" : "Boss → Form 1");

        boss.TriggerAnimator("FormChange");

        if (isForm2)
        {
            boss.Form2Attacks?.StartForm2Behavior();
        }
        else
        {
            boss.Form2Attacks?.CancelForm2Timers();
        }
    }

    public bool IsForm2 => isForm2;

    public float CurrentMoveSpeed => isForm2 ? boss.Movement.moveSpeedForm2 : boss.Movement.moveSpeedForm1;
    public float CurrentShootChance => isForm2 ? boss.ShootChase.shootChanceForm2 : boss.ShootChase.shootChanceForm1;
    public float CurrentShootCooldownChase => isForm2 ? boss.ShootChase.shootCooldownChaseForm2 : boss.ShootChase.shootCooldownChaseForm1;
}