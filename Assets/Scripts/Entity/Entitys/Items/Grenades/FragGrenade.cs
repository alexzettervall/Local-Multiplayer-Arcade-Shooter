using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragGrenade : Grenade
{
    [SerializeField] private int damage = 250;
    [SerializeField] private float radius = 5f;
    [SerializeField] private int shrapnel = 10;
    public override void OnExplosion()
    {
        if (holder != null)
        {
            holder.DropItem(true);
        }
        GameMan.Instance.GetLevel().CreateExplosion(transform.position, damage, radius, thrower, shrapnel);
        Kill();
    }
    public override float GetDPS() {
        return 100f;
    }
    public override float GetDamageLeft() {
        if (HasTag("deadly weapon")) {
            return 250f;
        }
        return 0f;
    }
    public override void OnPullBack()
    {
        base.OnPullBack();
        SetCircleColor(GameAssets.i.itemCircleDanger);
    }
}
