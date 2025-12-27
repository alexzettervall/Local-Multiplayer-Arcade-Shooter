using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJoinHandler : MonoBehaviour
{
    private int nextPlayerID = 1; // Unique ID generator

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        int playerID = nextPlayerID++;
        string playerName = "Player " + playerID; // Example naming
        
        playerInput.transform.parent = GameMan.Instance.transform;
        

        // Register player in PlayerManager
        GameMan.Instance.AddPlayer(playerInput.GetComponent<PersistentPlayer>(), playerID, playerName);
    }
}
