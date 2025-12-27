using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Crate : Structure
{
    [SerializeField] private List<LootTypeWeight> lootTypeWeights;
    [SerializeField] private int amountMin = 1;
    [SerializeField] private int amountMax = 1;
    protected override void Kill()
    {
        if (isDead)
        {
            return;
        }
        base.Kill();
        for (int i = 0; i < Random.Range(amountMin, amountMax+1); i++)
        {
            GameObject obj = Instantiate(GameAssets.i.lootPrefab, transform.position, Quaternion.identity, transform.parent);
            Loot loot = obj.GetComponent<Loot>();
            loot.SpawnLoot(lootTypeWeights, true);
        }
    }
}
