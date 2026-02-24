using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Bullet : Projectile
{
    [SerializeField] protected int damage;
    [SerializeField] private float lifeTime;
    [SerializeField] private float lifeTimer;
    private bool reflected = false;
    

    // Getters and Setters
    public float GetLifeTime()
    {
        return lifeTime;
    }
    // Add damage falloff here
    public int GetDamage()
    {
        return damage;
    }
    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    public virtual void UpdateLifeTimer() {
        lifeTimer += Time.fixedDeltaTime;
        if (lifeTimer > lifeTime)
        {
            Kill();
        }
    }

    // Override Functions
    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        UpdateLifeTimer();
    }
    public override void CollideWithEntity(Entity entity)
    {
        if (isDead)
        {
            return;
        }
        bool doDamage = true;
        if (entity == shooter)
        {
            if (!reflected)
            {
                return;
            }
        }
        if (doDamage)
        {
            HitEntity(entity);
        }
        bool killBullet = true;
        if (entity.ReflectsBullets())
        {
            killBullet = false;
            reflected = true;
        }
        if (killBullet)
        {
            Kill();
        }
    }
    public virtual void HitEntity(Entity entity)
    {
        entity.Damage(damage, shooter, DamageSource.Bullet);
    }
}
