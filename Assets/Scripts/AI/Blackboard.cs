using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Blackboard
{
    // Essential fields
    public AISettings settings;
    public LivingEntity entity;
    public string goal;
    public bool move;
    public Vector2? target;
    public bool isDirty = false; // If true immediately reavaluate goal

    public bool use = false; // Abstracted use input. Changing this will automatically trigger preformUse and cancelUse

    // Inputs to send to player
    public Vector2 lookDirection;
    public Vector2 movement;
    public bool preformUse {get; private set;}
    public bool cancelUse {get; private set;}
    public bool interact;
    public bool drop;

    // General perception data
    public PlayerData[] playerDatas;
    public ItemData[] itemDatas;
    public ContainerData[] containerDatas;
    public PlayerData? targetEnemy;
    public ItemData? targetLoot;
    public ContainerData? targetContainer;
    

    public Blackboard() {

    }

    public virtual void Update()
    {
        // inject inputs
        entity.OnMove(movement);
        entity.OnRotate(lookDirection, null);
        
        if (entity.IsUsing() && !use)
        {
            cancelUse = true;
            preformUse = false;
        }
        else if (!entity.IsUsing() && use)
        {
            cancelUse = false;
            preformUse = true;
        }
        entity.OnUse(preformUse, cancelUse);
        preformUse = false;
        cancelUse = false;

        entity.OnInteract(interact);
        interact = false;

        entity.OnDrop(drop);
        drop = false;
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
