using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Melle : Item
{
    [Header("Weapon Attributes")]
    public float attackSpeed;
    public float damage;
    public float attackDistance;
    public float attackRadius;

    [Header("General")]
    public int groundColLayer;
    public int heldColLayer;
    public int hitsToDestroy;
    public bool canBeDestroyed = false;
    public Animation swingAnimation;

    protected float attackTimer = 0f;

    protected override void OnUpdate()
    {
        base.OnUpdate();
        attackTimer -= Time.deltaTime;
    }
    public override void Use(bool use, bool unUse)
    {
        base.Use(use, unUse);
        if (attackTimer <= 0f && holder is Player)
        {
            Attack();
        }
    }

    public override void PickUp(Entity holder)
    {
        base.PickUp(holder);
        gameObject.layer = heldColLayer;
    }
    public override void OnDropped()
    {
        base.OnDropped();
        gameObject.layer = groundColLayer;
    }

    public void Attack()
    {
        Player player = (Player) holder;
        player.AttackMelee(damage, Sound.StonePunchHit, attackRadius, attackDistance);
        attackTimer = 1f / attackSpeed;
        player.GetAnimator().SetTrigger("Punch Right");
    }

    public override void Damage(int damage, Entity damager, DamageSource damageSource)
    {
        // Only get hit by bullets and when its being held
        if (holder == null) { return; }
        if (damageSource != DamageSource.Bullet) { return; }
        if (!canBeDestroyed) { return; }
        hitsToDestroy--;
        if (hitsToDestroy <= 0)
        {
            Kill();
        }
    }
    
}
