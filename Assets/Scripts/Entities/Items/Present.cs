using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Present : Item
{
    [SerializeField] private List<LootTypeWeight> lootTypeWeights;
    public override void Use(bool use, bool unUse)
    {
        if (isDead)
        {
            return;
        }
        base.Use(use, unUse);
        AudioMan.PlaySound(Sound.OpenPresent);
        GameObject obj = Instantiate(GameAssets.i.lootPrefab, transform.position, Quaternion.identity, transform.parent);
        Loot loot = obj.GetComponent<Loot>();
        loot.SpawnLoot(lootTypeWeights, true);
        Kill();
    }
}
