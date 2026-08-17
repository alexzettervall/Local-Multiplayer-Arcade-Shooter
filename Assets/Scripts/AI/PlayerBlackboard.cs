using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlackboard : Blackboard
{
    public float attackRange = 2f;

    public PlayerData[] playerDatas;
    public ItemData[] itemDatas;
    public PlayerData? targetEnemy;
    public ItemData? targetLoot;

    public readonly struct PlayerData : IPerceivedEntity
    {
        public PlayerData(Vector2 position, float health)
        {
            this.Position = position;
            this.health = health;
        }

        public Vector2 Position { get; }
        public float health { get; }
    }
    public readonly struct ItemData : IPerceivedEntity
    {
        public ItemData(Vector2 position, List<String> tags, bool held, float dps)
        {
            Position = position;
            this.tags = tags;
            this.held = held;
            this.dps = dps;
        }

        public Vector2 Position { get; }
        public List<String> tags { get; }
        public bool held { get; }
        public float dps { get; }
    }
}