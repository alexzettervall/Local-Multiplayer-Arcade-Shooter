using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

public class UtilityScorer
{
    Blackboard blackboard;
    float utilityTimer = 0f;
    Dictionary<string, float> needs = new Dictionary<string, float>();
    float maxLootDistance = 100f;
    float maxAttackDistance = 100f;
    float maxThreatDistance = 10f;
    float maxDPS = 100f;
    float maxDamageLeft = 100f;
    float alwaysAttackRadius = 3f;
    float minimumEnemyThreatLevel = 0.25f;

    // Utilities
    float attackUtility = 0f;
    float lootUtility = 0f;

    public UtilityScorer(Blackboard blackboard) {
        this.blackboard = blackboard;
    }

    public void Update() {
        utilityTimer -= Time.deltaTime;
        if (utilityTimer <= 0 || blackboard.isDirty) {
            CalculateUtility();
            utilityTimer = GameAssets.i.AISettings.utilityRecalcPeriod;
        }
    }

    public void CalculateUtility() {
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

        SetGoal("Loot");
        CalculateNeeds(); // Get loot utility needs this to be called first
        GetLootUtility(); // We only call this to set blackboard.targetLoot. This is bad but whatever.
        Item heldItem = blackboard.player.GetItem();
        if (heldItem != null && heldItem is Gun)
        {
            Gun gun = (Gun)heldItem;
            if (gun.GetAmmo() > 0)
            {
                SetGoal("Attack");
                blackboard.targetEnemy = GetClosestEnemy();
            }
        }
        if (blackboard.targetLoot == null)
        {
            SetGoal("Attack");
            blackboard.targetEnemy = GetClosestEnemy();
        }
    }
    public float GetAttackUtility() {
        Player highestEnemy = null;
        float highestAttackScore = -1f;
        List<Vector2> path = null;
        if (blackboard.enemyDatas == null || blackboard.enemyDatas.Length < 1) {
            blackboard.targetEnemy = null;
            return 0f;
        }
        foreach (Blackboard.EnemyData enemyData in blackboard.enemyDatas) {
            if (enemyData.player == null) continue;
            float distanceToEnemy = GameMan.Instance.GetLevel().FindDistance(blackboard.player.transform.position, enemyData.player.transform.position, out path);
            float distanceFactor = 1 - Mathf.Clamp01(distanceToEnemy / maxAttackDistance);

            float healthModifier = (float)blackboard.player.GetHealth() / (float)blackboard.player.GetMaxHealth();

            float threatModifier = GetEnemyThreatLevel(enemyData);

            float weaponModifier = GetWeaponSuitability(enemyData);
            
            float attackScore = distanceFactor * healthModifier * threatModifier * weaponModifier;
            Debug.Log("d: " + distanceFactor + " h: " + healthModifier + " t: " + threatModifier + " w: " + weaponModifier);
            attackScore = Mathf.Clamp01(attackScore);

            if (attackScore > highestAttackScore) {
                highestAttackScore = attackScore;
                highestEnemy = enemyData.player;
            }
        }
        blackboard.targetEnemy = highestEnemy;
        highestAttackScore += 0.1f; // Intrinsic attack
        return highestAttackScore;
    }
    public float GetLootUtility() {
        Item bestItem = null;
        float bestUtility = 0.01f;
        List<Vector2> path = null;

        foreach (Blackboard.ItemData itemData in blackboard.itemDatas) {
            if (itemData.item == null) continue;
            if (itemData.item.GetHolder() != null) continue;

            float distance = GameMan.Instance.GetLevel().FindDistance(blackboard.player.transform.position, itemData.item.transform.position, out path);
            float distanceFactor = 1f - distance / maxLootDistance;
            float needFactor = GetNeedFactor(itemData.item);
            float dangerFactor = 1f - GetDangerFactor(itemData.item);
            float valueFactor = GetWeaponValue(itemData.item);
            //float pathFactor = GetPathFactor(itemData.item);

            // Clamp all values
            distanceFactor = Mathf.Clamp01(distanceFactor);
            needFactor = Mathf.Clamp01(needFactor);
            dangerFactor = Mathf.Clamp01(dangerFactor);
            valueFactor = Mathf.Clamp01(valueFactor);

            float lootUtility = valueFactor * distanceFactor * needFactor * dangerFactor;
            lootUtility = Mathf.Clamp01(lootUtility);
            if (lootUtility > bestUtility) {
                bestUtility = lootUtility;
                bestItem = itemData.item;
            }
        }

        blackboard.targetLoot = bestItem;
        return bestUtility;
    }
    public float GetPathFactor(Item item) {
        List<Vector2> path = GameObject.FindObjectOfType<Level>().FindPath(blackboard.player.transform.position, item.transform.position);
        if (path.Count < 1f)
        {
            return 0f;
        }
        return 1f;
    }
    public float GetDangerFactor(Item item) {
        float max = 0f;
        foreach (Blackboard.EnemyData enemyData in blackboard.enemyDatas) {
            max = Mathf.Max(max, GetEnemyThreatLevel(enemyData));
        }
        return max;
    }
    public float GetNeedFactor(Item item) {
        if (item == null) return 0f;
        if (item.HasTag("deadly weapon")) return needs["weapon"];
        return 0f;
    }
    public float GetEnemyThreatLevel(Blackboard.EnemyData enemyData) {
        if (enemyData.player == null) return 0f;

        // 1. Distance (cloaser = more threatening)
        float distance = Vector3.Distance(blackboard.player.transform.position, enemyData.player.transform.position);
        float distanceFactor = 1f - Mathf.Clamp01(distance / maxThreatDistance);

        // 2. Health factor (higher health = more threatening)
        float healthFactor = (float)enemyData.player.GetHealth() / (float)enemyData.player.GetMaxHealth();

        // 3. DPS factor
        float DPSFactor = enemyData.player.GetDPS() / maxDPS;

        // 4. Aggression factor
        //float aggressionFactor = otherAI.aggression;

        // Combine weighted
        float threat = healthFactor * DPSFactor * distanceFactor;
        return Mathf.Clamp(threat, minimumEnemyThreatLevel, 1f); // Enemies are always threatening
    }
    public float GetWeaponValue(Item item) {
        
        float DPSFactor = item.GetDPS() / maxDPS;
        float damageFactor = item.GetDamageLeft() / maxDamageLeft;

        return DPSFactor * damageFactor;
    }
    public float GetWeaponSuitability(Blackboard.EnemyData enemyData) {
        float distance = Vector3.Distance(blackboard.player.transform.position, enemyData.player.transform.position);
        float range = blackboard.attackRange;
        float distanceFactor = range / distance;

        float DPSFactor = blackboard.player.GetDPS() / maxDPS;
        float damageFactor = 0.2f;
        if (blackboard.player.GetItem() != null) {
            damageFactor = blackboard.player.GetItem().GetDamageLeft() / maxDamageLeft;
        }

        return distanceFactor * DPSFactor * damageFactor;
    }
    public void CalculateNeeds() {
        // Weapon
        Item item = blackboard.player.GetItem();
        if (item == null || !item.HasTag("deadly weapon")) {
            needs["weapon"] = 1f;
        }
        else {
            needs["weapon"] = 0f;
        }

        // Health
        needs["health"] = 1f - (float)blackboard.player.GetHealth() / (float)blackboard.player.GetMaxHealth();
    }
    public Player GetClosestEnemy() {
        Player closestEnemy = null;
        float closestDist = float.MaxValue;

        foreach(Blackboard.EnemyData enemyData in blackboard.enemyDatas) {
            if (enemyData.player == null) {
                continue;
            }
            if (enemyData.player == blackboard.player) {
                continue;
            }
            float distance = Vector2.Distance(blackboard.player.transform.position, enemyData.player.transform.position);
            if (distance < closestDist) {
                closestDist = distance;
                closestEnemy = enemyData.player;
            }
        }

        return closestEnemy;
    }

    public void SetGoal(string goal) {
        // See if goal changes
        if (blackboard.goal != goal) {
            blackboard.target = null;
        }
        blackboard.goal = goal;
    }
    public void DrawGizmos() {

        if (blackboard.player == null) {
            return;
        }
        // Set gizmo position
        Vector3 position = blackboard.player.transform.position + Vector3.up * 2f;


        // Draw the text label
        GUIStyle style = new GUIStyle();
        style.normal.textColor = new Color(1f, 1f, 1f);
        style.fontStyle = FontStyle.Bold;
        style.fontSize = 20;

        #if UNITY_EDITOR
            Handles.Label(position, "Attack: " + attackUtility, style);
            Handles.Label(position + new Vector3(0, 0.5f), "Loot: " + lootUtility, style);
        #endif
    }
}
