using System;
using System.Collections.Generic;
using UnityEngine;

public class Blackboard
{
    public AISettings settings;
    public LivingEntity entity;
    public string goal;
    public PlayerData[] playerDatas;
    public ItemData[] itemDatas;
    public ContainerData[] containerDatas;
    public bool move;
    public Vector2? target;
    public PlayerData? targetEnemy;
    public ItemData? targetLoot;
    public ContainerData? targetContainer;
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

    public struct PlayerData
    {
        public PlayerData(Vector2 position, float health) {
            this.position = position;
            this.health = health;
        }

        public Vector2 position;
        public float health;
    }
    public struct ItemData
    {
        public ItemData(Vector2 position, List<String> tags, bool held, float dps) {
            this.position = position;
            this.tags = tags;
            this.held = held;
            this.dps = dps;
        }

        public Vector2 position;
        public List<String> tags;
        public bool held;
        public float dps;
    }
    public struct ContainerData
    {
        public ContainerData(Vector2 position)
        {
            this.position = position;
        }

        public Vector2 position;
    }
}
