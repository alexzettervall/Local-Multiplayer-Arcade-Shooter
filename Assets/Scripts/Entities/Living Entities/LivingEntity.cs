using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LivingEntity : Entity
{
    public override void Damage(int damage, Entity damager, DamageSource damageSource)
    {
        // Emit blood particles
        float change = damage;
        if (health - damage < 0)
        {
            change += (health - damage);
        }
        if (damage > 0)
        {
            float spread = Mathf.Lerp(0.2f, 1.5f, damage / 100f);
            for (int i = 0; i < Mathf.CeilToInt(change/2f); i++)
            {
                Destroy(Instantiate(GameAssets.i.bloodSplatter, transform.position, Quaternion.identity), 5f);
            }
        }
        base.Damage(damage, damager, damageSource);
    }
}
