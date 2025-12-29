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
    
}
