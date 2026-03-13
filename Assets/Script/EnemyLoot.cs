using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LootEntry
{
    public GameObject itemPrefab; // Prefab của item có gắn script ItemPickup
    [Range(0, 100)]
    public float dropChance;      // Tỉ lệ rơi (%)
}

public class EnemyLoot : MonoBehaviour
{
    public List<LootEntry> lootTable; // Danh sách các vật phẩm có thể rơi

    public void DropLoot()
    {
        foreach (LootEntry entry in lootTable)
        {
            float randomValue = Random.Range(0f, 100f);
            
            if (randomValue <= entry.dropChance)
            {
                if (entry.itemPrefab != null)
                {
                    // Sinh ra item tại vị trí quái chết
                    Instantiate(entry.itemPrefab, transform.position, Quaternion.identity);
                    Debug.Log($"Quái rơi ra: {entry.itemPrefab.name}");
                }
            }
        }
    }
}
