using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public int enemyCount;
    public GameObject teleport;

    private void Start()
    {
        teleport.SetActive(false);
    }

    public void RegisterEnemy()
    {
        enemyCount++;
        Debug.Log("RegisterEnemy → count = " + enemyCount);
    }

    public void EnemyDied()
    {
        enemyCount--;
        Debug.Log("EnemyDied → count = " + enemyCount);

        if (enemyCount <= 0)
        {
            Debug.Log("ALL ENEMY DEAD - OPEN TELEPORT");
            teleport.SetActive(true);
        }
    }

}
