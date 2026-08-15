using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Egg : Item
{
    [SerializeField] private float timeUntilHatch;
    [SerializeField] private float minHatchTimeWhileHeld;
    [SerializeField] private float minVelocityToHatchOnCollision;
    [SerializeField] private float entityCollisionRadius;
    [SerializeField] private int collisionDamage;
    private bool hatched = false;

    protected override void OnStart()
    {
        base.OnStart();
        SetCircleColor(GameAssets.i.itemCircleUseless);
        tags.Add("throwable");
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (holder != null)
        {
            timeUntilHatch = Mathf.Max(timeUntilHatch, minHatchTimeWhileHeld);
        }

        timeUntilHatch -= Time.deltaTime;
        if (timeUntilHatch <= 0)
        {
            Hatch();
        }
    }

    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        CheckForEntityCollisions();
    }

    public void Hatch()
    {
        Hatch(null);
    }
    public void Hatch(LivingEntity entityToAgro)
    {
        if (hatched) return;
        hatched = true;
        GameObject chickenObj = Instantiate(GameAssets.i.chickenPrefab, transform.position, quaternion.identity, transform.parent);
        Chicken chicken = chickenObj.GetComponent<Chicken>();
        if (entityToAgro != null && chicken != null)
        {
            chicken.Agro(entityToAgro);
        }

        Kill();
    }

    private void CheckForEntityCollisions()
    {
        if (!thrown) return;
        if (rb.velocity.magnitude < minVelocityToHatchOnCollision) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            entityCollisionRadius
        );

        foreach (Collider2D hit in hits)
        {
            LivingEntity entity = hit.GetComponent<LivingEntity>();

            if (entity != null && entity != this && entity != thrower)
            {
                Hatch(entity);
                entity.Damage(collisionDamage, thrower, DamageSource.Bullet);
                return;
            }
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.relativeVelocity.magnitude);
        if (collision.relativeVelocity.magnitude >= minVelocityToHatchOnCollision)
        {
            Hatch();
            Structure structure = collision.gameObject.GetComponent<Structure>();
            if (structure != null)
            {
                structure.Damage(collisionDamage, thrower, DamageSource.Bullet);
            }
        }
    }
}
