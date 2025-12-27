using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Perception
{
    Blackboard blackboard;
    float enemyTimer = 0f;
    float itemTimer = 0f;

    public Perception(Blackboard blackboard) {
        this.blackboard = blackboard;
    }

    public void Update() {
        enemyTimer -= Time.deltaTime;
        if (enemyTimer <= 0) {
            UpdateEnemies();
            enemyTimer = GameAssets.i.AISettings.enemyPeriod + Random.Range(-GameAssets.i.AISettings.jitter, GameAssets.i.AISettings.jitter);
        }
        itemTimer -= Time.deltaTime;
        if (itemTimer <= 0) {
            UpdateItems();
            itemTimer = GameAssets.i.AISettings.enemyPeriod + Random.Range(-GameAssets.i.AISettings.jitter, GameAssets.i.AISettings.jitter);
        }
        
    }

    public void UpdateEnemies() {
        List<Blackboard.EnemyData> enemyDatas = new List<Blackboard.EnemyData>();
        Player[] players = GameObject.FindObjectsOfType<Player>();
        foreach (Player player in players) {
            if (player == blackboard.player) {
                continue; // Ignore your own player object.
            }
            enemyDatas.Add(new Blackboard.EnemyData(player, (Vector2)player.transform.position, player.GetHealth()));
        }
        blackboard.enemyDatas = enemyDatas.ToArray();
    }
    public void UpdateItems() {
        List<Blackboard.ItemData> itemDatas = new List<Blackboard.ItemData>();
        Item[] items = GameObject.FindObjectsOfType<Item>();
        foreach (Item item in items) {
            if (item.GetHolder() != null) continue;
            // Ignore if its in the gas
            if (Physics2D.OverlapCircle(item.transform.position, 0.5f, LayerMask.NameToLayer("Poison Gas")))
            {
                continue;
            }
            itemDatas.Add(new Blackboard.ItemData(item, GetItemValue(item)));
        }
        blackboard.itemDatas = itemDatas.ToArray();
    }

    public float GetItemValue(Item item) {
        if (item is Gun) {
            Gun gun = (Gun)item;
            return (gun.GetDPS() / 100f) * (gun.GetDamageLeft() / 500f);
        }
        if (item is FragGrenade) {
            return 0.01f;
        }
        return 0f;
    }
}
