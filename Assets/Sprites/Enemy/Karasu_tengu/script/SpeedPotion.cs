using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Speed Potion")]
public class SpeedPotion : Item
{
    public float speedIncrease = 3f;
    public float duration = 5f;

    public override void Use()
    {
        PlayerController player = Object.FindFirstObjectByType<PlayerController>();

        if (player != null)
        {
            player.BoostSpeed(speedIncrease, duration);
        }
    }
}