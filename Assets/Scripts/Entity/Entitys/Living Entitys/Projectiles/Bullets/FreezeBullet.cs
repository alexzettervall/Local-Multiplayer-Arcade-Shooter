using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeBullet : Bullet
{
    public override void HitEntity(Entity entity)
    {
        base.HitEntity(entity);
        entity.ApplyStatusEffect(new StatusEffect(StatusEffectType.Frozen, 3f, shooter));
    }
}
