using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    public int level = 1;
    public int currentExp = 0;
    public int expToNextLevel = 100;

    public int maxHP = 100;
    public int attack = 10;
    public int defense = 5;

    public void AddExp(int exp)
    {
        currentExp += exp;
        Debug.Log("Nhận EXP: " + exp);

        if (currentExp >= expToNextLevel)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        level++;
        currentExp -= expToNextLevel;
        expToNextLevel += 50; // mỗi level cần nhiều exp hơn

        // tăng chỉ số
        maxHP += 20;
        attack += 5;
        defense += 3;

        Debug.Log("LEVEL UP! Level hiện tại: " + level);
    }
}