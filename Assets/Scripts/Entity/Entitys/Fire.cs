using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : Projectile
{
    [SerializeField] private bool eternal = false;
    [SerializeField] protected float strength = 1f;
    [SerializeField] protected float minSize = 0.3f;
    [SerializeField] protected float lifeTime = 10f;
    [SerializeField] protected float damage = 5;
    [SerializeField] protected float tickDelay = 0.3f;
    protected float lastHitTime = 0f;
    protected override void OnStart()
    {
        base.OnStart();
        UpdateSize();
    }
    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (isDead)
        {
            return;
        }
        if (!eternal)
        {
            strength -= Time.deltaTime / lifeTime;
        }
        else
        {
            strength += Time.deltaTime / lifeTime;
        }
        if (strength > 1f)
        {
            strength = 1f;
        }
        else if (strength <= 0f)
        {
            Kill();
            return;
        }
        UpdateSize();
        AudioMan.PlaySound(Sound.FireBurn);
    }
    public void UpdateSize()
    {
        float newSize = minSize + (1f - minSize) * strength;
        transform.localScale = new Vector3(newSize, newSize, 1f);
    }
    public override void CollideWithEntity(Entity entity)
    {
        base.CollideWithEntity(entity);
        if (isDead)
        {
            return;
        }
        if (Time.time - lastHitTime < tickDelay)
        {
            return;
        }
        lastHitTime = Time.time;
        entity.Damage(Mathf.RoundToInt(strength * damage), shooter, DamageSource.Fire);
    }
}
