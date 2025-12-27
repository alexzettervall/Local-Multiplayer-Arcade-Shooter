using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackboard
{
    public Player player;
    public string goal;
    public EnemyData[] enemyDatas;
    public ItemData[] itemDatas;
    public bool move;
    public AITarget target;
    public Player targetEnemy;
    public Item targetLoot;
    public float attackRange = 2f;
    public bool isUsing = false;
    public bool isDirty = false; // If true immediately reavaluate goal
    

    // Inputs to send to player
    public Vector2 lookDirection;
    public Vector2 movement;
    public bool preformUse;
    public bool cancelUse;
    public bool interact;
    public bool drop;
    

    public Blackboard() {

    }

    public struct EnemyData
    {
        public EnemyData(Player player, Vector2 position, float health) {
            this.player = player;
            this.position = position;
            this.health = health;
        }

        public Player player;
        public Vector2 position;
        public float health;
    }
    public struct ItemData
    {
        public ItemData(Item item, float value) {
            this.item = item;
            this.value = value;
        }

        public Item item;
        public float value;
    }
}
