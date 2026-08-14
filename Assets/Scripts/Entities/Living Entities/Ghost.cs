using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ghost : LivingEntity
{
    public float morphRange;

    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        rb.AddForce(movement * GetMoveSpeed() * 1000f * Time.fixedDeltaTime, ForceMode2D.Force);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (interact)
        {
            TryMorph();
        }
    }

    private void TryMorph()
    {
        LivingEntity closest = null;
        float closestDistance = float.MaxValue;

        foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position, morphRange))
        {
            LivingEntity entity = collider.GetComponent<LivingEntity>();
            if (entity == null || entity.controller != null) continue;
            float distance = (entity.transform.position - transform.position).sqrMagnitude;
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = entity;
            }
        }

        if (closest == null) return;

        controller.SetEntity(closest);
        Kill();
    }
}
