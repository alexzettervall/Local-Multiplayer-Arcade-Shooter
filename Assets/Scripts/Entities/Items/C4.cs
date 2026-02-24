using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C4 : Item
{
    [SerializeField] private int damage;
    [SerializeField] private float radius;
    [SerializeField] private int shrapnel = 20;
    protected override void OnStart()
    {
        base.OnStart();
        SetCircleColor(GameAssets.i.itemCircleDanger);
        tags.Add("deadly weapon");
        tags.Add("throwable");
    }
    public void Explode(Entity causer)
    {
        GameMan.Instance.GetLevel().CreateExplosion(transform.position, damage, radius, causer, shrapnel);
        Kill();
    }
}
