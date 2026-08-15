using System.Collections.Generic;
using UnityEngine;

public class ChickenPerception : Perception<ChickenBlackboard>
{
    float playerTimer = 0f;
    float itemTimer = 0f;
    float containerTimer = 0f;

    public ChickenPerception(ChickenBlackboard blackboard) : base(blackboard)
    {

    }

    public override void OnUpdate()
    {
        if (blackboard.settings.perceivePlayers)
        {
            playerTimer -= Time.deltaTime;
            if (playerTimer <= 0)
            {
                UpdatePlayers();
                playerTimer = blackboard.settings.playerPeriod + Random.Range(-blackboard.settings.jitter, blackboard.settings.jitter);
            }
        }
        if (blackboard.settings.perceiveItems)
        {
            itemTimer -= Time.deltaTime;
            if (itemTimer <= 0)
            {
                UpdateItems();
                itemTimer = blackboard.settings.itemPeriod + Random.Range(-blackboard.settings.jitter, blackboard.settings.jitter);
            }
        }
        if (blackboard.settings.perceiveContainers)
        {
            containerTimer -= Time.deltaTime;
            if (containerTimer <= 0)
            {
                UpdateContainers();
                containerTimer = blackboard.settings.containerPeriod + Random.Range(-blackboard.settings.jitter, blackboard.settings.jitter);
            }
        }
    }

    public void UpdatePlayers()
    {
        List<Blackboard.PlayerData> playerDatas = new List<Blackboard.PlayerData>();
        Player[] players = GameObject.FindObjectsOfType<Player>();
        foreach (Player player in players)
        {
            if (player == blackboard.entity)
            {
                continue; // Ignore your own player object.
            }
            playerDatas.Add(new Blackboard.PlayerData((Vector2)player.transform.position, player.GetHealth()));
        }
        blackboard.playerDatas = playerDatas.ToArray();
    }
    public void UpdateItems()
    {
        List<Blackboard.ItemData> itemDatas = new List<Blackboard.ItemData>();
        Item[] items = GameObject.FindObjectsOfType<Item>();
        foreach (Item item in items)
        {
            if (item.GetHolder() != null) continue;
            // Ignore if its in the gas
            if (Physics2D.OverlapCircle(item.transform.position, 0.5f, GameAssets.i.poisonGasLayer))
            {
                continue;
            }
            itemDatas.Add(new Blackboard.ItemData(item.transform.position, item.GetTags(), item.IsHeld(), item.GetDPS()));
        }
        blackboard.itemDatas = itemDatas.ToArray();
    }
    public void UpdateContainers()
    {
        List<Blackboard.ContainerData> containerDatas =
            new List<Blackboard.ContainerData>();

        Crate[] containers = GameObject.FindObjectsOfType<Crate>();

        foreach (Crate container in containers)
        {
            // Ignore if it's in the gas
            if (Physics2D.OverlapCircle(container.transform.position, 0.5f, GameAssets.i.poisonGasLayer))
            {
                continue;
            }

            containerDatas.Add(new Blackboard.ContainerData(container.transform.position));
        }

        blackboard.containerDatas = containerDatas.ToArray();
    }

    public float GetItemValue(Item item)
    {
        if (item is Gun)
        {
            Gun gun = (Gun)item;
            return (gun.GetDPS() / 100f) * (gun.GetDamageLeft() / 500f);
        }
        if (item is FragGrenade)
        {
            return 0.01f;
        }
        return 0f;
    }
}