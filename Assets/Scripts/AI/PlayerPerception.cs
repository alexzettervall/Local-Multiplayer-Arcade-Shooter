using System.Collections.Generic;
using UnityEngine;
using static PlayerBlackboard;

public class PlayerPerception : Perception<PlayerBlackboard>
{
    float playerTimer = 0f;
    float itemTimer = 0f;
    float containerTimer = 0f;

    public PlayerPerception(PlayerBlackboard blackboard) : base(blackboard)
    {

    }

    protected override void UpdatePerception()
    {
        UpdatePlayers();
        UpdateItems();
    }

    public void UpdatePlayers()
    {
        List<PlayerData> playerDatas = new List<PlayerData>();
        Player[] players = GameObject.FindObjectsOfType<Player>();
        foreach (Player player in players)
        {
            if (player == blackboard.entity)
            {
                continue; // Ignore your own player object.
            }
            playerDatas.Add(new PlayerData((Vector2)player.transform.position, player.GetHealth()));
        }
        blackboard.playerDatas = playerDatas.ToArray();
    }
    public void UpdateItems()
    {
        List<ItemData> itemDatas = new List<ItemData>();
        Item[] items = GameObject.FindObjectsOfType<Item>();
        foreach (Item item in items)
        {
            if (item.GetHolder() != null) continue;
            // Ignore if its in the gas
            if (Physics2D.OverlapCircle(item.transform.position, 0.5f, GameAssets.i.poisonGasLayer))
            {
                continue;
            }
            itemDatas.Add(new ItemData(item.transform.position, item.GetTags(), item.IsHeld(), item.GetDPS()));
        }
        blackboard.itemDatas = itemDatas.ToArray();
    }
}