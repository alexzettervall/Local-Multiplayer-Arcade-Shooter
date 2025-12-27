using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Loot Pools", fileName = "New Loot Pools")]
public class LootPools : ScriptableObject
{
    [SerializeField] private LootPool[] lootPools;

    public GameObject GetLoot(List<LootTypeWeight> lootTypeWeights)
    {
        LootType lootType = GetLootType(lootTypeWeights);
        float total = 0f;
        LootPool lootPool = GetLootPool(lootType);
        foreach (LootWeight lootWeight in lootPool.lootWeights)
        {
            total += lootWeight.weight;
        }
        float rand = Random.Range(0f, total);
        foreach (LootWeight lootWeight in lootPool.lootWeights)
        {
            if (rand <= lootWeight.weight)
            {
                return lootWeight.loot;
            }
            rand -= lootWeight.weight;
        }
        Debug.LogError("Couldn't find loot in loot pool!");
        return null;
    }

    private LootPool GetLootPool(LootType lootType)
    {
        foreach (LootPool lootPool in lootPools)
        {
            if (lootPool.lootType == lootType)
            {
                return lootPool;
            }
        }
        Debug.LogError("Couldn't find loot pool!");
        return new LootPool();
    }

    protected LootType GetLootType(List<LootTypeWeight> lootTypeWeights)
    {
        float total = 0f;
        foreach (LootTypeWeight lootTypeWeight in lootTypeWeights)
        {
            total += lootTypeWeight.weight;
        }
        float rand = Random.Range(0f, total);
        foreach (LootTypeWeight lootTypeWeight in lootTypeWeights)
        {
            if (rand <= lootTypeWeight.weight)
            {
                return lootTypeWeight.lootType;
            }
            rand -= lootTypeWeight.weight;
        }
        Debug.LogError("Couldn't get random loot type!");
        return LootType.LowQualityWeapon;
    }

    [System.Serializable]
    private struct LootPool
    {
        public LootType lootType;
        public List<LootWeight> lootWeights;
    }
    [System.Serializable]
    private struct LootWeight
    {
        public GameObject loot;
        public float weight;
    }
}
