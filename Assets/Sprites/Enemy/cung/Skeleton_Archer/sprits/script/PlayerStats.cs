using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Level")]
    public int level = 10;

    [Header("Energy")]
    public int maxEnergy = 50;
    public int currentEnergy = 10;

    [Header("Combat")]
    public int attack = 5;
    public int speed = 120;
}