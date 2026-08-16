using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Structure : Entity
{
    [SerializeField] protected float maxSize;
    [SerializeField] protected float minSize;
    [SerializeField] protected bool breakable = true;
    [SerializeField] protected bool dynamicSize = false;
    

    public override int Damage(int damage, Entity damager, DamageSource damageSource)
    {
        if (!breakable)
        {
            damage = 0;
        }
        int damageDelt = base.Damage(damage, damager, damageSource);
        if (dynamicSize)
        {
            ResizeStructure();
        }
        return damageDelt;
    }
    public virtual void ResizeStructure()
    {
        float newScale = minSize + (maxSize - minSize) * ((float)health / (float)maxHealth);
        transform.localScale = new Vector3(newScale, newScale, 1f);
    }
    
    protected override void Kill() {
        base.Kill();
    }

    public bool IsBreakable()
    {
        return breakable;
    }
}
