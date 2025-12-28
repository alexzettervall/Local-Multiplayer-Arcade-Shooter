using System.Collections.Generic;
using UnityEngine;

public class PlayerScoreUI : MonoBehaviour
{
    public int playerID;
    public List<Transform> nodes;
    public Player player;
    public Transform target;
    public bool reachedTarget = false;

    public void Start()
    {
        player.isStatic = true;
        reachedTarget = true;
        target = nodes[0];
        player.InitializePlayer(playerID);
        player.transform.position = nodes[0].position;
        player.transform.eulerAngles = new Vector3(0f, 0f, -90f); // Rotate towards the right
    }

    public void FixedUpdate()
    {
        if (reachedTarget) { return; }
        Vector2 futurePosition = (Vector2)player.transform.position + Vector2.right * player.GetMoveSpeed() * Time.fixedDeltaTime;
        if (futurePosition.x > target.position.x)
        {
            reachedTarget = true;
            player.transform.position = target.position;
        }
        else
        {
            player.Move(Vector2.right);
        }
    }

    public void SetScore(int score)
    {
        if (target == nodes[score]) {return;}
        target = nodes[score];
        reachedTarget = false;
    }
}
