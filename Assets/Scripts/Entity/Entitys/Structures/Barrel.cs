using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : Structure
{
    [SerializeField] private float radius;
    [SerializeField] private int damage;
    [SerializeField] private int shrapnel;
    protected override void Kill()
    {
        if (isDead)
        {
            return;
        }
        base.Kill();
        FindObjectOfType<Level>().CreateExplosion(transform.position, damage, radius, lastDamager, shrapnel);
    }
}
