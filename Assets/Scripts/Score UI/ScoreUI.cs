using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    public Transform finishLine;
    public float finishLinePadding; // The distance the finishline should be from the last node
    public Transform pathStart;
    public Transform pathEnd;
    public GameObject nodePrefab;
    public GameObject playerScorePrefab;
    public Transform playerScoreUIHolder;
    public float spacing = 2.5f;
    public Dictionary<int, PlayerScoreUI> playerScoreUIs = new Dictionary<int, PlayerScoreUI>();
    
    public void UpdateScoreUI(Dictionary<int, GameMan.PlayerData> playerDatas, int pointsNeeded)
    {
        AllignFinishLine();

        // Delete old player score UIs
        foreach (int playerID in playerScoreUIs.Keys)
        {
            if (!playerDatas.ContainsKey(playerID))
            {
                PlayerScoreUI playerScoreUI = playerScoreUIs[playerID];
                Destroy(playerScoreUI.gameObject);
                playerScoreUIs.Remove(playerID);
            }
        }

        // Add new players
        foreach (int playerID in playerDatas.Keys)
        {
            if (!playerScoreUIs.ContainsKey(playerID))
            {
                AddPlayerScoreUI(playerDatas[playerID], pointsNeeded);
            }
        }

        // Align and center score UI's
        List<PlayerScoreUI> players = this.playerScoreUIs.Values.ToList<PlayerScoreUI>();
        float totalSpace = spacing * (players.Count - 1);
        float topY = totalSpace / 2f;
        float bottomY = -totalSpace / 2f;
        float deltaY = totalSpace / (players.Count - 1);
        for (int i = 0; i < players.Count; i++)
        {
            players[i].transform.position = new Vector3(0, bottomY + deltaY * i, 0);
        }
    }

    // Make the finish line appear right before the last node
    public void AllignFinishLine()
    {
        finishLine.position = new Vector3(pathEnd.position.x - finishLinePadding, 0, 0);
    }

    public void AddPlayerScoreUI(GameMan.PlayerData playerData, int pointsNeeded)
    {
        GameObject playerScoreObj = Instantiate(playerScorePrefab, transform.position, Quaternion.identity, playerScoreUIHolder);
        PlayerScoreUI playerScoreUI = playerScoreObj.GetComponent<PlayerScoreUI>();
        playerScoreUIs.Add(playerData.PlayerID, playerScoreUI);
        playerScoreUI.playerID = playerData.PlayerID;

        float deltaX = (pathEnd.position.x - pathStart.position.x) / pointsNeeded;
        float startX = pathStart.position.x;
        float startY = 0;
        List<Transform> nodes = new List<Transform>();
        for (int i = 0; i < pointsNeeded + 1; i++) // Add 1 because the first point is for score = 0
        {
            Vector2 position = new Vector2(startX + deltaX * i, startY);
            GameObject node = Instantiate(nodePrefab, position, Quaternion.identity, playerScoreUI.transform);
            nodes.Add(node.transform);
        }
        playerScoreUI.nodes = nodes;
    }

    public float MoveScores(int pointsNeeded)
    {
        foreach (int playerID in playerScoreUIs.Keys)
        {
            playerScoreUIs[playerID].SetScore(GameMan.Instance.GetPlayer(playerID).Score);
        }
        float deltaX = (pathEnd.position.x - pathStart.position.x) / pointsNeeded;
        // Assume move speed of 5 units per second
        float timeToWalk = deltaX / 5f;
        return timeToWalk; 
    }
}
