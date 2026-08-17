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
    public bool preformUse { get; private set; }
    public bool cancelUse { get; private set; }
    public bool interact;
    public bool drop;

    public Blackboard()
    {

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

    public T? GetNearest<T>(IEnumerable<T> entities) where T : struct, IPerceivedEntity
    {
        T? nearest = null;
        float nearestDistance = float.MaxValue;

        foreach (T entity in entities)
        {
            float distance = Vector2.Distance(this.entity.transform.position, entity.Position);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = entity;
            }
        }

        return nearest;
    }
}

public interface IPerceivedEntity
{
    Vector2 Position { get; }
}