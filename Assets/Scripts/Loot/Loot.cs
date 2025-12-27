using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Loot : MonoBehaviour
{
    [SerializeField] private List<LootTypeWeight> lootTypeWeights;
    [SerializeField] private bool spawnLootOnStart;
    private void Start()
    {
        if (spawnLootOnStart)
        {
            SpawnLoot(lootTypeWeights, false);
        }
    }

    public void SpawnLoot(List<LootTypeWeight> lootTypeWeights, bool jiggleItem)
    {
        GameObject loot = GameAssets.i.lootPools.GetLoot(lootTypeWeights);
        GameObject obj = Instantiate(loot, transform.position, Quaternion.identity, transform.parent);
        obj.transform.eulerAngles = new Vector3(0, 0, Random.Range(0f, 360f));

        if (jiggleItem)
        {
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * 2f;
                rb.angularVelocity = Random.Range(-1000f, 1000f);
            }
        }

        Destroy(gameObject);
    }

    
}
