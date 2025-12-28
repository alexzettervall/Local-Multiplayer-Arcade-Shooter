using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PersistentPlayer : MonoBehaviour
{
    public Player player;
    public PlayerInput playerInput;
    public BotAI botAI;

    public float timeSinceLastRotation = 0f;
    public Vector2 lastDirection = Vector2.zero;

    public void Start() {
        playerInput = GetComponent<PlayerInput>();
        botAI = new BotAI(this);
    }
    
    
    public void Update() {
        
        timeSinceLastRotation += Time.deltaTime;
        // Detect if there is a real player controlling this player
        if (playerInput.devices.Count > 0) {
            return;
        }

        if (player == null) {
            return;
        }
        botAI.Update();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (player == null)
        {
            return;
        }
        player.OnMove(context.ReadValue<Vector2>().normalized);
        
    }
    public void OnRotate(InputAction.CallbackContext context)
    {
        if (player == null)
        {
            return;
        }
        if (timeSinceLastRotation < 0.05f && Vector2.Dot(lastDirection, context.ReadValue<Vector2>().normalized) < 0.5f)
        {
            return;
        } 
        timeSinceLastRotation = 0f;
        lastDirection = context.ReadValue<Vector2>().normalized;
        player.OnRotate(context.ReadValue<Vector2>().normalized, context.control.device);
        //player.OnRotate(context.ReadValue<Vector2>(), context.control.device);
    }
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (player == null)
        {
            return;
        }
        player.OnInteract(context.action.triggered);
    }
    public void OnDrop(InputAction.CallbackContext context)
    {
        if (player == null)
        {
            return;
        }
        player.OnDrop(context.action.triggered);
    }
    public void OnUse(InputAction.CallbackContext context)
    {
        if (player == null)
        {
            return;
        }
        player.OnUse(context.performed, context.canceled);
    }

    public void OnDrawGizmos() {
        if (botAI != null) {
            botAI.DrawGizmos();
        }
    }
}
