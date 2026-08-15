using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragGrenade : Grenade
{
    [SerializeField] private int damage = 250;
    [SerializeField] private float radius = 5f;
    [SerializeField] private int shrapnel = 10;
    [SerializeField] private bool proximityGrenade;
    [SerializeField] private float proximityRadius;

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
    public override void StartCooking()
    {
        base.StartCooking();
        SetCircleColor(GameAssets.i.itemCircleDanger);
    }
    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (!cooking) { return; }
        if (!proximityGrenade) { return; }
        if (exploded) { return; }

        Collider2D[] cols = Physics2D.OverlapCircleAll(rb.position, proximityRadius);
        foreach (Collider2D col in cols)
        {
            Player player = col.gameObject.GetComponent<Player>();
            if (player == null) { continue; }
            if (player == thrower) { continue; }
            Explode();
        }
    }
}
