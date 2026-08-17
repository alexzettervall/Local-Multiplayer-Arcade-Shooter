using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

public class PlayerUtilityScorer : UtilityScorer<PlayerBlackboard>
{
    Dictionary<string, float> needs = new Dictionary<string, float>();
    float maxLootDistance = 100f;
    float maxAttackDistance = 100f;
    float maxThreatDistance = 10f;
    float maxDPS = 100f;
    float maxDamageLeft = 100f;
    float alwaysAttackRadius = 3f;
    float minimumEnemyThreatLevel = 0.25f;

    public PlayerUtilityScorer(PlayerBlackboard blackboard) : base(blackboard)
    {
        
    }

    public override void CalculateUtility() {
        /*
        CalculateNeeds();
        attackUtility = GetAttackUtility();
        lootUtility = GetLootUtility();
        if (attackUtility > lootUtility) {
            SetGoal("Attack");
        }
        else {
            SetGoal("Loot");
        }*/

        CalculateNeeds(); // Get loot utility needs this to be called first
        GetLootUtility(); // We only call this to set blackboard.targetLoot. This is bad but whatever.
        GetAttackUtility();
        Item heldItem = blackboard.entity.GetItem();
        if (heldItem != null)
        {
            utilities["Loot"] = 0f;
            utilities["Attack"] = 100f;
        }
        else
        {
            utilities["Loot"] = 100f;
            utilities["Attack"] = 0f;
        }
    }
    public float GetAttackUtility() {
        Blackboard.PlayerData? bestPlayerToAttack = null;
        float highestAttackScore = -1f;
        List<Vector2> path = null;
        if (blackboard.playerDatas == null || blackboard.playerDatas.Length < 1) {
            blackboard.targetEnemy = null;
            return 0f;
        }
        foreach (Blackboard.PlayerData playerData in blackboard.playerDatas) {
            float distanceToEnemy = GameMan.Instance.GetLevel().FindDistance(blackboard.entity.transform.position, playerData.position, out path);
            float distanceFactor = 1 - Mathf.Clamp01(distanceToEnemy / maxAttackDistance);

            float healthModifier = (float)blackboard.entity.GetHealth() / (float)blackboard.entity.GetMaxHealth();

            float weaponModifier = GetWeaponSuitability(playerData);
            
            float attackScore = distanceFactor;
            //Debug.Log("d: " + distanceFactor + " h: " + healthModifier + " w: " + weaponModifier);
            attackScore = Mathf.Clamp01(attackScore);

            if (attackScore > highestAttackScore) {
                highestAttackScore = attackScore;
                bestPlayerToAttack = playerData;
            }
        }
        blackboard.targetEnemy = bestPlayerToAttack;
        highestAttackScore += 0.1f; // Intrinsic attack
        return highestAttackScore;
    }
    public float GetLootUtility() {
        Blackboard.ItemData? bestItem = null;
        float bestUtility = 0.01f;
        List<Vector2> path = null;

        foreach (Blackboard.ItemData itemData in blackboard.itemDatas) {
            if (itemData.held) continue;
            if (!itemData.tags.Contains("deadly weapon")) continue;

            float distance = GameMan.Instance.GetLevel().FindDistance(blackboard.entity.transform.position, itemData.position, out path);
            float distanceFactor = 1f - distance / maxLootDistance;
            
            //float pathFactor = GetPathFactor(itemData.item);

            // Clamp all values
            distanceFactor = Mathf.Clamp01(distanceFactor);


            float lootUtility = distanceFactor;
            lootUtility = Mathf.Clamp01(lootUtility);
            if (lootUtility > bestUtility) {
                bestUtility = lootUtility;
                bestItem = itemData;
            }
        }

        blackboard.targetLoot = bestItem;
        return bestUtility;
    }
    
    public float GetWeaponValue(Item item) {
        
        float DPSFactor = item.GetDPS() / maxDPS;
        float damageFactor = item.GetDamageLeft() / maxDamageLeft;

        return DPSFactor * damageFactor;
    }
    public float GetWeaponSuitability(Blackboard.PlayerData playerData) {
        float distance = Vector3.Distance(blackboard.entity.transform.position, playerData.position);
        float range = blackboard.attackRange;
        float distanceFactor = range / distance;

        float DPSFactor = blackboard.entity.GetDPS() / maxDPS;
        float damageFactor = 0.2f;
        if (blackboard.entity.GetItem() != null) {
            damageFactor = blackboard.entity.GetItem().GetDamageLeft() / maxDamageLeft;
        }

        return distanceFactor * DPSFactor * damageFactor;
    }
    public void CalculateNeeds() {
        // Weapon
        Item item = blackboard.entity.GetItem();
        if (item == null || !item.HasTag("deadly weapon")) {
            needs["weapon"] = 1f;
        }
        else {
            needs["weapon"] = 0f;
        }

        // Health
        needs["health"] = 1f - (float)blackboard.entity.GetHealth() / (float)blackboard.entity.GetMaxHealth();
    }
    public Blackboard.PlayerData? GetClosestEnemy() {
        Blackboard.PlayerData? closestEnemy = null;
        float closestDist = float.MaxValue;

        foreach(Blackboard.PlayerData playerData in blackboard.playerDatas) {
            float distance = Vector2.Distance(blackboard.entity.transform.position, playerData.position);
            if (distance < closestDist) {
                closestDist = distance;
                closestEnemy = playerData;
            }
        }

        return closestEnemy;
    }
}
