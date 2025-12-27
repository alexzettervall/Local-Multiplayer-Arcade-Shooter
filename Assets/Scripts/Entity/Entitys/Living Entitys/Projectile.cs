using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : Entity
{
    [SerializeField] protected Entity shooter;
    [SerializeField] protected CircleCollider2D col;
    [SerializeField] protected LayerMask layerMask;

    public void SetShooter(Entity shooter)
    {
        this.shooter = shooter;
    }
    public Entity GetShooter()
    {
        return shooter;
    }
    public override void Damage(int damage, Entity damager, DamageSource damageSource)
    {
        return;
    }
    protected override void OnFixedUpdate()
    {
        base.OnUpdate();
        CheckForCollisions(transform.position);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead)
        {
            return;
        }
        
        Entity entity = collision.gameObject.GetComponent<Entity>();
        if (entity != null)
        {
            CollideWithEntity(entity);
        }
    }

    public virtual void CheckForCollisions(Vector2 position, bool ignoreShooter = false)
    {
        if (isDead)
        {
            return;
        }
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, col.radius, layerMask);

        Collider2D closestCollider = null;
        float closestDist = float.MaxValue;

        foreach (Collider2D collider in colliders)
        {
            if (collider == col)
            {
                continue;
            }
            if (collider.GetComponent<Entity>() == null)
            {
                continue;
            }
            if ((collider.GetComponent<Entity>() == shooter) && ignoreShooter)
            {
                continue;
            }

            float dist = Vector2.Distance(collider.transform.position, transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestCollider = collider;
            }
        }

        if (closestCollider != null)
        {
            Entity entity = closestCollider.GetComponent<Entity>();
            CollideWithEntity(entity);
        }
    }
    public virtual void CollideWithEntity(Entity entity)
    {
        
    }
}
