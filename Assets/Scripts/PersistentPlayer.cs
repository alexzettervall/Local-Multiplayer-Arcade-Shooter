using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PersistentPlayer : MonoBehaviour
{
    private LivingEntity entity;

    public PlayerInput playerInput;
    public BotAI botAI;

    public float timeSinceLastRotation = 0f;
    public Vector2 lastDirection = Vector2.zero;

    public void Start() {
        playerInput = GetComponent<PlayerInput>();
        botAI = new PlayerBotAI();
    }
    
    
    public void Update() {
        
        timeSinceLastRotation += Time.deltaTime;
        // Detect if there is a real player controlling this player
        if (IsRealPlayer()) {
            return;
        }

        if (entity == null) {
            return;
        }
        botAI.SetEntity(entity);
        botAI.Update();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (entity == null)
        {
            return;
        }
        entity.OnMove(context.ReadValue<Vector2>().normalized);
        
    }
    public void OnRotate(InputAction.CallbackContext context)
    {
        if (entity == null)
        {
            return;
        }
        if (timeSinceLastRotation < 0.05f && Vector2.Dot(lastDirection, context.ReadValue<Vector2>().normalized) < 0.5f)
        {
            return;
        } 
        timeSinceLastRotation = 0f;
        lastDirection = context.ReadValue<Vector2>().normalized;
        entity.OnRotate(context.ReadValue<Vector2>().normalized, context.control.device);
        //player.OnRotate(context.ReadValue<Vector2>(), context.control.device);
    }
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (entity == null)
        {
            return;
        }
        entity.OnInteract(context.action.triggered);
    }
    public void OnDrop(InputAction.CallbackContext context)
    {
        if (entity == null)
        {
            return;
        }
        entity.OnDrop(context.action.triggered);
    }
    public void OnUse(InputAction.CallbackContext context)
    {
        if (entity == null)
        {
            return;
        }
        entity.OnUse(context.performed, context.canceled);
    }

    public void OnDrawGizmos() {
        if (botAI != null) {
            botAI.DrawGizmos();
        }
    }

    public void SetEntity(LivingEntity livingEntity)
    {
        if (entity != null)
        {
            entity.controller = null;
        }
        entity = livingEntity;
        livingEntity.controller = this;
    }

    public bool IsRealPlayer()
    {
        return playerInput.devices.Count > 0;
    }
}
